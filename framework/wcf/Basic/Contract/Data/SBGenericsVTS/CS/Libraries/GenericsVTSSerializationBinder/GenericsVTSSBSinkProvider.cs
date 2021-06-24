
//-----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All Rights Reserved.
//-----------------------------------------------------------------

using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Microsoft.Samples.SBGenericsVTS.GenericsVTSSerializationBinder
{

    public class GenericsVTSSBSinkProvider : IServerChannelSinkProvider
    {
        IServerChannelSinkProvider next;

        IServerChannelSink IServerChannelSinkProvider.CreateSink(IChannelReceiver channel)
        {
            return new GenericsVTSSBSink(next.CreateSink(channel));
        }

        void IServerChannelSinkProvider.GetChannelData(IChannelDataStore channelData)
        {
            if (next != null)
            {
                next.GetChannelData(channelData);
            }
        }

        IServerChannelSinkProvider IServerChannelSinkProvider.Next
        {
            get
            {
                return next;
            }
            set
            {
                next = value;
            }
        }

        class GenericsVTSSBSink : IServerChannelSink
        {
            IServerChannelSink next;

            public GenericsVTSSBSink(IServerChannelSink channel)
            {
                next = channel;
            }

            void IServerChannelSink.AsyncProcessResponse(IServerResponseChannelSinkStack sinkStack, object state, IMessage msg, ITransportHeaders headers, Stream stream)
            {
                sinkStack.AsyncProcessResponse(msg, headers, stream);
            }

            ServerProcessing IServerChannelSink.ProcessMessage(IServerChannelSinkStack sinkStack, IMessage requestMsg, ITransportHeaders requestHeaders, Stream requestStream, out IMessage responseMsg, out ITransportHeaders responseHeaders, out Stream responseStream)
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Binder = new GenericsVTSSerializationBinder();
                requestMsg = (IMessage)binaryFormatter.Deserialize(requestStream);
                requestMsg.Properties["__Uri"] = requestHeaders["__RequestUri"];

                next.ProcessMessage(sinkStack, requestMsg, null, null, out responseMsg, out responseHeaders, out responseStream);

                responseHeaders = new TransportHeaders();
                responseStream = new MemoryStream();
                binaryFormatter.Serialize(responseStream, responseMsg);
                responseStream.Position = 0;

                return ServerProcessing.Complete;
            }

            Stream IServerChannelSink.GetResponseStream(IServerResponseChannelSinkStack sinkStack, object state, IMessage msg, ITransportHeaders headers)
            {
                return next.GetResponseStream(sinkStack, state, msg, headers);
            }

            IServerChannelSink IServerChannelSink.NextChannelSink
            {
                get { return next; }
            }

            IDictionary IChannelSinkBase.Properties
            {
                get { return next.Properties; }
            }
        }

        class GenericsVTSSerializationBinder : SerializationBinder
        {
            string[] frameworkPublicKeyTokens = new string[] {
                "B7-7A-5C-56-19-34-E0-89",
                "B0-3F-5F-7F-11-D5-0A-3A",
                "31-BF-38-56-AD-36-4E-35",
                "89-84-5D-CD-80-80-CC-91"
            };

            bool IsFrameworkAssembly(Assembly assembly)
            {
                foreach (string frameworkToken in frameworkPublicKeyTokens)
                {
                    if (frameworkToken == BitConverter.ToString(assembly.GetName().GetPublicKeyToken()))
                    {
                        return true;
                    }
                }
                return false;
            }

            public override void BindToName(Type serializedType, out string assemblyName, out string typeName)
            {
                // To handle arrays
                if (serializedType.IsArray)
                {
                    string elementTypeName;
                    Type elementType = serializedType.GetElementType();
                    BindToName(elementType, out assemblyName, out elementTypeName);
                    StringBuilder typeNameBuilder = new StringBuilder(elementTypeName);
                    typeNameBuilder.Append("[");
                    int arrayRank = serializedType.GetArrayRank();
                    for (int i = 1; i < arrayRank; i++)
                    {
                        typeNameBuilder.Append(",");
                    }
                    if (arrayRank == 1 && serializedType == elementType.MakeArrayType(1))
                    {
                        typeNameBuilder.Append("*");
                    }
                    typeNameBuilder.Append("]");
                    typeName = typeNameBuilder.ToString();
                }
                // To handle generic types
                else if (serializedType.IsGenericType && !serializedType.IsGenericTypeDefinition)
                {
                    string definitionTypeName;
                    Type[] genericParameters = serializedType.GetGenericArguments();
                    BindToName(serializedType.GetGenericTypeDefinition(), out assemblyName, out definitionTypeName);
                    StringBuilder typeNameBuilder = new StringBuilder(definitionTypeName);
                    typeNameBuilder.Append("[");
                    for (int i = 0; i < genericParameters.Length; i++)
                    {
                        if (i > 0)
                        {
                            typeNameBuilder.Append(",");
                        }
                        string parameterTypeName, parameterAssemblyName;
                        BindToName(genericParameters[i], out parameterAssemblyName, out parameterTypeName);
                        typeNameBuilder.AppendFormat("[{0}, {1}]", parameterTypeName, parameterAssemblyName);
                    }
                    typeNameBuilder.Append("]");
                    typeName = typeNameBuilder.ToString();
                }
                // To handle the rest of types
                else
                {
                    assemblyName = serializedType.Assembly.FullName;
                    if (IsFrameworkAssembly(serializedType.Assembly))
                    {
                        assemblyName = assemblyName.Replace("Version=4.0.0.0", "Version=2.0.0.0");
                    }
                    typeName = serializedType.FullName;
                }
            }

            public override Type BindToType(string assemblyName, string typeName)
            {
                return null;
            }
        }
    }
}
