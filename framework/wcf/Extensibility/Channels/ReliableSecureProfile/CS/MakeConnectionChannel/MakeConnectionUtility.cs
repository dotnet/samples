//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System;
using System.ServiceModel.Security;
using System.Xml;

namespace Microsoft.Samples.ReliableSecureProfile
{
    static class MakeConnectionUtility
    {
        static MessagePartSpecification signedReliabilityMessageParts;

        public static bool IsAnonymousUri(Uri uri)
        {
            return (string.Compare(uri.AbsoluteUri, 0,
                MakeConnectionConstants.AnonymousUriTemplate, 0,
                MakeConnectionConstants.AnonymousUriTemplate.Length,
                StringComparison.OrdinalIgnoreCase) == 0);
        }

        public static Exception WrapAsyncException(Exception ex)
        {
            return ex;
        }

        public static MessagePartSpecification GetSignedReliabilityMessageParts()
        {
            return SignedReliabilityMessageParts;
        }

        internal static MessagePartSpecification SignedReliabilityMessageParts
        {
            get
            {
                if (signedReliabilityMessageParts == null)
                {
                    XmlQualifiedName[] wsmcMessageHeaders = new XmlQualifiedName[]
                    {
                        new XmlQualifiedName(MakeConnectionConstants.MessagePending.Name, MakeConnectionConstants.Namespace)
                    };

                    MessagePartSpecification s = new MessagePartSpecification(wsmcMessageHeaders);
                    s.MakeReadOnly();
                    signedReliabilityMessageParts = s;
                }

                return signedReliabilityMessageParts;
            }
        }
    }
}
