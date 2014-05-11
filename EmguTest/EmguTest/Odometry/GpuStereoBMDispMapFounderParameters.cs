using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmguTest.Odometry
{
    public class GpuStereoBMDispMapFounderParameters : DispMapFounderParameters
    {
        public int NumberOfDisparities { get; set; }
        public int BlockSize { get; set; }
    }
}
