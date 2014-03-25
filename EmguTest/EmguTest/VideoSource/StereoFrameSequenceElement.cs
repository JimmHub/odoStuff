using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace EmguTest.VideoSource
{
    public class StereoFrameSequenceElement
    {
        public Bitmap RawFrame { get; set; }
        public Bitmap LeftRawFrame { get; set; }
        public Bitmap RightRawFrame { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
