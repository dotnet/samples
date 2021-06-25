//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------ 

using System;
using System.IO;
using System.Text;
using System.Windows.Controls;

namespace Microsoft.Samples.DocumentApprovalProcess.ApprovalClient
{
    // This class creates a TextWriter given a WPF TextBox
    // In this sample, we use this as a way to output tracking information to the UI.
    class WindowTextWriter : TextWriter
    {
        const string textWriterClosed = "This TextWriter is Closed";

        Encoding encoding;
        bool isOpen;
        TextBox textBox;

        public WindowTextWriter(TextBox textBox)
        {
            this.textBox = textBox;
            this.isOpen = true;
        }

        public override Encoding Encoding
        {
            get
            {
                if (encoding == null)
                {
                    encoding = new UnicodeEncoding(false, false);
                }
                return encoding;
            }
        }

        public override void Close()
        {
            this.Dispose(true);
        }

        protected override void Dispose(bool disposing)
        {
            this.isOpen = false;
            base.Dispose(disposing);
        }

        public override string ToString()
        {
            return this.textBox.Text.ToString();
        }

        public override void Write(char value)
        {
            if (!this.isOpen)
            {
                throw new ApplicationException(textWriterClosed); ;
            }
            lock (this.textBox)
            {
                this.textBox.Dispatcher.BeginInvoke(new Action(() => this.textBox.Text = value.ToString() + this.textBox.Text));
            }
        }

        public override void Write(string value)
        {
            if (!this.isOpen)
            {
                throw new ApplicationException(textWriterClosed); ;
            }
            if (value != null)
            {
                lock (this.textBox)
                {
                    this.textBox.Dispatcher.BeginInvoke(new Action(() => this.textBox.Text = value + this.textBox.Text));
                }
            }
        }

        public override void Write(char[] buffer, int index, int count)
        {
            String toAdd = "";

            if (!this.isOpen)
            {
                throw new ApplicationException(textWriterClosed); ;
            }
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException("index");
            }
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("count");
            }
            if ((buffer.Length - index) < count)
            {
                throw new ArgumentException("The size of the buffer cannot accomodate the given index and count.");
            }
            
            for (int i = 0; i < count; i++)
                toAdd += buffer[i];
            lock (this.textBox)
            {
                this.textBox.Dispatcher.BeginInvoke(new Action(() => this.textBox.Text = toAdd + this.textBox.Text));
            }
        }
    }
}
