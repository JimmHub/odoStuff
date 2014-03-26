using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

using Emgu.CV;
using Emgu.Util;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using Emgu.CV.GPU;
using Emgu.CV.Features2D;

namespace EmguTest
{
    public class StereoCameraCalibrator
    {
        public StereoCameraCalibrator()
        {
        }

        public bool Calibrate(String imageFolder, Size boardSize)
        {
            List<String> images = Directory.GetFiles(imageFolder).ToList();
            images.Sort();

            return this.Calibrate(images, boardSize);
            
        }

        public bool Calibrate(List<String> imagePathList, Size boardSize)
        {
            if (imagePathList.Count % 2 != 0)
            {
                return false;
            }

            bool displayCorners = true;
            int maxScale = 2;
            float squareSize = 1.0F;
            int nImages = imagePathList.Count / 2;
            Size? imgSize = null;
            var imagePoints = new List<List<PointF>>[2];
            imagePoints[0] = new List<List<PointF>>();
            imagePoints[1] = new List<List<PointF>>();

            List<String> goodImagePathList = new List<string>();

            var objectPoints = new List<List<MCvPoint3D32f>>();
            int i = 0;
            int j = 0;
            int k = 0;
            for (i = j = 0; i < nImages; ++i)
            {
                for (k = 0; k < 2; ++k)
                {
                    String filename = imagePathList[i * 2 + k];
                    var image = new Image<Bgr, byte>(filename);
                    if (image.Data == null)
                    {
                        break;
                    }
                    if (imgSize == null)
                    {
                        imgSize = image.Size;
                    }
                    else if (image.Size != imgSize)
                    {
                        break;
                    }

                    bool found = false;
                    PointF[] corners = null;
                    for (int scale = 1; scale <= maxScale; ++scale)
                    {
                        Image<Bgr, byte> tImg = null;
                        if (scale == 1)
                        {
                            tImg = image;
                        }
                        else
                        {
                            tImg = image.Resize(scale, INTER.CV_INTER_LINEAR);
                        }

                        corners = CameraCalibration.FindChessboardCorners(tImg.Convert<Gray, byte>(), boardSize,
                            CALIB_CB_TYPE.ADAPTIVE_THRESH | CALIB_CB_TYPE.NORMALIZE_IMAGE);
                        found = (corners != null && corners.Count() != 0);
                        if (found)
                        {
                            if (scale > 1)
                            {
                                //TODO ??
                            }
                            break;
                        }
                    }
                    if (displayCorners)
                    {
                        //TODO: display corners
                    }
                    else
                    {
                        //TODO: not display corners
                    }

                    if (!found)
                    {
                        break;
                    }

                    var subCorners = new PointF[1][];
                    subCorners[0] = corners.ToArray();

                    image.Convert<Gray,byte>().FindCornerSubPix(subCorners, new Size(11, 11), new Size(-1, -1), new MCvTermCriteria(30, 0.01));
                    corners = subCorners[0];

                    imagePoints[k].Add(new List<PointF>(corners));
                }
                if (k == 2)
                {
                    goodImagePathList.Add(imagePathList[i * 2]);
                    goodImagePathList.Add(imagePathList[i * 2 + 1]);
                    ++j;
                }
            }

            var goodImgCount = j;

            if (goodImgCount < 2)
            {
                return false;
            }

            objectPoints = new List<List<MCvPoint3D32f>>();

            for (i = 0; i < goodImgCount; ++i)
            {
                objectPoints.Add(new List<MCvPoint3D32f>());
                for (j = 0; j < boardSize.Height; ++j)
                {
                    for (k = 0; k < boardSize.Width; ++k)
                    {
                        objectPoints[i].Add(new MCvPoint3D32f(j * squareSize, k * squareSize, 0));
                    }
                }
            }


            var cameraMatrix = new IntrinsicCameraParameters[2];
            cameraMatrix[0] = new IntrinsicCameraParameters(8);
            cameraMatrix[1] = new IntrinsicCameraParameters(8);

            ExtrinsicCameraParameters extrinsic;
            Matrix<double> foundamental;
            Matrix<double> essential;

            var objectPointsArray = objectPoints.Select(x => x.ToArray()).ToArray();
            var imagePoints0Array = imagePoints[0].Select(x => x.ToArray()).ToArray();
            var imagePoints1Array = imagePoints[1].Select(x => x.ToArray()).ToArray();

            CameraCalibration.StereoCalibrate(objectPointsArray, imagePoints0Array, imagePoints1Array,
                cameraMatrix[0], cameraMatrix[1], imgSize.Value,
                0,//CALIB_TYPE.CV_CALIB_FIX_ASPECT_RATIO | CALIB_TYPE.CV_CALIB_ZERO_TANGENT_DIST | CALIB_TYPE.CV_CALIB_FIX_FOCAL_LENGTH | CALIB_TYPE.CV_CALIB_RATIONAL_MODEL | CALIB_TYPE.CV_CALIB_FIX_K3 | CALIB_TYPE.CV_CALIB_FIX_K4 | CALIB_TYPE.CV_CALIB_FIX_K5,
                new MCvTermCriteria(100, 1e-5),
                out extrinsic, out foundamental, out essential
                );
            //end
            return true;
        }
    }
}
