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
        public static MCvPoint3D64f? GetTranslationAndRotation(double[][] rotMatrArray, OpticFlowFrameContainer prevFrame, OpticFlowFrameContainer currFrame, StereoCameraParams cameraParams, out List<PointF> currFeaturesList, out List<PointF> prevFeaturesList, out Matrix<double> resRotation)
        {
            if (
                rotMatrArray == null ||
                prevFrame == null ||
                currFrame == null ||
                cameraParams == null)
            {
                prevFeaturesList = null;
                currFeaturesList = null;
                resRotation = null;
                return null;
            }

            var leftPrevGrayImg = new Image<Gray, byte>(prevFrame.StereoFrame.LeftRawFrame);
            var leftCurrGrayImg = new Image<Gray, byte>(currFrame.StereoFrame.LeftRawFrame);
            
            var prevFeatures = GetFeaturesToTrack(leftPrevGrayImg);
            PointF[] prevCorrFeatures;
            PointF[] currCorrFeatures;

            GetCorrespFeatures(prevFeatures, leftPrevGrayImg, leftCurrGrayImg, out prevCorrFeatures, out currCorrFeatures);

            var prev3dPoints = ReprojectTo3d(leftPrevGrayImg, null, prevCorrFeatures, prevFrame.DepthMapImg, cameraParams.Q);
            var curr3dPoints = ReprojectTo3d(leftCurrGrayImg, null, currCorrFeatures, currFrame.DepthMapImg, cameraParams.Q);

            var prevAct3dPointsList = new List<Matrix<double>>();
            var currAct3dPointsList = new List<Matrix<double>>();

            for (int i = 0; i < prev3dPoints.Count(); ++i)
            {
                if (prev3dPoints[i] != null && curr3dPoints[i] != null)
                {
                    prevAct3dPointsList.Add(prev3dPoints[i]);
                    currAct3dPointsList.Add(curr3dPoints[i]);
                }
            }

            //old reproject
            //var depthSize = prevFrame.DepthMapImg.Size;
            //var reprojPrevDepthMap = new Image<Gray, short>(prevFrame.DepthMapImg.Size);
            //var reprojCurrDepthMap = new Image<Gray, short>(currFrame.DepthMapImg.Size);

            //for (int i = 0; i < depthSize.Height; ++i)
            //{
            //    for (int j = 0; j < depthSize.Width; ++j)
            //    {
            //        reprojPrevDepthMap.Data[i, j, 0] = 0;
            //        //reprojPrevDepthMap.Data[i, j, 1] = 0;
            //        //reprojPrevDepthMap.Data[i, j, 2] = 0;

            //        reprojCurrDepthMap.Data[i, j, 0] = 0;
            //        //reprojCurrDepthMap.Data[i, j, 1] = 0;
            //        //reprojCurrDepthMap.Data[i, j, 2] = 0;
            //    }
            //}

            //var actPrevFreatures = new List<PointF>();
            //var actCurrFreatures = new List<PointF>();

            //for (int i = 0; i < prevFeatures.Count(); ++i)
            //{
            //    if (status[i] == 1)
            //    {
            //        actPrevFreatures.Add(prevFeatures[i]);
            //        actCurrFreatures.Add(currFeatures[i]);

            //        int xp = (int)prevFeatures[i].X;
            //        int yp = (int)prevFeatures[i].Y;

            //        int xc = (int)currFeatures[i].X;
            //        int yc = (int)currFeatures[i].Y;
                    
            //        if (yp < reprojPrevDepthMap.Height && yp >=0 && xp < reprojPrevDepthMap.Width && xp >= 0)
            //        {
            //            if (yc < reprojCurrDepthMap.Height && yc >= 0 && xc < reprojCurrDepthMap.Width && xc >= 0)
            //            {
            //                reprojPrevDepthMap.Data[yp, xp, 0] = prevFrame.DepthMapImg.Data[yp, xp, 0];
            //                reprojCurrDepthMap.Data[yc, xc, 0] = currFrame.DepthMapImg.Data[yc, xc, 0];
            //            }
            //        }
            //        //reprojPrevDepthMap.Data[yp, xp, 1] = prevFrame.DepthMapImg.Data[yp, xp, 1];
            //        //reprojPrevDepthMap.Data[yp, xp, 2] = prevFrame.DepthMapImg.Data[yp, xp, 2];

            //        //reprojCurrDepthMap.Data[yc, xc, 1] = currFrame.DepthMapImg.Data[yc, xc, 1];
            //        //reprojCurrDepthMap.Data[yc, xc, 2] = currFrame.DepthMapImg.Data[yc, xc, 2];
            //    }
            //}

            //var prevPoints = PointCollection.ReprojectImageTo3D(reprojPrevDepthMap, cameraParams.Q);
            //var currPoints = PointCollection.ReprojectImageTo3D(reprojCurrDepthMap, cameraParams.Q);

            //var maxZ = prevPoints.Max(x => x.z);
            //var actPrevPoints = prevPoints.Where(x => x.z != maxZ).ToArray();
            //var actCurrPoints = currPoints.Where(x => x.z != maxZ).ToArray();
            //// old reproject end
            var actPrevPoints = prevAct3dPointsList.ToArray();
            var actCurrPoints = currAct3dPointsList.ToArray();
            var prevCentroid = GetCentroid(actPrevPoints);
            var currCentroid = GetCentroid(actCurrPoints);

            var rotMatrix = Utils.CvHelper.ArrayToMatrix(rotMatrArray, new Size(3, 3));
            Matrix<double> objRotMatrix;
            Matrix<double> resCamRotMatrix;
            //rotation with IMU
            //var objRotMatrix = Utils.CvHelper.InverseMatrix(rotMatrix);
            
            //rotation with SVD
            
            var detX = GetSVDRotation(actPrevPoints, actCurrPoints, out objRotMatrix);
            if (detX < 0)
            {
                objRotMatrix.SetIdentity();
                Console.WriteLine("detX = {0}", detX);
            }
            resCamRotMatrix = Utils.CvHelper.InverseMatrix(objRotMatrix);


            var rotPrevCentr = objRotMatrix.Mul(prevCentroid);
            
            //raw tranalstion
            var rawT = new Matrix<double>(rotPrevCentr.Size);
            rawT[0, 0] = currCentroid[0, 0] - rotPrevCentr[0, 0];
            rawT[1, 0] = currCentroid[1, 0] - rotPrevCentr[1, 0];
            rawT[2, 0] = currCentroid[2, 0] - rotPrevCentr[2, 0];
            
            //camera translation
            var camT = resCamRotMatrix.Mul(rawT);
            //var camT = rawT;

            var X = camT[0, 0];
            var Y = camT[1, 0];
            var Z = camT[2, 0];

            prevFeaturesList = prevCorrFeatures.ToList();
            currFeaturesList = currCorrFeatures.ToList();
            resRotation = resCamRotMatrix;
            return new MCvPoint3D64f(X, Y, Z);
        }

        protected static double GetSVDRotation(Matrix<double>[] p1, Matrix<double>[] p2, out Matrix<double> rotation)
        {
            var centr1 = GetCentroid(p1);
            var centr2 = GetCentroid(p2);

            var q1 = new List<Matrix<double>>();
            var q2 = new List<Matrix<double>>();
            var H = new Matrix<double>(3, 3);

            for (int i = 0; i < Math.Min(p1.Count(), p2.Count()); ++i)
            {
                var q1d = new Matrix<double>(new double[,]
                {
                    {p1[i][0, 0] - centr1[0, 0]},
                    {p1[i][1, 0] - centr1[1, 0]},
                    {p1[i][2, 0] - centr1[2, 0]}
                });

                q1.Add(q1d);

                var q2d = new Matrix<double>(new double[,]
                {
                    {p2[i][0, 0] - centr2[0, 0]},
                    {p2[i][1, 0] - centr2[1, 0]},
                    {p2[i][2, 0] - centr2[2, 0]}
                });

                q2.Add(q2d);

                H = H.Add(q1d.Mul(q2d.Transpose()));
            }

            var U = new Matrix<double>(3, 3);
            var W = new Matrix<double>(3, 3);
            var V = new Matrix<double>(3, 3);

            CvInvoke.cvSVD(H, W, U, V, SVD_TYPE.CV_SVD_DEFAULT);

            var X = V.Mul(U.Transpose());

            var detX = CvInvoke.cvDet(X);

            rotation = X;
            return detX;
        }

        protected static Matrix<double> GetCentroid(Matrix<double>[] points)
        {
            var pointsMCv = new MCvPoint3D32f[points.Count()];
            for (int i = 0; i < points.Count(); ++i)
            {
                pointsMCv[i] = new MCvPoint3D32f((float)points[i][0, 0], (float)points[i][1, 0], (float)points[i][2, 0]);
            }

            return GetCentroid(pointsMCv);
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

        public static PointF[] GetFeaturesToTrack(Image<Gray, byte> img)
        {
            int MaxFeaturesCount = 400;
            double QualityLevel = 0.01;
            double MinDistance = 1;
            int BlockSize = 10;
            var goodFeatures = img.GoodFeaturesToTrack(MaxFeaturesCount, QualityLevel, MinDistance, BlockSize);
            return goodFeatures[0];
        }

        public static void GetCorrespFeatures(PointF[] origF, Image<Gray, byte> img1, Image<Gray, byte> img2, out PointF[] prevCorrFeatures, out PointF[] currCorrFeatures)
        {
            Size WinSize = new Size(80, 80);
            int PyrLevel = 4;
            MCvTermCriteria PyrLkTerm = new MCvTermCriteria(100, 0.001);

            var status = new Byte[origF.Count()];
            var error = new float[origF.Count()];
            var currFeatures = new PointF[origF.Count()];

            OpticalFlow.PyrLK(
                img1,
                img2,
                origF,
                WinSize,
                PyrLevel,
                PyrLkTerm,
                out currFeatures,
                out status,
                out error);

            List<PointF> pAct = new List<PointF>();
            List<PointF> cAct = new List<PointF>();

            for (int i = 0; i < origF.Count(); ++i)
            {
                if (status[i] == 1)
                {
                    pAct.Add(origF[i]);
                    cAct.Add(currFeatures[i]);
                }
            }

            prevCorrFeatures = pAct.ToArray();
            currCorrFeatures = cAct.ToArray();
        }

        public static Matrix<double>[] ReprojectTo3d(Image<Gray, byte> leftImg, Image<Gray, byte> rightImg, PointF[] points, Image<Gray, short> precalcDepthMap, Matrix<double> Q)
        {
            var res = new Matrix<double>[points.Count()];
            var disps = GetDisparities(leftImg, rightImg, points, precalcDepthMap);
            for (int i = 0; i < points.Count(); ++i)
            {
                if (points[i].X >= leftImg.Width || points[i].X < 0 || points[i].Y >= leftImg.Height || points[i].Y < 0)
                {
                    res[i] = null;
                    continue;
                }

                //
                if (disps[i] <= 0)
                {
                    res[i] = null;
                    continue;
                }

                var p = new Matrix<double>(new double[,] 
                {
                    {points[i].X},
                    {points[i].Y},
                    {disps[i]},
                    {1}
                });

                var ph3d = Q.Mul(p);
                if (ph3d[3, 0] == 0)
                {
                    res[i] = null;
                    continue;
                }

                res[i] = new Matrix<double>(new double[,]
                    {
                        {ph3d[0, 0] / ph3d[3, 0]},
                        {ph3d[1, 0] / ph3d[3, 0]},
                        {ph3d[2, 0] / ph3d[3, 0]}
                    }); 
            }
            return res;
        }

        public static double[] GetDisparities(Image<Gray, byte> leftImg, Image<Gray, byte> rightImg, PointF[] points, Image<Gray, short> precalcDepthMap)
        {
            var res = new double[points.Count()];

            for (int i = 0; i < points.Count(); ++i)
            {
                if (points[i].X >= leftImg.Width || points[i].X < 0 || points[i].Y >= leftImg.Height || points[i].Y < 0)
                {
                    res[i] = -1;
                    continue;
                }

                res[i] = precalcDepthMap[(int)points[i].Y, (int)points[i].X].Intensity;
            }

            return res;
        }
    }
    
}
