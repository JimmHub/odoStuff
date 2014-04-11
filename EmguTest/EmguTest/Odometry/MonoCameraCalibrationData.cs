using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace EmguTest.Odometry
{
    class MonoCameraCalibrationData
    {
        //board params
        public Size BoardSquareSize { get; set; }
        public double SquareSize { get; set; }
        ////
        //image list
        public List<String> SampleImagesNames { get; set; }
        ////

    }
}
