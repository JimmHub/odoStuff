using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.IO;
using System.Threading;

using Emgu.CV;
using Emgu.Util;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;

using EmguTest.MEMS;
using EmguTest.Odometry;
using DirectShowLib;
namespace EmguTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            //this.RunEmgu();
            this.Logger = new EmguTest.Logger.StdOutLogger();
            this.Logger.WriteLn("qwe");

            //testMEMS
            this.MEMSRTBLogger = new Logger.RichTextBoxLogger(this.logMEMSRichTextBox);
            
            //offline mems
            //this.MemsProvider = new TestMEMSProvider(this.accPath, this.magnetPath, this.gyroPath);
            
            //online mems
            this.MemsProvider = new DeviceRTWebMEMSProvider(4243, new Logger.EmptyLogger());
            this.OrientationCalc = new MEMSOrientationCalculator();
            try
            {
                //this.CamLeft = new Capture(this.CamLeftId);
            }
            catch
            {
                this.CamLeft = null;
            }

            try
            {
                //this.CamRight = new Capture(this.CamRightId);
            }
            catch
            {
                this.CamRight = null;
            }
            ////
            //init orientation calibration matrix
            this.opticFlowProcessor = new OpticFlowProcessor();
            this.orientCalibMatrix = new Matrix<double>(3, 3);
            this.orientCalibMatrix.SetIdentity();
            this.orientCalibMatrix = Utils.CvHelper.InverseMatrix(this.orientCalibMatrix);
            ////
            //iterative rotation matrix
            this.svdDiffRotMatrix = new Matrix<double>(3, 3);
            this.svdDiffRotMatrix.SetIdentity();
            ////
        }

        private void RunEmgu()
        {
            this.EmguMain = new EmguMain();
            this.EmguMain.Run();
        }

        private void OpenMEMSRenderForm()
        {
            if (this.memsRenderForm == null || this.memsRenderForm.IsDisposed)
            {
                this.memsRenderForm = new MEMSForm();
                this.memsRenderForm.Show();
            }
        }

        private void OpenVideoForm()
        {
            if (this.videoForm == null || this.videoForm.IsDisposed)
            {
                this.videoForm = new VideoForm();
                this.videoForm.Show();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.OpenMEMSRenderForm();
            this.OpenVideoForm();
            StereoCameraCalibrator calib = new StereoCameraCalibrator();
            //uncomment for calibration
            calib.Calibrate(@"C:\CodeStuff\cvproj\resources\calibImages", new Size(9, 6));
            // passing 0 gets zeroth webcam
            try
            {
                //cap = new Capture(0);
            }
            catch { }
            // adjust path to find your xml
            haar = new HaarCascade(haarPath);
            gpuCC = new Emgu.CV.GPU.GpuCascadeClassifier(this.haarPath);
            
          

            //for feature tracker uncomment this

            if (this.UseGPUCascade)
            {
                deviceSwitchButton.Text = "GPU";
            }
            else
            {
                deviceSwitchButton.Text = "CPU";
            }

            //dispBuilder
            //String stereoPath = @"C:\CodeStuff\cvproj\resources\video1384849670808.mp4";
            //String stereoPath = @"G:\0HowToTrainYourDragon\How to Train Your Dragon.3d.1080p.hsbs.mkv";
            String stereoPath = @"C:\CodeStuff\cvproj\resources\video1381158297548.mp4";
            Capture stereoCap = new Capture(stereoPath);
            
            this.DispBuilder = new Disparity.DisparityBuilder(stereoCap);
            ////

            //timers
            this.timer1.Enabled = false;
            this.timer2.Enabled = false;
            this.timer3.Enabled = false;
            this.memsTestOutputTimer.Enabled = true;
            this.stereoCapTimer.Enabled = false;
            ////

            this.FeatureTracker = new LKFeatureTracker(stereoCap);

            if (this.timer2.Enabled)
            {
                this.FeatureTracker.Run();
            }

            this.position3d = new MCvPoint3D64f(0, 0, 0);

            //init start trans coeffs
            changeTransCoeffsButton_Click(null, null);

            //init visual odometer
            this.visualOdometer = new VisualOdometer();

            //init camera calibrator
            this.cameraCalibrator = new CameraCalibrator();
            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //this.CPUDetect();
            this.DetectFaces(useGPU: this.UseGPUCascade);
            ++this.LastFpsCount;
            var now = (long)DateTime.UtcNow.Subtract(DateTime.MinValue).TotalMilliseconds;
            if ((now - this.LastMill) > 1000)
            {
                this.LastMill = now;
                this.fpsLabel.Text = this.LastFpsCount.ToString();
                this.LastFpsCount = 0;
            }
            ////
            ////
        }

        private Bitmap DrawFaceRects(Bitmap inBmp, Rectangle[] faces)
        {
            using (var g = Graphics.FromImage(inBmp))
            {

                foreach (var face in faces)
                {
                    g.DrawRectangle(Pens.Red, face);
                    //nextFrame.Draw(face, new Bgr(0, 0, 0), 3);
                }
            }
            return inBmp;
        }

        private void DetectFaces(bool useGPU)
        {
            using (Image<Bgr, byte> nextFrame = cap.QueryFrame())
            {
                if (nextFrame != null)
                {
                    // there's only one channel (greyscale), hence the zero index
                    //var faces = nextFrame.DetectHaarCascade(haar)[0];
                    Image<Gray, byte> grayframe = nextFrame.Convert<Gray, byte>();

                    var faces = this.CascadeDetect(
                        frame: grayframe,
                        useGPU: useGPU);

                    var bmp = nextFrame.ToBitmap();

                    pictureBox1.Image = this.DrawFaceRects(bmp, faces);
                    
                }
            }
        }

        private Rectangle[] CPUCascadeDetect(Image<Gray, byte> frame)
        {
            var faces = frame.DetectHaarCascade(
                                    haar,
                                    scaleFactor,
                                    minNeighbors,
                                    HAAR_DETECTION_TYPE.DO_CANNY_PRUNING,
                                    new Size(frame.Width / 8, frame.Height / 8)
                                    )[0];
            return faces.Select(x => x.rect).ToArray();
        }

        private Rectangle[] GPUCasecadeDetect(Image<Gray, byte> frame)
        {
            var faces = this.gpuCC.DetectMultiScale<Gray>(
                new Emgu.CV.GPU.GpuImage<Gray, byte>(frame),
                scaleFactor, minNeighbors,
                new Size(frame.Width / 8, frame.Height / 8));

            return faces;
        }

        private Rectangle[] CascadeDetect(bool useGPU, Image<Gray, byte> frame)
        {
            if (useGPU)
            {
                return this.GPUCasecadeDetect(frame);
            }
            else
            {
                return this.CPUCascadeDetect(frame);
            }
        }

        private void deviceSwitchButton_Click(object sender, EventArgs e)
        {
            if (this.UseGPUCascade)
            {
                this.UseGPUCascade = false;
                deviceSwitchButton.Text = "CPU";
            }
            else
            {
                this.UseGPUCascade = true;
                deviceSwitchButton.Text = "GPU";
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {   
            var res = this.FeatureTracker.ResultFrame;
            if(res != null)
            {
                this.pictureBox1.Image = res;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                this.FeatureTracker.Stop();
            }
            catch
            {
            }
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            var res = this.DispBuilder.GetNextDisparityMap();
            if (res != null)
            {
                this.pictureBox1.Image = res;
            }

            var rawRes = this.DispBuilder.CurrentFrame;
            if (rawRes != null)
            {
                this.pictureBox2.Image = rawRes;
            }

            this.RotateWpfContent();

        }

        private void RotateWpfContent()
        {
        }

        private void elementHost1_ChildChanged(object sender, ChildChangedEventArgs e)
        {

        }

        private bool IsActualReadingsSet(MEMSReadingsSet3f set)
        {
            if (set == null)
            {
                return false;
            }
            if (!set.IsNotEmpty())
            {
                return false;
            }
            if (set.TimeStampOrigI == 0)
            {
                return false;
            }
            return true;
        }

        private void ProcessMEMSReadings(MEMSReadingsSet3f nextReadings)
        {
            if (this.isOrientCalcInUse)
            {
                return;
            }
            this.isOrientCalcInUse = true;

            if (this.IsActualReadingsSet(nextReadings))
            {
                double gyroCoeff = 0.5;
                double accMagnetCoeff = (double)this.accMagnetFilterTrackBar.Value / this.accMagnetFilterTrackBar.Maximum;

                bool useAccMagnet = this.accMagnetCheckBox.Checked;
                bool useGyro = this.gyroCheckBox.Checked;
                bool useAdoptiveFilter = this.adoptiveFilterCheckBox.Checked;

                this.ReadingsTestOuptut(nextReadings);
                var orientMatr3f = this.OrientationCalc.GetAccMagnetOrientationMatrix(
                    newReadings: nextReadings,
                    useAccMagnet: useAccMagnet,
                    useGyroscope: useGyro,
                    useLowpassFilter: true,
                    useAdaptiveFiltering: useAdoptiveFilter,
                    accMagnetFilterCoeff: accMagnetCoeff,
                    gyroFilterCoeff: gyroCoeff);

                lock (this.memsOrientationMatrLock)
                {
                    this.memsRotMatr = orientMatr3f;
                }
                //var res = this.MulReadingsVect(orientMatr3f, nextReadings.AccVector3f, true);

                //log readings check
                //this.MEMSRTBLogger.WriteLn("readings check");
                //this.PrintVector(this.MEMSRTBLogger, res);
                //this.MEMSRTBLogger.WriteLn("");
                ////

                ////log matrix
                //this.MEMSRTBLogger.WriteLn("orientationMatrix:");
                //this.PrintMatrix(this.MEMSRTBLogger, orientMatr3f);
                //this.MEMSRTBLogger.WriteLn("");
                ////

            }

            this.isOrientCalcInUse = false;
        }

        private void memsTestOutputTimer_Tick(object sender, EventArgs e)
        {
            (this.MemsProvider as DeviceRTWebMEMSProvider).IsEagerNextReadings = false;
            var nextReadings = this.MemsProvider.GetNextReadingsSet();
            //
            //
            //this.RotateWpfContent();

            this.ProcessMEMSReadings(nextReadings);

            //points test
            MCvPoint3D32f pointA = new MCvPoint3D32f(0, 1, 0);
            MCvPoint3D32f pointB = new MCvPoint3D32f(1, 0, 0);
            var pointC = pointA.CrossProduct(pointB);

            ////
        }

        private bool IsMEMSRenderFormAccessible()
        {
            if (this.memsRenderForm == null)
            {
                return false;
            }
            if(this.memsRenderForm.IsDisposed)
            {
                return false;
            }
            return true;
        }

        //SEARCH: render MEMStransformation
        private void RenderOrientationTransformation(double[][] transformMatr3f)
        {
            if (this.IsMEMSRenderFormAccessible())
            {
                this.memsRenderForm.SetMEMSTransformationMatrix(transformMatr3f);
            }
        }

        private double[] MulReadingsVect(double[][] orientMatix, ReadingsVector3f readings, bool normalize = false)
        {
            Matrix<double> m = new Matrix<double>(3, 3);
            Matrix<double> v = new Matrix<double>(3, 1);

            for (int i = 0; i < 3; ++i)
            {
                for (int j = 0; j < 3; ++j)
                {
                    m.Data[i, j] = orientMatix[i][j];
                }
            }

            for (int i = 0; i < 3; ++i)
            {
                v.Data[i, 0] = readings.Values[i];
            }

            var res = m.Transpose().Mul(v);

            if (normalize)
            {
                var norm = res.Norm;
                for (int i = 0; i < 3; ++i)
                {
                    res.Data[i, 0] /= norm;
                }
            }

            return new double[] { res.Data[0, 0], res.Data[1, 0], res.Data[2, 0] };
        }

        private void PrintMatrix(Logger.ILogger logger, double[][] matrix)
        {
            for (int i = 0; i < matrix.Length; ++i)
            {
                for (int j = 0; j < matrix[i].Length; ++j)
                {
                    logger.Write(matrix[i][j].ToString() + "; ");
                }
                logger.WriteLn("");
            }
        }

        private void PrintVector(Logger.ILogger logger, double[] vector)
        {
            for (int i = 0; i < vector.Length; ++i)
            {
                logger.Write(vector[i].ToString() + "; ");
            }
            logger.WriteLn("");
        }

        private void ReadingsTestOuptut(MEMSReadingsSet3f nextReadings)
        {
            //nextReadings = this.MemsProvider.GetNextReadingsSet();
            //MEMS READINGS to enable - comment
            return;
            //
            Console.WriteLine("timestamp: " + nextReadings.TimeStampOrigI.ToString());
            Console.WriteLine("acc: "
                + " Xa=" + nextReadings.AccVector3f.Values[0]
                + " Ya=" + nextReadings.AccVector3f.Values[1]
                + " Za=" + nextReadings.AccVector3f.Values[2]
                + "\nmagnet: "
                + " Xm=" + nextReadings.MagnetVector3f.Values[0]
                + " Ym=" + nextReadings.MagnetVector3f.Values[1]
                + " Zm=" + nextReadings.MagnetVector3f.Values[2]
                + "\ngyro: "
                + " Xg=" + nextReadings.GyroVector3f.Values[0]
                + " Yg=" + nextReadings.GyroVector3f.Values[1]
                + " Zg=" + nextReadings.GyroVector3f.Values[2]
                + "\n\n");
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void stereoCapTimer_Tick(object sender, EventArgs e)
        {
            Image<Bgr, byte> leftFrame = null;
            Image<Bgr, byte> rightFrame = null;
            if (this.CamLeft != null)
            {
                try
                {
                    leftFrame = this.CamLeft.QueryFrame();
                }
                catch 
                {
                    leftFrame = null;
                }
            }

            if (this.CamRight != null)
            {
                try
                {
                    rightFrame = this.CamRight.QueryFrame();
                }
                catch 
                {
                    rightFrame = null;
                }
            }

            if (leftFrame != null)
            {
                this.pictureBox1.Image = leftFrame.ToBitmap();
            }
            if (rightFrame != null)
            {
                this.pictureBox2.Image = rightFrame.ToBitmap();
            }

        }

        private void accMagnetFilterTrackBar_Scroll(object sender, EventArgs e)
        {

        }

        private void monoCameraCalibrateButton_Click(object sender, EventArgs e)
        {

            this.calibrationStatusLabel.Invoke((MethodInvoker)delegate { this.Text = "calibrating..."; });
            
            this.MonoCalibTestFiles = Directory.GetFiles(this.MonoCalibTestFolder).ToList();
            var calibData = new MonoCameraCalibrationData()
            {
                SquareSize = 1.0,
                SampleImagesNames = this.MonoCalibTestFiles,
                BoardSquareSize = new Size(9, 6)
            };
            var cameraRes = this.cameraCalibrator.CalibrateMono(calibData);

            this.MonoCameraParams = cameraRes;
            //var img = new Image<Bgr, byte>(images[0]);

            //var undistRes = cameraRes.IntrinsicCameraParameters.Undistort(img);

            this.calibrationStatusLabel.Text = "calibrated!";
        }

        private void nextCalibButton_Click(object sender, EventArgs e)
        {
            if (this.MonoCameraParams != null && this.MonoCalibTestFiles != null)
            {
                ++this.MonoCalibIdx;
                if (this.MonoCalibIdx > this.MonoCalibTestFiles.Count - 1)
                {
                    this.MonoCalibIdx = this.MonoCalibTestFiles.Count - 1;
                }

                this.RenderMonoCalibTestImages();
            }
        }

        private void PrevCalibButton_Click(object sender, EventArgs e)
        {
            if (this.MonoCameraParams != null && this.MonoCalibTestFiles != null)
            {
                --this.MonoCalibIdx;
                if (this.MonoCalibIdx < 0)
                {
                    this.MonoCalibIdx = 0;
                }

                this.RenderMonoCalibTestImages();
            }
        }

        private void RenderMonoCalibTestImages()
        {
            this.imageIdxLabel.Text = this.MonoCalibIdx.ToString();

            var rawImg = new Image<Bgr, byte>(this.MonoCalibTestFiles[this.MonoCalibIdx]);
            var undistImg = this.MonoCameraParams.IntrinsicCameraParameters.Undistort(rawImg);

            var rawBmp = rawImg.ToBitmap();
            var undistBmp = undistImg.ToBitmap();

            this.calibPictureBoxOriginal.Image = rawBmp;
            this.calibPictureBoxUndist.Image = undistBmp;
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {

        }

        private void stereoCalibrateButton_Click(object sender, EventArgs e)
        {
            this.StereoCalibIdx = 0;
            this.stereoCalibrationStatusLabel.Invoke((MethodInvoker)delegate { stereoCalibrationStatusLabel.Text = "calibrating..."; });
            //dirty hack
            //Thread.Sleep(2000);
            var stereoCalibTestFiles = this.ParseStereoFolder(this.stereoCalibFolderTextBox.Text);
            var stereoCalibData = new StereoCameraCalibrationFilesData()
            {
                SquareSize = 1.0,
                SampleImagesNames = stereoCalibTestFiles,
                BoardSquareSize = new Size(9, 6)
            };
            this.StereoCalibData = stereoCalibData;
            this.StereoCameraParams = this.cameraCalibrator.CalibrateStereo(stereoCalibData);

            this.stereoCalibrationStatusLabel.Text = "calibrated";
            if (this.StereoCameraParams != null && this.StereoCalibData != null)
            {
                this.RenderStereoCalibTestImages();
            }
        }

        private List<Tuple<String, String>> ParseStereoFolder(String path)
        {
            var res = new List<Tuple<String, String>>();

            var files = Directory.GetFiles(path);

            var leftFiles = files.Where(x => x.Contains("left")).OrderBy(x => x).ToList();
            var rightFiles = files.Where(x => x.Contains("right")).OrderBy(x => x).ToList();

            var count = Math.Min(leftFiles.Count(), rightFiles.Count());
            for (int i = 0; i < count; ++i)
            {
                res.Add(new Tuple<String, String>(leftFiles[i], rightFiles[i]));
            }

            return res;
        }

        private void stereoCalibPrevButton_Click(object sender, EventArgs e)
        {
            if (this.StereoCameraParams != null && this.StereoCalibData != null)
            {
                --this.StereoCalibIdx;
                if (this.StereoCalibIdx < 0)
                {
                    this.StereoCalibIdx = 0;
                }

                this.RenderStereoCalibTestImages();
            }
        }

        private void stereoCalibNextButton_Click(object sender, EventArgs e)
        {
            if (this.StereoCameraParams != null && this.StereoCalibData != null)
            {
                ++this.StereoCalibIdx;
                if (this.StereoCalibIdx > this.StereoCalibData.GetCalibListSize() - 1)
                {
                    this.StereoCalibIdx = this.StereoCalibData.GetCalibListSize() - 1;
                }

                this.RenderStereoCalibTestImages();
            }
        }

        private void RenderStereoCalibTestImages()
        {
            this.stereoImageNumLabel.Text = this.StereoCalibIdx.ToString();

            var nextStereoFrame = this.StereoCalibData.GetFrameById(StereoCalibIdx);
            var leftRawFrame = nextStereoFrame.LeftRawFrame;
            var rightRawFrame = nextStereoFrame.RightRawFrame;

            var leftOriginalImg = new Image<Bgr, byte>(leftRawFrame);
            var rightOriginalImg = new Image<Bgr, byte>(rightRawFrame);

            //image dispose
            if (leftStereoOriginalPictureBox.Image != null)
            {
                this.leftStereoOriginalPictureBox.Image.Dispose();
            }
            if (rightStereoOriginalPictureBox.Image != null)
            {
                this.rightStereoOriginalPictureBox.Image.Dispose();
            }
            //
            this.leftStereoOriginalPictureBox.Image = new Bitmap(leftRawFrame);
            this.rightStereoOriginalPictureBox.Image = new Bitmap(rightRawFrame);

            var leftCalibImg = this.StereoCameraParams.LeftIntrinsicCameraParameters.Undistort(leftOriginalImg);
            var rightCalibImg = this.StereoCameraParams.RightIntrinsicCameraParameters.Undistort(rightOriginalImg);

            var resLeftImg = leftCalibImg;
            var resRightImg = rightCalibImg;
            if (stereoCalibUseRectificationCheckBox.Checked)
            {
                //remap
                var leftCalibRectImg = new Image<Bgr, byte>(leftOriginalImg.ToBitmap());
                var rightCalibRectImg = new Image<Bgr, byte>(rightOriginalImg.ToBitmap());
                if (this.useMapTransformCheckBox.Checked)
                {
                    double mapXShift;
                    double mapYShift;
                    double mapScale;
                    this.GetMapTransformParameters(out mapXShift, out mapYShift, out mapScale);
                    CvInvoke.cvRemap(leftOriginalImg, leftCalibRectImg, this.StereoCameraParams.LeftMapX.Add((float)mapXShift).Mul(mapScale), this.StereoCameraParams.LeftMapY.Add((float)mapYShift).Mul(mapScale), (int)INTER.CV_INTER_LINEAR | (int)WARP.CV_WARP_FILL_OUTLIERS, new MCvScalar(0));
                    CvInvoke.cvRemap(rightOriginalImg, rightCalibRectImg, this.StereoCameraParams.RightMapX.Add((float)mapXShift).Mul(mapScale), this.StereoCameraParams.RightMapY.Add((float)mapYShift).Mul(mapScale), (int)INTER.CV_INTER_LINEAR | (int)WARP.CV_WARP_FILL_OUTLIERS, new MCvScalar(0));
                }
                else
                {
                    CvInvoke.cvRemap(leftOriginalImg, leftCalibRectImg, this.StereoCameraParams.LeftMapX, this.StereoCameraParams.LeftMapY, (int)INTER.CV_INTER_LINEAR | (int)WARP.CV_WARP_FILL_OUTLIERS, new MCvScalar(0));
                    CvInvoke.cvRemap(rightOriginalImg, rightCalibRectImg, this.StereoCameraParams.RightMapX, this.StereoCameraParams.RightMapY, (int)INTER.CV_INTER_LINEAR | (int)WARP.CV_WARP_FILL_OUTLIERS, new MCvScalar(0));
                }
                //leftCalibRectImg = leftCalibRectImg.Resize(0.01, INTER.CV_INTER_LINEAR);
                //rightCalibRectImg = rightCalibRectImg.Resize(0.01, INTER.CV_INTER_LINEAR);
                ////
                resLeftImg = leftCalibRectImg;
                resRightImg = rightCalibRectImg;
            }

            this.leftStereoCalibPictureBox.Image = resLeftImg.ToBitmap();
            this.rightStereoCalibPictureBox.Image = resRightImg.ToBitmap();

            //disposition
            resLeftImg.Dispose();
            resRightImg.Dispose();
            leftCalibImg.Dispose();
            rightCalibImg.Dispose();
            leftOriginalImg.Dispose();
            rightOriginalImg.Dispose();
            nextStereoFrame = null;
            ////

            this.StereoDrawLines();
        }

        private void GetMapTransformParameters(out double xShift, out double yShift, out double scale)
        {
            double resXShift = 0.0;
            double resYShift = 0.0;
            double resScale = 1.0;

            try
            {
                resXShift = double.Parse(this.mapXShiftTextBox.Text);
            }
            catch
            {
                resXShift = 0.0;
                this.mapXShiftTextBox.Text = resXShift.ToString();
            }

            try
            {
                resYShift = double.Parse(this.mapYShiftTextBox.Text);
            }
            catch
            {
                resYShift = 0.0;
                this.mapYShiftTextBox.Text = resYShift.ToString();
            }

            try
            {
                resScale = double.Parse(this.mapScaleTextBox.Text);
            }
            catch
            {
                resScale = 1.0;
                this.mapScaleTextBox.Text = resScale.ToString();
            }

            xShift = resXShift;
            yShift = resYShift;
            scale = resScale;
        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void StereoDrawLines()
        {
            int linesCount = 10;
            if (this.stereoCalibDrawLinesCheckBox.Checked)
            {
                List<PictureBox> boxes = new List<PictureBox>(new PictureBox[]
                {
                    leftStereoOriginalPictureBox,
                    leftStereoCalibPictureBox,
                    rightStereoOriginalPictureBox,
                    rightStereoCalibPictureBox
                });

                foreach (var box in boxes)
                {
                    var g = Graphics.FromImage(box.Image);

                    int step = box.Image.Height / linesCount;

                    for (int i = 0; i < linesCount; ++i)
                    {
                        g.DrawLine(Pens.Red, new Point(0, i * step), new Point(box.Image.Width, i * step));
                    }
                    //g.Save();
                }
            }
        }

        private void stereoCalibDrawLinesCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (this.StereoCameraParams != null && this.StereoCalibData != null)
            {
                this.RenderStereoCalibTestImages();
            }
        }

        private void stereoCalibUseRectificationCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (this.StereoCameraParams != null && this.StereoCalibData != null)
            {
                this.RenderStereoCalibTestImages();
            }
        }

        //SEARCH: init video provider
        private void startStereoCapButton_Click(object sender, EventArgs e)
        {
            if (this.camCapRadioButton.Checked)
            {
                this.InitStereoVideoStreamFromCamCap();
            }
            else if (this.fileCapRadioButton.Checked)
            {
                this.InitStereoVideoStreamFromFileCap();
            }
            else if (this.stereoPictureRadioButton.Checked)
            {
                this.InitStereoVideoStreamFromStereoPictures();
            }

            if (this.StereoVideoStreamProvider != null)
            {
                this.StereoVideoStreamProvider.StartStream();
                //event test
                if (!this.isNewStereoFrameEventSinged)
                {
                    this.StereoVideoStreamProvider.NewStereoFrameEvent += StereoVideoStreamProvider_NewStereoFrameEvent;
                }
                ////
                this.stereoStreamRenderTimer.Enabled = true;
            }
        }

        private void InitStereoVideoStreamFromFileCap()
        {
            String fileName = this.stereoFileNameTextBox.Text;
            if (File.Exists(fileName))
            {
                this.StereoVideoStreamProvider = new VideoSource.StereoGeminateAForgeFileVideoStreamProvider(fileName, (int)(1.0 / 30 * 1000));
            }
            else
            {
                MessageBox.Show("no such file exists: \"" + fileName + "\"");
            }
        }

        private void InitStereoVideoStreamFromCamCap()
        {
            int leftCapId = 0;
            int rightCapId = 0;
            try
            {
                leftCapId = Int32.Parse(this.leftCaptureTextBox.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while creating leftCap from cap number:\n" + ex.Message);
            }

            try
            {
                rightCapId = Int32.Parse(this.rightCaptureTextBox.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while creating rightCap from cap number:\n" + ex.Message);
            }
            this.StereoVideoStreamProvider = new VideoSource.StereoCamVideoStreamProvider(
                leftCapId: leftCapId,
                rightCapId: rightCapId);
        }

        private void InitStereoVideoStreamFromStereoPictures()
        {
            var stereoPath = this.stereoImgPathTextBox.Text;
            var files = Directory.GetFiles(stereoPath);
            var leftPath = files.Where(x => x.Contains("_left")).First();
            var rightPath = files.Where(x => x.Contains("_right")).First();

            this.StereoVideoStreamProvider = new VideoSource.StereoStaticPicturesAsVideoStreamProvider(leftPath, rightPath, (int)(1000.0 / 30));
        }

        void StereoVideoStreamProvider_NewStereoFrameEvent(object sender, VideoSource.NewStereoFrameEventArgs e)
        {
            if (this.isNewStereoFrameInProcess)
            {
                return;
            }
            this.isNewStereoFrameInProcess = true;
            
            if (this.StereoVideoStreamProvider.IsFunctioning())
            {
                this.StereoStreamFrameRender(e.NewStereoFrame);
            }

            this.isNewStereoFrameInProcess = false;
        }

        private void ChangeLeftCamCapStereoStreamProvider(int cap)
        {
            if (this.StereoVideoStreamProvider.CanChangeLeftCap())
            {
                try
                {
                    this.StereoVideoStreamProvider.ChangeLeftCap(cap);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error while changing leftCap from cap number:\n" + ex.Message);
                }
                
            }
        }

        private void ChangeRightCamCapStereoStreamProvider(int cap)
        {
            if (this.StereoVideoStreamProvider.CanChangeRightCap())
            {
                try
                {
                    this.StereoVideoStreamProvider.ChangeRightCap(cap);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error while changing rightCap from cap number:\n" + ex.Message);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void changeLeftCamCapButton_Click(object sender, EventArgs e)
        {
            if (this.StereoVideoStreamProvider != null)
            {
                try
                {
                    this.ChangeLeftCamCapStereoStreamProvider(Int32.Parse(this.leftCaptureTextBox.Text));
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error while changing leftCap from cap number:\n" + ex.Message);
                }
            }
        }

        private void changeRightCamCapButton_Click(object sender, EventArgs e)
        {
            if (this.StereoVideoStreamProvider != null)
            {
                try
                {
                    this.ChangeRightCamCapStereoStreamProvider(Int32.Parse(this.rightCaptureTextBox.Text));
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error while changing rightCap from cap number:\n" + ex.Message);
                }
            }
        }

        private void stereoStreamRenderTimer_Tick(object sender, EventArgs e)
        {
            //old timer method, now in newstereoframe event handler, uncomment to restore
            //if (this.StereoVideoStreamProvider.IsFunctioning())
            //{
            //    this.StereoStreamFrameRender(this.StereoVideoStreamProvider.GetCurrentFrame());
            //}
            
        }

        private double GetTranslationRenderCoeffX()
        {
            return this.transCoeffX;
        }

        private double GetTranslationRenderCoeffY()
        {
            return this.transCoeffY;
        }

        private void RenderTranslatoin(MCvPoint3D64f position)
        {
            double xScale = this.GetTranslationRenderCoeffX();
            double yScale = this.GetTranslationRenderCoeffY();
            Bitmap bmp = new Bitmap(1000, 1000);
            double xOffset = bmp.Width / 2;
            double yOffset = bmp.Height / 2;
            var pointSize = new Size(bmp.Width / 100, bmp.Height / 100);
            var g = Graphics.FromImage(bmp);
            g.Clear(Color.White);

            g.DrawRectangle(Pens.Red, new Rectangle(new Point((int)(position.x / xScale + xOffset), (int)(position.z / yScale + yOffset)), pointSize));

            this.videoForm.RenderToStuffPictureBox2(bmp);
        }

        //SEARCH: RENDER STEREO FRAME TO VIDEO FORM
        private void StereoStreamFrameRender(VideoSource.StereoFrameSequenceElement stereoFrame)
        {
            this.UpdateCurPrevMEMSOrient();
            if (!stereoFrame.IsNotFullFrame)
            {
                var leftImg = new Image<Bgr, byte>(stereoFrame.LeftRawFrame);
                var rightImg = new Image<Bgr, byte>(stereoFrame.RightRawFrame);
                Bitmap stuff1Bmp = null;

                if (this.useCalibratedStereoRenderCheckBox.Checked)
                {
                    if (this.StereoCameraParams != null)
                    {
                        var tmpLeft = this.StereoCameraParams.LeftIntrinsicCameraParameters.Undistort(leftImg);
                        var tmpRight = this.StereoCameraParams.RightIntrinsicCameraParameters.Undistort(rightImg);

                        CvInvoke.cvRemap(tmpLeft, leftImg, this.StereoCameraParams.LeftMapX, this.StereoCameraParams.LeftMapY, (int)INTER.CV_INTER_LINEAR | (int)WARP.CV_WARP_FILL_OUTLIERS, new MCvScalar(0));
                        CvInvoke.cvRemap(tmpRight, rightImg, this.StereoCameraParams.RightMapX, this.StereoCameraParams.RightMapY, (int)INTER.CV_INTER_LINEAR | (int)WARP.CV_WARP_FILL_OUTLIERS, new MCvScalar(0));

                        //Image<Gray, short> dispImg; 
                        //var points = this.Get3DFeatures(this.StereoCameraParams, stereoFrame, out dispImg);
                        //var centroid = this.GetPoint3DCloudCentroid(points);
                        //Console.WriteLine("Centr: {0}, {1}, {2};", centroid.x, centroid.y, centroid.z);
                        //this.videoForm.RenderStereoFrame(dispImg.ToBitmap(), null);
                    }

                }

                var leftGrayImg = leftImg.Convert<Gray, byte>();
                var rightGrayImg = rightImg.Convert<Gray, byte>();

                Bitmap leftFrameRender;
                Bitmap rightFrameRender;

                if (this.renderGrayCheckBox.Checked)
                {
                    leftFrameRender = new Bitmap(leftGrayImg.ToBitmap());
                    rightFrameRender = new Bitmap(rightGrayImg.ToBitmap());
                }
                else
                {
                    leftFrameRender = new Bitmap(leftImg.ToBitmap());
                    rightFrameRender = new Bitmap(rightImg.ToBitmap());
                }

                if (this.showDepthMapCheckBox.Checked)
                {
                    //var features = this.opticFlowProcessor.GetFeaturesToTrack(
                    //    stereoFrame: frame,
                    //    useGpu: true);
                    var depthMap = this.opticFlowProcessor.GetDispMap(leftGrayImg, rightGrayImg, this.useGPUCheckBox.Checked, this.GetParametersForStereoMapSolver(this.useGPUCheckBox.Checked));
                    stuff1Bmp = depthMap.ToBitmap();
                    
                    //update frame
                    this.prevStereoDepthFrame = this.currStereoDepthFrame;
                    this.currStereoDepthFrame = new DataSource.OpticFlowFrameContainer()
                    {
                        DepthMapImg = depthMap,
                        StereoFrame = new VideoSource.StereoFrameSequenceElement(stereoFrame)
                    };
                    ////
                }

                //try to use odometry
                if (this.perfOdometryCheckBox.Checked)
                {
                    if (this.StereoCameraParams != null)
                    {
                        var rotMatrix = new Matrix<double>(3, 3);
                        rotMatrix.SetIdentity();
                        var rotMatr = Utils.CvHelper.MatrixToArray(rotMatrix);
                        rotMatrix.Dispose();
                        if (this.prevMEMSRotMatr != null && this.currentMEMSRotMatr != null)
                        {
                            rotMatr = this.OrientationCalc.GetRotationMatrixBetweenTwoStates(this.prevMEMSRotMatr, this.currentMEMSRotMatr, this.orientCalibMatrix);
                        }
                        List<PointF> currFreatures;
                        List<PointF> prevFeatures;
                        Matrix<double> resRotation;
                        var featuresToTrackParams = this.GetVisualOdometerFeaturesToTrackParams();
                        var featuresOpticFlowParams = this.GetVisualOdometerFeaturesOpticFlowParams();
                        var tDiff = this.visualOdometer.GetTranslationAndRotation(
                            rotMatrArray: rotMatr,
                            prevFrame: this.prevStereoDepthFrame,
                            currFrame: this.currStereoDepthFrame,
                            cameraParams: this.StereoCameraParams,
                            currFeaturesList: out currFreatures,
                            prevFeaturesList: out prevFeatures,
                            resRotation: out resRotation,
                            featuresToTrackParams: featuresToTrackParams,
                            featuresOpticFlowParams: featuresOpticFlowParams
                            );

                        if (resRotation != null)
                        {
                            this.svdDiffRotMatrix = resRotation.Mul(this.svdDiffRotMatrix);
                        }

                        if (tDiff != null)
                        {
                            if (!(double.IsNaN(tDiff.Value.x) || double.IsNaN(tDiff.Value.y) || double.IsNaN(tDiff.Value.z)))
                            {
                                this.position3d.x += tDiff.Value.x;
                                this.position3d.y += tDiff.Value.y;
                                this.position3d.z += tDiff.Value.z;

                                this.RenderTranslatoin(this.position3d);
                            }
                            Console.WriteLine("TRANSLATION: X={0}; Y={1}; Z={2}", tDiff.Value.x, tDiff.Value.y, tDiff.Value.z);
                            Console.WriteLine("POSITION: X={0}; Y={1}; Z={2}", position3d.x, position3d.y, position3d.z);
                        }
                        if (this.renderFraturesCheckBox.Checked)
                        {
                            if (currFreatures != null && prevFeatures != null)
                            {
                                var dotSize = new Size(10, 10);
                                var g = Graphics.FromImage(leftFrameRender);
                                for (int i = 0; i < currFreatures.Count; ++i)
                                {
                                    g.DrawEllipse(Pens.Red, currFreatures[i].X - dotSize.Width / 2, currFreatures[i].Y - dotSize.Height / 2, dotSize.Width, dotSize.Height);
                                    g.DrawLine(Pens.Red, currFreatures[i], prevFeatures[i]);
                                }
                            }
                        }
                    }
                }
                ////
                this.memsRenderForm.Invoke((MethodInvoker)delegate { this.RenderOrientationTransformation(Utils.CvHelper.MatrixToArray(this.svdDiffRotMatrix)); });
                //general lr render
                this.videoForm.RenderStereoFrame(leftFrameRender, rightFrameRender);

                if (stuff1Bmp != null)
                {
                    this.videoForm.RenderToStuffPictureBox1(stuff1Bmp);
                }
                stereoFrame.Dispose();
            }
        }

        private VisualOdometerFeaturesToTrackParams GetVisualOdometerFeaturesToTrackParams()
        {
            return new VisualOdometerFeaturesToTrackParamsST()
            {
                MaxFeaturesCount = 400,
                QualityLevel = 0.01,
                MinDistance = 1,
                BlockSize = 10
            };
        }

        private VisualOdometerFeaturesOpticFlowParams GetVisualOdometerFeaturesOpticFlowParams()
        {
            return new VisualOdometerFeaturesOpticFlowParamsLK()
            {
                WinSize = new Size(80, 80),
                PyrLevel = 4,
                PyrLkTerm = new MCvTermCriteria(100, 0.001)
            };
        }

        //SEARCH: depth map solver parameters
        private DispMapFounderParameters GetParametersForStereoMapSolver(bool useGpu)
        {
            if (useGpu)
            {
                return this.GetParametersForStereoMapSolverGPU();
            }
            else
            {
                return this.GetParametersForStereoMapSolverCPU();
            }
        }

        private DispMapFounderParameters GetParametersForStereoMapSolverGPU()
        {
            int numDisparities = GetSliderValue(Num_Disparities);
            int SAD = GetSliderValue(SAD_Window);
            //no so a hack  
            if (SAD == 1)
            {
                SAD = 3;
            }
            //
            return new GpuStereoBMDispMapFounderParameters()
            {
                NumberOfDisparities = numDisparities,
                BlockSize = SAD
            };
        }

        private DispMapFounderParameters GetParametersForStereoMapSolverCPU()
        {
            int numDisparities = GetSliderValue(Num_Disparities);
            int minDispatities = GetSliderValue(Min_Disparities);
            int SAD = GetSliderValue(SAD_Window);
            //TODO: understand this
            int P1 = 8 * 1 * SAD * SAD;//GetSliderValue(P1_Slider);
            int P2 = 32 * 1 * SAD * SAD;//GetSliderValue(P2_Slider);
            int disp12MaxDiff = GetSliderValue(Disp12MaxDiff);
            int PreFilterCap = GetSliderValue(pre_filter_cap);
            int UniquenessRatio = GetSliderValue(uniquenessRatio);
            int SpeckleWindow = GetSliderValue(Speckle_Window);
            int SpeckleRange = GetSliderValue(specklerange);

            return new StereoSGBMDispMapFounderParameters()
            {
                MinDisparity = minDispatities,
                NumDisparities = numDisparities,
                BlockSize = SAD,
                P1 = P1,
                P2 = P2,
                Disp12MaxDiff = disp12MaxDiff,
                PreFilterCap = PreFilterCap,
                UniquenessRatio = SpeckleRange,
                SpeckleWindowSize = SpeckleWindow,
                SpeckleRange = SpeckleRange,
                Mode = StereoSGBM.Mode.SGBM
            };
        }

        private int GetSliderValue(TrackBar Control)
        {
            if (Control.InvokeRequired)
            {
                try
                {
                    return (int)Control.Invoke(new Func<int>(() => GetSliderValue(Control)));
                }
                catch (Exception ex)
                {
                    return 0;
                }
            }
            else
            {
                return Control.Value;
            }
        }

        private MCvPoint3D64f GetPoint3DCloudCentroid(MCvPoint3D32f[] points)
        {
            double resX = 0;
            double resY = 0;
            double resZ = 0;
            double maxZ = 10000;
            foreach (var p in points)
            {
                if (p.z != maxZ)
                {
                    resX += p.x;
                    resY += p.y;
                    resZ += p.z;
                }
            }
            var count = points.Count();
            resX /= count;
            resY /= count;
            resZ /= count;

            return new MCvPoint3D64f(resX, resY, resZ);
        }

        private Image<Gray, short> GetDispMap(VideoSource.StereoFrameSequenceElement stereoFrame)
        {
            int numDisparities = GetSliderValue(Num_Disparities);
            int minDispatities = GetSliderValue(Min_Disparities);
            int SAD = GetSliderValue(SAD_Window);
            int P1 = 8 * 1 * SAD * SAD;//GetSliderValue(P1_Slider);
            int P2 = 32 * 1 * SAD * SAD;//GetSliderValue(P2_Slider);
            int disp12MaxDiff = GetSliderValue(Disp12MaxDiff);
            int PreFilterCap = GetSliderValue(pre_filter_cap);
            int UniquenessRatio = GetSliderValue(uniquenessRatio);
            int SpeckleWindow = GetSliderValue(Speckle_Window);
            int SpeckleRange = GetSliderValue(specklerange);

            using (var gpuSBM = new Emgu.CV.GPU.GpuStereoBM(numDisparities, SAD))
            using (StereoSGBM stereoSolver = new StereoSGBM(
                            minDisparity: minDispatities,
                            numDisparities: numDisparities,
                            blockSize: SAD,
                            p1: P1,
                            p2: P2,
                            disp12MaxDiff: disp12MaxDiff,
                            preFilterCap: PreFilterCap,
                            uniquenessRatio: UniquenessRatio,
                            speckleRange: SpeckleRange,
                            speckleWindowSize: SpeckleWindow,
                            mode: StereoSGBM.Mode.SGBM
                            ))
            using (var leftImg = new Image<Gray, byte>(stereoFrame.LeftRawFrame))
            using (var rightImg = new Image<Gray, byte>(stereoFrame.RightRawFrame))
            using (var dispImg = new Image<Gray, short>(leftImg.Size))
            using (var gpuLeftImg = new Emgu.CV.GPU.GpuImage<Gray, byte>(leftImg))
            using (var gpuRightImg = new Emgu.CV.GPU.GpuImage<Gray, byte>(rightImg))
            using (var gpuDispImg = new Emgu.CV.GPU.GpuImage<Gray, byte>(leftImg.Size))
            {
                var dispMap = new Image<Gray, short>(leftImg.Size);
                //CPU
                //stereoSolver.FindStereoCorrespondence(leftImg, rightImg, dispImg);
                //dispMap = dispImg.Convert<Gray, short>();
                //
                //GPU
                gpuSBM.FindStereoCorrespondence(gpuLeftImg, gpuRightImg, gpuDispImg, null);
                dispMap = gpuDispImg.ToImage().Convert<Gray, short>();
                //

                return dispMap;
            }
        }

        private MCvPoint3D32f[] Get3DFeatures(StereoCameraParams stereoParams, VideoSource.StereoFrameSequenceElement stereoFrame, out Image<Gray, short> disparityImg)
        {
            using (var gpuSBM = new Emgu.CV.GPU.GpuStereoBM(128, 19))
            using (StereoSGBM stereoSolver = new StereoSGBM(
                            minDisparity: 0,
                            numDisparities: 32,
                            blockSize: 0,
                            p1: 0,
                            p2: 0,
                            disp12MaxDiff: 0,
                            preFilterCap: 0,
                            uniquenessRatio: 0,
                            speckleRange: 0,
                            speckleWindowSize: 0,
                            mode: StereoSGBM.Mode.HH
                            ))
            using (var leftImg = new Image<Gray, byte>(stereoFrame.LeftRawFrame))
            using (var rightImg = new Image<Gray, byte>(stereoFrame.RightRawFrame))
            using (var dispImg = new Image<Gray, short>(leftImg.Size))
            using (var gpuLeftImg = new Emgu.CV.GPU.GpuImage<Gray, byte>(leftImg))
            using (var gpuRightImg = new Emgu.CV.GPU.GpuImage<Gray, byte>(rightImg))
            using (var gpuDispImg = new Emgu.CV.GPU.GpuImage<Gray, byte>(leftImg.Size))
            {
                var dispMap = new Image<Gray, short>(leftImg.Size);
                //CPU
                //stereoSolver.FindStereoCorrespondence(leftImg, rightImg, dispImg);
                //dispMap = dispImg.Convert<Gray, short>();
                //
                //GPU
                gpuSBM.FindStereoCorrespondence(gpuLeftImg, gpuRightImg, gpuDispImg, null);
                dispMap = gpuDispImg.ToImage().Convert<Gray, short>();
                //

                var points = PointCollection.ReprojectImageTo3D(dispMap, stereoParams.Q);
                disparityImg = dispMap;
                return points;
            }
        }

        private void TestMethod()
        {
               
        }

        private void pauseStereoCapButton_Click(object sender, EventArgs e)
        {
            if (this.StereoVideoStreamProvider != null)
            {
                //this.StereoVideoStreamProvider.PauseStream();
                //TODO: fix this dirty hack
                this.stereoStreamRenderTimer.Enabled = false;
            }
        }

        private void resumeStereoCapButton_Click(object sender, EventArgs e)
        {
            if (this.StereoVideoStreamProvider != null)
            {
                //this.StereoVideoStreamProvider.ResumeStream();
                //TODO: fix this dirty hack
                this.stereoStreamRenderTimer.Enabled = true;
            }
        }

        private void grabFrameForCalibrationButton_Click(object sender, EventArgs e)
        {
            while (this.isNewStereoFrameInProcess)
            {
            }
            this.isNewStereoFrameInProcess = true;
            
            var frame = this.StereoVideoStreamProvider.GetCurrentFrame();
            if (frame.IsNotFullFrame)
            {
                return;
            }
            this.StereoCalibrationGrabbedList.Add(new VideoSource.StereoFrameSequenceElement()
            {
                LeftRawFrame = (Bitmap)frame.LeftRawFrame.Clone(),
                RightRawFrame = (Bitmap)frame.RightRawFrame.Clone(),
                TimeStamp = frame.TimeStamp
            });
            this.RenderStereoCalibGrabbedListCount();
            this.isNewStereoFrameInProcess = false;
        }

        private void clearCalibrationListButton_Click(object sender, EventArgs e)
        {
            this.StereoCalibrationGrabbedList.Clear();
            this.RenderStereoCalibGrabbedListCount();
        }

        private void RenderStereoCalibGrabbedListCount()
        {
            this.calibListCountLabel.Text = this.StereoCalibrationGrabbedList.Count.ToString();
        }

        private void calibrateFromGrabbedListButton_Click(object sender, EventArgs e)
        {
            //todo
            this.StereoCalibIdx = 0;
            this.stereoCalibrationStatusLabel.Invoke((MethodInvoker)delegate { stereoCalibrationStatusLabel.Text = "calibrating..."; });
            //dirty hack
            //Thread.Sleep(20);

            var stereoCalibData = new StereoCameraCalibrationGrabbedData()
            {
                SquareSize = 1.0,
                BoardSquareSize = new Size(9, 6),
                GrabbedFrames = this.StereoCalibrationGrabbedList
            };
            this.StereoCalibData = stereoCalibData;
            this.StereoCameraParams = this.cameraCalibrator.CalibrateStereo(stereoCalibData);

            this.stereoCalibrationStatusLabel.Text = "calibrated";
            if (this.StereoCameraParams != null && this.StereoCalibData != null)
            {
                this.RenderStereoCalibTestImages();
            }
        }

        private void saveGrabbedCalibrationListToButton_Click(object sender, EventArgs e)
        {
            if (this.StereoCalibrationGrabbedList == null || this.StereoCalibrationGrabbedList.Count == 0)
            {
                MessageBox.Show("calibration list is empty");
            }
            else
            {
                try
                {
                    this.stereoCalibListSaveFolderBrowserDialog.ShowDialog();
                    var path = this.stereoCalibListSaveFolderBrowserDialog.SelectedPath;
                    var prefix = this.prefixCalibListTextBox.Text;
                    if (!String.IsNullOrEmpty(path))
                    {
                        Utils.FramesStorageManager.SaveGrabbedStereoFramesToDisc(this.StereoCalibrationGrabbedList, path, prefix);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void stereoCalibListSaveFileDialog_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void mapTransformApplyButton_Click(object sender, EventArgs e)
        {
            if (this.StereoCameraParams != null && this.StereoCalibData != null)
            {
                this.RenderStereoCalibTestImages();
            }
        }

        private void useMapTransformCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (this.StereoCameraParams != null && this.StereoCalibData != null)
            {
                this.RenderStereoCalibTestImages();
            }
        }

        private void stereoCalibrationStatusLabel_TextChanged(object sender, EventArgs e)
        {
        }

        private void showMEMSFormButton_Click(object sender, EventArgs e)
        {
            this.OpenMEMSRenderForm();
        }

        private void calibratedStereoCaptureTabPage_Click(object sender, EventArgs e)
        {

        }

        private void testSyncDataSourceStartButton_Click(object sender, EventArgs e)
        {
            var path = this.syncDataSourcePathTextBox.Text;
            if (path != null)
            {
                var files = Directory.GetFiles(path);
                var videoPath = files.Where(x => x.Contains("video")).First();
                var accPath = files.Where(x => x.Contains("acc")).First();
                var magnetPath = files.Where(x => x.Contains("magnet")).First();
                var gyroPath = files.Where(x => x.Contains("gyro")).First();
                this.StereoMEMSDataProvider = new DataSource.StereoMEMSDataProviderCVCapFromFile(true, videoPath, 1000.0 / 30 * 2, true, accPath, magnetPath, gyroPath, false);
                this.StereoMEMSDataProvider.NewStereoFrameEvent += StereoMEMSDataProvider_NewStereoFrameEvent;
                this.StereoMEMSDataProvider.NewMEMSReadingsEvent += StereoMEMSDataProvider_NewMEMSReadingsEvent;
                this.StereoMEMSDataProvider.Start();
                this.stereoMEMSRenderTimer.Enabled = true;
            }
        }

        void StereoMEMSDataProvider_NewMEMSReadingsEvent(object sender, NewAMGFrameEventArgs e)
        {
            //this.accMagnetFilterTrackBar.Invoke((MethodInvoker)delegate { this.ProcessMEMSReadings(e.Readings); }); 
            
            this.stereoMEMSSet = e.Readings;
            this.ProcessMEMSReadings(this.stereoMEMSSet);  
        }

        void StereoMEMSDataProvider_NewStereoFrameEvent(object sender, VideoSource.NewStereoFrameEventArgs e)
        {
            this.StereoStreamFrameRender(e.NewStereoFrame);
        }

        private void stereoMEMSRenderTimer_Tick(object sender, EventArgs e)
        {
            //if (this.stereoMEMSSet != null)
            //{
                //if (this.StereoMEMSDataProvider != null && !this.StereoMEMSDataProvider.IsStarted())
                //{
                //    this.stereoMEMSRenderTimer.Enabled = false;
                //}
                this.RenderMEMSOrientation(); 
            //}
        }

        private void RenderMEMSOrientation()
        {
            double[][] orient = null;
            lock (memsOrientationMatrLock)
            {
                orient = Utils.CvHelper.CopyMatrix(memsRotMatr);
            }
            if (orient != null)
            {
                //render calibrated
                if (this.renderCalibOrientCheckBox.Checked)
                {
                    orient = Utils.CvHelper.MatrixToArray(
                                this.orientCalibMatrix.Mul(
                                    Utils.CvHelper.ArrayToMatrix(orient, new Size(3, 3))
                                )
                             );
                }
                this.RenderOrientationTransformation(orient);
            }
        }

        private void Num_Disparities_Scroll(object sender, EventArgs e)
        {

            if (Num_Disparities.Value % 16 != 0)
            {
                //value must be divisable by 16
                if (Num_Disparities.Value >= 152) Num_Disparities.Value = 160;
                else if (Num_Disparities.Value >= 136) Num_Disparities.Value = 144;
                else if (Num_Disparities.Value >= 120) Num_Disparities.Value = 128;
                else if (Num_Disparities.Value >= 104) Num_Disparities.Value = 112;
                else if (Num_Disparities.Value >= 88) Num_Disparities.Value = 96;
                else if (Num_Disparities.Value >= 72) Num_Disparities.Value = 80;
                else if (Num_Disparities.Value >= 56) Num_Disparities.Value = 64;
                else if (Num_Disparities.Value >= 40) Num_Disparities.Value = 48;
                else if (Num_Disparities.Value >= 24) Num_Disparities.Value = 32;
                else Num_Disparities.Value = 16;
            }
        }

        private void SAD_Window_Scroll(object sender, EventArgs e)
        {
            /*The matched block size. Must be an odd number >=1 . Normally, it should be somewhere in 3..11 range*/
            //This ensures only odd numbers are allowed from slider value
            if (SAD_Window.Value % 2 == 0)
            {
                if (SAD_Window.Value == SAD_Window.Maximum) SAD_Window.Value = SAD_Window.Maximum - 2;
                else SAD_Window.Value++;
            }
        }

        private void specklerange_Scroll(object sender, EventArgs e)
        {
            if (specklerange.Value % 16 != 0)
            {
                //value must be divisable by 16
                //TODO: we can do this in a loop
                if (specklerange.Value >= 152) specklerange.Value = 160;
                else if (specklerange.Value >= 136) specklerange.Value = 144;
                else if (specklerange.Value >= 120) specklerange.Value = 128;
                else if (specklerange.Value >= 104) specklerange.Value = 112;
                else if (specklerange.Value >= 88) specklerange.Value = 96;
                else if (specklerange.Value >= 72) specklerange.Value = 80;
                else if (specklerange.Value >= 56) specklerange.Value = 64;
                else if (specklerange.Value >= 40) specklerange.Value = 48;
                else if (specklerange.Value >= 24) specklerange.Value = 32;
                else if (specklerange.Value >= 8) specklerange.Value = 16;
                else specklerange.Value = 0;
            }
        }

        private void fullDP_State_Click(object sender, EventArgs e)
        {
            if (fullDP_State.Text == "True")
            {
                fullDP = false;
                fullDP_State.Text = "False";
            }
            else
            {
                fullDP = true;
                fullDP_State.Text = "True";
            }
        }

        private void testSyncLiveDataButton_Click(object sender, EventArgs e)
        {
            int leftCapId = 0;
            int rightCapId = 1;
            if(!Int32.TryParse(this.leftCaptureTextBox.Text, out leftCapId))
            {
                leftCapId = 0;
            }
            if(!Int32.TryParse(this.rightCaptureTextBox.Text, out rightCapId))
            {
                rightCapId = 1;
            }
            this.StereoMEMSDataProvider = new DataSource.StereoMEMSDataProviderCVUSBCapWebMEMS(
                useVideo: true,
                leftCapId: leftCapId,
                rightCapId: rightCapId,
                useMEMS: false,
                port: 0,
                isMEMSEager: false
                );
            this.StereoMEMSDataProvider.NewStereoFrameEvent += StereoMEMSDataProvider_NewStereoFrameEvent;
            this.StereoMEMSDataProvider.Start();
        }

        private void renderGrayCheckBox_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void uncalibDepthMapCheckBox_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void UpdateCurPrevMEMSOrient()
        {
            lock (this.memsOrientationMatrLock)
            {
                if (this.currentMEMSRotMatr == null)
                {
                    this.currentMEMSRotMatr = Utils.CvHelper.CopyMatrix(this.memsRotMatr);
                    return;
                }
                this.prevMEMSRotMatr = currentMEMSRotMatr;
                this.currentMEMSRotMatr = Utils.CvHelper.CopyMatrix(this.memsRotMatr);
            }
        }

        private void testDifRotationTimer_Tick(object sender, EventArgs e)
        {
            this.RenderMEMSDiffOrientation();
        }

        private void RenderMEMSDiffOrientation()
        {
            this.UpdateCurPrevMEMSOrient();
            if (this.prevMEMSRotMatr != null && this.currentMEMSRotMatr != null)
            {
                var rotMatr = this.OrientationCalc.GetRotationMatrixBetweenTwoStates(this.prevMEMSRotMatr, this.currentMEMSRotMatr, this.orientCalibMatrix);
                this.RenderOrientationTransformation(rotMatr);
            }
        }

        private void MEMSRotationTabPage_Click(object sender, EventArgs e)
        {

        }

        private void camCapRadioButton_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void label26_Click(object sender, EventArgs e)
        {

        }

        private void stopStereoCapButton_Click(object sender, EventArgs e)
        {
            if (this.StereoVideoStreamProvider != null)
            {
                this.StereoVideoStreamProvider.StopStream();
            }
        }

        private void renderFraturesCheckBox_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void changeTransCoeffsButton_Click(object sender, EventArgs e)
        {
            try
            {
                this.transCoeffX = double.Parse(this.transCoeffXTextBox.Text);
                if(this.transCoeffX == 0)
                {
                    this.transCoeffX = 1;
                    this.transCoeffXTextBox.Text = transCoeffX.ToString();
                }
            }
            catch
            {
                this.transCoeffX = 1;
                this.transCoeffXTextBox.Text = transCoeffX.ToString();
            }

            try
            {
                this.transCoeffY = double.Parse(this.transCoeffYTextBox.Text);
                if (this.transCoeffY == 0)
                {
                    this.transCoeffY = 1;
                    this.transCoeffYTextBox.Text = transCoeffY.ToString();
                }
            }
            catch
            {
                this.transCoeffY = 1;
                this.transCoeffYTextBox.Text = transCoeffY.ToString();
            }
            
        }

        private void grabOrientButton_Click(object sender, EventArgs e)
        {
            if (this.memsRotMatr != null)
            {
                this.orientCalibMatrix = Utils.CvHelper.InverseMatrix(Utils.CvHelper.ArrayToMatrix(this.memsRotMatr, new Size(3, 3)));
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void nullPointCoordButton_Click(object sender, EventArgs e)
        {
            this.position3d = new MCvPoint3D64f(0, 0, 0);
        }

        
        //private void CPUDetect()
        //{
        //    using (Image<Bgr, byte> nextFrame = cap.QueryFrame())
        //    {
        //        if (nextFrame != null)
        //        {
        //            // there's only one channel (greyscale), hence the zero index
        //            //var faces = nextFrame.DetectHaarCascade(haar)[0];
        //            Image<Gray, byte> grayframe = nextFrame.Convert<Gray, byte>();
        //            var faces =
        //                    grayframe.DetectHaarCascade(
        //                            haar, scaleFactor, minNeighbors,
        //                            HAAR_DETECTION_TYPE.DO_CANNY_PRUNING,
        //                            new Size(nextFrame.Width / 8, nextFrame.Height / 8)
        //                            )[0];

        //            foreach (var face in faces)
        //            {
        //                nextFrame.Draw(face.rect, new Bgr(0, double.MaxValue, 0), 3);
        //            }
        //            pictureBox1.Image = nextFrame.ToBitmap();
        //        }
        //    }
        //}
    }
}
