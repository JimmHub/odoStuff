using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmguTest.VideoSource;

using Emgu.CV;
using Emgu.Util;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;

namespace EmguTest.DataSource
{
    class OpticFlowFrameContainer
    {
        public StereoFrameSequenceElement StereoFrame { get; set; }
        public Image<Gray, short> DepthMapImg { get; set; }
    }
}
