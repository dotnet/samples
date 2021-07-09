//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.IO;
using System.ServiceModel;

namespace Microsoft.Samples.ChunkingChannel
{
    //for debugging, set ReturnUnknownExceptionsAsFaults to true
#if DEBUG
    [ServiceBehavior(IncludeExceptionDetailInFaults=true)]
#endif 
    class service : ITestService
    {
        #region ITestService Members

        public Stream EchoStream(Stream stream)
        {
            FileStream file = new FileStream(
                Path.Combine(System.Environment.CurrentDirectory, "EchoStream"),
                FileMode.Create,
                FileAccess.ReadWrite);
            int count;
            byte[] buffer=new byte[4096];
            while((count=stream.Read(buffer,0,buffer.Length))>0)
            {
                file.Write(buffer, 0, count);
                file.Flush();
            }
            stream.Close();
            file.Position = 0;
            return file;
        }

        public Stream DownloadStream()
        {
            //find any file under MyPictures
            string[] files = Directory.GetFiles(System.Environment.GetFolderPath(Environment.SpecialFolder.MyPictures));
            if(files.Length==0)
            {
                //no files to return
                return null;
            }
            FileStream file = new FileStream(
                Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), files[0]),
                FileMode.Open,
                FileAccess.Read);
            return file;
        }

        public void UploadStream(Stream stream)
        {
            FileStream file = new FileStream(
                Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "UploadStream"),
                FileMode.Create,
                FileAccess.Write);
            int count;
            byte[] buffer = new byte[4096];
            while ((count = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                file.Write(buffer, 0, count);
            }
            stream.Close();
            file.Close();            
        }

        #endregion

        #region ITestService Members

        public void ProcessMessage(System.ServiceModel.Channels.Message messsage)
        {
            Console.WriteLine("Message received with action {0}", messsage.Headers.Action);
        }

        #endregion
    }
}
