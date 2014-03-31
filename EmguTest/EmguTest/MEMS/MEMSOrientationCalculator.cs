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

        public MCvPoint3D32f? oldAcc;
        public MCvPoint3D32f? oldMagnet;

        public double[][] GetAccMagnetOrientationMatrix(MEMSReadingsSet3f newReadings, bool useLowpassFilter, double lowPassFilterCoeff)
        {
            double[][] res = new double[3][];
            for (int i = 0; i < 3; ++i)
            {
                res[i] = new double[3];
            }

            MCvPoint3D32f accPoint = new MCvPoint3D32f(newReadings.AccVector3f.Values[0], newReadings.AccVector3f.Values[1], newReadings.AccVector3f.Values[2]);
            MCvPoint3D32f magnetPoint = new MCvPoint3D32f(newReadings.MagnetVector3f.Values[0], newReadings.MagnetVector3f.Values[1], newReadings.MagnetVector3f.Values[2]);

            if (useLowpassFilter)
            {
                this.FilterAccMagnet(ref accPoint, ref magnetPoint, lowPassFilterCoeff);
            }

            this.oldAcc = accPoint;
            this.oldMagnet = magnetPoint;

            MCvPoint3D32f x;
            MCvPoint3D32f y;
            MCvPoint3D32f z;

            this.GetOrthoNormalAccMagnetBasis(accPoint, magnetPoint, out x, out y, out z);

            Matrix<double> m = new Matrix<double>(3, 3);

            m[0, 0] = x.x; m[0, 1] = x.y; m[0, 2] = x.z;
            m[1, 0] = y.x; m[1, 1] = y.y; m[1, 2] = y.z;
            m[2, 0] = z.x; m[2, 1] = z.y; m[2, 2] = z.z;

            m = m.Transpose();

            for (int i = 0; i < 3; ++i )
            {
                for (int j = 0; j < 3; ++j)
                {
                    res[i][j] = m[i, j];
                }
            }

            return res;
        }

        public void FilterAccMagnet(ref MCvPoint3D32f newAcc, ref MCvPoint3D32f newMagnet, double oldCoeff)
        {
            double newCoeff = 1 - oldCoeff;
            if (this.oldAcc == null || this.oldMagnet == null)
            {
                return;
            }

            MCvPoint3D32f ta = new MCvPoint3D32f((float)(this.oldAcc.Value.x * oldCoeff + newAcc.x * newCoeff), (float)(this.oldAcc.Value.y * oldCoeff + newAcc.y * newCoeff), (float)(this.oldAcc.Value.z * oldCoeff + newAcc.z * newCoeff));
            MCvPoint3D32f tm = new MCvPoint3D32f((float)(this.oldMagnet.Value.x * oldCoeff + newMagnet.x * newCoeff), (float)(this.oldMagnet.Value.y * oldCoeff + newMagnet.y * newCoeff), (float)(this.oldMagnet.Value.z * oldCoeff + newMagnet.z * newCoeff));

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
