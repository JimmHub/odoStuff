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

namespace EmguTest.Disparity
{
    public class DisparityBuilder
    {
        public Capture Capture { get; set; }
        public StereoSGBM StereoMapper { get; set; }
        public StereoBM StereoBMMapper { get; set; }
        public GpuStereoBM GpuStereoMapper { get; set; }
        public Bitmap CurrentFrame { get; protected set; }

        public DisparityBuilder(Capture cap)
        {
            this.Capture = cap;

            this.GpuStereoMapper = new GpuStereoBM(256, 9);

            this.StereoMapper = new StereoSGBM(
                minDisparity: 4,
                numDisparities: 16,
                blockSize: 0,
                p1: 0,
                p2: 0,
                disp12MaxDiff: 0,
                preFilterCap: 0,
                uniquenessRatio: 0,
                speckleWindowSize: 0,
                speckleRange: 0,
                mode: StereoSGBM.Mode.SGBM);

            this.StereoBMMapper = new StereoBM(STEREO_BM_TYPE.BASIC, 256);
        }

        public Bitmap GetNextDisparityMap()
        {
            Image<Bgr, byte> leftFrame;
            Image<Bgr, byte> rightFrame;

            this.GetNextLRPair(out leftFrame, out rightFrame);
            Image<Gray, short> dispMap = new Image<Gray, short>(width: leftFrame.Width, height: leftFrame.Height);
            GpuImage<Gray, byte> gpuDispMap = new GpuImage<Gray, byte>(cols: leftFrame.Width, rows: leftFrame.Height);
            var grayLeft = leftFrame.Convert<Gray, byte>();
            var grayRight = rightFrame.Convert<Gray, byte>();

            //this.StereoMapper.FindStereoCorrespondence(leftFrame.Convert<Gray, byte>(), rightFrame.Convert<Gray, byte>(), dispMap);
            //this.StereoBMMapper.FindStereoCorrespondence(leftFrame.Convert<Gray, byte>(), rightFrame.Convert<Gray, byte>(), dispMap);
            this.GpuStereoMapper.FindStereoCorrespondence(new GpuImage<Gray, byte>(grayLeft), new GpuImage<Gray, byte>(grayRight), gpuDispMap, null);
            return gpuDispMap.ToImage().ToBitmap();

            return dispMap.ToBitmap();
        }

        protected void GetNextLRPair(out Image<Bgr, byte> left, out Image<Bgr, byte> right)
        {
            var frame = this.Capture.QueryFrame();
            this.CurrentFrame = frame.ToBitmap();

            frame.ROI = new Rectangle(0, 0, frame.Width / 2, frame.Height);
            
            left = frame.Copy();

            frame.ROI = Rectangle.Empty;
            frame.ROI = new Rectangle(frame.Width / 2, 0, frame.Width / 2, frame.Height);
            right = frame.Copy();

            frame.ROI = Rectangle.Empty;
        }
    }
}
