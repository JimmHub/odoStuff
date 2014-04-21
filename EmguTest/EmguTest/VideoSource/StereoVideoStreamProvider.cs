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

namespace EmguTest.VideoSource
{
    public abstract class StereoVideoStreamProvider
    {
        public abstract StereoFrameSequenceElement GetCurrentFrame();
        public abstract StereoFrameSequenceElement GetNextFrame();
        public abstract bool IsFunctioning();

        public abstract bool CanChangeLeftCap();
        public abstract bool CanChangeRightCap();
        public abstract bool CanRewindStream();

        public abstract void ChangeLeftCap(int leftCapId);
        public abstract void ChangeRightCap(int rightCapId);

        public abstract bool StartStream();
        public abstract bool PauseStream();
        public abstract bool ResumeStream();
        public abstract bool StopStream();
    }
}
