
//-----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All Rights Reserved.
//-----------------------------------------------------------------

using System;
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

    // Main class
    public sealed class Test
    {
        private Test() { }

        public static void Main()
        {
            Console.WriteLine("Sample using the NetDataContractSerializer:\n");
            try
            {
                WriteObject("NetDataContractSerializer.xml");
                ReadObject("NetDataContractSerializer.xml");
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
            NetDataContractSerializer ser = new NetDataContractSerializer();

            ser.WriteObject(writer, number);
            writer.Close();
        }

        public static void ReadObject(string fileName)
        {
            Console.WriteLine("Deserializing an instance of the object.");

            // Creating the serializer
            FileStream fs = new FileStream(fileName, FileMode.Open);
            XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas());
            NetDataContractSerializer ser = new NetDataContractSerializer();

            // Deserialize the data and read it from the instance
            ComplexNumber deserializedNumber = (ComplexNumber)ser.ReadObject(reader, true);
            fs.Close();
            Console.WriteLine(String.Format("Real: {0}, Imaginary: {1}", deserializedNumber.Real, deserializedNumber.Imaginary));
        }
    }
}
