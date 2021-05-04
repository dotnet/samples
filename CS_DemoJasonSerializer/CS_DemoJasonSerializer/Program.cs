using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CS_DemoJsonSerializer
{
    class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime DateOfBirth { get; set; }

    }
    class Program
    {
        static void JsonSerilalize()
        {
            Person p = new Person()
            {
                Id = 101,
                Name = "Ruchita",
                DateOfBirth = DateTime.Parse("01-Apr-1997"),
            };
            string personData = JsonConvert.SerializeObject(p);
            Console.WriteLine($"Json Serilized Person : {personData}");  
        }

        static void JsonDeserialize()
        {
            Person p = JsonConvert.DeserializeObject<Person>("{'Id':101,'Name':'Ruchita','DateOfBirth':'1997 - 04 - 01T00: 00:00'}");
            Console.WriteLine($"Id : {p.Id}, Name : {p.Name}, DateOfBirth : {p.DateOfBirth}");
        }
        static void Main(string[] args)
        {
            JsonSerilalize();
            Console.WriteLine("\n\nSerialization Completed.......");
            JsonDeserialize();
            Console.ReadKey();

        }
    }
}
