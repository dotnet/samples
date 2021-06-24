//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.Runtime.Serialization;
using System.Xml;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Microsoft.ServiceModel.Samples
{
    // Define a couple rather arbitrary filters

    // Matches any message whose To address contains the letter 'e'
	class MatchEAddressFilter : MessageFilter 
	{
        public override bool Match(MessageBuffer buffer)
        {
            return Match(buffer.CreateMessage());
        }
        public override bool Match(Message message)
        {
            Uri to = null;
            try
            {
                to = message.Headers.To;
            }
            catch (XmlException)
            {
            }
            catch (SerializationException)
            {
            }
            catch (CommunicationException)
            {
            }
            if (to == null)
                return false;
            return to.AbsoluteUri.Contains("e");
        }
	}
    // Matches any message whose To address does not contain the letter 'e'
    class MatchNoEAddressFilter : MessageFilter
    {
        public override bool Match(MessageBuffer buffer)
        {
            return Match(buffer.CreateMessage());
        }
        public override bool Match(Message message)
        {
            return !(new MatchEAddressFilter().Match(message));
        }
    }
}
