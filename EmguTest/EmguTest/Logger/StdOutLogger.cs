using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmguTest.Logger
{
    class StdOutLogger : ILogger
    {
        public void Write(string message)
        {
            Console.Write(message);
        }

        public void WriteLn(string message)
        {
            Console.WriteLine(message);
        }
    }
}
