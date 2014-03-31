using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace EmguTest.Utils
{
    class StoppableThreadLoop
    {
        public Thread MainThread { get; set; }
        public bool IsRunning 
        {
            get
            {
                if (this.MainThread == null)
                {
                    return false;
                }
                if (this.MainThread.ThreadState == ThreadState.Running)
                {
                    return true;
                }
                if (this.MainThread.ThreadState == ThreadState.WaitSleepJoin)
                {
                    return true;
                }

                return false;
            }
        }

        protected bool ShouldStop;

        protected void MainFunc()
        {

        }

        public void Run()
        {
            if (!this.IsRunning)
            {
                
            }
        }
    }
}
