//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.IO;
using System.Linq;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Xml.Linq;

namespace Microsoft.Samples.LinqMessageQueryCorrelation.Service
{
    // This is a simple sample of a custom MessageQuery. 
    // This class takes an XName as input. It will try to match the XElements in the message body and return the value of the first match.
    public class LinqMessageQuery : MessageQuery
    {
        XName xName;

        public LinqMessageQuery(XName xName)
        {
            if (xName == null)
            {
                throw new ArgumentNullException("xName");
            }
            this.xName = xName;
        }

        public override TResult Evaluate<TResult>(MessageBuffer buffer)
        {
            Message message = buffer.CreateMessage();
            return this.Evaluate<TResult>(message);
        }

        public override TResult Evaluate<TResult>(Message message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            if (typeof(TResult) == typeof(string))
            {
                XDocument doc;
                using (StringReader sr = new StringReader(message.GetReaderAtBodyContents().ReadOuterXml()))
                {
                    doc = XDocument.Load(sr);
                }
                var resultSet = from c in doc.Descendants() where c.Name == this.xName select c;    // find all the XElement with the input XName
                foreach (var result in resultSet)
                {
                    return (TResult)(object)result.Value;
                }
                return (TResult)(object)(null);
            }
            throw new NotSupportedException();
        }
    }
}
