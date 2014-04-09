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
        public Size BoardSquareSize { get; set; }
        public double SquareSize { get; set; }

        public List<String> SampleImagesNames { get; set; }
    }
}
