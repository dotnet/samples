//-----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All Rights Reserved.
//-----------------------------------------------------------------

using System.ServiceModel.Security;

namespace Microsoft.Samples.CryptoAgility
{
    /// <summary>
    /// This class defines a custom algorithm suite to be used for crypto operations.
    /// </summary>
    public class MyCustomAlgorithmSuite : SecurityAlgorithmSuite
    {
        public override string DefaultAsymmetricKeyWrapAlgorithm
        {
            get
            {
                return SecurityAlgorithms.RsaOaepKeyWrap;
            }
        }

        public override string DefaultAsymmetricSignatureAlgorithm
        {
            get { return SecurityAlgorithms.RsaSha1Signature; }
        }

        public override string DefaultCanonicalizationAlgorithm
        {
            get { return SecurityAlgorithms.ExclusiveC14n; ; }
        }

        public override string DefaultDigestAlgorithm
        {
            get { return SecurityAlgorithms.MyCustomHashAlgorithm; }
        }

        public override string DefaultEncryptionAlgorithm
        {
            get { return SecurityAlgorithms.Aes128Encryption; }
        }

        public override int DefaultEncryptionKeyDerivationLength
        {
            get { return 128; }
        }

        public override int DefaultSignatureKeyDerivationLength
        {
            get { return 128; }
        }

        public override int DefaultSymmetricKeyLength
        {
            get { return 128; }
        }

        public override string DefaultSymmetricKeyWrapAlgorithm
        {
            get { return SecurityAlgorithms.Aes128Encryption; }
        }

        public override string DefaultSymmetricSignatureAlgorithm
        {
            get { return SecurityAlgorithms.HmacSha1Signature; }
        }

        public override bool IsAsymmetricKeyLengthSupported(int length)
        {
            return length >= 1024 && length <= 4096;
        }

        public override bool IsSymmetricKeyLengthSupported(int length)
        {
            return length >= 128 && length <= 256;
        }
    }

    /// <summary>
    /// This class defines the URI for the various crypto algorithms to be used in the MyCustomAlgorithmSuite class. 
    /// </summary>
    internal static class SecurityAlgorithms
    {
        // The following algorithm URIs are the standard URIs for the corresponding standard algorithms as mentioned 
        // in the WS-SecurityPolicy specification.
        public const string Aes128Encryption = "http://www.w3.org/2001/04/xmlenc#aes128-cbc";
        public const string Aes128KeyWrap = "http://www.w3.org/2001/04/xmlenc#kw-aes128";
        public const string ExclusiveC14n = "http://www.w3.org/2001/10/xml-exc-c14n#";
        public const string HmacSha1Signature = "http://www.w3.org/2000/09/xmldsig#hmac-sha1";
        public const string RsaOaepKeyWrap = "http://www.w3.org/2001/04/xmlenc#rsa-oaep-mgf1p";
        public const string RsaSha1Signature = "http://www.w3.org/2000/09/xmldsig#rsa-sha1";
        
        // This defines a custom URI for the custom hash algorithm. 
        public const string MyCustomHashAlgorithm = "http://constoso.com/CustomAlgorithms/CustomHashAlgorithm";
    }
}
