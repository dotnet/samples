
//-----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All Rights Reserved.
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml;

namespace Microsoft.Samples.KAA.Common
{

    public class MyDataContractResolver : DataContractResolver
    {
        XmlDictionary dictionary = new XmlDictionary();
        Assembly assembly;

        public MyDataContractResolver(string assemblyName)
        {
            // It will store the list of (known) types extracted from the assembly
            this.KnownTypes = new List<Type>();

            // Process each of the types in the assembly
            assembly = Assembly.Load(new AssemblyName(assemblyName));
            foreach (Type type in assembly.GetTypes())
            {
                bool knownTypeFound = false;
                System.Attribute[] attrs = System.Attribute.GetCustomAttributes(type);
                if (attrs.Length != 0)
                {
                    foreach (System.Attribute attr in attrs)
                    {
                        if (attr is KnownTypeAttribute)
                        {
                            Type t = ((KnownTypeAttribute)attr).Type;
                            if (this.KnownTypes.IndexOf(t) < 0)
                            {
                                // Adding the type to the known types list
                                this.KnownTypes.Add(t);
                            }
                            knownTypeFound = true;
                        }
                    }
                }
                if (!knownTypeFound)
                {
                    // Add the name and namespace of the type into the dictionary
                    string name = type.Name;
                    string namesp = type.Namespace;
                    XmlDictionaryString result;
                    if (!dictionary.TryLookup(name, out result))
                    {
                        dictionary.Add(name);
                    }
                    if (!dictionary.TryLookup(namesp, out result))
                    {
                        dictionary.Add(namesp);
                    }
                }
            }
        }

        public IList<Type> KnownTypes
        {
            get; set;
        }

        // Used at deserialization
        // Allows users to map xsi:type name to any Type 
        public override Type ResolveName(string typeName, string typeNamespace, Type declaredType, DataContractResolver knownTypeResolver)
        {
            XmlDictionaryString tName;
            XmlDictionaryString tNamespace;

            // If the type is found in the dictionary then extract it from the assembly. Use the default KnownTypeResolver otherwise.
            if (dictionary.TryLookup(typeName, out tName) && dictionary.TryLookup(typeNamespace, out tNamespace))
            {
                return this.assembly.GetType(tNamespace.Value + "." + tName.Value);
            }
            else
            {
                return knownTypeResolver.ResolveName(typeName, typeNamespace, declaredType, null);
            }
        }

        // Used at serialization
        // Maps any Type to a new xsi:type representation
        public override bool TryResolveType(Type type, Type declaredType, DataContractResolver knownTypeResolver, out XmlDictionaryString typeName, out XmlDictionaryString typeNamespace)
        {
            // Resolve the type and return the type name and namespace
            if (!knownTypeResolver.TryResolveType(type, declaredType, null, out typeName, out typeNamespace))
            {
                typeName = new XmlDictionaryString(XmlDictionary.Empty, type.Name, 0);
                typeNamespace = new XmlDictionaryString(XmlDictionary.Empty, type.Namespace, 0);
            }
            return true;
        }
    }
}
