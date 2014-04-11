using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

using Emgu.CV;

namespace EmguTest.Odometry
{
    class StereoCameraParams
    {
        public IntrinsicCameraParameters LeftIntrinsicCameraParameters { get; set; }
        public IntrinsicCameraParameters RightIntrinsicCameraParameters { get; set; }

        public ExtrinsicCameraParameters ExtrinsicCameraParams { get; set; }
        public Matrix<double> FoundamentalMatrix { get; set; }
        public Matrix<double> EssentialMatrix { get; set; }
        public Matrix<float> LeftMapX { get; set; }
        public Matrix<float> LeftMapY { get; set; }
        public Matrix<float> RightMapX { get; set; }
        public Matrix<float> RightMapY { get; set; }

        public Rectangle Rec1 { get; set; }//Rectangle Calibrated in camera 1
        public Rectangle Rec2 { get; set; } //Rectangle Caliubrated in camera 2
        public Matrix<double> Q { get; set; } //This is what were interested in the disparity-to-depth mapping matrix
        public Matrix<double> R1 { get; set; } //rectification transforms (rotation matrices) for Camera 1.
        public Matrix<double> R2 { get; set; } //rectification transforms (rotation matrices) for Camera 1.
        public Matrix<double> P1 { get; set; } //projection matrices in the new (rectified) coordinate systems for Camera 1.
        public Matrix<double> P2 { get; set; } //projection matrices in the new (rectified) coordinate systems for Camera 2.
    }
}
