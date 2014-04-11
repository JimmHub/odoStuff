using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

using Emgu.CV;
using Emgu.Util;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;

namespace EmguTest.Odometry
{
    class CameraCalibrator
    {
        public static StereoCameraParams CalibrateStereo(StereoCameraCalibrationData calibData)
        {
            var imgCount = calibData.SampleImagesNames.Count;
            
            var leftImagePoints = new List<List<PointF>>();
            var rightImagePoints = new List<List<PointF>>();
            
            int foundImagesCount = 0;
            Size? imgSize = null;

            for (int i = 0; i < imgCount; ++i)
            {
                //left
                var leftFileName = calibData.SampleImagesNames[i].Item1;
                var leftImage = new Image<Bgr, byte>(leftFileName);

                var leftRes = FindCorners(leftImage, calibData, ref imgSize);
                if (!leftRes.Found)
                {
                    continue;
                }
                ////
                //right
                var rightFileName = calibData.SampleImagesNames[i].Item2;
                var rightImage = new Image<Bgr, byte>(rightFileName);

                var rightRes = FindCorners(rightImage, calibData, ref imgSize);
                if (!rightRes.Found)
                {
                    continue;
                }
                ////
                if (leftRes.Found && rightRes.Found)
                {
                    leftImagePoints.Add(new List<PointF>(leftRes.Corners));
                    rightImagePoints.Add(new List<PointF>(rightRes.Corners));
                    ++foundImagesCount;
                }
            }

            var objectPoints = new List<List<MCvPoint3D32f>>();

            for (int i = 0; i < foundImagesCount; ++i)
            {
                objectPoints.Add(new List<MCvPoint3D32f>());
                for (int j = 0; j < calibData.BoardSquareSize.Height; ++j)
                {
                    for (int k = 0; k < calibData.BoardSquareSize.Width; ++k)
                    {
                        objectPoints[i].Add(new MCvPoint3D32f((float)(j * calibData.SquareSize), (float)(k * calibData.SquareSize), 0));
                    }
                }
            }

            var objectPointsArray = objectPoints.Select(x => x.ToArray()).ToArray();
            
            var leftImagePointsArray = leftImagePoints.Select(x => x.ToArray()).ToArray();
            var rightImagePointsArray = rightImagePoints.Select(x => x.ToArray()).ToArray();

            IntrinsicCameraParameters leftIntrinsicCameraParameters = new IntrinsicCameraParameters(8);
            IntrinsicCameraParameters rightIntrinsicCameraParameters = new IntrinsicCameraParameters(8);

            ExtrinsicCameraParameters extrinsic;
            Matrix<double> foundamental;
            Matrix<double> essential;

            CameraCalibration.StereoCalibrate(
                objectPoints: objectPointsArray,
                imagePoints1: leftImagePointsArray,
                imagePoints2: rightImagePointsArray,
                intrinsicParam1: leftIntrinsicCameraParameters,
                intrinsicParam2: rightIntrinsicCameraParameters,
                imageSize: imgSize.Value,
                flags: CALIB_TYPE.DEFAULT,
                termCrit: new MCvTermCriteria(100, 1e-5),
                extrinsicParams: out extrinsic,
                foundamentalMatrix: out foundamental,
                essentialMatrix: out essential
                );

            //rectification
            Rectangle Rec1 = new Rectangle(); //Rectangle Calibrated in camera 1
            Rectangle Rec2 = new Rectangle(); //Rectangle Caliubrated in camera 2
            Matrix<double> Q = new Matrix<double>(4, 4); //This is what were interested in the disparity-to-depth mapping matrix
            Matrix<double> R1 = new Matrix<double>(3, 3); //rectification transforms (rotation matrices) for Camera 1.
            Matrix<double> R2 = new Matrix<double>(3, 3); //rectification transforms (rotation matrices) for Camera 1.
            Matrix<double> P1 = new Matrix<double>(3, 4); //projection matrices in the new (rectified) coordinate systems for Camera 1.
            Matrix<double> P2 = new Matrix<double>(3, 4); //projection matrices in the new (rectified) coordinate systems for Camera 2.

            CvInvoke.cvStereoRectify(
                cameraMatrix1: leftIntrinsicCameraParameters.IntrinsicMatrix,
                cameraMatrix2: rightIntrinsicCameraParameters.IntrinsicMatrix,
                distCoeffs1: leftIntrinsicCameraParameters.DistortionCoeffs,
                distCoeffs2: rightIntrinsicCameraParameters.DistortionCoeffs,
                imageSize: imgSize.Value,
                R: extrinsic.RotationVector.RotationMatrix,
                T: extrinsic.TranslationVector,
                R1: R1,
                R2: R2,
                P1: P1,
                P2: P2,
                Q: Q,
                flags: STEREO_RECTIFY_TYPE.DEFAULT,
                alpha: -1,
                newImageSize: Size.Empty,
                validPixROI1: ref Rec1,
                validPixROI2: ref Rec2
                );

            var leftMapX = new Matrix<float>(imgSize.Value.Height, imgSize.Value.Width);
            var rightMapX = new Matrix<float>(imgSize.Value.Height, imgSize.Value.Width);
            var leftMapY = new Matrix<float>(imgSize.Value.Height, imgSize.Value.Width);
            var rightMapY = new Matrix<float>(imgSize.Value.Height, imgSize.Value.Width);
            
            CvInvoke.cvInitUndistortRectifyMap(
                cameraMatrix: leftIntrinsicCameraParameters.IntrinsicMatrix,
                distCoeffs: leftIntrinsicCameraParameters.DistortionCoeffs,
                R: R1,
                newCameraMatrix: P1,
                mapx: leftMapX,
                mapy: leftMapY
                );

            CvInvoke.cvInitUndistortRectifyMap(
                cameraMatrix: rightIntrinsicCameraParameters.IntrinsicMatrix,
                distCoeffs: rightIntrinsicCameraParameters.DistortionCoeffs,
                R: R2,
                newCameraMatrix: P2,
                mapx: rightMapX,
                mapy: rightMapY
                );

            ////
            return new StereoCameraParams()
            {
                EssentialMatrix = essential,
                ExtrinsicCameraParams = extrinsic,
                FoundamentalMatrix = foundamental,
                LeftIntrinsicCameraParameters = leftIntrinsicCameraParameters,
                RightIntrinsicCameraParameters = rightIntrinsicCameraParameters,
                Rec1 = Rec1,
                Rec2 = Rec2,
                Q = Q,
                R1 = R1,
                R2 = R2,
                P1 = P1,
                P2 = P2,
                LeftMapX = leftMapX, 
                LeftMapY = leftMapY,
                RightMapX = rightMapX,
                RightMapY = rightMapY
            };
        }

        protected static FindCornersCalibrationResult FindCorners(Image<Bgr, byte> image, StereoCameraCalibrationData calibData, ref Size? imgSize)
        {
            if (imgSize == null)
            {
                imgSize = image.Size;
            }
            else if (!image.Size.Equals(imgSize.Value))
            {
                return new FindCornersCalibrationResult()
                    {
                        Found = false
                    };
            }
            PointF[] corners = null;

            var tImg = image;
            corners = CameraCalibration.FindChessboardCorners(
                tImg.Convert<Gray, byte>(),
                calibData.BoardSquareSize,
                CALIB_CB_TYPE.ADAPTIVE_THRESH | CALIB_CB_TYPE.NORMALIZE_IMAGE
                );

            var leftFound = (corners != null && corners.Count() != 0);
            if (leftFound)
            {
                var subCorners = new PointF[1][];
                subCorners[0] = corners.ToArray();
                image.Convert<Gray, byte>().FindCornerSubPix(
                    subCorners,
                    new Size(11, 11),
                    new Size(-1, -1),
                    new MCvTermCriteria(30, 0.01));

                corners = subCorners[0];
            }
            else
            {
                return new FindCornersCalibrationResult()
                    {
                        Found = false
                    };
            }

            return new FindCornersCalibrationResult()
            {
                Found = true,
                Corners = corners
            };
        }

        public static MonoCameraParams CalibrateMono(MonoCameraCalibrationData calibData)
        {
            var imgCount = calibData.SampleImagesNames.Count;
            var imagePoints = new List<List<PointF>>();
            int foundImagesCount = 0;
            Size? imgSize = null;

            for (int i = 0; i < imgCount; ++i)
            {
                String fileName = calibData.SampleImagesNames[i];
                var image = new Image<Bgr, byte>(fileName);
                if (imgSize == null)
                {
                    imgSize = image.Size;
                }
                else if(!image.Size.Equals(imgSize.Value))
                {
                    //TODO: maybe implement scaling to the right size, who knows...
                    continue;
                }
                PointF[] corners = null;
                //
                Image<Bgr, byte> tImg = image;
                corners = CameraCalibration.FindChessboardCorners(
                    tImg.Convert<Gray, byte>(),
                    calibData.BoardSquareSize,
                    CALIB_CB_TYPE.ADAPTIVE_THRESH | CALIB_CB_TYPE.NORMALIZE_IMAGE
                    );
                //TODO: implement scaling if not found
                bool found = (corners != null && corners.Count() != 0);
                if (found)
                {
                    var subCorners = new PointF[1][];
                    subCorners[0] = corners.ToArray();
                    image.Convert<Gray, byte>().FindCornerSubPix(
                        subCorners,
                        new Size(11, 11),
                        new Size(-1, -1),
                        new MCvTermCriteria(30, 0.01));

                    corners = subCorners[0];

                    imagePoints.Add(new List<PointF>(corners));
                    ++foundImagesCount;
                }
                //
            }

            var objectPoints = new List<List<MCvPoint3D32f>>();

            for (int i = 0; i < foundImagesCount; ++i)
            {
                objectPoints.Add(new List<MCvPoint3D32f>());
                for (int j = 0; j < calibData.BoardSquareSize.Height; ++j)
                {
                    for (int k = 0; k < calibData.BoardSquareSize.Width; ++k)
                    {
                        objectPoints[i].Add(new MCvPoint3D32f((float)(j * calibData.SquareSize), (float)(k * calibData.SquareSize), 0));
                    }
                }
            }

            var objectPointsArray = objectPoints.Select(x => x.ToArray()).ToArray();
            var imagePointsArray = imagePoints.Select(x => x.ToArray()).ToArray();
            var intrinsicCameraParameters = new IntrinsicCameraParameters(8);
            ExtrinsicCameraParameters[] extrinsicParams;
            CameraCalibration.CalibrateCamera(
                objectPointsArray,
                imagePointsArray,
                imgSize.Value,
                intrinsicCameraParameters,
                CALIB_TYPE.DEFAULT,
                new MCvTermCriteria(100, 1e-5),
                out extrinsicParams
                );

            return new MonoCameraParams()
            {
               ExtrinsicCameraParameters = extrinsicParams,
               IntrinsicCameraParameters = intrinsicCameraParameters
            };
        }
    }

    class FindCornersCalibrationResult
    {
        public PointF[] Corners { get; set; }
        public bool Found { get; set; }
    }
}
