using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmguTest.Odometry
{
    class StereoCameraCalibrationGrabbedData : StereoCameraCalibrationData
    {
        public List<VideoSource.StereoFrameSequenceElement> GrabbedFrames;

        override public VideoSource.StereoFrameSequenceElement GetFrameById(int id)
        {
            return this.GrabbedFrames[id];
        }

        override public int GetCalibListSize()
        {
            if (this.GrabbedFrames == null)
            {
                return 0;
            }
            return this.GrabbedFrames.Count;
        }
    }
}
