using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace JSON_Serialization
{
    //Command : "Install-Package Newtonsoft.josn" to install JSON
    //[Serializable]
    class Person 
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime DOB { get; set; }
    } 
    class Program
    {
        //JSON Serialization using JsonConvert
        static void JsonSerializationJsonConvert() 
        {
            Person p = new Person()
            {
                Id = 101,
                Name = "Insiya",
                DOB = DateTime.Parse("10-Apr-1997")
            };
            string PersonData = JsonConvert.SerializeObject(p);
            Console.WriteLine($"JSON Serialized Person : {PersonData}");
        }

        static void JsonDeSerializationJsonConvert()
        {
            Person p = JsonConvert.DeserializeObject<Person>("{'Id':101,'Name':'Insiya','DOB':'1997 - 04 - 10T00: 00:00'}");
            Console.WriteLine($"JSON De-Serialized Person - ID : {p.Id}, Name : {p.Name}, DOB : {p.DOB}");
        }

        //JSON Serialization using JsonSerializer
        static void JsonSerializationUsingJsonSerializer() 
        {
            Person p = new Person() 
            { 
                Id = 101, 
                Name = "Robert", 
                DOB = DateTime.Parse("10-Apr-1997") 
            };
            //StreamWriter sw = new StreamWriter("JSON.txt");
            //JsonWriter writer = new JsonTextWriter(sw);
            //JsonSerializer serializer = new JsonSerializer();
            //serializer.Serialize(writer, p);
            using (StreamWriter sw = new StreamWriter("JSON1.txt"))
            {
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(writer, p);
                }
            }

        }

        static void JsonDeSerializationUsingJsonSerializer()
        {
           
            StreamReader sr = new StreamReader("JSON1.txt");
            JsonReader reader = new JsonTextReader(sr);
            JsonSerializer serializer = new JsonSerializer();
            Person p = serializer.Deserialize<Person>(reader);
            Console.WriteLine($"ID : {p.Id}, Name : {p.Name}, DOB : {p.DOB}");
        }

        //JSON Serialization using DataContractJsonSerializer
        static void JsonS_DataContractJsonSerializer()
        {
            Person p = new Person() { Id = 101, Name = "Robert", DOB = DateTime.Parse("10-Apr-1997") };
            Stream s = new FileStream("JSON2.txt", FileMode.Create, FileAccess.Write);
            DataContractJsonSerializer js = new DataContractJsonSerializer(typeof(Person));
            js.WriteObject(s, p);
            s.Close();

        }

        static void JsonDS_DataContractJsonSerializer()
        {
            Stream s = new FileStream("JSON2.txt", FileMode.Open, FileAccess.Read);
            DataContractJsonSerializer js = new DataContractJsonSerializer(typeof(Person));
            Person p = js.ReadObject(s) as Person;
            s.Close();
            Console.WriteLine($"ID : {p.Id}, Name : {p.Name}, DOB : {p.DOB}");
        }

        static void Main(string[] args)
        {
            //JSON Serialization using JsonConvert
            //JsonSerializationJsonConvert();
            //Console.WriteLine("JSON Serialization Completed using JSONConvert.");
            //JsonDeSerializationJsonConvert();

            //JSON Serialization using JsonSerializer
            //JsonSerializationUsingJsonSerializer();
            //Console.WriteLine("JSON Serialization Completed using JsonSerializer.");
            //JsonDeSerializationUsingJsonSerializer();

            //JSON Serialization using DataContractJsonSerializer
            //JsonS_DataContractJsonSerializer();
            //Console.WriteLine("JSON Serialization Completed using DataContractJsonSerializer.");
            //JsonDS_DataContractJsonSerializer();
            //Console.ReadKey();

            //hw - try josnSerializer and josnDeSerializer method from pdf and run code
        }
    }
}
