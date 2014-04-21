using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmguTest.MEMS
{
    public class MEMSReadingsSet3f
    {
        public ReadingsVector3f AccVector3f { get; set; }
        public ReadingsVector3f MagnetVector3f { get; set; }
        public ReadingsVector3f GyroVector3f { get; set; }

        public DateTime TimeStamp { get; set; }
        public Int64 TimeStampI 
        {
            get
            {
                if (this.IsNotEmpty())
                {
                    return (int)(((double)this.AccVector3f.TimeStampI + this.MagnetVector3f.TimeStampI + this.GyroVector3f.TimeStampI) / 3);
                }
                else
                {
                    return 0;
                }
            }
        }

        public bool IsNotEmpty()
        {
            return 
                !(this.AccVector3f == null || this.AccVector3f.IsEmpty ||
                this.MagnetVector3f == null || this.MagnetVector3f.IsEmpty ||
                this.GyroVector3f == null || this.GyroVector3f.IsEmpty);

        }

        public MEMSReadingsSet3f()
        {
            this.AccVector3f = new ReadingsVector3f()
            {
                IsEmpty = true
            };

            this.MagnetVector3f = new ReadingsVector3f()
            {
                IsEmpty = true
            };

            this.GyroVector3f = new ReadingsVector3f()
            {
                IsEmpty = true
            };
        }
    }
}
