using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CS_DemoSerialization.Demographics;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Soap;



namespace CS_DemoSerialization
{
    class Program
    {
        static void Binary_Serialize()
        {
            Person p = new Person()
            {
                Id = 101,
            FirstName = "Rohi",
            MiddleName = "Raj",
            LastName = "Rao",
            DateOfBirth = DateTime.Parse("20-Apr-1976")
            };

            FileStream fs = new FileStream("PersonData.bin", FileMode.Create, FileAccess.Write);
            BinaryFormatter formatter = new BinaryFormatter() ;
            formatter.Serialize(fs, p);
            fs.Close();
            fs.Dispose();
        }

        static void Binary_Deserialize()
        {
            FileStream fs = new FileStream("PersonData.bin", FileMode.Open, FileAccess.Read);
            BinaryFormatter formatter = new BinaryFormatter();
            Person p = formatter.Deserialize(fs) as Person;
            if (p != null)
            {
                Console.WriteLine($"Id : {p.Id}, Name : {p.FirstName } {p.MiddleName } {p.LastName }, Date Of Birth : {p.DateOfBirth }, Age : {p.Age}");
            }
        }


        static void Xml_Serialize()
        {
            Person p = new Person()
            {
                Id = 101,
                FirstName = "Rohi",
                MiddleName = "Raj",
                LastName = "Rao",
                DateOfBirth = DateTime.Parse("20-Apr-1976")
            };

            FileStream fs = new FileStream("PersonData.xml", FileMode.Create, FileAccess.Write);
            XmlSerializer xs = new XmlSerializer(typeof(Person));
            xs.Serialize(fs, p);
            fs.Close();
            fs.Dispose();
        }

        static void Xml_Deserialize()
        {
            FileStream fs = new FileStream("PersonData.xml", FileMode.Open, FileAccess.Read);
            XmlSerializer xs = new XmlSerializer(typeof(Person));
            Person p = xs.Deserialize(fs) as Person;
            if (p != null)
            {
                Console.WriteLine($"Id : {p.Id}, Name : {p.FirstName } {p.MiddleName } {p.LastName }, Date Of Birth : {p.DateOfBirth }, Age : {p.Age}");
            }
        }


        static void Soap_Serialize()
        {
            Person p = new Person()
            {
                Id = 101,
                FirstName = "Rohi",
                MiddleName = "Raj",
                LastName = "Rao",
                DateOfBirth = DateTime.Parse("20-Apr-1976")
            };

            FileStream fs = new FileStream("PersonData-Soap.xml", FileMode.Create, FileAccess.Write);
            SoapFormatter formatter = new SoapFormatter();
            formatter.Serialize(fs, p);
            fs.Close();
            fs.Dispose();
        }

        static void Soap_Deserialize()
        {
            FileStream fs = new FileStream("PersonData-Soap.xml", FileMode.Open, FileAccess.Read);
            SoapFormatter formatter = new SoapFormatter();
            Person p = formatter.Deserialize(fs) as Person;
            if (p != null)
            {
                Console.WriteLine($"Id : {p.Id}, Name : {p.FirstName } {p.MiddleName } {p.LastName }, Date Of Birth : {p.DateOfBirth }, Age : {p.Age}");
            }
        }

        static void Main(string[] args)
        {
            //Binary_Serialize();
            //Console.WriteLine("Binary Serialization is successfull!");
            //Binary_Deserialize();

            //Xml_Serialize();
            //Console.WriteLine("Xml Serialization is successfull!");
            //Xml_Deserialize();

            Soap_Serialize();
            Console.WriteLine("Soap Serialization is successfull!");
            Soap_Deserialize();

            Console.ReadKey();
        }
    }
}
