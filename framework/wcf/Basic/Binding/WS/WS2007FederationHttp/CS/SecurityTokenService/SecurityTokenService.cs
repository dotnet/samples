//  Copyright (c) Microsoft Corporation. All rights reserved.

using System;
using System.Collections.Generic;
using System.IdentityModel.Claims;
using System.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security.Tokens;

namespace Microsoft.Samples.WS2007FederationHttpBinding
{
    class SecurityTokenService : IWSTrust13
    {
        string issuer;
        X509SecurityToken issuerToken;
        
        // Public constructors.
        public SecurityTokenService() : this("STS")
        {            
        }

        public SecurityTokenService(string issuerName)
        {
            issuer = issuerName;
            issuerToken = GetToken(issuerName, StoreName.My, StoreLocation.LocalMachine);
        }

        /// <summary>
        /// Finds a certificate and returns an X509SecurityToken for that certificate.
        /// </summary>
        /// <param name="subjectName">The subject name of the certificate to use as the basis for the security token</param>
        /// <returns>An X509SecurityToken</returns>
        static X509SecurityToken GetToken(string subjectName, StoreName storeName, StoreLocation storeLocation)
        {
            // Open the CurrentUser/TrustedPeople store.
            X509Store store = new X509Store(storeName, storeLocation);
            store.Open(OpenFlags.ReadOnly);
            try
            {
                // Find the certificates that match the Subject Name.
                X509Certificate2Collection matches = store.Certificates.Find(X509FindType.FindBySubjectDistinguishedName, "CN=" + subjectName, false);
                if (matches == null || matches.Count == 0)
                {
                    throw new ArgumentException(String.Format("No certificates with subjectName {0} were found in CurrentUser\\TrustedPeople store", subjectName));
                }
                if (matches.Count > 1)
                {
                    throw new ArgumentException(String.Format("Multiple certificates with subjectName {0} were found in CurrentUser\\TrustedPeople store", subjectName));
                }
                return new X509SecurityToken(matches[0]);
            }
            finally
            {
                store.Close();
            }
        }

        #region IWSTrust13 Members

        public Message Issue(Message request)
        {
            try
            {
                Console.WriteLine("Call to IWSTrust13::Issue");

                // if the request is null, an exception is thrown.
                if (request == null)
                    throw new ArgumentNullException("request");

                // Create an RST object from the request message.
                RequestSecurityTokenWSTrust13 rst = RequestSecurityTokenWSTrust13.CreateFrom(request.GetReaderAtBodyContents());

                // Check that it really is an Issue request.
                if (rst.RequestType == null || rst.RequestType != Constants.Trust13.RequestTypes.Issue)
                    throw new InvalidOperationException(rst.RequestType);
            
                // Create an RSTR object.
                RequestSecurityTokenResponseWSTrust13 rstr = Issue(rst);

                // Create response message.
                Message response = Message.CreateMessage(request.Version, Constants.Trust13.Actions.IssueReply, rstr);

                // Set RelatesTo of response to message id of request.
                response.Headers.RelatesTo = request.Headers.MessageId;

                // Address response to ReplyTo of request.
                request.Headers.ReplyTo.ApplyTo(response);
                return response;
            }
            catch (Exception e)
            {
                Console.WriteLine("**** Exception thrown while processing Issue request:");
                Console.WriteLine(e.Message);
                throw;
            }            
        }

        #endregion

        private RequestSecurityTokenResponseWSTrust13 Issue(RequestSecurityTokenWSTrust13 rst)
        {
            // If rst is null, an exception is thrown.
            if (rst == null)
                throw new ArgumentNullException("rst");

            // Create an RSTR object.
            RequestSecurityTokenResponseWSTrust13 rstr = new RequestSecurityTokenResponseWSTrust13();

            string tokenType = rst.TokenType;
            Console.WriteLine("Issue: Request for token type {0}", tokenType);
            if (tokenType != null && tokenType != "http://docs.oasis-open.org/wss/oasis-wss-saml-token-profile-1.1#SAMLV1.1")
            {
                throw new NotSupportedException("Unsupported token type " + tokenType);
            }

            SecurityKey signingKey = issuerToken.SecurityKeys[0];
            SecurityKeyIdentifier signingKeyIdentifier = new SecurityKeyIdentifier(issuerToken.CreateKeyIdentifierClause<X509ThumbprintKeyIdentifierClause>());
            SecurityKeyIdentifier proofKeyIdentifier = null;

            if (rst.IsProofKeyAsymmetric())
            {
                throw new NotSupportedException("Public key issuance is not supported");
            }
            // Symmetric proof key.
            Console.WriteLine("Constructing Symmetric Proof Key");
         
            // Construct the session key. This is the symmetric key that the client and the service share. 
            // It appears twice in the response message; once for the service and 
            // once for the client. For the service, it is typically embedded in the issued token, 
            // for the client, it is returned in a wst:RequestedProofToken element.
            byte[] sessionKey = GetSessionKey(rst, rstr);

            // Get a token to use when encrypting key material for the service.
            SecurityToken encryptingToken = DetermineEncryptingToken(rst);

            // Encrypt the session key for the service.
            GetEncryptedKey(encryptingToken, sessionKey, out proofKeyIdentifier);

            // Issued tokens are valid for 12 hours by default.
            DateTime effectiveTime = DateTime.Now;
            DateTime expirationTime = DateTime.Now + new TimeSpan(12, 0, 0);
            SecurityToken samlToken = CreateSAMLToken(effectiveTime, expirationTime, signingKey, signingKeyIdentifier, proofKeyIdentifier);

            rstr.RequestedSecurityToken = samlToken;
            rstr.Context = rst.Context;
            rstr.TokenType = tokenType;
            SecurityKeyIdentifierClause samlReference = samlToken.CreateKeyIdentifierClause<SamlAssertionKeyIdentifierClause>();
            rstr.RequestedAttachedReference = samlReference;
            rstr.RequestedUnattachedReference = samlReference;
            return rstr;
        }

        // The RST message can contain a wsp:AppliesTo, which can be used to indicate the service that 
        // the issued token is intended for. This method extracts the URI for that service, if any such
        // URI is present in the RST. Otherwise it returns null.
        private static Uri DetermineEndpointUri(RequestSecurityTokenWSTrust13 rst)
        {
            // If rst is null, an exception is thrown.
            if (rst == null)
                throw new ArgumentNullException("rst");

            EndpointAddress epr = rst.AppliesTo;

            // If AppliesTo is missing or does not contain a 2004/08 or 
            // 2005/10 EPR, then the EPR local variable is null, in which case we return null.
            // Otherwise we return the URI portion of the EPR.
            return epr == null ? null : epr.Uri;
        }

        #region Session key methods
        private static byte[] GetSenderEntropy(RequestSecurityTokenWSTrust13 rst)
        {
            // If rst is null, an exception is thrown.
            if (rst == null)
                throw new ArgumentNullException("rst");

            SecurityToken senderEntropyToken = rst.RequestorEntropy;
            byte[] senderEntropy = null;

            if (senderEntropyToken != null)
            {
                BinarySecretSecurityToken bsst = senderEntropyToken as BinarySecretSecurityToken;

                if (bsst != null)
                    senderEntropy = bsst.GetKeyBytes();
            }

            return senderEntropy;
        }

        private static byte[] GetIssuerEntropy(int keySize)
        {
            RNGCryptoServiceProvider random = new RNGCryptoServiceProvider();
            byte[] issuerEntropy = new byte[keySize / 8];
            random.GetNonZeroBytes(issuerEntropy);
            return issuerEntropy;
        }

        private static byte[] GetSessionKey(RequestSecurityTokenWSTrust13 rst, RequestSecurityTokenResponseWSTrust13 rstr)
        {
            // If rst is null, an exception is thrown.
            if (rst == null)
                throw new ArgumentNullException("rst");

            // If rstr is null, an exception is thrown.
            if (rstr == null)
                throw new ArgumentNullException("rstr");

            // Figure out the keySize
            int keySize = 256;

            if (rst.KeySize != 0)
                keySize = rst.KeySize;

            Console.WriteLine("Proof key size {0}", keySize);

            // Figure out whether  Combined or Issuer entropy is being used.
            byte[] sessionKey = null;
            byte[] senderEntropy = GetSenderEntropy(rst);
            byte[] issuerEntropy = GetIssuerEntropy(keySize);

            if (senderEntropy != null)
            {
                // Combined entropy.
                Console.WriteLine("Combined Entropy");                
                sessionKey = RequestSecurityTokenResponseWSTrust13.ComputeCombinedKey(senderEntropy, issuerEntropy, keySize);
                rstr.IssuerEntropy = new BinarySecretSecurityToken ( issuerEntropy );
                rstr.ComputeKey = true;
            }
            else
            {
                // Issuer-only entropy.
                Console.WriteLine("Issuer-only entropy");
                sessionKey = issuerEntropy;
                rstr.RequestedProofToken = new BinarySecretSecurityToken(sessionKey);
            }

            rstr.KeySize = keySize;
            return sessionKey;
        }
        #endregion

        // This method determines the security token that contains the key material that 
        // the STS should encrypt a session key with. 
        // For the service, the issued token is intended to be able to extract that session key.
        private static SecurityToken DetermineEncryptingToken(RequestSecurityTokenWSTrust13 rst)
        {
            // If rst is null, an exception is thrown.
            if (rst == null)
                throw new ArgumentNullException("rst");

            // Figure out service URI.
            Uri uri = DetermineEndpointUri(rst);

            string encryptingTokenSubjectName = (uri == null) ? "localhost" : uri.DnsSafeHost;
            return GetToken(encryptingTokenSubjectName, StoreName.TrustedPeople, StoreLocation.LocalMachine);
        }


        // This method returns a security token to be used as the requested proof token portion of the RSTR.
        // The key parameter is the proof key that is shared by the client and the service.
        private static SecurityToken GetRequestedProofToken(byte[] key)
        {
            // If key is null, an exception is thrown.
            if (key == null)
                throw new ArgumentNullException("key");
            return new BinarySecretSecurityToken(key);
        }

        private static string GetKeyWrapAlgorithm(SecurityKey key)
        {
            // If key is null, an exception is thrown.
            if (key == null)
                throw new ArgumentNullException("key");

            // Set keywrapAlgorithm to null.
            string keywrapAlgorithm = null;

            // If the security key supports RsaOaep then use that ...
            if (key.IsSupportedAlgorithm(SecurityAlgorithms.RsaOaepKeyWrap))
                keywrapAlgorithm = SecurityAlgorithms.RsaOaepKeyWrap;
            // ... otherwise if it supports RSA15 use that ...
            else if (key.IsSupportedAlgorithm(SecurityAlgorithms.RsaV15KeyWrap))
                keywrapAlgorithm = SecurityAlgorithms.RsaV15KeyWrap;
            // ... otherwise if it supports AES256 use that ...
            else if (key.IsSupportedAlgorithm(SecurityAlgorithms.Aes256KeyWrap))
                keywrapAlgorithm = SecurityAlgorithms.Aes256KeyWrap;
            // ... otherwise if it supports AES192 use that ...
            else if (key.IsSupportedAlgorithm(SecurityAlgorithms.Aes192KeyWrap))
                keywrapAlgorithm = SecurityAlgorithms.Aes192KeyWrap;
            // ... otherwise if it supports AES128 use that ...
            else if (key.IsSupportedAlgorithm(SecurityAlgorithms.Aes128KeyWrap))
                keywrapAlgorithm = SecurityAlgorithms.Aes128KeyWrap;

            return keywrapAlgorithm;
        }

        private static string GetSignatureAlgorithm(SecurityKey key)
        {
            // If key is null, an exception is thrown.
            if (key == null)
                throw new ArgumentNullException("key");

            // Set signatureAlgorithm to null.
            string signatureAlgorithm = null;

            // If the security key supports RsaSha1 then use that ...
            if (key.IsSupportedAlgorithm(SecurityAlgorithms.RsaSha1Signature))
                signatureAlgorithm = SecurityAlgorithms.RsaSha1Signature;
            // ... otherwise if it supports HMACSha1 use that ...
            else if (key.IsSupportedAlgorithm(SecurityAlgorithms.HmacSha1Signature))
                signatureAlgorithm = SecurityAlgorithms.HmacSha1Signature;

            return signatureAlgorithm;
        }

        private static string GetEncryptionAlgorithm(SecurityKey key)
        {
            // If key is null, an exception is thrown.
            if (key == null)
                throw new ArgumentNullException("key");

            // Set encryptionAlgorithm to null.
            string encryptionAlgorithm = null;

            // If the security key supports AES256 use that ...
            if (key.IsSupportedAlgorithm(SecurityAlgorithms.Aes256Encryption))
                encryptionAlgorithm = SecurityAlgorithms.Aes256Encryption;
            // ... otherwise if it supports AES192 use that ...
            else if (key.IsSupportedAlgorithm(SecurityAlgorithms.Aes192Encryption))
                encryptionAlgorithm = SecurityAlgorithms.Aes192Encryption;
            // ... otherwise if it supports AES128 use that ...
            else if (key.IsSupportedAlgorithm(SecurityAlgorithms.Aes128Encryption))
                encryptionAlgorithm = SecurityAlgorithms.Aes128Encryption;

            return encryptionAlgorithm;
        }

        // This method encrypts the provided key using the key material associated with the certificate
        // returned by DetermineEncryptingCert.
        private static byte[] GetEncryptedKey(SecurityToken encryptingToken, byte[] key, out SecurityKeyIdentifier ski)
        {
            // If encryptingToken is null, an exception is thrown.
            if (encryptingToken == null)
                throw new ArgumentNullException("encryptingToken");

            // If key is null, an exception is thrown.
            if (key == null)
                throw new ArgumentNullException("key");

            // Get the zeroth security key.
            SecurityKey encryptingKey = encryptingToken.SecurityKeys[0];

            // Get the encryption algorithm to use.
            string keywrapAlgorithm = GetKeyWrapAlgorithm(encryptingKey);
            
            // Encrypt the passed in key. 
            byte[] encryptedKey = encryptingKey.EncryptKey ( keywrapAlgorithm, key );

            // Get a key identifier for the encrypting key.
            SecurityKeyIdentifier eki = new SecurityKeyIdentifier(encryptingToken.CreateKeyIdentifierClause<X509ThumbprintKeyIdentifierClause>());

            // Return the proof key identifier.
            ski = GetProofKeyIdentifier ( encryptedKey, keywrapAlgorithm, eki );

            // Return the encrypted key.
            return encryptedKey;
        }

        private static SecurityKeyIdentifier GetProofKeyIdentifier(byte[] key, string algorithm, SecurityKeyIdentifier eki )
        {
            // If key is null, an exception is thrown.
            if (key == null)
                throw new ArgumentNullException("key");

            // Create list of SecurityKeyIdentifierClauses.
            List<SecurityKeyIdentifierClause> skics = new List<SecurityKeyIdentifierClause>();
            skics.Add(new EncryptedKeyIdentifierClause(key, algorithm, eki));
            return new SecurityKeyIdentifier(skics.ToArray());
        }

        private SecurityToken CreateSAMLToken(DateTime validFrom, DateTime validTo, SecurityKey signingKey, SecurityKeyIdentifier signingKeyIdentifier, SecurityKeyIdentifier proofKeyIdentifier )
        {
            // Create list of confirmation strings.
            List<string> confirmations = new List<string>();

            // Add holder-of-key string to list of confirmation strings.
            confirmations.Add("urn:oasis:names:tc:SAML:1.0:cm:holder-of-key");

            // Create SAML subject statement based on issuer member variable, confirmation string collection 
            // local variable and proof key identifier parameter.
            SamlSubject subject = new SamlSubject(null, null, issuer, confirmations, null, proofKeyIdentifier);

            // Create a list of SAML attributes.
            List<SamlAttribute> attributes = new List<SamlAttribute>();

            // Get the claimset to place into the SAML assertion.
            ClaimSet cs = GetClaimSet();

            // Iterate through the claims and add a SamlAttribute for each claim.
            // Note that GetClaimSet call above returns a claimset that only contains PossessProperty claims.
            foreach (Claim c in cs)
                attributes.Add(new SamlAttribute(c));

            // Create list of SAML statements.
            List<SamlStatement> statements = new List<SamlStatement>();

            // Add a SAML attribute statement to the list of statements. An attribute statement is based on 
            // a subject statement and SAML attributes that result from claims.
            statements.Add(new SamlAttributeStatement(subject, attributes));

            // Create a valid from/until condition.
            SamlConditions conditions = new SamlConditions(validFrom, validTo);
            
            // Create the SAML assertion.
            SamlAssertion assertion = new SamlAssertion("_" + Guid.NewGuid().ToString(), issuer, validFrom, conditions, null, statements);

            // Set the signing credentials for the SAML assertion.
            string signatureAlgorithm = GetSignatureAlgorithm(signingKey);
            assertion.SigningCredentials = new SigningCredentials(signingKey, signatureAlgorithm, SecurityAlgorithms.Sha1Digest, signingKeyIdentifier);

            return new SamlSecurityToken(assertion);
        }


        // Return a ClaimSet to be serialized into an issued SAML token.
        private static ClaimSet GetClaimSet()
        {
            // Create an empty list of claims.
            List<Claim> claims = new List<Claim>();
                        
            // Iterate through all the ClaimSets in the current AuthorizationContext.
            foreach (ClaimSet cs in OperationContext.Current.ServiceSecurityContext.AuthorizationContext.ClaimSets)
            {
                IEnumerable<Claim> nameClaims = cs.FindClaims(ClaimTypes.Name, Rights.PossessProperty);
                if (nameClaims != null)
                {
                    foreach (Claim claim in nameClaims)
                    {
                        claims.Add(claim);
                    }
                }
            }
                
            // Create a new ClaimSet based on the claims list and return that ClaimSet.
            return new DefaultClaimSet ( DefaultClaimSet.System, claims );
        }
    }
}

