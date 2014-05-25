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

using EmguTest.DataSource;
using EmguTest.MEMS;
namespace EmguTest.Odometry
{
    class VisualOdometer
    {
        public static MCvPoint3D64f? GetTranslation(double[][] rotMatrArray, OpticFlowFrameContainer prevFrame, OpticFlowFrameContainer currFrame, StereoCameraParams cameraParams, out List<PointF> currFeaturesList, out List<PointF> prevFeaturesList)
        {

            int MaxFeaturesCount = 400;
            double QualityLevel = 0.01;
            double MinDistance = 1;
            int BlockSize = 10;
            Size WinSize = new Size(80, 80);
            int PyrLevel = 4;
            MCvTermCriteria PyrLkTerm = new MCvTermCriteria(100, 0.001);

            if (
                rotMatrArray == null ||
                prevFrame == null ||
                currFrame == null ||
                cameraParams == null)
            {
                prevFeaturesList = null;
                currFeaturesList = null;
                return null;
            }

            var leftPrevGrayImg = new Image<Gray, byte>(prevFrame.StereoFrame.LeftRawFrame);
            var leftCurrGrayImg = new Image<Gray, byte>(currFrame.StereoFrame.LeftRawFrame);
            var prevFeatures = leftPrevGrayImg.GoodFeaturesToTrack(MaxFeaturesCount, QualityLevel, MinDistance, BlockSize)[0];
            var currFeatures = new PointF[prevFeatures.Count()];

            var status = new Byte[prevFeatures.Count()];
            var error = new float[prevFeatures.Count()];

            OpticalFlow.PyrLK(
                leftPrevGrayImg,
                leftCurrGrayImg,
                prevFeatures,
                WinSize,
                PyrLevel,
                PyrLkTerm,
                out currFeatures,
                out status,
                out error);

            var depthSize = prevFrame.DepthMapImg.Size;
            var reprojPrevDepthMap = new Image<Gray, short>(prevFrame.DepthMapImg.Size);
            var reprojCurrDepthMap = new Image<Gray, short>(currFrame.DepthMapImg.Size);

            for (int i = 0; i < depthSize.Height; ++i)
            {
                for (int j = 0; j < depthSize.Width; ++j)
                {
                    reprojPrevDepthMap.Data[i, j, 0] = 0;
                    //reprojPrevDepthMap.Data[i, j, 1] = 0;
                    //reprojPrevDepthMap.Data[i, j, 2] = 0;

                    reprojCurrDepthMap.Data[i, j, 0] = 0;
                    //reprojCurrDepthMap.Data[i, j, 1] = 0;
                    //reprojCurrDepthMap.Data[i, j, 2] = 0;
                }
            }

            var actPrevFreatures = new List<PointF>();
            var actCurrFreatures = new List<PointF>();

            for (int i = 0; i < prevFeatures.Count(); ++i)
            {
                if (status[i] == 1)
                {
                    actPrevFreatures.Add(prevFeatures[i]);
                    actCurrFreatures.Add(currFeatures[i]);

                    int xp = (int)prevFeatures[i].X;
                    int yp = (int)prevFeatures[i].Y;
                    if (yp < reprojPrevDepthMap.Height && yp >=0 && xp < reprojPrevDepthMap.Width && xp >= 0)
                    {
                        reprojPrevDepthMap.Data[yp, xp, 0] = prevFrame.DepthMapImg.Data[yp, xp, 0];
                    }
                    //reprojPrevDepthMap.Data[yp, xp, 1] = prevFrame.DepthMapImg.Data[yp, xp, 1];
                    //reprojPrevDepthMap.Data[yp, xp, 2] = prevFrame.DepthMapImg.Data[yp, xp, 2];

                    int xc = (int)currFeatures[i].X;
                    int yc = (int)currFeatures[i].Y;
                    if (yc < reprojCurrDepthMap.Height && yc >=0 && xc < reprojCurrDepthMap.Width && xc >= 0)
                    {
                        reprojCurrDepthMap.Data[yc, xc, 0] = currFrame.DepthMapImg.Data[yc, xc, 0];
                    }
                    //reprojCurrDepthMap.Data[yc, xc, 1] = currFrame.DepthMapImg.Data[yc, xc, 1];
                    //reprojCurrDepthMap.Data[yc, xc, 2] = currFrame.DepthMapImg.Data[yc, xc, 2];
                }
            }

            var prevPoints = PointCollection.ReprojectImageTo3D(reprojPrevDepthMap, cameraParams.Q);
            var currPoints = PointCollection.ReprojectImageTo3D(reprojCurrDepthMap, cameraParams.Q);

            var maxZ = prevPoints.Max(x => x.z);
            var actPrevPoints = prevPoints.Where(x => x.z != maxZ).ToArray();
            var actCurrPoints = currPoints.Where(x => x.z != maxZ).ToArray();

            var prevCentroid = GetCentroid(actPrevPoints);
            var currCentroid = GetCentroid(actCurrPoints);

            var rotMatrix = Utils.CvHelper.ArrayToMatrix(rotMatrArray, new Size(3, 3));
            var inRotMatrix = Utils.CvHelper.InverseMatrix(rotMatrix);
            
            var rotPrevCentr = inRotMatrix.Mul(prevCentroid);
            
            //raw tranalstion
            var rawT = new Matrix<double>(rotPrevCentr.Size);
            rawT[0, 0] = currCentroid[0, 0] - rotPrevCentr[0, 0];
            rawT[1, 0] = currCentroid[1, 0] - rotPrevCentr[1, 0];
            rawT[2, 0] = currCentroid[2, 0] - rotPrevCentr[2, 0];
            
            //camera translation
            var camT = rotMatrix.Mul(rawT);
            //var camT = rawT;

            var X = camT[0, 0];
            var Y = camT[1, 0];
            var Z = camT[2, 0];

            prevFeaturesList = actPrevFreatures;
            currFeaturesList = actCurrFreatures;
            return new MCvPoint3D64f(X, Y, Z);
        }

        protected static Matrix<double> GetCentroid(MCvPoint3D32f[] points)
        {
            int count = points.Length;
            double X = 0;
            double Y = 0;
            double Z = 0;

            foreach (var p in points)
            {
                X += p.x;
                Y += p.y;
                Z += p.z;
            }

            X /= count;
            Y /= count;
            Z /= count;

            return new Matrix<double>( new double[,]
                {
                    {X},
                    {Y},
                    {Z}
                });
        }
    }
    
}
