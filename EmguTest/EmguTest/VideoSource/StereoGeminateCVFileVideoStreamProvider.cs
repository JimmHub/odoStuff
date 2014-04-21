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
    public class StereoGeminateCVFileVideoStreamProvider : StereoVideoStreamProvider
    {
        public StereoGeminateCVFileVideoStreamProvider(Capture cap)
        {
            this.Init();
            this.Capture = cap;
            this.FrameInterval = 1.0 / 30 * 1000;   
        }

        public StereoGeminateCVFileVideoStreamProvider(String fileName)
        {
            this.Init();
            this.Capture = new Capture(fileName);
            this.FrameInterval = 1.0 / 30 * 1000;
        }

        public StereoGeminateCVFileVideoStreamProvider(String fileName, double frameInterval)
        {
            this.Init();
            this.Capture = new Capture(fileName);
            this.FrameInterval = frameInterval;
        }

        protected void Init()
        {
            this.emptyFrame = new StereoFrameSequenceElement()
            {
                IsLeftFrameEmpty = true,
                IsRightFrameEmpty = true
            };
            this.CurrentFrame = new StereoFrameSequenceElement()
            {
                IsLeftFrameEmpty = true,
                IsRightFrameEmpty = true
            };
        }

        override public event NewStereoFrameEventHandler NewStereoFrameEvent;
        public Capture Capture { get; set; }
       
        public int TotalFrames { get; set; }
        public double FrameInterval { get; set; }
        public StereoFrameSequenceElement CurrentFrame { get; protected set; }

        protected bool IsStarted = false;
        protected bool IsPaused = false;
        protected StereoFrameSequenceElement emptyFrame;
        override public StereoFrameSequenceElement GetNextFrame()
        {
            if (!this.CurrentFrame.IsNotFullFrame)
            {
                var passedTime = DateTime.UtcNow.Subtract(this.CurrentFrame.TimeStamp).TotalMilliseconds;
                if (passedTime < this.FrameInterval)
                {
                    return this.emptyFrame;
                }
            }
            var rawFrame = this.Capture.QueryFrame();
            StereoFrameSequenceElement frame = this.ElementFromRawFrame(rawFrame);
            rawFrame.Dispose();
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
                    RawFrame = Utils.CvHelper.ConvertImageToBitmap(rawFrame),
                    LeftRawFrame = Utils.CvHelper.ConvertImageToBitmap(left),
                    RightRawFrame = Utils.CvHelper.ConvertImageToBitmap(right),
                    TimeStamp = DateTime.UtcNow
                };
            rawFrame.Dispose();
            left.Dispose();
            right.Dispose();
            return res;
        }

        override public StereoFrameSequenceElement GetCurrentFrame()
        {
            return this.CurrentFrame;
        }

        override public bool StartStream()
        {
            this.IsStarted = true;
            return true;
        }

        override public bool PauseStream()
        {
            if (this.IsStarted)
            {
                this.Capture.Pause();
                this.IsPaused = true;
            }
            return true;
        }

        override public bool StopStream()
        {
            this.IsStarted = false;
            return true;
        }


        override public bool IsFunctioning()
        {
            if (
                this.CurrentFrame != null &&
                this.IsStarted
                )//&& !this.CurrentFrame.IsNotFullFrame)
            {
                return true;
            }
            return false;
        }


        override public bool CanChangeLeftCap()
        {
            return false;
        }

        override public bool CanChangeRightCap()
        {
            return false;
        }


        override public void ChangeLeftCap(int leftCapId)
        {
            throw new NotImplementedException();
        }

        override public void ChangeRightCap(int rightCapId)
        {
            throw new NotImplementedException();
        }


        override public bool CanRewindStream()
        {
            return true;
        }

        override public bool ResumeStream()
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
