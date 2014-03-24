using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

namespace EmguTest
{
    public class EmguMain
    {
        private String _windowName;

        public EmguMain()
        {
            this._windowName = "hi emgu!";
        }

        public void Run()
        {
            CvInvoke.cvNamedWindow(this._windowName);


            //Create an image of 400x200 of Blue color
            using (Image<Bgr, Byte> img = new Image<Bgr, byte>(400, 200, new Bgr(255, 0, 0)))
            {
                //Create the font
                MCvFont f = new MCvFont(FONT.CV_FONT_HERSHEY_COMPLEX, 1.0, 1.0);
                //Draw "Hello, world." on the image using the specific font
                img.Draw("Hello, world", ref f, new Point(10, 80), new Bgr(0, 255, 0));

                //Show the image
                CvInvoke.cvShowImage(_windowName, img.Ptr);
                //Wait for the key pressing event
                CvInvoke.cvWaitKey(0);
                //Destory the window
                CvInvoke.cvDestroyWindow(_windowName);
            }
        }

        public void FaceDetectorRun()
        {
            Capture cap = new Capture(0);
            
            
        }
    }
}
