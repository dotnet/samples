
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.IO;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Xml;

namespace Microsoft.Samples.DataContractSerializer
{

    class Client
    {
        static void Main()
        {
            Record record1 = new Record(1, 2, "+", 3);
            Console.WriteLine("Original record: {0}", record1.ToString());

            MemoryStream stream1 = new MemoryStream();

            //Serialize the Record object to a memory stream using DataContractSerializer.
            System.Runtime.Serialization.DataContractSerializer serializer = new System.Runtime.Serialization.DataContractSerializer(typeof(Record));
            serializer.WriteObject(stream1, record1);

            stream1.Position = 0;

            //Deserialize the Record object back into a new record object
            Record record2 = (Record)serializer.ReadObject(stream1);

            Console.WriteLine("Deserialized record: {0}", record2.ToString());

            MemoryStream stream2 = new MemoryStream();

            XmlDictionaryWriter binaryDictionaryWriter = XmlDictionaryWriter.CreateBinaryWriter(stream2);
            serializer.WriteObject(binaryDictionaryWriter, record1);
            binaryDictionaryWriter.Flush();

            //report the length of the streams
            Console.WriteLine("Text Stream is {0} bytes long", stream1.Length);
            Console.WriteLine("Binary Stream is {0} bytes long", stream2.Length);

            Console.WriteLine();
            Console.WriteLine("Press <ENTER> to terminate client.");
            Console.ReadLine();
        }
    }

    [DataContract(Namespace = "http://Microsoft.Samples.DataContractSerializer")]
    internal class Record
    {
        private double n1;
        private double n2;
        private string operation;
        private double result;

        internal Record(double n1, double n2, string operation, double result)
        {
            this.n1 = n1;
            this.n2 = n2;
            this.operation = operation;
            this.result = result;
        }

        [DataMember]
        internal double OperandNumberOne
        {
            get { return n1; }
            set { n1 = value; }
        }

        [DataMember]
        internal double OperandNumberTwo
        {
            get { return n2; }
            set { n2 = value; }
        }

        [DataMember]
        internal string Operation
        {
            get { return operation; }
            set { operation = value; }
        }

        [DataMember]
        internal double Result
        {
            get { return result; }
            set { result = value; }
        }

        public override string ToString()
        {
            return string.Format("Record: {0} {1} {2} = {3}", n1, operation, n2, result);
        }

    }
}
