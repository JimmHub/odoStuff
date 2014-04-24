using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmguTest.MEMS
{
    public class ReadingsVector
    {
        public ReadingsVector()
        {
            this.IsEmpty = false;
        }
        public Int64 TimeStampI { get; set; }
        public bool IsEmpty { get; set; }

    }
}
