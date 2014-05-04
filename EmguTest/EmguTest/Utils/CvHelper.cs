using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

using Emgu.CV;
using Emgu.Util;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using Emgu.CV.GPU;
using Emgu.CV.Features2D;

namespace EmguTest.Utils
{
    static public class CvHelper
    {
        public static Bitmap ConvertImageToBitmap(Image<Bgr, byte> img)
        {
            return new Bitmap(img.Bitmap);
        }

        public static Matrix<double> ArrayToMatrix(double[][] array, Size size)
        {
            Matrix<double> resMatrix = new Matrix<double>(size);
            for (int i = 0; i < size.Height; ++i)
            {
                for (int j = 0; j < size.Width; ++j)
                {
                    resMatrix[i, j] = array[i][j];
                }
            }
            return resMatrix;
        }

        public static double[][] MatrixToArray(Matrix<double> matrix)
        {
            double[][] res = new double[matrix.Height][];
            for (int i = 0; i < matrix.Height; ++i)
            {
                res[i] = new double[matrix.Width];
                for (int j = 0; j < matrix.Width; ++j)
                {
                    res[i][j] = matrix[i, j];
                }
            }
            return res;
        }

        public static double[][] CopyMatrix(double[][] original)
        {
            if (original == null)
            {
                return null;
            }
            var res = new double[original.Length][];
            for (int i = 0; i < original.Length; ++i)
            {
                res[i] = new double[original[i].Length];
                for (int j = 0; j < res[i].Length; ++j)
                {
                    res[i][j] = original[i][j];
                }
            }
            return res;
        }
    }
}
