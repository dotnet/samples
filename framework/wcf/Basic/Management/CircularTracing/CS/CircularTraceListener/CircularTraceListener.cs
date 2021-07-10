//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;

namespace Microsoft.Samples.ServiceModel
{

    public class CircularTraceListener : XmlWriterTraceListener
    {

        static CircularStream m_stream = null;
        bool MaxQuotaInitialized = false;
        const string FileQuotaAttribute = "maxFileSizeKB";
        const long DefaultMaxQuota = 1000;
        const string DefaultTraceFile = "E2ETraces.svclog";


        #region Member Functions

        private long MaxQuotaSize
        {
            //Get the MaxQuotaSize from configuration file
            //Set to Default Value if there are any problems

            get
            {
                long MaxFileQuota = 0;
                if (!this.MaxQuotaInitialized)
                {
                    try
                    {
                        string MaxQuotaOption = this.Attributes[CircularTraceListener.FileQuotaAttribute];
                        if (MaxQuotaOption == null)
                        {
                            MaxFileQuota = DefaultMaxQuota;
                        }
                        else
                        {
                            MaxFileQuota = int.Parse(MaxQuotaOption, CultureInfo.InvariantCulture);
                        }
                    }
                    catch (Exception)
                    {
                        MaxFileQuota = DefaultMaxQuota;
                    }
                    finally
                    {
                        this.MaxQuotaInitialized = true;
                    }
                }

                if (MaxFileQuota <= 0)
                {
                    MaxFileQuota = DefaultMaxQuota;
                }

                //MaxFileQuota is in KB in the configuration file, convert to bytes

                MaxFileQuota = MaxFileQuota * 1024;
                return MaxFileQuota;
            }
        }

        private void DetermineOverQuota()
        {

            //Set the MaxQuota on the stream if it hasn't been done

            if (!this.MaxQuotaInitialized)
            {
                m_stream.MaxQuotaSize = this.MaxQuotaSize;
            }

            //If we're past the Quota, flush, then switch files

            if (m_stream.IsOverQuota)
            {
                base.Flush();
                m_stream.SwitchFiles();
            }
        }

        #endregion               

        #region XmlWriterTraceListener Functions

        public CircularTraceListener(string file)
            : base(m_stream = new CircularStream(file))
        {
        }

        public CircularTraceListener()
            : base(m_stream = new CircularStream(DefaultTraceFile))
        {
        }

        protected override string[] GetSupportedAttributes()
        {
            return new string[] { CircularTraceListener.FileQuotaAttribute };
        }

        public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data)
        {
            DetermineOverQuota();
            base.TraceData(eventCache, source, eventType, id, data);
        }

        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id)
        {
            DetermineOverQuota();
            base.TraceEvent(eventCache, source, eventType, id);
        }

        public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, params object[] data)
        {
            DetermineOverQuota();
            base.TraceData(eventCache, source, eventType, id, data);
        }

        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, params object[] args)
        {
            DetermineOverQuota();
            base.TraceEvent(eventCache, source, eventType, id, format, args);
        }

        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
        {
            DetermineOverQuota();
            base.TraceEvent(eventCache, source, eventType, id, message);
        }

        public override void TraceTransfer(TraceEventCache eventCache, string source, int id, string message, Guid relatedActivityId)
        {
            DetermineOverQuota();
            base.TraceTransfer(eventCache, source, id, message, relatedActivityId);

        }

        #endregion

    }

    public class CircularStream : System.IO.Stream
    {
        private FileStream[] FStream = null;
        private String[] FPath = null;
        private long DataWritten = 0;
        private long FileQuota = 0;
        private int CurrentFile = 0;
        private string stringWritten = string.Empty;


        public CircularStream(string FileName)
        {
            //Handle all exceptions within this class, since tracing shouldn't crash a service

            //Add 00 and 01 to FileNames and open streams

            try
            {
                string filePath = Path.GetDirectoryName(FileName);
                string fileBase = Path.GetFileNameWithoutExtension(FileName);
                string fileExt = Path.GetExtension(FileName);

                if (string.IsNullOrEmpty(filePath))
                {
                    filePath = AppDomain.CurrentDomain.BaseDirectory;
                }

                FPath = new String[2];
                FPath[0] = Path.Combine(filePath, fileBase + "00" + fileExt);
                FPath[1] = Path.Combine(filePath, fileBase + "01" + fileExt);

                FStream = new FileStream[2];
                FStream[0] = new FileStream(FPath[0], FileMode.Create);
            }
            catch { }

        }

        public long MaxQuotaSize
        {
            get
            {
                return FileQuota;
            }
            set
            {
                FileQuota = value;
            }
        }

        public void SwitchFiles()
        {
            try
            {
                //Close current file, open next file (deleting its contents)

                DataWritten = 0;
                FStream[CurrentFile].Close();

                CurrentFile = (CurrentFile + 1) % 2;

                FStream[CurrentFile] = new FileStream(FPath[CurrentFile], FileMode.Create);
            }
            catch (Exception) { }
        }

        public bool IsOverQuota
        {
            get
            {
                return (DataWritten >= FileQuota);
            }

        }

        public override bool CanRead
        {
            get
            {
                try
                {
                    return FStream[CurrentFile].CanRead;
                }
                catch (Exception)
                {
                    return true;
                }
            }
        }

        public override bool CanSeek
        {
            get
            {
                try
                {
                    return FStream[CurrentFile].CanSeek;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public override long Length
        {
            get
            {
                try
                {
                    return FStream[CurrentFile].Length;
                }
                catch (Exception)
                {
                    return -1;
                }
            }
        }

        public override long Position
        {
            get
            {
                try
                {
                    return FStream[CurrentFile].Position;
                }
                catch (Exception)
                {
                    return -1;
                }
            }
            set
            {
                try
                {
                    FStream[CurrentFile].Position = Position;
                }
                catch (Exception) { }
            }
        }

        public override bool CanWrite
        {
            get
            {
                try
                {
                    return FStream[CurrentFile].CanWrite;
                }
                catch (Exception)
                {
                    return true;
                }
            }
        }

        public override void Flush()
        {
            try
            {
                FStream[CurrentFile].Flush();
            }
            catch (Exception) { }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            try
            {
                return FStream[CurrentFile].Seek(offset, origin);
            }
            catch (Exception)
            {
                return -1;
            }
        }

        public override void SetLength(long value)
        {
            try
            {
                FStream[CurrentFile].SetLength(value);
            }
            catch (Exception) { }
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            try
            {
                //Write to current file

                FStream[CurrentFile].Write(buffer, offset, count);
                DataWritten += count;

            }
            catch (Exception){ }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            try
            {
                return FStream[CurrentFile].Read(buffer, offset, count);
            }
            catch (Exception)
            {
                return -1;
            }
        }

        public override void Close()
        {
            try
            {
                FStream[CurrentFile].Close();
            }
            catch (Exception) { }
        }


    }

}
