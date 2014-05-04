using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace EmguTest.VideoSource
{
    public class StereoFrameSequenceElement : IDisposable
    {
        public StereoFrameSequenceElement()
        {
        }


        public StereoFrameSequenceElement(StereoFrameSequenceElement original)
        {
            this.RawFrame = original.RawFrame == null ? null : new Bitmap(original.RawFrame);
            this.LeftRawFrame = original.LeftRawFrame == null ? null : new Bitmap(original.LeftRawFrame) ;
            this.RightRawFrame = original.RightRawFrame == null ? null : new Bitmap(original.RightRawFrame);
            this.TimeStamp = original.TimeStamp;
            this.IsLeftFrameEmpty = original.IsLeftFrameEmpty;
            this.IsRightFrameEmpty = original.IsRightFrameEmpty;
        }
        //TODO: implement this constructor
        //public StereoFrameSequenceElement(StereoFrameSequenceElement elm)
        //{
        //}

        public Bitmap RawFrame { get; set; }
        public Bitmap LeftRawFrame { get; set; }
        public Bitmap RightRawFrame { get; set; }
        public DateTime TimeStamp { get; set; }
        public Int64 TimeStampI
        {
            get
            {
                return Utils.DateTimeHelper.DateTimeToLongMS(this.TimeStamp);
            }
        }
        public bool IsLeftFrameEmpty { get; set; }
        public bool IsRightFrameEmpty { get; set; }
        public bool IsNotFullFrame 
        {
            get
            {
                return this.IsLeftFrameEmpty || this.IsRightFrameEmpty;
            }
        }

        public void Dispose()
        {
            if (this.RawFrame != null)
            {
                try
                {
                    this.RawFrame.Dispose();
                }
                catch { }
            }

            if (this.LeftRawFrame != null)
            {
                try
                {
                    this.LeftRawFrame.Dispose();
                }
                catch { }
            }

            if (this.RightRawFrame != null)
            {
                try
                {
                    this.RightRawFrame.Dispose();
                }
                catch { }
            }
        }
    }
}
