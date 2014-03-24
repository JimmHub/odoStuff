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

namespace EmguTest
{
    public abstract partial class FeatureTracker
    {
        public FeatureTracker(Capture cap)
        {
            this.Capture = cap;
        }

        public Bitmap ResultFrame { get; protected set; }
        public Capture Capture { get; set; }

        protected bool _isRunning;
        public bool IsRunning 
        {
            get
            {
                if (this.TrackerThread == null)
                {
                    return false;
                }
                if (this.TrackerThread.ThreadState == ThreadState.Running)
                {
                    return true;
                }
                if (this.TrackerThread.ThreadState == ThreadState.WaitSleepJoin)
                {
                    return true;
                }

                return false;
            }
            protected set
            {
                this._isRunning = value;
            }
        }

        protected bool ShouldStop;

        protected Thread TrackerThread;

        public void Run()
        {
            if (!this.IsRunning)
            {
                this.IsRunning = true;
                TrackerThread = new Thread(this.MainProcessing);
                TrackerThread.Start();
            }
        }

        public void Stop()
        {
            //TODO
            this.ShouldStop = true;
        }

        protected void SetResultFrame(Bitmap frame)
        {
            this.ResultFrame = frame;
        }


    }
}
