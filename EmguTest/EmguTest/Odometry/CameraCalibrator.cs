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
            throw new NotImplementedException();
        }

        public static MonoCameraParams CalibrateMono(MonoCameraCalibrationData calibData)
        {
            var imgCount = calibData.SampleImagesNames.Count;
            var imagePoints = new List<List<PointF>>();
            Size? imgSize = null;

            for (int i = 0; i < imgCount; ++i)
            {
                String fileName = calibData.SampleImagesNames[i];
                var image = new Image<Bgr, byte>(fileName);
                if(imgSize == null)
                {
                    imgSize = image.Size;
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

                var subCorners = new PointF[1][];
                subCorners[0] = corners.ToArray();
                image.Convert<Gray, byte>().FindCornerSubPix(
                    subCorners,
                    new Size(11, 11),
                    new Size(-1, -1),
                    new MCvTermCriteria(30, 0.01));

                corners = subCorners[0];

                imagePoints.Add(new List<PointF>(corners));
                //
            }

            var objectPoints = new List<List<MCvPoint3D32f>>();

            for (int i = 0; i < imgCount; ++i)
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
}
