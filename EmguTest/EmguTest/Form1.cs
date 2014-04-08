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

using Emgu.CV;
using Emgu.Util;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;

using EmguTest.MEMS;

namespace EmguTest
{
    public partial class Form1 : Form
    {
        private Logger.ILogger Logger { get; set; }
        private Logger.ILogger MEMSRTBLogger { get; set; }
        private Capture cap;
        private HaarCascade haar;
        private Emgu.CV.GPU.GpuCascadeClassifier gpuCC;
        private FeatureTracker FeatureTracker;
        //classifier params
        private const double scaleFactor = 1.01;
        private const int minNeighbors = 5;
        //
        private String haarPath = "C:\\CodeStuff\\cvproj\\resources\\javacv\\haarcascade_frontalface_alt.xml";
        private String videoPath = @"G:\Karty,_den'gi,_dva_stvola_DVDRip_by_Matysh.avi";

        private bool UseGPUCascade = false;

        private Disparity.DisparityBuilder DispBuilder;

        private long LastMill = 0;
        private int LastFpsCount = 0;

        public Wpf3DControl.UserControl1 Wpf3DControl;

        //readings test
        MEMSProvider MemsProvider;
        
        //
        //String accPath = @"C:\CodeStuff\cvproj\resources\str1396085909463\acc1396085909463.rdn";
        //String magnetPath = @"C:\CodeStuff\cvproj\resources\str1396085909463\magnet1396085909463.rdn";
        //String gyroPath = @"C:\CodeStuff\cvproj\resources\str1396085909463\gyro1396085909463.rdn";
        //

        //
        //String accPath = @"C:\CodeStuff\cvproj\resources\str1381158297548\acc1381158297548.rdn";
        //String magnetPath = @"C:\CodeStuff\cvproj\resources\str1381158297548\magnet1381158297548.rdn";
        //String gyroPath = @"C:\CodeStuff\cvproj\resources\str1381158297548\gyro1381158297548.rdn";
        //

        //
        //String accPath =    @"C:\CodeStuff\cvproj\resources\str1396117763898\acc1396117763898.rdn";
        //String magnetPath = @"C:\CodeStuff\cvproj\resources\str1396117763898\magnet1396117763898.rdn";
        //String gyroPath =   @"C:\CodeStuff\cvproj\resources\str1396117763898\gyro1396117763898.rdn";
        //

        //
        String accPath = @"C:\CodeStuff\cvproj\resources\str1396261734695\acc1396261734695.rdn";
        String magnetPath = @"C:\CodeStuff\cvproj\resources\str1396261734695\magnet1396261734695.rdn";
        String gyroPath = @"C:\CodeStuff\cvproj\resources\str1396261734695\gyro1396261734695.rdn";
        //
        MEMS.MEMSOrientationCalculator OrientationCalc;
        ////
        public Capture CamLeft;
        public Capture CamRight;
        public int CamLeftId = 2;
        public int CamRightId = 3;
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
                this.CamLeft = new Capture(this.CamLeftId);
            }
            catch
            {
                this.CamLeft = null;
            }

            try
            {
                this.CamRight = new Capture(this.CamRightId);
            }
            catch
            {
                this.CamRight = null;
            }
            ////
        }

        private EmguMain EmguMain;

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
                cap = new Capture(0);
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
