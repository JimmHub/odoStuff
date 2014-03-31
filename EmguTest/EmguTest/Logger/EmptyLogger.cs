using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmguTest.Logger
{
    class EmptyLogger : ILogger
    {
        public void Write(string message)
        {
        }

        public void WriteLn(string message)
        {
        }
    }
}
