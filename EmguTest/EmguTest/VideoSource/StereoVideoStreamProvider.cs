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
    public class StereoVideoStreamProvider
    {
        public StereoVideoStreamProvider(Capture cap)
        {
            this.Capture = cap;
        }

        public Capture Capture { get; set; }
        
        public int TotalFrames { get; set; }

        public StereoFrameSequenceElement CurrentFrame { get; protected set; }


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
    }
}
