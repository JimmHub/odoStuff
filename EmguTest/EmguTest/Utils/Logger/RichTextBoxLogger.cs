using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EmguTest.Logger
{
    class RichTextBoxLogger : ILogger
    {
        public RichTextBox TextBox { get; set; }

        public RichTextBoxLogger(RichTextBox rtb)
        {
            this.TextBox = rtb;
        }

        public void Write(string message)
        {
            this.TextBox.Text += message;
        }

        public void WriteLn(string message)
        {
            this.TextBox.Text += message + "\n";
        }
    }
}
