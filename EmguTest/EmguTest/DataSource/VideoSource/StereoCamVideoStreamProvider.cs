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
    public class StereoCamVideoStreamProvider : StereoVideoStreamProvider
    {
        public StereoCamVideoStreamProvider(Capture leftCap, Capture rightCap)
        {
            this.LeftCapture = leftCap;
            this.RightCapture = rightCap;
        }

        public StereoCamVideoStreamProvider(int leftCapId, int rightCapId)
        {
            this.LeftCapture = new Capture(leftCapId);
            this.RightCapture = new Capture(rightCapId);
        }

        override public event NewStereoFrameEventHandler NewStereoFrameEvent;
        public Capture LeftCapture { get; set; }
        public Capture RightCapture { get; set; }
        protected StereoFrameSequenceElement CurrentFrame;
        //interface
        override public StereoFrameSequenceElement GetCurrentFrame()
        {
            return this.CurrentFrame;
        }

        override public StereoFrameSequenceElement GetNextFrame()
        {
            var left = this.LeftCapture.QueryFrame();
            var right = this.RightCapture.QueryFrame();
            this.CurrentFrame = new StereoFrameSequenceElement()
            {
                LeftRawFrame = left.ToBitmap(),
                RightRawFrame = right.ToBitmap(),
                TimeStamp = DateTime.UtcNow
            };
            return this.CurrentFrame;
        }

        override public bool StartStream()
        {
            throw new NotImplementedException();
        }

        override public bool PauseStream()
        {
            throw new NotImplementedException();
        }

        override public bool StopStream()
        {
            throw new NotImplementedException();
        }
        ////


        override public bool IsFunctioning()
        {
            if (LeftCapture != null && RightCapture != null)
            {
                return true;
            }
            return false;
        }


        override public bool CanChangeLeftCap()
        {
            return true;
        }

        override public bool CanChangeRightCap()
        {
            return true;
        }


        override public void ChangeLeftCap(int leftCapId)
        {
            this.LeftCapture = new Capture(leftCapId);
        }

        override public void ChangeRightCap(int rightCapId)
        {
            this.RightCapture = new Capture(rightCapId);
        }


        override public bool CanRewindStream()
        {
            return false;
        }

        override public bool ResumeStream()
        {
            return true;
        }
    }
}
