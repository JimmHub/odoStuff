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

        public AsyncVideoSource LeftCapture { get; set; }
        public AsyncVideoSource RightCapture { get; set; }
        protected StereoFrameSequenceElement CurrentFrame;
        protected bool IsStarted = false;
        //interface

        protected void Init()
        {
            this.CurrentFrame = new StereoFrameSequenceElement()
            {
                IsLeftFrameEmpty = true,
                IsRightFrameEmpty = true
            };
            
        }

        public StereoFrameSequenceElement GetCurrentFrame()
        {
            return this.CurrentFrame;
        }

        private void NewLeftFrame(object obj, NewFrameEventArgs e)
        {
            Image temp = this.CurrentFrame.LeftRawFrame;
            Bitmap bitmap = e.Frame;
            this.CurrentFrame.LeftRawFrame = new Bitmap(bitmap);
            
            this.CurrentFrame.TimeStamp = DateTime.UtcNow;
            this.CurrentFrame.IsLeftFrameEmpty = false;
        }

        private void NewRightFrame(object obj, NewFrameEventArgs e)
        {
            Image temp = this.CurrentFrame.RightRawFrame;
            Bitmap bitmap = e.Frame;
            this.CurrentFrame.RightRawFrame = new Bitmap(bitmap);
            
            this.CurrentFrame.TimeStamp = DateTime.UtcNow;
            this.CurrentFrame.IsRightFrameEmpty = false;
        }

        public StereoFrameSequenceElement GetNextFrame()
        {
            return this.CurrentFrame;
        }

        public bool StartStream()
        {
            this.LeftCapture.Start();
            this.RightCapture.Start();
            this.IsStarted = true;
            return true;
        }

        public bool PauseStream()
        {
            return true;
        }

        public bool StopStream()
        {
            throw new NotImplementedException();
        }
        ////


        public bool IsFunctioning()
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


        public bool CanChangeLeftCap()
        {
            return true;
        }

        public bool CanChangeRightCap()
        {
            return true;
        }

        public void ChangeLeftCap(int leftCapId)
        {
            this.LeftCapture.Stop();
            var videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            var leftCaptureDevice = new VideoCaptureDevice(videoDevices[leftCapId].MonikerString);
            this.LeftCapture = new AsyncVideoSource(leftCaptureDevice);
            this.LeftCapture.NewFrame += NewLeftFrame;
            this.LeftCapture.Start();


        }

        public void ChangeRightCap(int rightCapId)
        {
            this.RightCapture.Stop();
            var videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            var rightCaptureDevice = new VideoCaptureDevice(videoDevices[rightCapId].MonikerString);
            this.RightCapture = new AsyncVideoSource(rightCaptureDevice);
            this.RightCapture.NewFrame += NewRightFrame;
            this.RightCapture.Start();
        }


        public bool CanRewindStream()
        {
            return false;
        }

        public bool ResumeStream()
        {
            return true;
        }
    }
}
