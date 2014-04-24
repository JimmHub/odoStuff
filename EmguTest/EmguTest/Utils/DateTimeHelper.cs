using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmguTest.Utils
{
    static class DateTimeHelper
    {
        static public Int64 DateTimeToLongMS(DateTime date)
        {
            return Convert.ToInt64(date.Subtract(DateTime.MinValue).TotalMilliseconds);
        }
    }
}
