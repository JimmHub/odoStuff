using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Emgu.CV.Structure;

namespace EmguTest.Odometry
{
    class VisualOdometerFeaturesOpticFlowParamsLK : VisualOdometerFeaturesOpticFlowParams
    {
        public Size WinSize { get; set; }
        public int PyrLevel { get; set; }
        public MCvTermCriteria PyrLkTerm { get; set; }
    }
}
