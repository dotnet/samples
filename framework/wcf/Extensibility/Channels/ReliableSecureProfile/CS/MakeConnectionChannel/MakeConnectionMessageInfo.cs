//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System.ServiceModel.Channels;
using System.Xml;

namespace Microsoft.Samples.ReliableSecureProfile
{
    sealed class MakeConnectionMessageInfo
    {
        public MakeConnectionMessageInfo()
        {
        }

        public string Address { get; set; }
        public UniqueId Identifier { get; set; }
        public string UnknownSelection { get; set; }
        public bool MultipleAddressHeaders { get; set; }
        public bool MultipleIdentifierHeaders { get; set; }

        public static MakeConnectionMessageInfo ReadMessage(Message message)
        {
            if (message.IsEmpty)
            {
                return null;
            }

            MakeConnectionMessageInfo info;
            using (XmlDictionaryReader reader = message.GetReaderAtBodyContents())
            {
                info = MakeConnectionBodyWriter.Create(reader);
            }

            return info;
        }
    }
}
