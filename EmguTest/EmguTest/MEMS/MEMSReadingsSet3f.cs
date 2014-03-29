using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmguTest.MEMS
{
    class MEMSReadingsSet3f
    {
        public ReadingsVector3f AccVector3f { get; set; }
        public ReadingsVector3f MagnetVector3f { get; set; }
        public ReadingsVector3f GyroVector3f { get; set; }

        public DateTime TimeStamp { get; set; }
        public Int64 TimeStampI { get; set; }
        public bool IsNotEmpty()
        {
            return 
                !(this.AccVector3f == null || this.AccVector3f.IsEmpty ||
                this.MagnetVector3f == null || this.MagnetVector3f.IsEmpty ||
                this.GyroVector3f == null || this.GyroVector3f.IsEmpty);

        }
    }
}
