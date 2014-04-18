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
    public class StereoGeminateVideoStreamProvider : StereoVideoStreamProvider
    {
        public StereoGeminateVideoStreamProvider(Capture cap)
        {
            this.Capture = cap;
        }

        public Capture Capture { get; set; }
        
        public int TotalFrames { get; set; }

        public StereoFrameSequenceElement CurrentFrame { get; protected set; }

        protected bool IsStarted = false;
        protected bool IsPaused = false;

        public StereoFrameSequenceElement GetNextFrame()
        {
            var rawFrame = this.Capture.QueryFrame();
            StereoFrameSequenceElement frame = this.ElementFromRawFrame(rawFrame);
            this.CurrentFrame = frame;
            return frame;
        }

        protected StereoFrameSequenceElement ElementFromRawFrame(Image<Bgr, byte> rawFrame)
        {
            rawFrame.ROI = new Rectangle(0, 0, rawFrame.Width / 2, rawFrame.Height);

            var left = rawFrame.Copy();

            rawFrame.ROI = Rectangle.Empty;
            rawFrame.ROI = new Rectangle(rawFrame.Width / 2, 0, rawFrame.Width / 2, rawFrame.Height);
            var right = rawFrame.Copy();

            rawFrame.ROI = Rectangle.Empty;

            StereoFrameSequenceElement res = new StereoFrameSequenceElement()
                {
                    RawFrame = rawFrame.ToBitmap(),
                    LeftRawFrame = left.ToBitmap(),
                    RightRawFrame = right.ToBitmap(),
                    TimeStamp = DateTime.UtcNow
                };

            return res;
        }

        public StereoFrameSequenceElement GetCurrentFrame()
        {
            return this.CurrentFrame;
        }

        public bool StartStream()
        {
            this.IsStarted = true;
            return true;
        }

        public bool PauseStream()
        {
            if (this.IsStarted)
            {
                this.Capture.Pause();
                this.IsPaused = true;
            }
            return true;
        }

        public bool StopStream()
        {
            this.IsStarted = false;
            return true;
        }


        public bool IsFunctioning()
        {
            if (
                this.CurrentFrame != null &&
                this.IsStarted &&
                !this.CurrentFrame.IsNotFullFrame)
            {
                return true;
            }
            return false;
        }


        public bool CanChangeLeftCap()
        {
            return false;
        }

        public bool CanChangeRightCap()
        {
            return false;
        }


        public void ChangeLeftCap(int leftCapId)
        {
            throw new NotImplementedException();
        }

        public void ChangeRightCap(int rightCapId)
        {
            throw new NotImplementedException();
        }


        public bool CanRewindStream()
        {
            return true;
        }

        public bool ResumeStream()
        {
            if (this.IsStarted)
            {
                this.Capture.Start();
                this.IsPaused = false;
            }
            return true;
        }
    }
}
