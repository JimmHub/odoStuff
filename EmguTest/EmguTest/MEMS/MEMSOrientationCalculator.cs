using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Emgu.CV;
using Emgu.Util;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;

namespace EmguTest.MEMS
{
    class MEMSOrientationCalculator
    {

        public MCvPoint3D32f? OldAcc;
        public MCvPoint3D32f? OldMagnet;
        public ReadingsVector3f OldGyroReadings;

        public Matrix<double> OldGAMFusedMatrix; 

        public double[][] GetAccMagnetOrientationMatrix(MEMSReadingsSet3f newReadings, bool useLowpassFilter, double accMagnetFilterCoeff, double gyroFilterCoeff)
        {
            double[][] res = new double[3][];
            for (int i = 0; i < 3; ++i)
            {
                res[i] = new double[3];
            }

            MCvPoint3D32f accPoint = new MCvPoint3D32f(newReadings.AccVector3f.Values[0], newReadings.AccVector3f.Values[1], newReadings.AccVector3f.Values[2]);
            MCvPoint3D32f magnetPoint = new MCvPoint3D32f(newReadings.MagnetVector3f.Values[0], newReadings.MagnetVector3f.Values[1], newReadings.MagnetVector3f.Values[2]);
            var gyroMatr = this.GetGyroRotationMatrix(newReadings.GyroVector3f);

            //gyro orientation
            //if (this.OldGyroMatrix == null)
            //{
            //    this.OldGyroMatrix = gyroMatr;
            //}
            //else
            //{
            //    //this.NormalizeMatrix(ref gyroMatr);
            //    this.OldGyroMatrix = this.OldGyroMatrix.Mul(gyroMatr);
            //}
            ////

            if (useLowpassFilter)
            {
                this.FilterAccMagnet(ref accPoint, ref magnetPoint, accMagnetFilterCoeff);
            }

            this.OldAcc = accPoint;
            this.OldMagnet = magnetPoint;
            this.OldGyroReadings = newReadings.GyroVector3f;

            MCvPoint3D32f x;
            MCvPoint3D32f y;
            MCvPoint3D32f z;
            
            this.GetOrthoNormalAccMagnetBasis(accPoint, magnetPoint, out x, out y, out z);

            Matrix<double> accMagnetMatrix = new Matrix<double>(3, 3);

            accMagnetMatrix[0, 0] = x.x; accMagnetMatrix[0, 1] = x.y; accMagnetMatrix[0, 2] = x.z;
            accMagnetMatrix[1, 0] = y.x; accMagnetMatrix[1, 1] = y.y; accMagnetMatrix[1, 2] = y.z;
            accMagnetMatrix[2, 0] = z.x; accMagnetMatrix[2, 1] = z.y; accMagnetMatrix[2, 2] = z.z;

            //AccMagnetGyro fuse
            Matrix<double> newGAMFusedMatrix = null;
            if (this.OldGAMFusedMatrix == null)
            {
                newGAMFusedMatrix = accMagnetMatrix;
            }
            else
            {
                var newGyroMatrix = this.OldGAMFusedMatrix.Mul(gyroMatr);
                newGAMFusedMatrix = this.FuseAccMagnetGyro(accMagnetMatrix, newGyroMatrix, gyroFilterCoeff);
            }
            OldGAMFusedMatrix = newGAMFusedMatrix;
            
            ////

            var resMatrix = newGAMFusedMatrix.Transpose();

            //res = this.ConvertMatrix(m);
            //gyro test
            //m = this.OldGyroMatrix;
            //m = m.Transpose();
            //
            for (int i = 0; i < 3; ++i)
            {
                for (int j = 0; j < 3; ++j)
                {
                    res[i][j] = resMatrix[i, j];
                }
            }

            return res;
        }

        public Matrix<double> FuseAccMagnetGyro(Matrix<double> accMagnet, Matrix<double> gyro, double gyroFilterCoeff)
        {
            var accMagnetFilterCoeff = 1 - gyroFilterCoeff;

            var h = accMagnet.Height;
            var w = accMagnet.Width;

            Matrix<double> res = new Matrix<double>(h, w);

            for (int i = 0; i < h; ++i)
            {
                for (int j = 0; j < w; ++j)
                {
                    res[i, j] = accMagnet[i, j] * accMagnetFilterCoeff + gyro[i, j] * gyroFilterCoeff;
                }
            }

            return res;
        }

        public Matrix<double> MCvPoint3D32fToMatrix(MCvPoint3D32f point)
        {
            return new Matrix<double>(new double[,] 
            {
                {point.x, point.y, point.z}
            });
        }

        public MCvPoint3D32f MatrixToMCvPoint3D32f(Matrix<double> m)
        {
            if (m.Height == 3)
            {
                return new MCvPoint3D32f((float)m[0, 0], (float)m[1, 0], (float)m[2, 0]);
            }
            else
            {
                return new MCvPoint3D32f((float)m[0, 0], (float)m[0, 1], (float)m[0, 1]);
            }
        }

        public double[][] ConvertMatrix(Matrix<double> m)
        {
            if (m == null)
            {
                return null;
            }
            var res = new double[m.Height][];
            for (int i = 0; i < res.Length; ++i)
            {
                res[i] = new double[m.Width];
            }

            for (int i = 0; i < m.Height; ++i)
            {
                for (int j = 0; j < m.Width; ++j)
                {
                    res[i][j] = m[i, j];
                }
            }
            return res;
        }

        public void NormalizeMatrix(ref Matrix<double> m)
        {
            double min;
            double max;
            System.Drawing.Point minPoint;
            System.Drawing.Point maxPoint;
            m.MinMax(out min, out max, out minPoint, out maxPoint);
            max = Math.Abs(max);
            min = Math.Abs(min);
            if (min > max)
            {
                max = min;
            }

            for (int i = 0; i < m.Height; ++i)
            {
                for (int j = 0; j < m.Width; ++j)
                {
                    m.Data[i, j] *= 1 / max;
                }
            }
        }

        public Matrix<double> GetGyroRotationMatrix(ReadingsVector3f gyroVect)
        {
            Matrix<double> res = new Matrix<double>(3, 3);
            if (this.OldGyroReadings == null)
            {
                res.SetIdentity();
            }
            else
            {
                long diffMS = gyroVect.TimeStampI - this.OldGyroReadings.TimeStampI;

                double xr = -((double)gyroVect.Values[0] * diffMS) / 1000;
                double yr = ((double)gyroVect.Values[1] * diffMS) / 1000;
                double zr = -((double)gyroVect.Values[2] * diffMS) / 1000;

                double sinxr = Math.Sin(xr);
                double cosxr = Math.Cos(xr);

                double sinyr = Math.Sin(yr);
                double cosyr = Math.Cos(yr);

                double sinzr = Math.Sin(zr);
                double coszr = Math.Cos(zr);

                Matrix<double> xM = new Matrix<double>(new double[,]
                    {
                        {1,   0,      0    },
                        {0,   cosxr,  sinxr},
                        {0,   -sinxr, cosxr}
                    });

                Matrix<double> yM = new Matrix<double>(new double[,]
                    {
                        {cosyr,  0,   sinyr},
                        {0,      1,   0    },
                        {-sinyr, 0,   cosyr}
                    });

                Matrix<double> zM = new Matrix<double>(new double[,]
                    {
                        {coszr,  sinzr, 0},
                        {-sinzr, coszr, 0},
                        {0,      0,     1}
                    });

                res = xM.Mul(yM).Mul(zM);
            }

            return res;
        }

        public void FilterAccMagnet(ref MCvPoint3D32f newAcc, ref MCvPoint3D32f newMagnet, double newCoeff)
        {
            double oldCoeff = 1 - newCoeff;
            if (this.OldAcc == null || this.OldMagnet == null)
            {
                return;
            }

            MCvPoint3D32f ta = new MCvPoint3D32f((float)(this.OldAcc.Value.x * oldCoeff + newAcc.x * newCoeff), (float)(this.OldAcc.Value.y * oldCoeff + newAcc.y * newCoeff), (float)(this.OldAcc.Value.z * oldCoeff + newAcc.z * newCoeff));
            MCvPoint3D32f tm = new MCvPoint3D32f((float)(this.OldMagnet.Value.x * oldCoeff + newMagnet.x * newCoeff), (float)(this.OldMagnet.Value.y * oldCoeff + newMagnet.y * newCoeff), (float)(this.OldMagnet.Value.z * oldCoeff + newMagnet.z * newCoeff));

            newAcc = ta;
            newMagnet = tm;

        }

        public void GetOrthoNormalAccMagnetBasis(MCvPoint3D32f accPoint, MCvPoint3D32f magnetPoint, out MCvPoint3D32f x, out MCvPoint3D32f y, out MCvPoint3D32f z)
        {
            var accMagnetCross = accPoint.CrossProduct(magnetPoint);

            var magnetNormal = accMagnetCross.CrossProduct(accPoint);

            var aNorm = accPoint.GetNormalizedPoint();
            var mNorm = magnetNormal.GetNormalizedPoint();
            var amNorm = accMagnetCross.GetNormalizedPoint();

            x = mNorm.Norm.CompareTo(double.NaN) == 0 ? new MCvPoint3D32f(1, 0, 0) : mNorm;
            //y = aNorm.Norm.CompareTo(double.NaN) == 0 ? new MCvPoint3D32f(0, 1, 0) : new MCvPoint3D32f(-aNorm.x, -aNorm.y, -aNorm.z);
            y = aNorm.Norm.CompareTo(double.NaN) == 0 ? new MCvPoint3D32f(0, 1, 0) : aNorm;
            z = amNorm.Norm.CompareTo(double.NaN) == 0 ? new MCvPoint3D32f(0, 0, 1) : amNorm;
        }
    }
}
