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

        public MCvPoint3D64f? OldAMGFusedAcc;
        public MCvPoint3D64f? OldAMGFusedMagnet;
        public MCvPoint3D64f? OldRawAcc;
        public MCvPoint3D64f? OldRawMagnet;
        public MCvPoint3D64f? OldFilteredAcc;
        public MCvPoint3D64f? OldFilteredMagnet;

        public ReadingsVector3f OldGyroReadings;

        public Matrix<double> OldGAMFusedMatrix; 

        public double[][] GetAccMagnetOrientationMatrix(MEMSReadingsSet3f newReadings, bool useLowpassFilter, double accMagnetFilterCoeff, double gyroFilterCoeff)
        {
            double[][] res = new double[3][];
            for (int i = 0; i < 3; ++i)
            {
                res[i] = new double[3];
            }

            MCvPoint3D64f rawAccPoint = new MCvPoint3D64f(newReadings.AccVector3f.Values[0], newReadings.AccVector3f.Values[1], newReadings.AccVector3f.Values[2]);
            MCvPoint3D64f rawMagnetPoint = new MCvPoint3D64f(newReadings.MagnetVector3f.Values[0], newReadings.MagnetVector3f.Values[1], newReadings.MagnetVector3f.Values[2]);
            var gyroDiffMatr = this.GetGyroRotationMatrix(newReadings.GyroVector3f);

            MCvPoint3D64f accFilteredPoint;
            MCvPoint3D64f magnetFilteredPoint;

            if (useLowpassFilter && this.OldAMGFusedAcc != null && this.OldAMGFusedMagnet != null)
            {
                //this.FilterAccMagnet(ref resAccPoint, ref resMagnetPoint, accMagnetFilterCoeff);
                accFilteredPoint = new MCvPoint3D64f(
                    rawAccPoint.x * accMagnetFilterCoeff + OldAMGFusedAcc.Value.x * (1 - accMagnetFilterCoeff),
                    rawAccPoint.y * accMagnetFilterCoeff + OldAMGFusedAcc.Value.y * (1 - accMagnetFilterCoeff),
                    rawAccPoint.z * accMagnetFilterCoeff + OldAMGFusedAcc.Value.z * (1 - accMagnetFilterCoeff)
                    );

                magnetFilteredPoint = new MCvPoint3D64f(
                    rawMagnetPoint.x * accMagnetFilterCoeff + OldAMGFusedMagnet.Value.x * (1 - accMagnetFilterCoeff),
                    rawMagnetPoint.y * accMagnetFilterCoeff + OldAMGFusedMagnet.Value.y * (1 - accMagnetFilterCoeff),
                    rawMagnetPoint.z * accMagnetFilterCoeff + OldAMGFusedMagnet.Value.z * (1 - accMagnetFilterCoeff)
                    );
            }
            else
            {
                accFilteredPoint = new MCvPoint3D64f(rawAccPoint.x, rawAccPoint.y, rawAccPoint.z);
                magnetFilteredPoint = new MCvPoint3D64f(rawMagnetPoint.x, rawMagnetPoint.y, rawMagnetPoint.z);
            }
            
            MCvPoint3D64f resAccPoint;
            MCvPoint3D64f resMagnetPoint;


            if(this.OldAMGFusedAcc == null || this.OldAMGFusedMagnet == null)
            {
                resAccPoint = accFilteredPoint;
                resMagnetPoint = magnetFilteredPoint;
            }
            else
            {
                resAccPoint = MatrixToMCvPoint3D64f(MCvPoint3D64fToMatrix(OldAMGFusedAcc.Value).Mul(gyroDiffMatr).Mul(gyroFilterCoeff).Add(MCvPoint3D64fToMatrix(accFilteredPoint).Mul(1 - gyroFilterCoeff)));
                resMagnetPoint = MatrixToMCvPoint3D64f(MCvPoint3D64fToMatrix(OldAMGFusedMagnet.Value).Mul(gyroDiffMatr).Mul(gyroFilterCoeff).Add(MCvPoint3D64fToMatrix(magnetFilteredPoint).Mul(1 - gyroFilterCoeff)));
            }

            this.OldAMGFusedAcc = resAccPoint;
            this.OldAMGFusedMagnet = resMagnetPoint;
            
            this.OldRawAcc = rawAccPoint;
            this.OldRawMagnet = rawMagnetPoint;
            
            this.OldFilteredAcc = accFilteredPoint;
            this.OldFilteredMagnet = magnetFilteredPoint;

            this.OldGyroReadings = newReadings.GyroVector3f;

            MCvPoint3D64f x;
            MCvPoint3D64f y;
            MCvPoint3D64f z;
            
            this.GetOrthoNormalAccMagnetBasis(resAccPoint, resMagnetPoint, out x, out y, out z);

            Matrix<double> accMagnetMatrix = new Matrix<double>(3, 3);

            accMagnetMatrix[0, 0] = x.x; accMagnetMatrix[0, 1] = x.y; accMagnetMatrix[0, 2] = x.z;
            accMagnetMatrix[1, 0] = y.x; accMagnetMatrix[1, 1] = y.y; accMagnetMatrix[1, 2] = y.z;
            accMagnetMatrix[2, 0] = z.x; accMagnetMatrix[2, 1] = z.y; accMagnetMatrix[2, 2] = z.z;

            //AccMagnetGyro fuse
            Matrix<double> newGAMFusedMatrix = null;
            //old amg fusion
            //if (this.OldGAMFusedMatrix == null)
            //{
            //    newGAMFusedMatrix = accMagnetMatrix;
            //}
            //else
            //{
            //    var newGyroMatrix = this.OldGAMFusedMatrix.Mul(gyroDiffMatr);
            //    newGAMFusedMatrix = this.FuseAccMagnetGyro(accMagnetMatrix, newGyroMatrix, gyroFilterCoeff);
            //}
            newGAMFusedMatrix = accMagnetMatrix;
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

        public MCvPoint3D64f ScalePoint(MCvPoint3D64f point, double scale)
        {
            return new MCvPoint3D64f(point.x * scale, point.y * scale, point.z * scale);
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

        public Matrix<double> MCvPoint3D64fToMatrix(MCvPoint3D64f point)
        {
            return new Matrix<double>(new double[,] 
            {
                {point.x, point.y, point.z}
            });
        }

        public MCvPoint3D64f MatrixToMCvPoint3D64f(Matrix<double> m)
        {
            if (m.Height == 3)
            {
                return new MCvPoint3D64f(m[0, 0], m[1, 0], m[2, 0]);
            }
            else
            {
                return new MCvPoint3D64f(m[0, 0], m[0, 1], m[0, 2]);
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

        public void FilterAccMagnet(ref MCvPoint3D64f newAcc, ref MCvPoint3D64f newMagnet, double newCoeff)
        {
            double oldCoeff = 1 - newCoeff;
            if (this.OldAMGFusedAcc == null || this.OldAMGFusedMagnet == null)
            {
                return;
            }

            MCvPoint3D64f ta = new MCvPoint3D64f((this.OldAMGFusedAcc.Value.x * oldCoeff + newAcc.x * newCoeff), (this.OldAMGFusedAcc.Value.y * oldCoeff + newAcc.y * newCoeff), (this.OldAMGFusedAcc.Value.z * oldCoeff + newAcc.z * newCoeff));
            MCvPoint3D64f tm = new MCvPoint3D64f((this.OldAMGFusedMagnet.Value.x * oldCoeff + newMagnet.x * newCoeff), (this.OldAMGFusedMagnet.Value.y * oldCoeff + newMagnet.y * newCoeff), (this.OldAMGFusedMagnet.Value.z * oldCoeff + newMagnet.z * newCoeff));

            newAcc = ta;
            newMagnet = tm;

        }

        public void GetOrthoNormalAccMagnetBasis(MCvPoint3D64f accPoint, MCvPoint3D64f magnetPoint, out MCvPoint3D64f x, out MCvPoint3D64f y, out MCvPoint3D64f z)
        {
            var accMagnetCross = accPoint.CrossProduct(magnetPoint);

            var magnetNormal = accMagnetCross.CrossProduct(accPoint);

            var aNorm = this.GetNormalizedPoint(accPoint);
            var mNorm = this.GetNormalizedPoint(magnetNormal);
            var amNorm = this.GetNormalizedPoint(accMagnetCross);

            x = this.GetPointNorm(mNorm).CompareTo(double.NaN) == 0 ? new MCvPoint3D64f(1, 0, 0) : mNorm;
            //y = aNorm.Norm.CompareTo(double.NaN) == 0 ? new MCvPoint3D64f(0, 1, 0) : new MCvPoint3D64f(-aNorm.x, -aNorm.y, -aNorm.z);
            y = this.GetPointNorm(aNorm).CompareTo(double.NaN) == 0 ? new MCvPoint3D64f(0, 1, 0) : aNorm;
            z = this.GetPointNorm(amNorm).CompareTo(double.NaN) == 0 ? new MCvPoint3D64f(0, 0, 1) : amNorm;
        }

        public double GetPointNorm(MCvPoint3D64f point)
        {
            return Math.Sqrt(point.x * point.x + point.y * point.y + point.z * point.z);
        }

        public MCvPoint3D64f GetNormalizedPoint(MCvPoint3D64f point)
        {
            var norm = this.GetPointNorm(point);
            var res = new MCvPoint3D64f(point.x / norm, point.y / norm, point.z / norm);
            return res;
        }
    }
}
