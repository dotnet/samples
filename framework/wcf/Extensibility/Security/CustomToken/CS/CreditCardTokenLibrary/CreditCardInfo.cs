//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;

namespace Microsoft.Samples.CustomToken
{
    public class CreditCardInfo
    {
        string cardNumber;
        string cardIssuer;
        DateTime expirationDate;

        public CreditCardInfo(string cardNumber, string cardIssuer, DateTime expirationDate)
        {
            this.cardNumber = cardNumber;
            this.cardIssuer = cardIssuer;
            this.expirationDate = expirationDate;
        }

        public string CardNumber
        {
            get { return this.cardNumber; }
        }

        public string CardIssuer
        {
            get { return this.cardIssuer; }
        }

        public DateTime ExpirationDate
        {
            get { return this.expirationDate; }
        }
    }
}

