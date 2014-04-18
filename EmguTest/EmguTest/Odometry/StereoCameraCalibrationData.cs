using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace EmguTest.Odometry
{
    abstract class StereoCameraCalibrationData
    {
        //board params
        public Size BoardSquareSize { get; set; }
        public double SquareSize { get; set; }
        ////
        //image list
        ////
        abstract public VideoSource.StereoFrameSequenceElement GetFrameById(int id);
        abstract public int GetCalibListSize();

    }
}
