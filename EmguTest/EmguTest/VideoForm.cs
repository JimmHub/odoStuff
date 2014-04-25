using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Emgu.CV;
using Emgu.Util;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;

namespace EmguTest
{
    public partial class VideoForm : Form
    {
        public VideoForm()
        {
            InitializeComponent();
        }

        public void RenderDisparityMap(Image<Gray, short> dispMap)
        {
            this.depthMapPictureBox.Image = new Bitmap(dispMap.ToBitmap());
        }

        private void VideoForm_Load(object sender, EventArgs e)
        {

        }

        public void RenderStereoFrame(Bitmap leftFrame, Bitmap rightFrame)
        {
            if (this.capLeftPictureBox.Image != null)
            {
                this.capLeftPictureBox.Image.Dispose();
            }
            if (this.capRightPictureBox.Image != null)
            {
                this.capRightPictureBox.Image.Dispose();
            }

            this.capLeftPictureBox.Image = new Bitmap(leftFrame);
            this.capRightPictureBox.Image = new Bitmap(rightFrame);
        }
    }
}
