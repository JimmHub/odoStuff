using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

using Emgu.CV;
using Emgu.Util;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using Emgu.CV.GPU;
using Emgu.CV.Features2D;

namespace EmguTest.VideoSource
{
    class StereoStaticPicturesAsVideoStreamProvider : StereoVideoStreamProvider
    {

        public StereoStaticPicturesAsVideoStreamProvider(String leftImgPath, String rightImgPath, int timerPeriod)
        {
            this._leftImgPath = leftImgPath;
            this._rightImgPath = rightImgPath;
            this._leftBmp = new Bitmap(this._leftImgPath);
            this._rightBmp = new Bitmap(this._rightImgPath);
            this._timerPeriod = timerPeriod;

            this._Init();
        }

        protected String _leftImgPath;
        protected String _rightImgPath;
        protected Bitmap _leftBmp;
        protected Bitmap _rightBmp;
        protected System.Threading.Timer _newFrameTimer;
        protected bool _isTimerActive;
        protected int _timerPeriod;
        protected bool _isTimerCallbackInUse;

        protected void _RestartTimer()
        {
            this._newFrameTimer = new System.Threading.Timer(this._TimerCallback, null, (uint)this._timerPeriod, (uint)this._timerPeriod);
        }

        protected void _StopTimer()
        {
            this._newFrameTimer.Dispose();
        }

        protected void _Init()
        {
            this._isTimerActive = false;
            this._isTimerCallbackInUse = false;
        }

        protected void _TimerCallback(object stateObject)
        {
            if (this._isTimerCallbackInUse)
            {
                return;
            }
            this._isTimerCallbackInUse = true;

            if (this.NewStereoFrameEvent != null)
            {
                this.NewStereoFrameEvent(this, new NewStereoFrameEventArgs()
                {
                    NewStereoFrame = this.GetNextFrame()
                });
            }

            this._isTimerCallbackInUse = false;
        }

        protected StereoFrameSequenceElement _ReturnNewStereoFrame()
        {
            return new StereoFrameSequenceElement()
            {
                IsLeftFrameEmpty = false,
                IsRightFrameEmpty = false,
                LeftRawFrame = new Bitmap(this._leftBmp),
                RightRawFrame = new Bitmap(this._rightBmp),
                TimeStamp = DateTime.UtcNow
            };
        }

        public override StereoFrameSequenceElement GetCurrentFrame()
        {
            return this._ReturnNewStereoFrame();
        }

        public override StereoFrameSequenceElement GetNextFrame()
        {
            return this._ReturnNewStereoFrame();
        }

        public override bool IsFunctioning()
        {
            return this._isTimerActive;
        }

        public override bool CanChangeLeftCap()
        {
            return false;
        }

        public override bool CanChangeRightCap()
        {
            return false;
        }

        public override bool CanRewindStream()
        {
            return false;
        }

        public override void ChangeLeftCap(int leftCapId)
        {
            throw new NotImplementedException();
        }

        public override void ChangeRightCap(int rightCapId)
        {
            throw new NotImplementedException();
        }

        public override bool StartStream()
        {
            if (!this._isTimerActive)
            {
                this._isTimerActive = true;
                this._RestartTimer();
            }
            return true;
        }

        public override bool PauseStream()
        {
            throw new NotImplementedException();
        }

        public override bool ResumeStream()
        {
            throw new NotImplementedException();
        }

        public override bool StopStream()
        {
            if (this._isTimerActive)
            {
                this._isTimerActive = false;
                this._StopTimer();
            }
            return true;
        }

        public override event NewStereoFrameEventHandler NewStereoFrameEvent;
    }
}
