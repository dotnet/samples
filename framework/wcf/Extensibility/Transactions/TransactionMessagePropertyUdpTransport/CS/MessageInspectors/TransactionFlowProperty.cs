//  Copyright (c) Microsoft Corporation. All rights reserved.

using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Microsoft.ServiceModel.Samples
{
    public class TransactionFlowProperty
    {
        public const string PropertyName = "TransactionFlowProperty";
        byte[] propToken;

        private TransactionFlowProperty()
        {
        }

        public static byte[] Get(Message message)
        {
            if (message == null)
            {
                return null;
            }

            if (message.Properties.ContainsKey(PropertyName))
            {
                TransactionFlowProperty tfp = (TransactionFlowProperty)message.Properties[PropertyName]; 
                return tfp.propToken;
            }

            return null;
        }

        public static void Set(byte[] propToken, Message message)
        {
            if (message.Properties.ContainsKey(PropertyName))
            {
                throw new CommunicationException("A transaction flow property is already set on the message.");
            }

            TransactionFlowProperty property = new TransactionFlowProperty();
            property.propToken = propToken;
            message.Properties.Add(PropertyName, property);
        }
    }
}
