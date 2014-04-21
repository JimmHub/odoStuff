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
        }

        private void RunEmgu()
        {
            this.EmguMain = new EmguMain();
            this.EmguMain.Run();
        }

        private void LoadWpf3dControl()
        {
            // Create the ElementHost control for hosting the
            // WPF UserControl.
            ElementHost host = this.elementHost1;
            //host.Dock = DockStyle.Fill;

            // Create the WPF UserControl.
            this.Wpf3DControl =
                new Wpf3DControl.UserControl1();

            // Assign the WPF UserControl to the ElementHost control's
            // Child property.
            host.Child = this.Wpf3DControl;
            
            // Add the ElementHost control to the form's
            // collection of child controls.
            //this.Controls.Add(host);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.LoadWpf3dControl();
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
            this.stereoCapTimer.Enabled = true;
            ////

            this.FeatureTracker = new LKFeatureTracker(stereoCap);

            if (this.timer2.Enabled)
            {
                this.FeatureTracker.Run();
            }
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
            this.Wpf3DControl.RotateTest();
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
            if (set.TimeStampI == 0)
            {
                return false;
            }
            return true;
        }

        

        private void memsTestOutputTimer_Tick(object sender, EventArgs e)
        {
            (this.MemsProvider as DeviceRTWebMEMSProvider).IsEagerNextReadings = false;
            var nextReadings = this.MemsProvider.GetNextReadingsSet();
            //
            //
            //this.RotateWpfContent();
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

                var res = this.MulReadingsVect(orientMatr3f, nextReadings.AccVector3f, true);

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

                this.Wpf3DControl.SetTransformMatrix(orientMatr3f);
            }

            //points test
            MCvPoint3D32f pointA = new MCvPoint3D32f(0, 1, 0);
            MCvPoint3D32f pointB = new MCvPoint3D32f(1, 0, 0);
            var pointC = pointA.CrossProduct(pointB);

            ////
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

            Console.WriteLine("timestamp: " + nextReadings.TimeStampI.ToString());
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
            var cameraRes = CameraCalibrator.CalibrateMono(calibData);

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
            this.StereoCameraParams = CameraCalibrator.CalibrateStereo(stereoCalibData);

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

            var leftRawFrame = this.StereoCalibData.GetFrameById(StereoCalibIdx).LeftRawFrame;
            var rightRawFrame = this.StereoCalibData.GetFrameById(StereoCalibIdx).RightRawFrame;

            var leftOriginalImg = new Image<Bgr, byte>(leftRawFrame);
            var rightOriginalImg = new Image<Bgr, byte>(rightRawFrame);

            this.leftStereoOriginalPictureBox.Image = leftRawFrame;
            this.rightStereoOriginalPictureBox.Image = rightRawFrame;

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
            
            if (this.StereoVideoStreamProvider != null)
            {
                this.stereoStreamRenderTimer.Enabled = true;
            }
        }

        private void InitStereoVideoStreamFromFileCap()
        {
            String fileName = this.stereoFileNameTextBox.Text;
            if (File.Exists(fileName))
            {
                this.StereoVideoStreamProvider = new VideoSource.StereoGeminateCVFileVideoStreamProvider(fileName, 1.0 / 30 * 1000);
                this.StereoVideoStreamProvider.StartStream();
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
            this.StereoVideoStreamProvider = new VideoSource.StereoAForgeCamVideoStreamProvider(
                leftCapId: leftCapId,
                rightCapId: rightCapId);

            this.StereoVideoStreamProvider.StartStream();
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
            if (this.StereoVideoStreamProvider.IsFunctioning())
            {
                var frame = this.StereoVideoStreamProvider.GetNextFrame();
                if (!frame.IsNotFullFrame)
                {
                    this.calibStereoCapLeftPictureBox.Image = frame.LeftRawFrame;
                    this.calibStereoCapRightPictureBox.Image = frame.RightRawFrame;
                }
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
            this.StereoCalibrationGrabbedList.Add(this.StereoVideoStreamProvider.GetCurrentFrame());
            this.RenderStereoCalibGrabbedListCount();
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
            this.StereoCameraParams = CameraCalibrator.CalibrateStereo(stereoCalibData);

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
                this.stereoCalibListSaveFolderBrowserDialog.ShowDialog();
                var path = this.stereoCalibListSaveFolderBrowserDialog.SelectedPath;
                var prefix = this.prefixCalibListTextBox.Text;
                if (!String.IsNullOrEmpty(path))
                {
                    Utils.FramesStorageManager.SaveGrabbedStereoFramesToDisc(this.StereoCalibrationGrabbedList, path, prefix);
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
