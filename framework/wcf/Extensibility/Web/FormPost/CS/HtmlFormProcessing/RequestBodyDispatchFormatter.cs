//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Microsoft.Samples.FormPost
{
    // This is the base class that our custom form post formatter will derive from.  This class holds the
    //  logic for deserializing the Uri parts of the message.  The logic for deserializing the request body
    //  content part of the message is handled by the derived class.
    public abstract class RequestBodyDispatchFormatter : IDispatchMessageFormatter
    {
        IDispatchMessageFormatter innerFormatter;
        OperationParameterInfo[] operationParameterInfo;
        int partsCount;

        public QueryStringConverter QueryStringConverter
        {
            get;
            private set;
        }

        public Type BodyParameterType
        {
            get;
            private set;
        }

        public string BodyParameterName
        {
            get;
            private set;
        }

        protected RequestBodyDispatchFormatter(OperationDescription operation, UriTemplate uriTemplate, QueryStringConverter converter, IDispatchMessageFormatter innerFormatter)
        {
            // The inner formatter is the default formatter that WCF normally uses.  When the request can't be deserialized
            //  by our custom formatter, we'll use the default formatter.
            this.innerFormatter = innerFormatter;

            // We'll use the query string converter for both Uri query string parameters and the values of the 
            //  form post data
            this.QueryStringConverter = converter;

            // Messages[0] is the request message
            MessagePartDescriptionCollection parts = operation.Messages[0].Body.Parts;

            // This partsCount includes the Uri parts (both path segment variables and query string variables) 
            //  1 body content part
            partsCount = parts.Count;

            ReadOnlyCollection<string> uriPathVariables = uriTemplate.PathSegmentVariableNames;
            ReadOnlyCollection<string> uriQueryVariables = uriTemplate.QueryValueVariableNames;

            // For each part of the message, we need to capture it's name, type, and whether it is
            //  a Uri path segment variable, a Uri query string variable or the body content
            operationParameterInfo = new OperationParameterInfo[partsCount];
            for (int x = 0; x < partsCount; x++)
            {
                string name = parts[x].Name;
                Type type = parts[x].Type;

                // We'll assume this part is the message body, but then check if there are
                //  uri variables that match the name
                OperationParameterKind kind = OperationParameterKind.MessageBody;
                bool canConvert = false;
                CaseInsensitiveEqualityComparer<string> comparer = new CaseInsensitiveEqualityComparer<string>();

                if (uriPathVariables.Contains(name, comparer))
                {
                    kind = OperationParameterKind.UriPathVariable;
                }
                else if (uriQueryVariables.Contains(name, comparer))
                {
                    canConvert = converter.CanConvert(type);
                    kind = OperationParameterKind.UriQueryVariable;
                }
                else
                {
                    // If we reached here, then this part really is the message body part.
                    //  We'll store the name and type in the class properties so that derived
                    //  types have access to this information
                    this.BodyParameterName = name;
                    this.BodyParameterType = type;
                }

                operationParameterInfo[x] = new OperationParameterInfo(kind, type, name, canConvert);
            }
        }

        public void DeserializeRequest(Message message, object[] parameters)
        {
            // Classes that derive from this class will implement CanDeserializeRequestBody() to
            //  tell us if we should deserialize this request or allow the default inner formatter to deserailize it.
            if (CanDeserializeRequestBody(message))
            {
                // The UriTemplateMatch instance for the request will tell us the values of the Uri variable parts
                UriTemplateMatch match = message.Properties["UriTemplateMatchResults"] as UriTemplateMatch;

                for (int x = 0; x < this.partsCount; x++)
                {
                    // For each part of the message we check if the part is the message body or a Uri variable
                    OperationParameterKind kind = operationParameterInfo[x].Kind;
                    if (kind == OperationParameterKind.MessageBody)
                    {
                        // Since this part is the message body we'll let the derived class deserailize it.
                        //  We'll provide the message body as a stream to make it easier to work with
                        Stream stream = message.GetBodyAsStream();
                        parameters[x] = DeserializeRequestBody(stream);
                    }
                    else
                    {
                        // Since this part is a Uri variable, we'll get the value for it from the UriTemplateMAtch instance
                        string value = match.BoundVariables[operationParameterInfo[x].Name];

                        if (kind == OperationParameterKind.UriQueryVariable &&
                            operationParameterInfo[x].CanConvert &&
                            value != null)
                        {
                            // Query string Uri variables should be converted to the type of the parameter from the method signature 
                            parameters[x] = QueryStringConverter.ConvertStringToValue(value, operationParameterInfo[x].Type); ;
                        }
                        else
                        {
                            // Path segment Uri variables are always provided as strings
                            parameters[x] = value;
                        }
                    }
                }
            }
            else
            {
                this.innerFormatter.DeserializeRequest(message, parameters);
            }
        }

        protected abstract object DeserializeRequestBody(Stream body);

        protected abstract bool CanDeserializeRequestBody(Message message);

        public Message SerializeReply(MessageVersion messageVersion, object[] parameters, object result)
        {
            throw new NotSupportedException("The BodyRequestFormatter only supports deserializing incoming requests.");
        }

        private class CaseInsensitiveEqualityComparer<T> : IEqualityComparer<T>
        {
            public bool Equals(T x, T y)
            {
                return string.Equals(x.ToString(), y.ToString(), StringComparison.OrdinalIgnoreCase);
            }

            public int GetHashCode(T obj)
            {
                throw new NotSupportedException();
            }
        }

        private enum OperationParameterKind
        {
            UriPathVariable,
            UriQueryVariable,
            MessageBody
        }

        private class OperationParameterInfo
        {
            public OperationParameterInfo(OperationParameterKind kind, Type type, string name, bool canConvert)
            {
                this.Kind = kind;
                this.Type = type;
                this.Name = name;
                this.CanConvert = canConvert;
            }

            public OperationParameterKind Kind
            {
                get;
                private set;
            }

            public Type Type
            {
                get;
                private set;
            }

            public string Name
            {
                get;
                private set;
            }

            public bool CanConvert
            {
                get;
                private set;
            }

        }
    }
}
