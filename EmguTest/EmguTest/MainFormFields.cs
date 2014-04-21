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
using EmguTest.VideoSource;

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
        //mono calibration
        private MonoCameraParams MonoCameraParams;
        private String MonoCalibTestFolder = @"C:\CodeStuff\cvproj\resources\phonemonocalibimages";
        private List<String> MonoCalibTestFiles;
        public int MonoCalibIdx = 0;
        ////
        //stereo calibration
        private StereoCameraParams StereoCameraParams;
        //private String StereoCalibTestFolder = @"C:\CodeStuff\cvproj\resources\phonestereocalibtest";
        //private List<Tuple<String, String>> StereoCalibTestFiles;
        private Odometry.StereoCameraCalibrationData StereoCalibData;
        private int StereoCalibIdx = 0;
        ////
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

        private bool StereoCapIsPaused { get; set; }
        private StereoVideoStreamProvider StereoVideoStreamProvider;
        private EmguMain EmguMain;

        private List<VideoSource.StereoFrameSequenceElement> StereoCalibrationGrabbedList = new List<StereoFrameSequenceElement>();

        private bool isNewStereoFrameEventSinged = false;
        private bool isNewStereoFrameInProcess = false;
        //private VideoSource
    }
}
