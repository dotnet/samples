//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System.IO;
using System.ServiceModel;

namespace Microsoft.Samples.WSStreamedHttpBinding
{
    [ServiceContract(Namespace="http://Microsoft.Samples.WSStreamedHttpBinding")]
    public interface IStreamedEchoService
    {
        [OperationContract]
        Stream Echo(Stream data);
    }

    public class StreamedEchoService : IStreamedEchoService
    {
        public Stream Echo(Stream data)
        {
            MemoryStream dataStorage = new MemoryStream();
            byte[] byteArray = new byte[8192];
            int bytesRead = data.Read(byteArray, 0, 8192);
            while (bytesRead > 0)
            {
                dataStorage.Write(byteArray, 0, bytesRead);
                bytesRead = data.Read(byteArray, 0, 8192);
            }
            data.Close();
            dataStorage.Seek(0, SeekOrigin.Begin);

            return dataStorage;
        }
    }
}
