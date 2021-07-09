//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IdentityModel.Tokens; 

namespace Microsoft.Samples.CustomToken
{
    class CreditCardToken : SecurityToken
    {
        CreditCardInfo cardInfo;
        DateTime effectiveTime = DateTime.UtcNow;
        string id;
        ReadOnlyCollection<SecurityKey> securityKeys;

        public CreditCardToken(CreditCardInfo cardInfo) : this(cardInfo, Guid.NewGuid().ToString()) { }

        public CreditCardToken(CreditCardInfo cardInfo, string id)
        {
            if (cardInfo == null)
                throw new ArgumentNullException("cardInfo");

            if (id == null)
                throw new ArgumentNullException("id");

            this.cardInfo = cardInfo;
            this.id = id;

            // the credit card token is not capable of any crypto
            this.securityKeys = new ReadOnlyCollection<SecurityKey>(new List<SecurityKey>());
        }

        public CreditCardInfo CardInfo { get { return this.cardInfo; } }

        public override ReadOnlyCollection<SecurityKey> SecurityKeys { get { return this.securityKeys; } }

        public override DateTime ValidFrom { get {return this.effectiveTime; } }
        public override DateTime ValidTo { get { return this.cardInfo.ExpirationDate; } }
        public override string Id { get { return this.id; } }


    }
}
