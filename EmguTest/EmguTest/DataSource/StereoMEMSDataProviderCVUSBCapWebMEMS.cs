using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;

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
    class StereoMEMSDataProviderCVUSBCapWebMEMS : StereoMEMSDataProvider
    {
        public StereoMEMSDataProviderCVUSBCapWebMEMS(
            bool useVideo,
            int leftCapId,
            int rightCapId,

            bool useMEMS,
            int port,
            bool isMEMSEager)
        {
            this._useVideo = useVideo;
            this._useMEMS = useMEMS;
            this._leftCapture = new Capture(leftCapId);
            this._rightCapture = new Capture(rightCapId);
            this._port = port;
            this._isMEMSEager = isMEMSEager;

            //
            this.Init();
        }

        protected int _port;
        protected Capture _leftCapture;
        protected Capture _rightCapture;
        protected bool _useMEMS;
        protected bool _useVideo;
        protected Socket _clientSocket;
        protected Socket _serverSocket;
        protected Thread _memsReceiveThread;
        protected Thread _videoCapThread;
        protected bool _isPaused;
        protected bool _isStarted;
        protected VideoSource.StereoFrameSequenceElement _currentStereoFrame;
        protected bool _isMEMSEager;
        protected object _currentVideoFrameLock = new object();

        protected void Init()
        {
            this._isPaused = false;
            this._isStarted = false;
            this._memsReceiveThread = new Thread(this.MemsReceiveRoutine);
            this._videoCapThread = new Thread(this.VideoCapRoutine);
        }

        protected bool IsMEMSClientConnected()
        {
            //TODO
            return true;
        }

        protected void VideoCapRoutine()
        {
            while (this._isStarted)
            {   
                var leftFrame = this._leftCapture.QueryFrame();
                var rightFrame = this._rightCapture.QueryFrame();
                var timeStamp = DateTime.UtcNow;
                Bitmap oldLeft = null;
                Bitmap oldRight = null;

                if (this._currentStereoFrame != null)
                {
                    oldLeft = this._currentStereoFrame.LeftRawFrame;
                    oldRight = this._currentStereoFrame.RightRawFrame;
                }
                else
                {
                    this._currentStereoFrame = new StereoFrameSequenceElement();
                }

                lock (this._currentVideoFrameLock)
                {
                    this._currentStereoFrame.LeftRawFrame = new Bitmap(leftFrame.Convert<Bgr, byte>().ToBitmap());
                    this._currentStereoFrame.RightRawFrame = new Bitmap(rightFrame.Convert<Bgr, byte>().ToBitmap());
                    this._currentStereoFrame.TimeStamp = timeStamp;

                    if (this.NewStereoFrameEvent != null)
                    {
                        ThreadPool.QueueUserWorkItem(s => this.NewStereoFrameEvent(this, new NewStereoFrameEventArgs()
                            {
                                NewStereoFrame = this.GetOutputStereoFrame()
                            }));
                    }

                    //cleanup
                    if (oldLeft != null)
                    {
                        oldLeft.Dispose();
                    }
                    if (oldRight != null)
                    {
                        oldRight.Dispose();
                    }
                }
                leftFrame.Dispose();
                rightFrame.Dispose();
                ////
                //pause
                while (this._isPaused && this._isStarted)
                {
                }
                ////
            }
        }

        protected StereoFrameSequenceElement GetOutputStereoFrame()
        {
            if (this._currentStereoFrame == null)
            {
                return null;
            }
            return new StereoFrameSequenceElement()
            {
                IsLeftFrameEmpty = false,
                IsRightFrameEmpty = false,
                LeftRawFrame = new Bitmap(this._currentStereoFrame.LeftRawFrame),
                RightRawFrame = new Bitmap(this._currentStereoFrame.RightRawFrame),
                TimeStamp = this._currentStereoFrame.TimeStamp
            };
        }

        protected void MemsReceiveRoutine()
        {
            while (this._isStarted)
            {
                while (this._isPaused && this._isStarted)
                {
                }
            }
        }

        public override MEMSReadingsSet3f GetCurrentMEMSReadingsSet3f()
        {
            throw new NotImplementedException();
        }

        public override StereoFrameSequenceElement GetCurrentStereoFrame()
        {
            return this.GetOutputStereoFrame();
        }

        public override event NewMEMSReadingsSetEventHandler NewMEMSReadingsEvent;

        public override event NewStereoFrameEventHandler NewStereoFrameEvent;
        
        protected void CloseThreadsIfActive()
        {
        }

        public override void Start()
        {
            if (!this._isStarted)
            {
                this.CloseThreadsIfActive();
                if (this._useMEMS)
                {
                    this._memsReceiveThread.Start();
                }

                if (this._useVideo)
                {
                    this._videoCapThread.Start();
                }
                this._isStarted = true;
            }
        }

        public override void Stop()
        {
            throw new NotImplementedException();
        }

        public override void Pause()
        {
            throw new NotImplementedException();
        }

        public override void Resume()
        {
            throw new NotImplementedException();
        }

        public override bool IsStarted()
        {
            throw new NotImplementedException();
        }

        public override bool IsPaused()
        {
            throw new NotImplementedException();
        }
    }
}
