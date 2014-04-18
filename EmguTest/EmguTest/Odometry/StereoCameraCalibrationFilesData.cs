using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace EmguTest.Odometry
{
    class StereoCameraCalibrationFilesData : StereoCameraCalibrationData
    {
        public List<Tuple<String, String>> SampleImagesNames { get; set; }

        override public VideoSource.StereoFrameSequenceElement GetFrameById(int id)
        {
            return new VideoSource.StereoFrameSequenceElement()
            {
                IsLeftFrameEmpty = false,
                IsRightFrameEmpty = false,
                LeftRawFrame = new Bitmap(this.SampleImagesNames[id].Item1),
                RightRawFrame = new Bitmap(this.SampleImagesNames[id].Item2)
            };
        }

        override public int GetCalibListSize()
        {
            if (this.SampleImagesNames == null)
            {
                return 0;
            }
            return this.SampleImagesNames.Count;
        }
    }
}
