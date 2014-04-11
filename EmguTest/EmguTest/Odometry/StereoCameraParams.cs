using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
