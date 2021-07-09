using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Net;
using System.IO;
using System.Threading;
using System.Security.AccessControl;

namespace Microsoft.ServiceModel.Samples
{
    public enum ContextType
    {
        MessageHeader,
        HttpCookie,
    }

    // This interface is implemented by the context managers.
    interface IContextManager
    {
        void WriteContext(Message message);
        string ReadContext(Message message);
    }

    //This is the abstract base class for the context managers.
    //This contains the common functionality for all context 
    //managers.
    abstract class ContextManagerBase : IContextManager
    {

        string contextStoreLocation;
        EndpointAddress endpointAddress;

        public ContextManagerBase(string contextStoreLocation,
            EndpointAddress endpointAddress)
        {
            this.contextStoreLocation = contextStoreLocation;
            this.endpointAddress = endpointAddress;
        }

        //Returns a new context id. This method initially tries to 
        //read the context id from an existing file. If the context 
        //id is available this simply returns that. Otherwise this creates
        //a new context id (Guid) and returns it. This also saves the newly
        //created context id into a file whose file name is created 
        //according to the endpoint address.
        protected string GetContextId()
        {
            // Build the file name.
            string fileName = this.endpointAddress.ToString();
            ReplaceInvalidChars(ref fileName);
            fileName = string.Format(CultureInfo.InvariantCulture, @"{0}.txt", fileName);

            string absFilePath = string.Format(CultureInfo.InvariantCulture, @"{0}\{1}", this.contextStoreLocation,
                fileName);

            // Synchronize access to this file using a mutex.
            Mutex fileMutex = new Mutex(false, fileName);
            fileMutex.WaitOne();

            string contextId = null;

            if (File.Exists(absFilePath))
            {
                // Try to read the content from the file.
                using (StreamReader textReader = File.OpenText(absFilePath))
                {
                    if (textReader != null)
                    {
                        contextId = textReader.ReadLine();
                        try
                        {
                            Guid guid = new Guid(contextId);
                        }
                        catch (Exception e)
                        {
                            throw new InvalidOperationException(
                                ResourceHelper.GetString("ExInvalidContextId"),
                                e);
                        }
                    }
                }
            }
            else
            {
                // Create a new context id and write it to a file in the 
                // directory specified by contextStorageLocation.
                contextId = Guid.NewGuid().ToString();
                UTF8Encoding encoding = new UTF8Encoding(true, false);
                byte[] buffer = encoding.GetBytes(contextId);

                // Do not other processes to access this file until 
                // we are done.
                FileStream fs = new FileStream(absFilePath,
                    FileMode.Create, FileAccess.ReadWrite,
                    FileShare.None);

                fs.Write(buffer, 0, buffer.Length);
                fs.Close();
            }

            // Release the mutex.
            fileMutex.ReleaseMutex();
            return contextId;
        }

        //Replaces the invalid file name chars with "@"
        //char.
        private void ReplaceInvalidChars(ref string fileName)
        {
            char[] invalidChars = Path.GetInvalidFileNameChars();

            for (int index = 0; index < invalidChars.Length; index++)
            {
                fileName = fileName.Replace(invalidChars[index], '@');
            }
        }

        #region IContextManager Members

        public abstract void WriteContext(Message message);
        public abstract string ReadContext(Message message);

        #endregion
    }

    //This class implements the context manager for 
    //managing the context using the HTTP cookie header.
    class HttpCookieContextManager : ContextManagerBase
    {
        public HttpCookieContextManager(string contextStoreLocation,
            EndpointAddress endpointAddress)
            : base(contextStoreLocation, endpointAddress)
        {
        }


        //Writes the context to the HTTP cookie header.
        public override void WriteContext(Message message)
        {
            // Generate the context id.
            string contextId = this.GetContextId();
            contextId = DurableInstanceContextUtility.HttpCookieKey + contextId;

            // Write the context id to the http-cookie http request header.            
            if (message.Properties.ContainsKey("httpRequest"))
            {
                HttpRequestMessageProperty httpRequest =
                    (HttpRequestMessageProperty)message.Properties["httpRequest"];
                httpRequest.Headers[HttpRequestHeader.Cookie] = contextId;
            }
            else
            {
                HttpRequestMessageProperty httpRequest =
                    new HttpRequestMessageProperty();
                httpRequest.Headers[HttpRequestHeader.Cookie] = contextId;

                message.Properties.Add("httpRequest", httpRequest);
            }
        }


        // Reads the context from the http cookie.
        public override string ReadContext(Message message)
        {
            if (message.Properties.ContainsKey("httpRequest"))
            {
                HttpRequestMessageProperty httpRequest =
                    (HttpRequestMessageProperty)message.Properties["httpRequest"];
                string contextId = httpRequest.Headers[HttpRequestHeader.Cookie];

                if (contextId != null)
                {
                    // Read the value part of the http cookie string.
                    int delimiter = contextId.IndexOf('=');

                    if (delimiter > -1)
                    {
                        contextId = contextId.Substring(delimiter + 1);
                    }
                }

                return contextId;
            }

            return null;
        }

    }


    // This class implements the context manager for 
    // managing the context using the message header.
    class MessageHeaderContextManager : ContextManagerBase
    {

        public MessageHeaderContextManager(string contextStoreLocation,
            EndpointAddress endpointAddress)
            : base(contextStoreLocation, endpointAddress)
        {
        }


        /// Writes the context to the message header.

        public override void WriteContext(Message message)
        {
            string contextId = this.GetContextId();

            MessageHeader contextHeader =
                MessageHeader.CreateHeader(DurableInstanceContextUtility.HeaderName,
                DurableInstanceContextUtility.HeaderNamespace,
                contextId,
                true);

            message.Headers.Add(contextHeader);
        }


        // Reads the context from the message header.
        public override string ReadContext(Message message)
        {
            string contextId = null;
            int headerPosition = message.Headers.FindHeader(
                DurableInstanceContextUtility.HeaderName,
                DurableInstanceContextUtility.HeaderNamespace);

            if (headerPosition > -1)
            {
                contextId =
                    message.Headers.GetHeader<string>(headerPosition);

                // Add the header to the understood headers collection.
                message.Headers.UnderstoodHeaders.Add(
                    message.Headers[headerPosition]);
            }

            return contextId;
        }
    }


    // This class contains the static methods to create the context managers
    // appropriately.
    static class ContextManagerFactory
    {
        // Returns an IContextManager based on the context type passed into the 
        // function.
        public static IContextManager CreateContextManager(ContextType contextType,
            string contextStoreLocation, EndpointAddress endpointAddress)
        {
            IContextManager contextManager = null;

            switch (contextType)
            {
                case ContextType.HttpCookie:
                    contextManager = new HttpCookieContextManager(contextStoreLocation,
                        endpointAddress);
                    break;
                case ContextType.MessageHeader:
                    contextManager = new MessageHeaderContextManager(contextStoreLocation,
                        endpointAddress);
                    break;
                default:
                    break;
            }

            return contextManager;
        }
    }
}

