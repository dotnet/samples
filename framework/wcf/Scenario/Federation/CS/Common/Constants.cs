//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------
using System;

namespace Microsoft.Samples.Federation
{
    public class Constants
    {
        public const string BookNameHeaderNamespace = "http://tempuri.org/";
        public const string BookNameHeaderName = "BookName";
        public const string PurchaseClaimNamespace = "http://tempuri.org/";
        public const string PurchaseAuthorizedClaim = PurchaseClaimNamespace + "PurchaseAuthorizedClaim";
        public const string PurchaseLimitClaim = PurchaseClaimNamespace + "PurchaseLimitClaim";
        public const string SamlTokenTypeUri = "http://docs.oasis-open.org/wss/oasis-wss-saml-token-profile-1.1#SAMLV1.1";

        public const string BookStoreServiceAudienceUri = "http://localhost/FederationSample/BookStoreService/store.svc/buy";
        public const string BookStoreSTSAudienceUri = "http://localhost/FederationSample/BookStoreSTS/STS.svc";

        // Various constants for WS-Trust
        public class Trust
        {
            public const string NamespaceUri = "http://schemas.xmlsoap.org/ws/2005/02/trust";

            public class Actions
            {
                public const string Issue = "http://schemas.xmlsoap.org/ws/2005/02/trust/RST/Issue";
                public const string IssueReply = "http://schemas.xmlsoap.org/ws/2005/02/trust/RSTR/Issue";
            }

            public class Attributes
            {
                public const string Context = "Context";
            }

            public class Elements
            {
                public const string KeySize = "KeySize";
                public const string Entropy = "Entropy";
                public const string BinarySecret = "BinarySecret";
                public const string RequestSecurityToken = "RequestSecurityToken";
                public const string RequestSecurityTokenResponse = "RequestSecurityTokenResponse";
                public const string TokenType = "TokenType";
                public const string RequestedSecurityToken = "RequestedSecurityToken";
                public const string RequestedAttachedReference = "RequestedAttachedReference";
                public const string RequestedUnattachedReference = "RequestedUnattachedReference";
                public const string RequestedProofToken = "RequestedProofToken";
                public const string ComputedKey = "ComputedKey";
            }

            public class ComputedKeyAlgorithms
            {
                public const string PSHA1 = "http://schemas.xmlsoap.org/ws/2005/02/trust/CK/PSHA1";
            }
        }

        // Various constants for WS-Policy
        public class Policy
        {
            public const string NamespaceUri = "http://schemas.xmlsoap.org/ws/2004/09/policy";

            public class Elements
            {
                public const string AppliesTo = "AppliesTo";
            }
        }

        // Various constants for WS-Addressing
        public class Addressing
        {
            public const string NamespaceUri = "http://www.w3.org/2005/08/addressing";

            public class Elements
            {
                public const string EndpointReference = "EndpointReference";
            }
        }
    }
}

