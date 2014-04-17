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

        public Capture LeftCapture { get; set; }
        public Capture RightCapture { get; set; }
        protected StereoFrameSequenceElement CurrentFrame;
        //interface
        public StereoFrameSequenceElement GetCurrentFrame()
        {
            return this.CurrentFrame;
        }

        public StereoFrameSequenceElement GetNextFrame()
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

        public bool StartStream()
        {
            throw new NotImplementedException();
        }

        public bool PauseStream()
        {
            throw new NotImplementedException();
        }

        public bool StopStream()
        {
            throw new NotImplementedException();
        }
        ////


        public bool IsFunctioning()
        {
            if (LeftCapture != null && RightCapture != null)
            {
                return true;
            }
            return false;
        }
    }
}
