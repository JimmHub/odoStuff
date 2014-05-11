using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Emgu.CV;

namespace EmguTest.Odometry
{
    public class StereoSGBMDispMapFounderParameters : DispMapFounderParameters
    {
        public int MinDisparity { get; set; }
        public int NumDisparities { get; set; }
        public int BlockSize { get; set; }
        public int P1 { get; set; }
        public int P2 { get; set; }
        public int Disp12MaxDiff { get; set; }
        public int PreFilterCap { get; set; }
        public int UniquenessRatio { get; set; }
        public int SpeckleWindowSize { get; set; }
        public int SpeckleRange { get; set; }
        public StereoSGBM.Mode Mode { get; set; }
    }
}
