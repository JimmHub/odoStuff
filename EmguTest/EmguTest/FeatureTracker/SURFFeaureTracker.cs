using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

using Emgu.CV;
using Emgu.Util;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using Emgu.CV.GPU;
using Emgu.CV.Features2D;

namespace EmguTest
{
    public class SURFFeaureTracker : FeatureTracker
    {
        public SURFFeaureTracker(Capture cap)
            : base (cap)
        {
        }

        protected override void MainProcessing()
        {

        }
    }
}
