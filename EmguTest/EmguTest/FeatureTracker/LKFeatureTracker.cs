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

namespace EmguTest
{
    public class LKFeatureTracker : FeatureTracker
    {
        public LKFeatureTracker(Capture cap)
            : base(cap)
        {
        }

        public int MaxFeaturesCount = 40;
        public double QualityLevel = 0.01;
        public double MinDistance = 1;
        public int BlockSize = 10;
        public Size WinSize = new Size(80, 80);
        public int PyrLevel = 4;
        public MCvTermCriteria PyrLkTerm = new MCvTermCriteria(100, 0.001);

        protected override void MainProcessing()
        {
            Image<Bgr, byte> previousRawFrame = this.Capture.QueryFrame();
            Image<Bgr, byte> currentRawFrame = this.Capture.QueryFrame();
            Image<Gray, byte> currGrayFrame = currentRawFrame.Convert<Gray, byte>();
            Image<Gray, byte> prevGrayFrame = previousRawFrame.Convert<Gray, byte>();

            Image<Gray, byte> currPyrBuffer = new Image<Gray, byte>(currGrayFrame.Width + 8, currGrayFrame.Height / 3, new Gray(1.0));
            Image<Gray, byte> prevPyrBuffer = new Image<Gray, byte>(currGrayFrame.Width + 8, currGrayFrame.Height / 3, new Gray(1.0));

            while (!this.ShouldStop && currentRawFrame != null)
            {
                prevGrayFrame = currGrayFrame;
                currGrayFrame = currentRawFrame.Convert<Gray, byte>();
                
                var prevFeaturesToTrack = this.GetFeaturesToTrack(prevGrayFrame);
                var currFeaturesToTrack = new PointF[prevFeaturesToTrack.Count()];

                var status = new Byte[prevFeaturesToTrack.Count()];
                var error = new float[prevFeaturesToTrack.Count()];
                
                OpticalFlow.PyrLK(
                    prevGrayFrame,
                    currGrayFrame,
                    prevFeaturesToTrack,
                    this.WinSize,
                    this.PyrLevel,
                    this.PyrLkTerm,
                    out currFeaturesToTrack,
                    out status,
                    out error);

                Bitmap bmp = currentRawFrame.ToBitmap();
                using (var g = Graphics.FromImage(bmp))
                {
                    for(int i = 0; i < prevFeaturesToTrack.Count(); ++i)
                    {
                        if (status[i] == 1)
                        {
                            g.DrawPie(Pens.Red, prevFeaturesToTrack[i].X, prevFeaturesToTrack[i].Y, 8, 8, 0, 360);
                            g.DrawLine(Pens.Red, prevFeaturesToTrack[i], currFeaturesToTrack[i]);
                        }
                    }
                }

                this.SetResultFrame(bmp);

                //end of iteration
                currentRawFrame = this.Capture.QueryFrame();
            }
            if (this.ShouldStop)
            {
                this.ShouldStop = false;
            }
        }

        private PointF[] GetFeaturesToTrack(Image<Gray, byte> image)
        {
            //var img = image;
            //var features = img.GoodFeaturesToTrack(this.MaxFeaturesCount, this.QualityLevel, this.MinDistance, this.BlockSize);
            //return features[0];
            int widthDiv = 20;
            int heightDiv = 20;

            List<PointF> res = new List<PointF>();
            for(int i = 0; i < widthDiv; ++i)
            {
                for (int j = 0; j < heightDiv; ++j)
                {
                    res.Add(new PointF(i * (float)image.Width / widthDiv, j * (float)image.Height / heightDiv));
                }
            }

            return res.ToArray();
        }
    }
}
