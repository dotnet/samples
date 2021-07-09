//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

namespace Microsoft.Samples.Discovery
{

    static class Utility
    {
        public static void IfNullThrowNullArgumentException(object parameter, string parameterName)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException(parameterName);
            }
        }

        public static bool CanCanonicalizeAndFragment(XmlDictionaryWriter writer)
        {
            if (!writer.CanCanonicalize)
            {
                return false;
            }

            IFragmentCapableXmlDictionaryWriter fragmentingWriter = writer as IFragmentCapableXmlDictionaryWriter;
            return fragmentingWriter != null && fragmentingWriter.CanFragment;
        }

        public static XmlDictionaryWriter CreateWriter(Stream stream)
        {
            return XmlDictionaryWriter.CreateBinaryWriter(stream, new XmlDictionary(), null, true);
        }

        public static XmlDictionaryReader CreateReader(byte[] buffer)
        {
            return XmlDictionaryReader.CreateBinaryReader(buffer, XmlDictionaryReaderQuotas.Max);
        }

        public static void WriteNodeToWriter(byte[] buffer, XmlDictionaryWriter writer)
        {
            using (XmlDictionaryReader reader = Utility.CreateReader(buffer))
            {
                writer.WriteNode((XmlReader)reader, false);
            }
        }

        public static void AppendLocalName(StringBuilder sb, string prefix, string localName, bool endElement, bool close)
        {
            // sb should not be null
            sb.Append(endElement ? "</" : "<");
            sb.Append(prefix);
            sb.Append(":");
            sb.Append(localName);
            sb.Append(close ? ">" : "");
        }

        public static void AppendAttribute(StringBuilder sb, string attributeName, string attributeValue, bool close)
        {
            // sb should not be null
            sb.Append(" ");
            sb.Append(attributeName);
            sb.Append("=\"");
            sb.Append(attributeValue);
            sb.Append("\"");
            sb.Append(close ? ">" : "");
        }

        public static void AppendXmlsnAttribute(StringBuilder sb, string attributeName, string attributeValue, bool close)
        {
            // sb should not be null
            sb.Append(" xmlns:");
            sb.Append(attributeName);
            sb.Append("=\"");
            sb.Append(attributeValue);
            sb.Append("\"");
            sb.Append(close ? ">" : "");
        }
        
        public static string ToHexString(string base64String)
        {
            return ToHexString(Convert.FromBase64String(base64String));
        }

        static string ToHexString(byte[] bytes)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                sb.Append(bytes[i].ToString("X2"));
            }
            return sb.ToString();
        }

        public static string ToBase64String(string hexString)
        {
            byte[] bytes = new byte[hexString.Length / 2];
            for (int i = 0; i < hexString.Length; i+=2)
            {
                string temp = new string(new char[]{hexString[i], hexString[i + 1]});
                bytes[i/2] = byte.Parse(temp, NumberStyles.HexNumber);
            }
            
            return Convert.ToBase64String(bytes);
        }
    }
}
