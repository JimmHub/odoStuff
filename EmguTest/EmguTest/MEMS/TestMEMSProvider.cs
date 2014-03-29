using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using EmguTest.MEMS;

namespace EmguTest.MEMS
{
    class TestMEMSProvider : MEMSProvider
    {
        public TestMEMSProvider(String accReadingsPath, String magnetReadingsPath, String gyroReadingsPath)
        {
            this.AccReadingsPath = accReadingsPath;
            this.MagnetReadingsPath = magnetReadingsPath;
            this.GyroReadingsPath = gyroReadingsPath;

            this.AccBinStream = new BinaryReader(new FileStream(accReadingsPath, FileMode.Open));
            this.MagnetBinStream = new BinaryReader(new FileStream(magnetReadingsPath, FileMode.Open));
            this.GyroBinStream = new BinaryReader(new FileStream(gyroReadingsPath, FileMode.Open));

        }

        public String AccReadingsPath { get; set; }
        public String MagnetReadingsPath { get; set; }
        public String GyroReadingsPath { get; set; }

        public BinaryReader AccBinStream { get; set; }
        public BinaryReader MagnetBinStream { get; set; }
        public BinaryReader GyroBinStream { get; set; }

        protected ReadingsVector3f GetNextAccVector3f()
        {
            return this.Get3fReadings(this.AccBinStream);
        }

        protected ReadingsVector3f GetNextMagnetVector3f()
        {
            return this.Get3fReadings(this.MagnetBinStream);
        }

        protected ReadingsVector3f GetNextGyroVector3f()
        {
            return this.Get3fReadings(this.GyroBinStream);
        }

        protected ReadingsVector3f Get3fReadings(BinaryReader stream)
        {
            ReadingsVector3f res = new ReadingsVector3f();
            try
            {
                if (stream.BaseStream.Length <= stream.BaseStream.Position)
                {
                    res.IsEmpty = true;
                }
                else
                {
                    res.TimeStampI = Convert.ToInt64(this.ReadingsBytesToUInt(stream.ReadBytes(4)));

                    res.Values[0] = this.ReadingsBytesToFloat(stream.ReadBytes(4));
                    res.Values[1] = this.ReadingsBytesToFloat(stream.ReadBytes(4));
                    res.Values[2] = this.ReadingsBytesToFloat(stream.ReadBytes(4));
                }
            }
            catch
            {
                res.IsEmpty = true;
            }
            return res;
        }

        public MEMSReadingsSet3f CurrentReadingsSet3f { get; protected set; }

        public MEMSReadingsSet3f GetNextReadingsSet()
        {
            MEMSReadingsSet3f res = new MEMSReadingsSet3f();
            
            res.AccVector3f = this.GetNextAccVector3f();
            res.MagnetVector3f = this.GetNextMagnetVector3f();
            res.GyroVector3f = this.GetNextGyroVector3f();

            if (res.IsNotEmpty())
            {
                res.TimeStampI = Convert.ToInt64(((double)res.AccVector3f.TimeStampI + (double)res.MagnetVector3f.TimeStampI +(double) res.GyroVector3f.TimeStampI) / 3);
            }
            else
            {
                res.TimeStampI = 0;
            }
            this.CurrentReadingsSet3f = res;

            return res;
        }

        protected float ReadingsBytesToFloat(byte[] value)
        {
            return BitConverter.ToSingle(value.Reverse().ToArray(), 0);
        }

        protected uint ReadingsBytesToUInt(byte[] value)
        {
            return BitConverter.ToUInt32(value.Reverse().ToArray(), 0);
        }

        public bool CloseStreams()
        {
            bool isErr = false;

            try
            {
                this.AccBinStream.Close();
            }
            catch
            {
                isErr = true;
            }

            try
            {
                this.MagnetBinStream.Close();
            }
            catch
            {
                isErr = true;
            }

            try
            {
                this.GyroBinStream.Close();
            }
            catch
            {
                isErr = true;
            }

            return !isErr;
        }
    }
}
