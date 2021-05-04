using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace DemoSerialization
{
    [Serializable]
    public class Person:ISerializable
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public float Age { get; set; }

        public Person()
        {

        }

        public Person(SerializationInfo info, StreamingContext context)
        {
            this.Id =(int) info.GetValue("Identity", typeof(int));
            this.Name =(string) info.GetValue("FullName", typeof(string));
            this.Age =(float) info.GetValue("Age", typeof(float));
        }

        //public Person(int Id, string Name, float Age)
        //{
        //    this.Id = Id;
        //    this.Name = Name;
        //    this.Age = Age;
        //}

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Identity", Id);
            info.AddValue("FullName", Name);
            info.AddValue("Age", Age);
        }

        public override string ToString()
        {
            return $"Id : {Id}, Name : {Name}, Age : {Age}";
        }
    }
    class Program
    {
        static void SerializeBinary()
        {
            Person p = new Person()
            {
                Id=101,
                Name="James",
                Age=45.5f
            };

            using (FileStream fs=new FileStream("PersonData.bin",FileMode.Create,FileAccess.Write))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(fs, p);
                fs.Close();
            }
        }

        static Person DeserializeBinary()
        {
            Person p ;

            using (FileStream fs = new FileStream("PersonData.bin", FileMode.Open, FileAccess.Read))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                p = formatter.Deserialize(fs) as Person;
                fs.Close();
            }
            return p;
        }

        static void SerializeSOAP()
        {
            Person p = new Person()
            {
                Id = 101,
                Name = "James",
                Age = 45.5f
            };

            using (FileStream fs = new FileStream("PersonDataSoap.xml", FileMode.Create, FileAccess.Write))
            {
                SoapFormatter formatter = new SoapFormatter();
                formatter.Serialize(fs, p);
                fs.Close();
            }
        }

        static Person DeserializeSOAP()
        {
            Person p;

            using (FileStream fs = new FileStream("PersonDataSoap.xml", FileMode.Open, FileAccess.Read))
            {
                SoapFormatter formatter = new SoapFormatter();
                p = formatter.Deserialize(fs) as Person;
                fs.Close();
            }
            return p;
        }

        static void SerializeXML()
        {
            Person p = new Person()
            {
                Id = 101,
                Name = "James",
                Age = 45.5f
            };

            using (FileStream fs = new FileStream("PersonDataXML.xml", FileMode.Create, FileAccess.Write))
            {
                XmlSerializer xs = new XmlSerializer(p.GetType());
                xs.Serialize(fs, p);
                fs.Close();
            }
        }

        static Person DeserializeXML()
        {
            Person p;

            using (FileStream fs = new FileStream("PersonDataXML.xml", FileMode.Open, FileAccess.Read))
            {
                XmlSerializer xs = new XmlSerializer(typeof(Person));
                p = xs.Deserialize(fs) as Person;
                fs.Close();
            }
            return p;
        }

        static void Main(string[] args)
        {
            SerializeBinary();
            var p = DeserializeBinary();
            Console.WriteLine($"From Binary : {p}");
            SerializeSOAP();
            p = DeserializeSOAP();
            Console.WriteLine($"From SOAP : {p}");
            SerializeXML();
            p = DeserializeXML();
            Console.WriteLine($"From XML : {p}");

            Console.ReadKey();
        }
    }
}
