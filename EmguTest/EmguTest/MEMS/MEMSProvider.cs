﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EmguTest.MEMS;

namespace EmguTest.MEMS
{
    abstract class MEMSProvider
    {
        abstract public MEMSReadingsSet3f GetCurrentReadingsSet();
        abstract public MEMSReadingsSet3f GetNextReadingsSet();
        abstract public event NewAMGFrameEventHandler NewAMGFrameEvent;
    }

    public delegate void NewAMGFrameEventHandler(object sender, NewAMGFrameEventArgs e);
}
