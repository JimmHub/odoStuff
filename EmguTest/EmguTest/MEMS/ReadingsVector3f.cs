using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmguTest.MEMS
{
    class ReadingsVector3f : ReadingsVector
    {
        public ReadingsVector3f()
            : base()
        {
            this.Values = new float[3];
        }

        public float[] Values { get; set; }
        public Int64 TimeStampI { get; set; }
        
    }
}
