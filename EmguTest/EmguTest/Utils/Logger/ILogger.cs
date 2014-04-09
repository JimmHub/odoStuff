using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmguTest.Logger
{
    interface ILogger
    {
        void Write(String message);
        void WriteLn(String message);
    }
}
