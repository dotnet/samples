//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved. 
//----------------------------------------------------------------

using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace Microsoft.Samples.JsonSerialization
{

    class Sample
    {
        static void Main()
        {
            Person p = new Person();
            p.name = "John";
            p.age = 42;

            MemoryStream stream1 = new MemoryStream();

            //Serialize the Person object to a memory stream using DataContractJsonSerializer.
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Person));
            ser.WriteObject(stream1, p);

            //Show the JSON output.
            stream1.Position = 0;
            StreamReader sr = new StreamReader(stream1);
            Console.Write("JSON form of Person object: ");
            Console.WriteLine(sr.ReadToEnd());

            //Deserialize the JSON back into a new Person object.
            stream1.Position = 0;
            Person p2 = (Person)ser.ReadObject(stream1);

            //Show the results.
            Console.Write("Deserialized back, got name=");
            Console.Write(p2.name);
            Console.Write(", age=");
            Console.WriteLine(p2.age);

            Console.WriteLine("Press <ENTER> to terminate the program.");
            Console.ReadLine();
        }
    }

    [DataContract]
    class Person
    {
        [DataMember]
        internal string name;

        [DataMember]
        internal int age;
    }
    
}
