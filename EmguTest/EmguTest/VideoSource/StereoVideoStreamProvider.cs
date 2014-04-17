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
    public interface StereoVideoStreamProvider
    {
        StereoFrameSequenceElement GetCurrentFrame();
        StereoFrameSequenceElement GetNextFrame();
        bool IsFunctioning();

        bool CanChangeLeftCap();
        bool CanChangeRightCap();
        
        void ChangeLeftCap(int leftCapId);
        void ChangeRightCap(int rightCapId);
        
        bool StartStream();
        bool PauseStream();
        bool StopStream();
    }
}
