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
        protected bool _isNewAccVal;
        protected bool _isNewMagnetVal;
        protected bool _isNewGyroVal;
        protected bool _isVideoStreamExpired;
        protected bool _isAccStreamExpired;
        protected bool _isMagnetStreamExpired;
        protected bool _isGyroStreamExpired;
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
            this._isNewAccVal = false;
            this._isNewMagnetVal = false;
            this._isNewGyroVal = false;
            _isVideoStreamExpired = false;
            _isAccStreamExpired = false;
            _isMagnetStreamExpired = false;
            _isGyroStreamExpired = false;

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
            //return Utils.DateTimeHelper.DateTimeToLongMS(DateTime.UtcNow);
            return Utils.DateTimeHelper.DateTimeToLongMS(this._startMEMSTimeStamp.AddMilliseconds(timeStampI - this._startMEMSTimeStampIOrig));
        }

        protected void CheckIfAllStreamsStopped()
        {
            if (this._isVideoStreamExpired && this._isAccStreamExpired && this._isMagnetStreamExpired && this._isGyroStreamExpired)
            {
                this.Stop();
            }
        }

        void StereoMEMSDataProviderCVCapFromFile__nextVideoFrameEvent(object sender, EventArgs e)
        {
            var rawFrame = this._videoSourceCap.QueryFrame();
            if (rawFrame == null)
            {
                this._isVideoStreamExpired = true;
                this.CheckIfAllStreamsStopped();
                return;
            }
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
                double diffDT;
                while ((diffDT = (DateTime.UtcNow.Subtract(time).TotalMilliseconds)) < this._framesInterval)
                {
                    Thread.Sleep((int)(this._framesInterval - diffDT));
                }
                lock (this._videoFrameLock)
                {
                    stereoFrame.TimeStamp = DateTime.UtcNow;
                    this._currentStereoFrame = stereoFrame;
                }
            }

            if (this.NewStereoFrameEvent != null)
            {
                ThreadPool.QueueUserWorkItem(s => this.NewStereoFrameEvent(this, new NewStereoFrameEventArgs()
                {
                    NewStereoFrame = this._currentStereoFrame
                }));
            }
            //
            while (this._isPaused && this._isStarted)
            {
            }
            if (this._isStarted)
            {
                ThreadPool.QueueUserWorkItem(s => this._nextVideoFrameEvent(this, null));
            }
        }
        void StereoMEMSDataProviderCVCapFromFile__nextAccValEvent(object sender, EventArgs e)
        {
            var rawVal = this.GetNextAccVector3f();
            if (rawVal.IsEmpty)
            {
                this._isAccStreamExpired = true;
                this.CheckIfAllStreamsStopped();
                return;
            }
            if (this._isFirstMEMSSet)
            {
                this._isFirstMEMSSet = false;
                this._startMEMSTimeStamp = DateTime.UtcNow.AddMilliseconds(-(rawVal.TimeStampI - this._startMEMSTimeStampIOrig));
            }
            else
            {
                //while (DateTime.UtcNow.Subtract(this._startMEMSTimeStamp).TotalMilliseconds < (rawVal.TimeStampI - this._startMEMSTimeStampIOrig))
                //{
                //}
                double diffDT;
                long diffDI;
                while ((diffDT = (DateTime.UtcNow.Subtract(this._startMEMSTimeStamp).TotalMilliseconds)) < (diffDI = (rawVal.TimeStampI - this._startMEMSTimeStampIOrig)))
                {
                    Thread.Sleep((int)(diffDI - diffDT));
                }
            }
            lock (this._MEMSSetLock)
            {
                this._currentMEMSSet.AccVector3f = rawVal;
                this._isNewAccVal = true;
            }
            this.InvokeNewMEMSSetEvent();

            //
            while (this._isPaused && this._isStarted)
            {
            }
            if (this._isStarted)
            {
                ThreadPool.QueueUserWorkItem(s => this._nextAccValEvent(this, null));
            }
        }

        protected void InvokeNewMEMSSetEvent()
        {
            if (!this._isEagerMEMS)
            {
                if (!(this._isNewAccVal && this._isNewMagnetVal && this._isNewGyroVal))
                {
                    return;
                }
            }

            if (this.NewMEMSReadingsEvent != null)
            {
                ThreadPool.QueueUserWorkItem(s => this.NewMEMSReadingsEvent(this, new NewMEMSReadingsSetEventArgs()
                {
                    Readings = this.GetResultMEMSSet()
                }));
            }
            this._isNewAccVal = false;
            this._isNewMagnetVal = false;
            this._isNewGyroVal = false;
        }

        void StereoMEMSDataProviderCVCapFromFile__nextMagnetValEvent(object sender, EventArgs e)
        {
            var rawVal = this.GetNextMagnetVector3f();
            if (rawVal.IsEmpty)
            {
                this._isMagnetStreamExpired = true;
                this.CheckIfAllStreamsStopped();
                return;
            }
            if (this._isFirstMEMSSet)
            {
                this._isFirstMEMSSet = false;
                this._startMEMSTimeStamp = DateTime.UtcNow.AddMilliseconds(-(rawVal.TimeStampI - this._startMEMSTimeStampIOrig));
            }
            else
            {
                //while (DateTime.UtcNow.Subtract(this._startMEMSTimeStamp).TotalMilliseconds < (rawVal.TimeStampI - this._startMEMSTimeStampIOrig))
                //{
                //}
                double diffDT;
                long diffDI;
                while ((diffDT = (DateTime.UtcNow.Subtract(this._startMEMSTimeStamp).TotalMilliseconds)) < (diffDI = (rawVal.TimeStampI - this._startMEMSTimeStampIOrig)))
                {
                    Thread.Sleep((int)(diffDI - diffDT));
                }
            }
            lock (this._MEMSSetLock)
            {
                this._currentMEMSSet.MagnetVector3f = rawVal;
                this._isNewMagnetVal = true;
            }
            this.InvokeNewMEMSSetEvent();

            //
            while (this._isPaused && this._isStarted)
            {
            }
            if (this._isStarted)
            {
                ThreadPool.QueueUserWorkItem(s => this._nextMagnetValEvent(this, null));
            }
        }

        void StereoMEMSDataProviderCVCapFromFile__nextGyroValEvent(object sender, EventArgs e)
        {
            var rawVal = this.GetNextGyroVector3f();
            if (rawVal.IsEmpty)
            {
                this._isGyroStreamExpired = true;
                this.CheckIfAllStreamsStopped();
                return;
            }
            if (this._isFirstMEMSSet)
            {
                this._isFirstMEMSSet = false;
                this._startMEMSTimeStamp = DateTime.UtcNow.AddMilliseconds(-(rawVal.TimeStampI - this._startMEMSTimeStampIOrig));
            }
            else
            {
                double diffDT;
                long diffDI;
                while ((diffDT = (DateTime.UtcNow.Subtract(this._startMEMSTimeStamp).TotalMilliseconds)) < (diffDI = (rawVal.TimeStampI - this._startMEMSTimeStampIOrig)))
                {
                    Thread.Sleep((int)(diffDI - diffDT));
                }
            }
            lock (this._MEMSSetLock)
            {
                this._currentMEMSSet.GyroVector3f = rawVal;
                this._isNewGyroVal = true;
            }
            this.InvokeNewMEMSSetEvent();

            //
            while (this._isPaused && this._isStarted)
            {
            }
            if (this._isStarted)
            {
                ThreadPool.QueueUserWorkItem(s => this._nextGyroValEvent(this, null));
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
            _isVideoStreamExpired = false;
            _isAccStreamExpired = false;
            _isMagnetStreamExpired = false;
            _isGyroStreamExpired = false;

            if (!this._isStarted)
            {
                this._isStarted = true;
                if (this._useMEMS)
                {
                    this._isFirstMEMSSet = true;
                    ThreadPool.QueueUserWorkItem(s => this._nextAccValEvent(this, null));
                    ThreadPool.QueueUserWorkItem(s => this._nextMagnetValEvent(this, null));
                    ThreadPool.QueueUserWorkItem(s => this._nextGyroValEvent(this, null));
                }

                if (this._useVideo)
                {
                    ThreadPool.QueueUserWorkItem(s => this._nextVideoFrameEvent(this, null));
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
