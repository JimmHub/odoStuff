using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace EmguTest.Utils
{
    static class FramesStorageManager
    {
        public static void SaveGrabbedStereoFramesToDisc(List<VideoSource.StereoFrameSequenceElement> frames, string folderPath, string prefix)
        {
            if(!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            var format = System.Drawing.Imaging.ImageFormat.Bmp;
            for(int i = 0; i < frames.Count; ++i)
            {
                var fullCommonName = folderPath + @"\" + prefix + i.ToString();
                frames[i].LeftRawFrame.Save(fullCommonName + "_left." + format.ToString(), format);
                frames[i].RightRawFrame.Save(fullCommonName + "_right." + format.ToString(), format);
            }
        }
    }
}
