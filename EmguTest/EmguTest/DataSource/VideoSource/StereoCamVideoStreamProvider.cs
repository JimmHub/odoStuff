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
            this.LeftCapture.SetCaptureProperty(Emgu.CV.CvEnum.CAP_PROP.CV_CAP_PROP_FRAME_WIDTH, 1280);
            this.LeftCapture.SetCaptureProperty(Emgu.CV.CvEnum.CAP_PROP.CV_CAP_PROP_FRAME_HEIGHT, 720);
            
            this.RightCapture = new Capture(rightCapId);
            this.RightCapture.SetCaptureProperty(Emgu.CV.CvEnum.CAP_PROP.CV_CAP_PROP_FRAME_WIDTH, 1280);
            this.RightCapture.SetCaptureProperty(Emgu.CV.CvEnum.CAP_PROP.CV_CAP_PROP_FRAME_HEIGHT, 720);
        }

        override public event NewStereoFrameEventHandler NewStereoFrameEvent;
        public Capture LeftCapture { get; set; }
        public Capture RightCapture { get; set; }
        protected StereoFrameSequenceElement CurrentFrame;
        protected Thread _mainThread;
        protected bool _isStarted;
        protected object _currentFrameLock = new object();
        //interface
        override public StereoFrameSequenceElement GetCurrentFrame()
        {
            //TODO: ditry hack
            try
            {
                lock (_currentFrameLock)
                {
                    return new StereoFrameSequenceElement()
                    {
                        LeftRawFrame = Image.FromHbitmap(this.CurrentFrame.LeftRawFrame.GetHbitmap()),
                        RightRawFrame = Image.FromHbitmap(this.CurrentFrame.RightRawFrame.GetHbitmap()),
                        TimeStamp = this.CurrentFrame.TimeStamp
                    };
                }
            }
            catch 
            {
                return new StereoFrameSequenceElement()
                    {
                         IsLeftFrameEmpty = true,
                         IsRightFrameEmpty = true
                    };
            }
        }

        override public StereoFrameSequenceElement GetNextFrame()
        {
            lock (_currentFrameLock)
            {
                var left = this.LeftCapture.QueryFrame();
                var right = this.RightCapture.QueryFrame();
                this.CurrentFrame = new StereoFrameSequenceElement()
                {
                    LeftRawFrame = new Bitmap(left.ToBitmap()),//.Clone(),
                    RightRawFrame = new Bitmap(right.ToBitmap()),
                    TimeStamp = DateTime.UtcNow
                };
                //left.Dispose();
                //right.Dispose();
                return this.CurrentFrame;
            }
        }

        override public bool StartStream()
        {
            this._mainThread = new Thread(this.MainThreadRoutine);
            this._mainThread.Start();
            this._isStarted = true;
            return true;
        }

        protected void MainThreadRoutine()
        {
            while (this._isStarted)
            {
                var nextFrame = this.GetNextFrame();
                if (this.NewStereoFrameEvent != null)
                {
                    this.NewStereoFrameEvent(this, new NewStereoFrameEventArgs()
                        {
                            NewStereoFrame = nextFrame
                        });
                }
            }
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
