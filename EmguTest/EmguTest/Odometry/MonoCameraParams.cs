using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Emgu.CV;

namespace EmguTest.Odometry
{
    class MonoCameraParams
    {
        public IntrinsicCameraParameters IntrinsicCameraParameters { get; set; }
        public ExtrinsicCameraParameters[] ExtrinsicCameraParameters { get; set; }
    }
}
