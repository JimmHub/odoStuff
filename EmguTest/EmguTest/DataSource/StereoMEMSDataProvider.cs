using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EmguTest.MEMS;
using EmguTest.VideoSource;

namespace EmguTest.DataSource
{
    abstract class StereoMEMSDataProvider
    {
        abstract public MEMSReadingsSet3f GetCurrentMEMSReadingsSet3f();
        abstract public StereoFrameSequenceElement GetCurrentStereoFrame();
        abstract public event NewMEMSReadingsSetEventHandler NewMEMSReadingsEvent;
        abstract public event NewStereoFrameEventHandler NewStereoFrameEvent;

        abstract public void Start();
        abstract public void Stop();
        abstract public void Pause();
        abstract public bool IsStarted();
        abstract public bool IsPaused();
    }
}
