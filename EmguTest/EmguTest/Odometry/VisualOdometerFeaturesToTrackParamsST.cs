using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmguTest.Odometry
{
    class VisualOdometerFeaturesToTrackParamsST : VisualOdometerFeaturesToTrackParams
    {
        public int MaxFeaturesCount { get; set; }
        public double QualityLevel { get; set; }
        public double MinDistance { get; set; }
        public int BlockSize { get; set; }
    }
}
