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

using AForge.Video;
using AForge.Video.DirectShow;
using AForge.Video.FFMPEG;

namespace EmguTest.VideoSource
{
    public class StereoGeminateAForgeFileVideoStreamProvider : StereoVideoStreamProvider
    {   
        public StereoGeminateAForgeFileVideoStreamProvider(String fileName, int frameInterval)
        {
            VideoFileSource fileVideo = new VideoFileSource(fileName);
            this.frameInterval = fileVideo.FrameInterval = (int)(1.0 / 30 * 1000);
            //VideoFileSource f = new VideoFileSource("");
            asyncVideoSource = new AsyncVideoSource(fileVideo);
            this.Init();
            asyncVideoSource.NewFrame += NewFrameCallback;
        }

        override public event NewStereoFrameEventHandler NewStereoFrameEvent;
        protected StereoFrameSequenceElement currentFrame;
        protected StereoFrameSequenceElement emptyFrame;
        protected AsyncVideoSource asyncVideoSource;
        protected bool IsStarted = false;
        protected object currentFrameLock = new object();
        protected bool isCurrentFrameGiven = true;
        protected int frameInterval;
        protected bool IsPaused = false;

        protected void NewFrameCallback(object obj, NewFrameEventArgs e)
        {
            Image temp = this.currentFrame.RawFrame;
            Bitmap bitmap = e.Frame;
            lock (currentFrameLock)
            {
                if (!this.currentFrame.IsNotFullFrame)
                {
                    var actInt = (Int64)DateTime.UtcNow.Subtract(currentFrame.TimeStamp).TotalMilliseconds;
                    if (actInt < this.frameInterval)
                    {
                        Thread.Sleep((int)(this.frameInterval - actInt) / 2);
                    }
                }
                this.currentFrame = this.ElementFromRawFrame(bitmap);
                //event
                if (NewStereoFrameEvent != null)
                {
                    NewStereoFrameEvent(this, new NewStereoFrameEventArgs()
                        {
                            NewStereoFrame = this.currentFrame
                        });
                }
                ////
                this.isCurrentFrameGiven = false;
            }
        }

        protected StereoFrameSequenceElement ElementFromRawFrame(Bitmap bmp)
        {
            Image<Bgr, byte> rawFrame = new Image<Bgr, byte>(bmp);
            rawFrame.ROI = new Rectangle(0, 0, rawFrame.Width / 2, rawFrame.Height);

            var left = rawFrame.Copy();

            rawFrame.ROI = Rectangle.Empty;
            rawFrame.ROI = new Rectangle(rawFrame.Width / 2, 0, rawFrame.Width / 2, rawFrame.Height);
            var right = rawFrame.Copy();

            rawFrame.ROI = Rectangle.Empty;

            StereoFrameSequenceElement res = new StereoFrameSequenceElement()
            {
                RawFrame = rawFrame.ToBitmap(),
                LeftRawFrame = left.ToBitmap(),
                RightRawFrame = right.ToBitmap(),
                TimeStamp = DateTime.UtcNow
            };

            return res;
        }

        protected void Init()
        {
            this.currentFrame = new StereoFrameSequenceElement()
            {
                IsLeftFrameEmpty = true,
                IsRightFrameEmpty = true
            };
            this.emptyFrame = new StereoFrameSequenceElement()
            {
                IsLeftFrameEmpty = true,
                IsRightFrameEmpty = true
            };
        }

        override public StereoFrameSequenceElement GetCurrentFrame()
        {
            return this.currentFrame;
        }

        override public StereoFrameSequenceElement GetNextFrame()
        {
            if (this.isCurrentFrameGiven)
            {
                return this.emptyFrame;
            }
            else
            {
                lock (currentFrameLock)
                {
                    this.isCurrentFrameGiven = true;
                    return this.currentFrame;
                }
            }
        }

        override public bool IsFunctioning()
        {
            if (
                this.asyncVideoSource != null &&
                this.IsStarted &&
                !this.currentFrame.IsNotFullFrame)
            {
                return true;
            }
            return false;
        }

        override public bool CanChangeLeftCap()
        {
            return false;
        }

        override public bool CanChangeRightCap()
        {
            return false;
        }

        override public void ChangeLeftCap(int leftCapId)
        {
            //nothing
        }

        override public void ChangeRightCap(int rightCapId)
        {
            //nothing
        }

        override public bool StartStream()
        {
            try
            {
                this.asyncVideoSource.Start();
                this.IsStarted = true;
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        override public bool PauseStream()
        {
            if (this.IsStarted)
            {
                this.IsPaused = true;
            }
            return true;
        }

        override public bool StopStream()
        {
            try
            {
                this.asyncVideoSource.Stop();
                this.IsStarted = false;
                this.IsPaused = false;
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        override public bool CanRewindStream()
        {
            if (this.IsStarted)
            {
                this.asyncVideoSource.Start();
                this.IsPaused = false;
            }
            return true;
        }

        override public bool ResumeStream()
        {
            if (this.IsStarted)
            {
                this.asyncVideoSource.Start();
                this.IsPaused = false;
            }
            return true;
        }
    }
}
