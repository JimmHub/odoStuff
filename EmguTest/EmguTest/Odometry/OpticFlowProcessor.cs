using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using EmguTest.VideoSource;

using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Features2D;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Emgu.CV.GPU;

namespace EmguTest.Odometry
{
    class OpticFlowProcessor
    {
        public Bitmap GetFeaturesToTrack(StereoFrameSequenceElement stereoFrame, bool useGpu)
        {
            if (useGpu)
            {
                return this.GetFraturesToTrackGPU(stereoFrame).ToBitmap();
            }
            else
            {
                return this.GetFeaturesToTrackCPU(stereoFrame);
            }
        }

        private Bitmap GetFeaturesToTrackCPU(StereoFrameSequenceElement stereoFrame)
        {
            throw new NotImplementedException();
        }

        protected Image<Bgr, byte> GetFraturesToTrackGPU(StereoFrameSequenceElement stereoFrame)
        {

            var modelImage = new Image<Gray, byte>(stereoFrame.LeftRawFrame);
            var observedImage = new Image<Gray, byte>(stereoFrame.LeftRawFrame);
            
            HomographyMatrix homography = null;

            SURFDetector surfCPU = new SURFDetector(500, false);
            VectorOfKeyPoint modelKeyPoints;
            VectorOfKeyPoint observedKeyPoints;
            Matrix<int> indices;

            Matrix<byte> mask;
            int k = 2;
            double uniquenessThreshold = 0.8;

            GpuSURFDetector surfGPU = new GpuSURFDetector(surfCPU.SURFParams, 0.01f);
            using (GpuImage<Gray, Byte> gpuModelImage = new GpuImage<Gray, byte>(modelImage))
            //extract features from the object image
            using (GpuMat<float> gpuModelKeyPoints = surfGPU.DetectKeyPointsRaw(gpuModelImage, null))
            using (GpuMat<float> gpuModelDescriptors = surfGPU.ComputeDescriptorsRaw(gpuModelImage, null, gpuModelKeyPoints))
            using (GpuBruteForceMatcher<float> matcher = new GpuBruteForceMatcher<float>(DistanceType.L2))
            {
                modelKeyPoints = new VectorOfKeyPoint();
                surfGPU.DownloadKeypoints(gpuModelKeyPoints, modelKeyPoints);

                // extract features from the observed image
                using (GpuImage<Gray, Byte> gpuObservedImage = new GpuImage<Gray, byte>(observedImage))
                using (GpuMat<float> gpuObservedKeyPoints = surfGPU.DetectKeyPointsRaw(gpuObservedImage, null))
                using (GpuMat<float> gpuObservedDescriptors = surfGPU.ComputeDescriptorsRaw(gpuObservedImage, null, gpuObservedKeyPoints))
                using (GpuMat<int> gpuMatchIndices = new GpuMat<int>(gpuObservedDescriptors.Size.Height, k, 1, true))
                using (GpuMat<float> gpuMatchDist = new GpuMat<float>(gpuObservedDescriptors.Size.Height, k, 1, true))
                using (GpuMat<Byte> gpuMask = new GpuMat<byte>(gpuMatchIndices.Size.Height, 1, 1))
                using (Stream stream = new Stream())
                {
                    matcher.KnnMatchSingle(gpuObservedDescriptors, gpuModelDescriptors, gpuMatchIndices, gpuMatchDist, k, null, stream);
                    indices = new Matrix<int>(gpuMatchIndices.Size);
                    mask = new Matrix<byte>(gpuMask.Size);

                    //gpu implementation of voteForUniquess
                    using (GpuMat<float> col0 = gpuMatchDist.Col(0))
                    using (GpuMat<float> col1 = gpuMatchDist.Col(1))
                    {
                        GpuInvoke.Multiply(col1, new MCvScalar(uniquenessThreshold), col1, stream);
                        GpuInvoke.Compare(col0, col1, gpuMask, CMP_TYPE.CV_CMP_LE, stream);
                    }

                    observedKeyPoints = new VectorOfKeyPoint();
                    surfGPU.DownloadKeypoints(gpuObservedKeyPoints, observedKeyPoints);

                    //wait for the stream to complete its tasks
                    //We can perform some other CPU intesive stuffs here while we are waiting for the stream to complete.
                    stream.WaitForCompletion();

                    gpuMask.Download(mask);
                    gpuMatchIndices.Download(indices);

                    if (GpuInvoke.CountNonZero(gpuMask) >= 4)
                    {
                        int nonZeroCount = Features2DToolbox.VoteForSizeAndOrientation(modelKeyPoints, observedKeyPoints, indices, mask, 1.5, 20);
                        if (nonZeroCount >= 4)
                            homography = Features2DToolbox.GetHomographyMatrixFromMatchedFeatures(modelKeyPoints, observedKeyPoints, indices, mask, 2);
                    }
                }
            }



            //Draw the matched keypoints
            Image<Bgr, Byte> result = Features2DToolbox.DrawMatches(modelImage, modelKeyPoints, observedImage, observedKeyPoints,
               indices, new Bgr(255, 255, 255), new Bgr(255, 255, 255), mask, Features2DToolbox.KeypointDrawType.DEFAULT);

            #region draw the projected region on the image
            if (homography != null)
            {  //draw a rectangle along the projected model
                Rectangle rect = modelImage.ROI;
                PointF[] pts = new PointF[] { 
               new PointF(rect.Left, rect.Bottom),
               new PointF(rect.Right, rect.Bottom),
               new PointF(rect.Right, rect.Top),
               new PointF(rect.Left, rect.Top)};
                homography.ProjectPoints(pts);

                result.DrawPolyline(Array.ConvertAll<PointF, Point>(pts, Point.Round), true, new Bgr(Color.Red), 5);
            }
            #endregion

            return result;
        }

        public  Image<Gray, short> GetDispMapGPU(Image<Gray, byte> leftImg, Image<Gray, byte> rightImg)
        {
            using(var leftGpuImg = new GpuImage<Gray, byte>(leftImg))
            using(var rightGpuImg = new GpuImage<Gray, byte>(rightImg))
            using (GpuStereoBM sbm = new GpuStereoBM(256, 19))
            {
                var dispMap = new GpuImage<Gray, byte>(leftGpuImg.Size);
                sbm.FindStereoCorrespondence(leftGpuImg, rightGpuImg, dispMap, null);
                return dispMap.ToImage().Convert<Gray, short>();
            }
        }

        public Image<Gray, short> GetDispMapCPU(Image<Gray, byte> leftImg, Image<Gray, byte> rightImg)
        {
            using (StereoSGBM sgbm = new StereoSGBM(0, 128, 0, 0, 0, 0, 0, 0, 0, 0, StereoSGBM.Mode.HH))
            using (var leftProcessImg = leftImg.Copy())
            using (var rightProcessImg = rightImg.Copy())
            {
                var dispMap = new Image<Gray, short>(leftProcessImg.Size);
                sgbm.FindStereoCorrespondence(leftProcessImg, rightProcessImg, dispMap);
                return dispMap;
            }
        }

        public Image<Gray, short> GetDispMap(Image<Gray, byte> leftImg, Image<Gray, byte> rightImg, bool useGPU)
        {
            if (useGPU)
            {
                return this.GetDispMapGPU(leftImg, rightImg);
            }
            else
            {
                return this.GetDispMapCPU(leftImg, rightImg);
            }
        }

        public Image<Gray, short> GetDispMap(Image<Bgr, byte> leftImg, Image<Bgr, byte> rightImg, bool useGPU)
        {
            return this.GetDispMap(leftImg.Convert<Gray, byte>(), rightImg.Convert<Gray, byte>(), useGPU);
        }
    }
}
