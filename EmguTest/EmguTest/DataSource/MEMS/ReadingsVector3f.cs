using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmguTest.MEMS
{
    public class ReadingsVector3f : ReadingsVector
    {
        public ReadingsVector3f()
            : base()
        {
            this.Values = new float[3];
        }

        public ReadingsVector3f(ReadingsVector3f original)
            : base()
        {
            var size  = 3;
            this.Values = new float[size];
            for (int i = 0; i < size; ++i)
            {
                this.Values[i] = original.Values[i];
            }
            this.TimeStampI = original.TimeStampI;
        }

        public float[] Values { get; set; }
    }
}
