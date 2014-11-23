using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Logging
{
    public class TextBoxLogHandler: ILogHandler
    {
        private TextBox mTextBox;

        public TextBoxLogHandler(TextBox textBox)
        {
            mTextBox = textBox;
        }

        public void HandleMessage(string tag, string message)
        {
            mTextBox.AppendText(tag + ": " + message + Environment.NewLine);
        }
    }
}
