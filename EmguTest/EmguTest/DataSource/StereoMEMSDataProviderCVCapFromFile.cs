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
        protected DateTime _startMEMSTimeStamp;
        protected DateTime? _startVideoTimeStamp;
        protected Int64 _startMEMSTimeStampIOrig;
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
        //protected bool _is_videoSourceCap_ImageGrabbed_inUse;
        //protected bool _is_StereoMEMSDataProviderCVCapFromFile__nextAccValEvent_inUse;
        //protected bool _is_StereoMEMSDataProviderCVCapFromFile__nextMagnetValEvent_inUse;
        //protected bool _is_StereoMEMSDataProviderCVCapFromFile__nextGyroValEvent_inUse;
        //protected bool _is_StereoMEMSDataProviderCVCapFromFile__nextVideoFrameEvent_inUse;
        protected bool _isFirstVideoFrame;
        protected bool _isFirstMEMSSet;
        protected object _videoFrameLock = new object();
        protected object _MEMSSetLock = new object();
        protected bool _newAccVal;
        protected bool _newMagnetVal;
        protected bool _newGyroVal;

        //
        protected delegate void NextVideoFrame(object sender, EventArgs e);
        protected delegate void NextAccVal(object sender, EventArgs e);
        protected delegate void NextMagnetVal(object sender, EventArgs e);
        protected delegate void NextGyroVal(object sender, EventArgs e);

        protected event NextVideoFrame _nextVideoFrameEvent;
        protected event NextAccVal _nextAccValEvent;
        protected event NextMagnetVal _nextMagnetValEvent;
        protected event NextGyroVal _nextGyroValEvent;
        //
        protected void Init()
        {
            this._isStarted = false;
            this._isPaused = false;
            //this._is_videoSourceCap_ImageGrabbed_inUse = false;
            //this._is_StereoMEMSDataProviderCVCapFromFile__nextAccValEvent_inUse = false;
            //this._is_StereoMEMSDataProviderCVCapFromFile__nextMagnetValEvent_inUse = false;
            //this._is_StereoMEMSDataProviderCVCapFromFile__nextGyroValEvent_inUse = false;
            //this._is_StereoMEMSDataProviderCVCapFromFile__nextVideoFrameEvent_inUse = false;
            this._newAccVal = false;
            this._newMagnetVal = false;
            this._newGyroVal = false;

            this._currentMEMSSet = new MEMSReadingsSet3f();

            if (this._useMEMS)
            {
                this._currentMEMSSet.AccVector3f = this.GetNextAccVector3f();
                this._currentMEMSSet.MagnetVector3f = this.GetNextMagnetVector3f();
                this._currentMEMSSet.GyroVector3f = this.GetNextGyroVector3f();

                this._startMEMSTimeStampIOrig = this._currentMEMSSet.AccVector3f.TimeStampI;

                this._nextAccValEvent += StereoMEMSDataProviderCVCapFromFile__nextAccValEvent;
                this._nextMagnetValEvent += StereoMEMSDataProviderCVCapFromFile__nextMagnetValEvent;
                this._nextGyroValEvent += StereoMEMSDataProviderCVCapFromFile__nextGyroValEvent;
            }

            if (this._useVideo)
            {
                var rawFrame = this._videoSourceCap.QueryFrame();
                this._currentStereoFrame = this.ElementFromRawFrame(rawFrame);
                rawFrame = null;

                this._nextVideoFrameEvent += StereoMEMSDataProviderCVCapFromFile__nextVideoFrameEvent;
                this._isFirstVideoFrame = true;
            }
        }

        protected MEMSReadingsSet3f GetResultMEMSSet()
        {
            var res = new MEMSReadingsSet3f()
            {
                AccVector3f = new ReadingsVector3f(this._currentMEMSSet.AccVector3f),
                MagnetVector3f = new ReadingsVector3f(this._currentMEMSSet.MagnetVector3f),
                GyroVector3f = new ReadingsVector3f(this._currentMEMSSet.GyroVector3f)
            };
            res.AccVector3f.TimeStampI = this.RecalcMEMSTimeStamp(res.AccVector3f.TimeStampI);
            res.MagnetVector3f.TimeStampI = this.RecalcMEMSTimeStamp(res.MagnetVector3f.TimeStampI);
            res.GyroVector3f.TimeStampI = this.RecalcMEMSTimeStamp(res.GyroVector3f.TimeStampI);
            return res;
        }

        protected long RecalcMEMSTimeStamp(long timeStampI)
        {
            return Utils.DateTimeHelper.DateTimeToLongMS(this._startMEMSTimeStamp.AddMilliseconds(timeStampI - this._startMEMSTimeStampIOrig));
        }

        void StereoMEMSDataProviderCVCapFromFile__nextVideoFrameEvent(object sender, EventArgs e)
        {
            var rawFrame = this._videoSourceCap.RetrieveBgrFrame();

            var stereoFrame = this.ElementFromRawFrame(rawFrame);
            rawFrame = null;
            if (this._isFirstVideoFrame)
            {
                this._startVideoTimeStamp = stereoFrame.TimeStamp;
                this._isFirstVideoFrame = false;

                lock (this._videoFrameLock)
                {
                    this._currentStereoFrame = stereoFrame;
                }
            }
            else
            {
                var time = this._currentStereoFrame.TimeStamp;
                
                while (DateTime.UtcNow.Subtract(time).TotalMilliseconds < this._framesInterval)
                {
                    //try to sleep
                    Thread.Sleep((int)(this._framesInterval - DateTime.UtcNow.Subtract(time).TotalMilliseconds));
                }
                lock (this._videoFrameLock)
                {
                    stereoFrame.TimeStamp = DateTime.UtcNow;
                    this._currentStereoFrame = stereoFrame;
                }
            }

            if (this.NewStereoFrameEvent != null)
            {
                this.NewStereoFrameEvent(this, new NewStereoFrameEventArgs()
                {
                    NewStereoFrame = this._currentStereoFrame
                });
            }
            //
            while (this._isPaused && this._isStarted)
            {
            }
            if (this._isStarted)
            {
                this._nextVideoFrameEvent(this, null);
            }
        }
        void StereoMEMSDataProviderCVCapFromFile__nextAccValEvent(object sender, EventArgs e)
        {
            var rawVal = this.GetNextAccVector3f();
            if (this._isFirstMEMSSet)
            {
                this._isFirstMEMSSet = false;
                this._startMEMSTimeStamp = DateTime.UtcNow.AddMilliseconds(-(rawVal.TimeStampI - this._startMEMSTimeStampIOrig));
            }
            else
            {
                while (DateTime.UtcNow.Subtract(this._startMEMSTimeStamp).TotalMilliseconds < (rawVal.TimeStampI - this._currentMEMSSet.AccVector3f.TimeStampI))
                {
                }
            }
            this._currentMEMSSet.AccVector3f = rawVal;
            this._newAccVal = true;
            this.InvokeNewMEMSSetEvent();

            //
            while (this._isPaused && this._isStarted)
            {
            }
            if (this._isStarted)
            {
                this._nextAccValEvent(this, null);
            }
        }

        protected void InvokeNewMEMSSetEvent()
        {
            if (!this._isEagerMEMS)
            {
                if (!(this._newAccVal && this._newMagnetVal && this._newGyroVal))
                {
                    return;
                }
            }

            if (this.NewMEMSReadingsEvent != null)
            {
                this.NewMEMSReadingsEvent(this, new NewAMGFrameEventArgs()
                {
                    Readings = this.GetResultMEMSSet()
                });
            }
            this._newAccVal = false;
            this._newMagnetVal = false;
            this._newGyroVal = false;
        }

        void StereoMEMSDataProviderCVCapFromFile__nextMagnetValEvent(object sender, EventArgs e)
        {
            var rawVal = this.GetNextMagnetVector3f();
            if (this._isFirstMEMSSet)
            {
                this._isFirstMEMSSet = false;
                this._startMEMSTimeStamp = DateTime.UtcNow.AddMilliseconds(-(rawVal.TimeStampI - this._startMEMSTimeStampIOrig));
            }
            else
            {
                while (DateTime.UtcNow.Subtract(this._startMEMSTimeStamp).TotalMilliseconds < (rawVal.TimeStampI - this._currentMEMSSet.MagnetVector3f.TimeStampI))
                {
                }
            }
            this._currentMEMSSet.MagnetVector3f = rawVal;
            this._newMagnetVal = true;
            this.InvokeNewMEMSSetEvent();

            //
            while (this._isPaused && this._isStarted)
            {
            }
            if (this._isStarted)
            {
                this._nextMagnetValEvent(this, null);
            }
        }

        void StereoMEMSDataProviderCVCapFromFile__nextGyroValEvent(object sender, EventArgs e)
        {
            var rawVal = this.GetNextGyroVector3f();
            if (this._isFirstMEMSSet)
            {
                this._isFirstMEMSSet = false;
                this._startMEMSTimeStamp = DateTime.UtcNow.AddMilliseconds(-(rawVal.TimeStampI - this._startMEMSTimeStampIOrig));
            }
            else
            {
                while (DateTime.UtcNow.Subtract(this._startMEMSTimeStamp).TotalMilliseconds < (rawVal.TimeStampI - this._currentMEMSSet.GyroVector3f.TimeStampI))
                {
                }
            }
            this._currentMEMSSet.GyroVector3f = rawVal;
            this._newGyroVal = true;
            this.InvokeNewMEMSSetEvent();

            //
            while (this._isPaused && this._isStarted)
            {
            }
            if (this._isStarted)
            {
                this._nextGyroValEvent(this, null);
            }
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
                    res.TimeStampI = Convert.ToInt64(this.ReadingsBytesToUInt(buff.Take(4).ToArray()));

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
            lock (this._MEMSSetLock)
            {
                return this.GetResultMEMSSet();
            }
        }

        override public StereoFrameSequenceElement GetCurrentStereoFrame()
        {
            return this._currentStereoFrame;
        }
        override public event NewMEMSReadingsSetEventHandler NewMEMSReadingsEvent;
        override public event NewStereoFrameEventHandler NewStereoFrameEvent;

        public override void Start()
        {
            if (!this._isStarted)
            {
                this._isStarted = true;
                if (this._useMEMS)
                {
                    this._nextAccValEvent(this, null);
                    this._nextMagnetValEvent(this, null);
                    this._nextGyroValEvent(this, null);
                }

                if (this._useVideo)
                {
                    this._nextVideoFrameEvent(this, null);
                }
            }
        }

        public override void Stop()
        {
            this._isStarted = false;
        }

        public override void Pause()
        {
            this._isPaused = true;
        }

        public override void Resume()
        {
            this._isPaused = false;
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
