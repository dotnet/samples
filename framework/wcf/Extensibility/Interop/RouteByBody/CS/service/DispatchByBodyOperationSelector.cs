
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Xml;

namespace Microsoft.Samples.RouteByBody
{
	class DispatchByBodyElementOperationSelector : IDispatchOperationSelector
	{
        Dictionary<XmlQualifiedName, string> dispatchDictionary;

        public DispatchByBodyElementOperationSelector(Dictionary<XmlQualifiedName, string> dispatchDictionary)
        {
            this.dispatchDictionary = dispatchDictionary;            
        }

        #region IDispatchOperationSelector Members

        private Message CreateMessageCopy(Message message, XmlDictionaryReader body)
        {
            Message copy = Message.CreateMessage(message.Version,message.Headers.Action,body);
            copy.Headers.CopyHeaderFrom(message,0);
            copy.Properties.CopyProperties(message.Properties);
            return copy;
        }

        public string SelectOperation(ref System.ServiceModel.Channels.Message message)
        {
            XmlDictionaryReader bodyReader = message.GetReaderAtBodyContents();
            
            XmlQualifiedName lookupQName = new XmlQualifiedName(bodyReader.LocalName, bodyReader.NamespaceURI);
            message = CreateMessageCopy(message,bodyReader);
            if (dispatchDictionary.ContainsKey(lookupQName))
            {
                return dispatchDictionary[lookupQName];
            }
            else
            {
                return null;
            }
        }

        #endregion
    }
}
