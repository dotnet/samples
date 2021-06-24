//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System.ServiceModel.Discovery;

namespace Microsoft.Samples.Discovery
{

    static class ProtocolStrings
    {
        internal const string IdAttributeName = "Id";
        internal const string DiscoveryPrefix = "d";
        internal const string InclusiveNamespacesPrefix = "ec";

        // Element names found in the expanded signature
        internal const string FullSignatureElementName = "Signature";
        internal const string SignedInfoElementName = "SignedInfo";
        internal const string CanonicalizationMethodElement = "CanonicalizationMethod";
        internal const string SignatureMethodElement = "SignatureMethod";
        internal const string AlgorithmAttributeName = "Algorithm";
        internal const string ReferenceElementName = "Reference";
        internal const string ReferenceURIElementName = "URI";
        internal const string SignedInfoTransforms = "Transforms";
        internal const string SignedInfoTransform = "Transform";
        internal const string DigestMethodElement = "DigestMethod";
        internal const string DigestValueElement = "DigestValue";
        internal const string SignatureValueElementName = "SignatureValue";
        internal const string KeyInfoElementName = "KeyInfo";
        internal const string InclusiveNamespacesElementName = "InclusiveNamespaces";
        internal const string PrefixListAttribute = "PrefixList";

        // URI defined in the expanded signature
        internal const string CanonicalizationAlgorithmUri = "http://www.w3.org/2001/10/xml-exc-c14n#";
        internal const string SignatureAlgorithmSHA1Uri = "http://www.w3.org/2000/09/xmldsig#rsa-sha1";
        internal const string DigestAlgorithmUri = "http://www.w3.org/2000/09/xmldsig#sha1";

        // Attributes found in the expanded signature
        internal const string CompactSignatureElementName = "Sig";
        internal const string CompactSignatureSchemeAttribute = "Scheme";
        internal const string CompactSignatureKeyIdAttribute = "KeyId";
        internal const string CompactSignatureRefsAttribute = "Refs";

        internal const string SignaturePrefix = "ds";
        internal const string SignatureNamespace = "http://www.w3.org/2000/09/xmldsig#";

        internal const string SoapPrefix = "s";
        internal const string SoapNamespace12Uri = "http://www.w3.org/2003/05/soap-envelope";
        internal const string SoapNamespace11Uri = "http://schemas.xmlsoap.org/soap/envelope/";

        internal const string SecurityHeaderName = "Security";
        internal const string HeaderHeaderName = "Header";
        internal const string BodyElementName = "Body";
        internal const string EnvelopeElementName = "Envelope";

        internal const string WsaNamespaceAugust2004 = "http://schemas.xmlsoap.org/ws/2004/08/addressing";
        internal const string WsaNamespace10 = "http://www.w3.org/2005/08/addressing";

        internal static string DiscoveryNamespaceApril2005 = DiscoveryVersion.WSDiscoveryApril2005.Namespace;
        internal static string DiscoveryNamespace11 = DiscoveryVersion.WSDiscovery11.Namespace;
        internal static string SchemeUriAugust2004 = ProtocolStrings.DiscoveryNamespaceApril2005 + "/rsa";
        internal static string SchemeUri11 = ProtocolStrings.DiscoveryNamespace11 + "/rsa";

        internal static char[] WhitespaceChars = new char[] { ' ', '\t', '\n', '\r' };
    }
}
