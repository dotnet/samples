//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;
using System.Xml;

namespace Microsoft.Samples.WeaklyTypedJson
{
    public class JsonObject : JsonCollection
    {
        // In an object (dictionary), the [string] indexer is appropriate, so implement it.
        // The [int] indexer will throw as per JsonBaseType.
        public override JsonBaseType this[string key]
        {
            get
            {
                return ((Dictionary<string, JsonBaseType>)InternalValue)[key];
            }
            set
            {
                ((Dictionary<string, JsonBaseType>)InternalValue)[key] = (JsonBaseType)value;
            }
        }

        // Implement the correct add method for an object (dictionary)
        internal void Add(string key, JsonBaseType value)
        {
            ((Dictionary<string, JsonBaseType>)InternalValue).Add(key, value);
        }

        public JsonObject(XmlReader reader)
            : base(reader, new Dictionary<string, JsonBaseType>())
        { }
    }


    public class JsonArray : JsonCollection
    {
        // In an array, the [int] indexer is appropriate, so implement it.
        // The [string] indexer will throw as per JsonBaseType.
        public override JsonBaseType this[int index]
        {
            get
            {
                return ((List<JsonBaseType>)InternalValue)[index];
            }
            set
            {
                ((List<JsonBaseType>)InternalValue)[index] = (JsonBaseType)value;
            }
        }
        
        // Implement the correct add method for an array
        internal void Add(JsonBaseType value)
        {
            ((List<JsonBaseType>)InternalValue).Add(value);
        }

        internal JsonArray(XmlReader reader)
            : base(reader, new List < JsonBaseType >()) 
        { }
    }

    // JSON contains two collection types: an object and an array. This class abstracts some 
    // common functionality used by both.
    public class JsonCollection : JsonBaseType
    {
        private const string exceptionString = "You cannot use this Add method on this JSON type";

        // We will be working with one if this class' children - object or array - so we need to 
        // use the add method that's appropriate for that child
        private void AddSelector(string key, JsonBaseType value)
        {
            JsonArray thisArray = this as JsonArray;

            if (thisArray != null) 
                thisArray.Add(value); 
            else 
                ((JsonObject)this).Add(key, value);

        }

        private void AddValueType<T>(XmlReader reader)
        {
            // Use DataContractJsonSerialzier to deserialize JXML into a CLR type
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T), reader.Name);
            JsonBaseType jsonElement = new JsonBaseType((T)serializer.ReadObject(reader.ReadSubtree()));
            AddSelector(reader.Name, jsonElement);
        }

        internal JsonCollection(XmlReader reader, object value): base (value)
        {
            string nodeName, nodeType;
            string rootName = reader.Name;
 
            while (reader.Read())
            {
                if (reader.IsStartElement())
                {

                    nodeName = reader.Name;
                    reader.MoveToAttribute("type");
                    nodeType = reader.Value;
                    reader.MoveToElement();

                    switch (nodeType)
                    {
                        // The object JSON type needs to be handled by us recursively, since 
                        // DataContractJsonSerializer cannot deserialize it without a DataContract, 
                        // which we don't have
                        case "object":
                            AddSelector(nodeName, new JsonObject(reader));
                            break;

                        // Normally DataContractJson serializer can deserialize arrays, but we could have
                        // an array with an object as one if its items. In that case the serializer
                        // wouldn't work, so we need to handle the entire array case manually
                        case "array":
                            AddSelector(nodeName, new JsonArray(reader));
                            break;

                        // The number, string, and bool JSON types can be deferred to 
                        // DataContractJsonSerializer
                        case "number":
                            AddValueType<double>(reader);
                            break;

                        case "string":
                            AddValueType<string>(reader);
                            break;

                        case "boolean":
                            AddValueType<bool>(reader);
                            break;

                        // For null values, we just use the CLR null type
                        case "null":
                            AddSelector(nodeName, null);
                            break;
                    }
                }
                else if (reader.Name == rootName) break;
            }

        }
    }

    public class JsonBaseType
    {
        private const string exceptionString = "You cannot use indexers on this JSON type";
        private object internalValue;

        protected object InternalValue
        {
            get
            {
                return internalValue;
            }
            set
            {
                internalValue = value;
            }
        }

        // The generic JSON type needs to support indexers of both [string] and [int] (for the JSON
        // object and array) and also cast easily into int, bool, and string (for the JSON number,
        // boolean, and string types)

        // Indexers
        public virtual JsonBaseType this[string key]
        {
            get
            {
                throw new NotSupportedException(exceptionString);
            }
            set
            {
                throw new NotSupportedException(exceptionString);
            }
        }

        public virtual JsonBaseType this[int index]
        {
            get
            {
                throw new NotSupportedException(exceptionString);
            }
            set
            {
                throw new NotSupportedException(exceptionString);
            }
        }

        // Type cast operator overloads
        public static implicit operator int(JsonBaseType type)
        {
            // Have to do this to unbox correctly
            return (int)(double)type.InternalValue;
        }

        public static implicit operator double(JsonBaseType type)
        {
            // Have to do this to unbox correctly
            return (double)type.InternalValue;
        }

        public static implicit operator bool(JsonBaseType type)
        {
            return (bool)type.InternalValue;
        }

        public static implicit operator string(JsonBaseType type)
        {
            return (string)type.InternalValue;
        }

        // Constructor
        internal JsonBaseType(object value)
        {
            InternalValue = value;
        }
    }
}
