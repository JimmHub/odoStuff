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
            int port)
        {
            this._useVideo = useVideo;
            this._useMEMS = useMEMS;
            this._leftCapture = new Capture(leftCapId);
            this._rightCapture = new Capture(rightCapId);
            this._port = port;

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

        protected void Init()
        {

        }

        public override MEMSReadingsSet3f GetCurrentMEMSReadingsSet3f()
        {
            throw new NotImplementedException();
        }

        public override StereoFrameSequenceElement GetCurrentStereoFrame()
        {
            throw new NotImplementedException();
        }

        public override event NewMEMSReadingsSetEventHandler NewMEMSReadingsEvent;

        public override event NewStereoFrameEventHandler NewStereoFrameEvent;

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
