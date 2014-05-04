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

using AForge.Video;
using AForge.Video.DirectShow;
namespace EmguTest.VideoSource
{
    public class StereoAForgeCamVideoStreamProvider : StereoVideoStreamProvider
    {
        public StereoAForgeCamVideoStreamProvider(int leftCapId, int rightCapId)
        {
            var videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            var leftCaptureDevice = new VideoCaptureDevice(videoDevices[leftCapId].MonikerString);
            var rightCaptureDevice = new VideoCaptureDevice(videoDevices[rightCapId].MonikerString);
            this.LeftCapture = new AsyncVideoSource(leftCaptureDevice);
            this.RightCapture = new AsyncVideoSource(rightCaptureDevice);

            this.Init();
            this.LeftCapture.NewFrame += NewLeftFrame;
            this.RightCapture.NewFrame += NewRightFrame;
        }

        override public event NewStereoFrameEventHandler NewStereoFrameEvent;
        public AsyncVideoSource LeftCapture { get; set; }
        public AsyncVideoSource RightCapture { get; set; }
        protected StereoFrameSequenceElement CurrentFrame;
        protected bool IsStarted = false;
        protected object _currentFrameLock = new object();
        protected bool _isLeftNew;
        protected bool _isRightNew;
        //interface

        protected void Init()
        {
            this._isLeftNew = false;
            this._isRightNew = false;

            this.CurrentFrame = new StereoFrameSequenceElement()
            {
                IsLeftFrameEmpty = true,
                IsRightFrameEmpty = true
            };
            
        }

        override public StereoFrameSequenceElement GetCurrentFrame()
        {
            return this.CurrentFrame;
        }

        protected void TryFireNewStereoFrameEvent()
        {
            if (this._isRightNew && this._isLeftNew)
            {
                if (!this.CurrentFrame.IsNotFullFrame)
                {
                    if (this.NewStereoFrameEvent != null)
                    {
                        lock (this._currentFrameLock)
                        {
                            this.NewStereoFrameEvent(this, new NewStereoFrameEventArgs()
                            {
                                NewStereoFrame = new StereoFrameSequenceElement(this.CurrentFrame)
                            });
                            this._isLeftNew = false;
                            this._isRightNew = false;
                        }
                    }
                }
            }
        }

        private void NewLeftFrame(object obj, NewFrameEventArgs e)
        {
            Image temp = this.CurrentFrame.LeftRawFrame;
            Bitmap bitmap = e.Frame;
            lock (this._currentFrameLock)
            {
                this.CurrentFrame.LeftRawFrame = new Bitmap(bitmap);
                this.CurrentFrame.TimeStamp = DateTime.UtcNow;
                this.CurrentFrame.IsLeftFrameEmpty = false;
                this._isLeftNew = true;
            }
            this.TryFireNewStereoFrameEvent();
        }

        private void NewRightFrame(object obj, NewFrameEventArgs e)
        {
            Image temp = this.CurrentFrame.RightRawFrame;
            Bitmap bitmap = e.Frame;
            lock (_currentFrameLock)
            {
                this.CurrentFrame.RightRawFrame = new Bitmap(bitmap);
                this.CurrentFrame.TimeStamp = DateTime.UtcNow;
                this.CurrentFrame.IsRightFrameEmpty = false;
                this._isRightNew = true;
            }
            this.TryFireNewStereoFrameEvent();
        }

        override public StereoFrameSequenceElement GetNextFrame()
        {
            return this.CurrentFrame;
        }

        override public bool StartStream()
        {
            this.LeftCapture.Start();
            this.RightCapture.Start();
            this.IsStarted = true;
            return true;
        }

        override public bool PauseStream()
        {
            return true;
        }

        override public bool StopStream()
        {
            throw new NotImplementedException();
        }
        ////


        override public bool IsFunctioning()
        {
            if (
                this.LeftCapture != null &&
                this.RightCapture != null &&
                this.IsStarted &&
                this.CurrentFrame != null &&
                !this.CurrentFrame.IsNotFullFrame)
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
            this.LeftCapture.Stop();
            var videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            var leftCaptureDevice = new VideoCaptureDevice(videoDevices[leftCapId].MonikerString);
            this.LeftCapture = new AsyncVideoSource(leftCaptureDevice);
            this.LeftCapture.NewFrame += NewLeftFrame;
            this.LeftCapture.Start();


        }

        override public void ChangeRightCap(int rightCapId)
        {
            this.RightCapture.Stop();
            var videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            var rightCaptureDevice = new VideoCaptureDevice(videoDevices[rightCapId].MonikerString);
            this.RightCapture = new AsyncVideoSource(rightCaptureDevice);
            this.RightCapture.NewFrame += NewRightFrame;
            this.RightCapture.Start();
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
