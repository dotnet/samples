//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using System;
using System.Activities;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;

namespace Microsoft.Samples.Activities.Statements
{
    // SendMail activity allows sending mail using SMTP in Workflow applications. 
    // To achieve this goal, SendMail activity uses the functionality in System.Net.Mail.
    // To use this activity, you will need to have access to an operational SMTP server.

    [Designer(typeof(SendMailDesigner))]
    public sealed class SendMail : AsyncCodeActivity
    {
        [RequiredArgument]
        public InArgument<MailAddressCollection> To { get; set; }

        [RequiredArgument]
        public InArgument<MailAddress> From { get; set; }

        [RequiredArgument]
        public InArgument<string> Subject { get; set; }

        [DefaultValue(null)]
        public InArgument<MailAddress> TestMailTo { get; set; }

        public InArgument<Collection<Attachment>> Attachments { get; set; }
        public InArgument<MailAddressCollection> CC { get; set; }
        public InArgument<MailAddressCollection> Bcc { get; set; }
        public InArgument<IDictionary<string, string>> Tokens { get; set; }
        public string Body { get; set; }
        public string BodyTemplateFilePath { get; set; }
        public string TestDropPath { get; set; }
        public int Port { get; set; }
        public bool EnableSsl { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }

        public SendMail()
        {
            this.Port = 25;
        }

        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            if (string.IsNullOrEmpty(this.Host))
            {
                metadata.AddValidationError("Property 'Host' of SendMail activity cannot be null or empty");
            }

            if (this.From == null)
            {
                metadata.AddValidationError("Property 'From' of SendMail activity cannot be null or empty");
            }

            if (this.Port <= 0)
            {
                metadata.AddValidationError("The value of property 'Port' of SendMail activity must be larger than 0");
            }

            if (this.BodyTemplateFilePath != null && !File.Exists(this.BodyTemplateFilePath))
            {
                metadata.AddValidationError("The provided path for the body template (argument 'BodyTemplateFilePath') does not exist or access is denied.");
            }

            base.CacheMetadata(metadata);            
        }       

        // Replaces tokens found in the body with the values specified 
        // by the user in the tokens dictionary
        private void ReplaceTokensInBody(CodeActivityContext context)
        {
            IDictionary<string, string> t = Tokens.Get(context);

            foreach (string key in t.Keys)
            {
                this.Body = this.Body.Replace(key, t[key]);
            }
        }

        // Loads a template for the body of the mail if the 
        // bodyTemplateFile property is specified
        private void LoadBodyTemplate(CodeActivityContext context)
        {
            if (!string.IsNullOrEmpty(this.BodyTemplateFilePath))
            {
                using (StreamReader re = File.OpenText(this.BodyTemplateFilePath))
                {
                    this.Body = re.ReadToEnd();
                }
            }
        }

        // If a testMailToAdress is specified, then 1) the to of the message
        // is changed to that address and 2) a note is added at the bottom of the email
        private void AddTestInformationToBody(CodeActivityContext context)
        {
            StringBuilder buffer = new StringBuilder();

            buffer.Append("<br/>");
            buffer.Append("<hr/>");
            buffer.Append(string.Format("<b>Test Mode</b> - TestMailTo address is {0}", this.TestMailTo.Get(context).Address));
            buffer.Append("<hr/>");

            string bodyWithTestInfo = this.Body + buffer.ToString();

            this.Body = bodyWithTestInfo;
        }

        // If testMailDropPath attribute is set, the email is written in files
        // in that path: 
        //    xxxx.body.html with body 
        //    xxxx.data.txt with message data (from, to, cc, bcc, and subject)
        private void WriteMailInTestDropPath(CodeActivityContext context)
        {
            // create file with Html of the body
            string testDropBodyFileName = string.Format("{0}\\{1}.body.htm", this.TestDropPath, DateTime.Now.ToString("yyyyMMddhhmmssff"));
            using (TextWriter writer = new StreamWriter(testDropBodyFileName))
            {
                writer.Write(this.Body);
            }

            // create file with from, to, cc, bcc, subject
            string testDropDataFileName = string.Format("{0}\\{1}.data.txt", this.TestDropPath, DateTime.Now.ToString("yyyyMMddhhmmssff"));
            MailAddressCollection toList = this.To.Get(context);
            MailAddressCollection bccList = this.Bcc.Get(context);
            MailAddressCollection ccList = this.CC.Get(context);

            using (TextWriter writer = new StreamWriter(testDropDataFileName))
            {
                writer.Write("From: {0}", this.From.Get(context).Address);

                writer.Write("\r\nTo: ");
                foreach (MailAddress address in toList)
                {
                    writer.Write(string.Format("{0} ", address.Address));
                }

                if (TestMailTo.Expression != null)
                {
                    writer.WriteLine("\r\nTest MailTo Mode Enable...Address: {0}", TestMailTo.Get(context).Address);
                }

                if (ccList != null)
                {
                    writer.Write("\r\nCc: ");
                    foreach (MailAddress address in ccList)
                    {
                        writer.Write("{0} ", address.Address);
                    }
                }

                if (bccList != null)
                {
                    writer.Write("\r\nBcc: ");
                    foreach (MailAddress address in bccList)
                    {
                        writer.Write("{0} ", address.Address);
                    }
                }

                writer.Write("\r\nSubject: {0}", this.Subject.Get(context));
            }
        }

        protected override void Cancel(AsyncCodeActivityContext context)
        {
            SendMailAsyncResult sendMailAsyncResult = (SendMailAsyncResult) context.UserState;
            sendMailAsyncResult.Client.SendAsyncCancel();
        }

        protected override IAsyncResult BeginExecute(AsyncCodeActivityContext context, AsyncCallback callback, object state)
        {
            MailMessage message = new MailMessage();
            message.From = this.From.Get(context);

            if (TestMailTo.Expression == null)
            {
                foreach (MailAddress address in this.To.Get(context))
                {
                    message.To.Add(address);
                }

                MailAddressCollection ccList = this.CC.Get(context);
                if (ccList != null)
                {
                    foreach (MailAddress address in ccList)
                    {
                        message.CC.Add(address);
                    }
                }

                MailAddressCollection bccList = this.Bcc.Get(context);
                if (bccList != null)
                {
                    foreach (MailAddress address in bccList)
                    {
                        message.Bcc.Add(address);
                    }
                }
            }
            else
            {
                message.To.Add(TestMailTo.Get(context));
            }

            Collection<Attachment> attachments = this.Attachments.Get(context);
            if (attachments != null)
            {
                foreach (Attachment attachment in attachments)
                {
                    message.Attachments.Add(attachment);
                }
            }

            if (!string.IsNullOrEmpty(this.BodyTemplateFilePath))
            {
                LoadBodyTemplate(context);
            }

            if ((this.Tokens.Get(context) != null) && (this.Tokens.Get(context).Count > 0))
            {
                ReplaceTokensInBody(context);
            }

            if (this.TestMailTo.Expression != null)
            {
                AddTestInformationToBody(context);
            }

            message.Subject = this.Subject.Get(context);
            message.Body = this.Body;

            SmtpClient client = new SmtpClient();
            client.Host = this.Host;
            client.Port = this.Port;
            client.EnableSsl = this.EnableSsl;

            if (string.IsNullOrEmpty(this.UserName))
            {
                client.UseDefaultCredentials = true;
            }
            else
            {
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(this.UserName, this.Password);
            }

            if (!string.IsNullOrEmpty(this.TestDropPath))
            {
                WriteMailInTestDropPath(context);
            }

            var sendMailAsyncResult = new SendMailAsyncResult(client, message, callback, state);
            context.UserState = sendMailAsyncResult;
            return sendMailAsyncResult;
        }

        protected override void EndExecute(AsyncCodeActivityContext context, IAsyncResult result)
        {
            // Nothing needs to be done to wrap up the execution.
        }

        class SendMailAsyncResult : IAsyncResult
        {
            SmtpClient client;
            AsyncCallback callback;
            object asyncState;
            EventWaitHandle asyncWaitHandle;

            public bool CompletedSynchronously { get { return false; } }
            public object AsyncState { get { return this.asyncState; } }
            public WaitHandle AsyncWaitHandle { get { return this.asyncWaitHandle; } }
            public bool IsCompleted { get { return true; } }
            public SmtpClient Client { get { return client; } }

            public SendMailAsyncResult(SmtpClient client, MailMessage message, AsyncCallback callback, object state)
            {
                this.client = client;
                this.callback = callback;
                this.asyncState = state;
                this.asyncWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
                client.SendCompleted += new SendCompletedEventHandler(SendCompleted);
                client.SendAsync(message, null);
            }

            void SendCompleted(object sender, AsyncCompletedEventArgs e)
            {
                this.asyncWaitHandle.Set();
                callback(this);
            }
        }
    }
}
