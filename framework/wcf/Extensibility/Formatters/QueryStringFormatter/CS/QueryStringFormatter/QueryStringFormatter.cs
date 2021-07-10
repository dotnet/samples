
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Web;
using System.ComponentModel;
using System.Collections.Specialized;

namespace Microsoft.ServiceModel.Samples
{

    class QueryStringFormatter : IClientMessageFormatter, IDispatchMessageFormatter
    {
        IClientMessageFormatter innerClientFormatter;
        IDispatchMessageFormatter innerDispatchFormatter;
        ParameterInfo[] parameterInfos;
        string operationName;
        string action;
        EndpointAddress address;

        public QueryStringFormatter(string operationName, ParameterInfo[] parameterInfos, IClientMessageFormatter innerClientFormatter, string action, EndpointAddress address)
        {
            if (operationName == null)
                throw new ArgumentNullException("methodName");

            if (parameterInfos == null)
                throw new ArgumentNullException("parameterInfos");

            if (innerClientFormatter == null)
                throw new ArgumentNullException("innerClientFormatter");

            if (action == null)
                throw new ArgumentNullException("action");

            if (address == null)
                throw new ArgumentNullException("address");

            QueryStringFormatter.ValidateSupportedType(parameterInfos);
            this.innerClientFormatter = innerClientFormatter;
            this.parameterInfos = parameterInfos;
            this.operationName = operationName;
            this.action = action;
            this.address = address;
        }

        public QueryStringFormatter(string operationName, ParameterInfo[] parameterInfos, IDispatchMessageFormatter innerDispatchFormatter)
        {
            if (operationName == null)
                throw new ArgumentNullException("operationName");

            if (parameterInfos == null)
                throw new ArgumentNullException("parameterInfos");

            if (innerDispatchFormatter == null)
                throw new ArgumentNullException("innerDispatchFormatter");

            QueryStringFormatter.ValidateSupportedType(parameterInfos);
            this.innerDispatchFormatter = innerDispatchFormatter;
            this.operationName = operationName;
            this.parameterInfos = parameterInfos;
        }

        Message IClientMessageFormatter.SerializeRequest(MessageVersion messageVersion, object[] parameters)
        {
            Message message = Message.CreateMessage(messageVersion, action);

            address.ApplyTo(message);
            UriBuilder builder = new UriBuilder(message.Headers.To);
            builder.Path = builder.Path + "/" + this.operationName;
            builder.Query = SerializeParameters(parameterInfos, parameters);
            message.Headers.To = builder.Uri;
            message.Properties.Via = builder.Uri;

            return message;
        }

        object IClientMessageFormatter.DeserializeReply(Message message, object[] parameters)
        {
            return innerClientFormatter.DeserializeReply(message, parameters);
        }

        void IDispatchMessageFormatter.DeserializeRequest(Message message, object[] parameters)
        {
            Uri via = null;
            if (message.Properties.ContainsKey("Via"))
            {
                via = message.Properties["Via"] as Uri;
            }

            string queryString = null;
            if (via != null && !String.IsNullOrEmpty(via.ToString()))
            {
                queryString = via.Query;
            }
            DeserializeParameters(parameterInfos, ParseQueryString(queryString), parameters);
        }

        Message IDispatchMessageFormatter.SerializeReply(MessageVersion messageVersion, object[] parameters, object result)
        {
            return innerDispatchFormatter.SerializeReply(messageVersion, parameters, result);
        }

        string SerializeParameters(ParameterInfo[] parameterInfos, object[] parameters)
        {
            if (parameterInfos.Length != parameters.Length)
                throw new ArgumentException("ParameterInfo(s) and parameter(s) must conain the same number of items.");

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < parameters.Length; i++)
            {
                if (i > 0)
                    builder.Append('&');

                builder.Append(HttpUtility.UrlEncode(parameterInfos[i].Name));
                builder.Append('=');
                TypeConverter converter = TypeDescriptor.GetConverter(parameterInfos[i].ParameterType);
                builder.Append(HttpUtility.UrlEncode(converter.ConvertToString(parameters[i])));
            }
            return builder.ToString();
        }

        void DeserializeParameters(ParameterInfo[] parameterInfos, Dictionary<string, string> dictionary, object[] parameters)
        {
            if (parameterInfos.Length != parameters.Length)
                throw new ArgumentException("ParameterInfo(s) and parameter(s) must conain the same number of items.");

            for (int i = 0; i < parameters.Length; i++)
            {
                string value;
                if (dictionary.TryGetValue(parameterInfos[i].Name.ToLowerInvariant(), out value))
                {
                    TypeConverter converter = TypeDescriptor.GetConverter(parameterInfos[i].ParameterType);
                    object typedValue = converter.ConvertFromString(value);
                    parameters[i] = typedValue;
                }
                else
                {
                    //ignore missing parameters
                }
            }
        }

        internal static void ValidateSupportedType(ParameterInfo[] parameterInfos)
        {
            Type stringtype = typeof(String);
            foreach (ParameterInfo param in parameterInfos)
            {
                TypeConverter converter = TypeDescriptor.GetConverter(param.ParameterType);
                if (!converter.CanConvertFrom(stringtype))
                {
                    throw new Exception(String.Format("QueryStringFormatter does not support parameters of type {0}. Only simple types are supported.", param.ParameterType));
                }
            }
        }

        public static Dictionary<string, string> ParseQueryString(string queryString)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            if (queryString == null)
            {
                return dictionary;
            }

            if ((queryString.Length > 0) && (queryString[0] == '?'))
                queryString = queryString.Substring(1);

            // parse and populate dictionary containing parameter names and values
            string[] keyValuePairs = queryString.Split('&');
            foreach (string keyValuePair in keyValuePairs)
            {
                int equalsIndex = keyValuePair.IndexOf('=');
                string key, value;
                if (equalsIndex < 0)
                {
                    key = keyValuePair;
                    value = string.Empty;
                }
                else
                {
                    key = keyValuePair.Substring(0, equalsIndex);
                    value = keyValuePair.Substring(equalsIndex + 1, keyValuePair.Length - equalsIndex - 1);
                }

                dictionary.Add(
                    HttpUtility.UrlDecode(key).ToLowerInvariant(),
                    HttpUtility.UrlDecode(value));
            }
            return dictionary;
        }

    }
}
