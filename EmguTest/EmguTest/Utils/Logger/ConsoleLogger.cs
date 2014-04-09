using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace EmguTest.Logger
{
    class ConsoleLogger : ILogger
    {
        public ConsoleLogger()
        {
            this.AttachConsole();
        }

        [DllImport("kernel32.dll")]
        static extern bool AttachConsole(int dwProcessId);
        private const int ATTACH_PARENT_PROCESS = -1;

        [STAThread]
        public void Write(string message)
        {
            Console.Write(message);
        }

        [STAThread]
        public void WriteLn(string message)
        {
            Console.WriteLine(message);
        }

        protected void AttachConsole()
        {
            AttachConsole(ATTACH_PARENT_PROCESS);
        }
    }
}
