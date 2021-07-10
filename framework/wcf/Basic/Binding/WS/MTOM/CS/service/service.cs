//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System.IO;
using System.ServiceModel;

namespace Microsoft.Samples.Mtom
{
    // Service contract for an upload service
    [ServiceContract(Namespace="http://Microsoft.Samples.Mtom")]
    public interface IUpload
    {
        [OperationContract]
        int Upload(Stream data);
    }

    // An upload service which takes a Stream and returns its size
    public class UploadService : IUpload
    {
        public int Upload(Stream data)
        {
            int size = 0;
            int bytesRead = 0;
            byte[] buffer = new byte[1024];

            // Read all the data from the stream
            do
            {
                bytesRead = data.Read(buffer, 0, buffer.Length);
                size += bytesRead;
            } while (bytesRead > 0);
            data.Close();

            return size;
        }
    }
}
