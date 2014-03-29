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
        public double[][] GetAccMagnetOrientationMatrix(MEMSReadingsSet3f newReadings)
        {
            double[][] res = new double[3][];
            for (int i = 0; i < 3; ++i)
            {
                res[i] = new double[3];
            }

            MCvPoint3D32f accPoint = new MCvPoint3D32f(newReadings.AccVector3f.Values[0], newReadings.AccVector3f.Values[1], newReadings.AccVector3f.Values[2]);
            MCvPoint3D32f magnetPoint = new MCvPoint3D32f(newReadings.MagnetVector3f.Values[0], newReadings.MagnetVector3f.Values[1], newReadings.MagnetVector3f.Values[2]);

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

        public void GetOrthoNormalAccMagnetBasis(MCvPoint3D32f accPoint, MCvPoint3D32f magnetPoint, out MCvPoint3D32f x, out MCvPoint3D32f y, out MCvPoint3D32f z)
        {
            var accMagnetCross = accPoint.CrossProduct(magnetPoint);

            var magnetNormal = accMagnetCross.CrossProduct(accPoint);

            var aNorm = accPoint.GetNormalizedPoint();
            var mNorm = magnetNormal.GetNormalizedPoint();
            var amNorm = accMagnetCross.GetNormalizedPoint();

            x = mNorm.Norm.CompareTo(double.NaN) == 0 ? new MCvPoint3D32f(1, 0, 0) : mNorm;
            y = aNorm.Norm.CompareTo(double.NaN) == 0 ? new MCvPoint3D32f(0, 1, 0) : new MCvPoint3D32f(-aNorm.x, -aNorm.y, -aNorm.z);
            z = amNorm.Norm.CompareTo(double.NaN) == 0 ? new MCvPoint3D32f(0, 0, 1) : amNorm;
        }
    }
}
