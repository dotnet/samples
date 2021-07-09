
//-----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All Rights Reserved.
//-----------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;

namespace Microsoft.Samples.NetDCSasDCSwithDCR
{

    // Types to serialize
    [DataContract]
    public class ComplexNumber
    {
        [DataMember]
        private double real;

        [DataMember]
        private double imaginary;

        public ComplexNumber(double r1, double i1)
        {
            this.Real = r1;
            this.Imaginary = i1;
        }

        public double Real
        {
            get { return real; }
            set { real = value; }
        }

        public double Imaginary
        {
            get { return imaginary; }
            set { imaginary = value; }
        }
    }

    [DataContract]
    public class ComplexNumberWithMagnitude : ComplexNumber
    {
        public ComplexNumberWithMagnitude(double real, double imaginary) : base(real, imaginary) { }

        [DataMember]
        public double Magnitude
        {
            get { return Math.Sqrt(Imaginary * Imaginary + Real * Real); }
            set { }
        }
    }

    // DataContractResolver to be used by the DataContractSerializer
    class MyDataContractResolver : DataContractResolver
    {
        private XmlDictionary dictionary = new XmlDictionary();

        public MyDataContractResolver()
        {
        }

        // Used at deserialization
        // Allows users to map xsi:type name to any Type 
        public override Type ResolveName(string typeName, string typeNamespace, Type declaredType, DataContractResolver knownTypeResolver)
        {
            Type type = knownTypeResolver.ResolveName(typeName, typeNamespace, declaredType, null);
            if (type == null)
            {
                type = Type.GetType(typeName + ", " + typeNamespace);
            }
            return type;
        }

        // Used at serialization
        // Maps any Type to a new xsi:type representation
        public override bool TryResolveType(Type type, Type declaredType, DataContractResolver knownTypeResolver, out XmlDictionaryString typeName, out XmlDictionaryString typeNamespace)
        {
            if (!knownTypeResolver.TryResolveType(type, declaredType, null, out typeName, out typeNamespace))
            {
                XmlDictionary dictionary = new XmlDictionary();
                typeName = dictionary.Add(type.FullName);
                typeNamespace = dictionary.Add(type.Assembly.FullName);
            }
            return true;
        }
    }

    // Main class
    public sealed class Test
    {
        private Test() { }

        public static void Main()
        {
            Console.WriteLine("Sample using the DataContractSerializer with the DataContractResolver:\n");
            try
            {
                WriteObject("DataContractSerializerWithDCR.xml");
                ReadObject("DataContractSerializerWithDCR.xml");
            }
            catch (SerializationException serExc)
            {
                Console.WriteLine("Serialization Failed");
                Console.WriteLine(serExc.Message);
            }
            catch (Exception exc)
            {
                Console.WriteLine("The serialization operation failed: {0} StackTrace: {1}", exc.Message, exc.StackTrace);
            }
            finally
            {
                Console.WriteLine("Press <Enter> to exit...");
                Console.ReadLine();
            }
        }

        public static void WriteObject(string fileName)
        {
            Console.WriteLine("Creating a Person object and serializing it.");

            // Creating the serializer
            ComplexNumber number = new ComplexNumberWithMagnitude(3, 4);
            FileStream fs = new FileStream(fileName, FileMode.Create);
            XmlDictionaryWriter writer = XmlDictionaryWriter.CreateTextWriter(fs);
            DataContractSerializer ser = new DataContractSerializer(typeof(ComplexNumber), null, int.MaxValue, false, true, null, new MyDataContractResolver());

            // Serialize the data and write it to a file
            ser.WriteObject(writer, number);
            writer.Close();
        }

        public static void ReadObject(string fileName)
        {
            Console.WriteLine("Deserializing an instance of the object.");

            // Creating the serializer
            FileStream fs = new FileStream(fileName, FileMode.Open);
            XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas());
            DataContractSerializer ser = new DataContractSerializer(typeof(ComplexNumber), null, int.MaxValue, false, true, null, new MyDataContractResolver());

            // Deserialize the data and read it from the instance
            ComplexNumber deserializedNumber = (ComplexNumber)ser.ReadObject(reader, true);
            fs.Close();
            Console.WriteLine(String.Format("Real: {0}, Imaginary: {1}", deserializedNumber.Real, deserializedNumber.Imaginary));
        }
    }
}
