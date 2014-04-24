using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

using Emgu.CV;
using Emgu.Util;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using Emgu.CV.GPU;
using Emgu.CV.Features2D;

using EmguTest.MEMS;
using EmguTest.VideoSource;

namespace EmguTest.DataSource
{
    class StereoMEMSDataProviderCVCapFromFile : StereoMEMSDataProvider
    {
        public StereoMEMSDataProviderCVCapFromFile(bool useVideo, String videoSourcePath, double framesInterval, bool useMEMS, String MEMSAccPath, String MEMSMagnetPath, String MEMSGyroPath, bool isEagerMEMS)
        {
            this._isEagerMEMS = isEagerMEMS;
            this._framesInterval = framesInterval;

            this._useMEMS = useMEMS;
            this._useVideo = useVideo;
            
            if (this._useMEMS)
            {
                this._accBinStream = new FileStream(MEMSAccPath, FileMode.Open);
                this._magnetBinStream = new FileStream(MEMSMagnetPath, FileMode.Open);
                this._gyroBinStream = new FileStream(MEMSGyroPath, FileMode.Open);
            }
            if (this._useVideo)
            {
                this._videoSourceCap = new Capture(videoSourcePath);
            }

            //at the end
            this.Init();
        }

        protected bool _isStarted;
        protected bool _isPaused;
        protected Int64 _startMEMSTimeStamp;
        protected DateTime? _startVideoTimeStamp;
        protected FileStream _accBinStream;
        protected FileStream _magnetBinStream;
        protected FileStream _gyroBinStream;
        protected bool _isEagerMEMS;
        protected MEMSReadingsSet3f _currentMEMSSet;
        protected StereoFrameSequenceElement _currentStereoFrame;
        protected double _framesInterval;
        protected Capture _videoSourceCap;
        protected bool _useMEMS;
        protected bool _useVideo;
        protected bool _is_videoSourceCap_ImageGrabbed_inUse;
        protected bool _is_accReadCallback_inUse;
        protected bool _is_magnetReadCallback_inUse;
        protected bool _is_gyroReadCallback_inUse;

        protected void Init()
        {
            this._isStarted = false;
            this._isPaused = false;
            this._is_videoSourceCap_ImageGrabbed_inUse = false;
            this._is_accReadCallback_inUse = false;
            this._is_magnetReadCallback_inUse = false;
            this._is_gyroReadCallback_inUse = false;

            this._startMEMSTimeStamp = 0;
            if (this._useMEMS)
            {
                var startRes = this.GetNextAccVector3f();
                this.GetNextGyroVector3f();
                this.GetNextMagnetVector3f();

                this._startMEMSTimeStamp = startRes.TimeStampI;
            }

            if (this._useVideo)
            {
                var rawFrame = this._videoSourceCap.QueryFrame();
                this._currentStereoFrame = this.ElementFromRawFrame(rawFrame);
                rawFrame = null;

                this._videoSourceCap.ImageGrabbed += _videoSourceCap_ImageGrabbed;
            }
        }

        protected void _videoSourceCap_ImageGrabbed(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        protected StereoFrameSequenceElement ElementFromRawFrame(Image<Bgr, byte> rawFrame)
        {
            rawFrame.ROI = new Rectangle(0, 0, rawFrame.Width / 2, rawFrame.Height);

            var left = rawFrame.Copy();

            rawFrame.ROI = Rectangle.Empty;
            rawFrame.ROI = new Rectangle(rawFrame.Width / 2, 0, rawFrame.Width / 2, rawFrame.Height);
            var right = rawFrame.Copy();

            rawFrame.ROI = Rectangle.Empty;

            StereoFrameSequenceElement res = new StereoFrameSequenceElement()
            {
                RawFrame = Utils.CvHelper.ConvertImageToBitmap(rawFrame),
                LeftRawFrame = Utils.CvHelper.ConvertImageToBitmap(left),
                RightRawFrame = Utils.CvHelper.ConvertImageToBitmap(right),
                TimeStamp = DateTime.UtcNow
            };
            rawFrame.Dispose();
            left.Dispose();
            right.Dispose();
            return res;
        }

        protected ReadingsVector3f GetNextAccVector3f()
        {
            return this.Get3fReadings(this._accBinStream);
        }

        protected ReadingsVector3f GetNextMagnetVector3f()
        {
            return this.Get3fReadings(this._magnetBinStream);
        }

        protected ReadingsVector3f GetNextGyroVector3f()
        {
            return this.Get3fReadings(this._gyroBinStream);
        }

        protected ReadingsVector3f Get3fReadings(FileStream stream)
        {
            ReadingsVector3f res = new ReadingsVector3f();
            try
            {
                if (stream.Length <= stream.Position)
                {
                    res.IsEmpty = true;
                }
                else
                {
                    byte[] buff = new byte[16];
                    stream.Read(buff, 0, 16);
                    res.TimeStampI = Convert.ToInt64(buff.Take(4));

                    res.Values[0] = this.ReadingsBytesToFloat(buff.Skip(4).Take(4).ToArray());
                    res.Values[1] = this.ReadingsBytesToFloat(buff.Skip(8).Take(4).ToArray());
                    res.Values[2] = this.ReadingsBytesToFloat(buff.Skip(12).Take(4).ToArray());
                }
            }
            catch
            {
                res.IsEmpty = true;
            }
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

        override public MEMSReadingsSet3f GetCurrentMEMSReadingsSet3f()
        {
            return this._currentMEMSSet;
        }

        override public StereoFrameSequenceElement GetCurrentStereoFrame()
        {
            return this._currentStereoFrame;
        }
        override public event NewMEMSReadingsSetEventHandler NewMEMSReadingsEvent;
        override public event NewStereoFrameEventHandler NewStereoFrameEvent;

        public override void Start()
        {
            throw new NotImplementedException();
        }

        public override void Stop()
        {
            throw new NotImplementedException();
        }

        public override void Pause()
        {
            throw new NotImplementedException();
        }

        public override bool IsStarted()
        {
            return this._isStarted;
        }

        public override bool IsPaused()
        {
            return this._isPaused;
        }
    }
}
