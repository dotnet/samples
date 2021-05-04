using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CS_DemoJsonSerialization
{
    class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
    class Program
    {
        static void JsonSerialize()
        {
            Person p = new Person()
            {
                Id = 101,
                Name = "bob",
                DateOfBirth = DateTime.Parse("20-Apr-1976")
            };
            string personData = JsonConvert.SerializeObject(p);
            Console.WriteLine($"Json Serialized Person:{personData}");
        }

        static void JsonDeserialize()
        {
            
            Person p = JsonConvert.DeserializeObject<Person>("{'Id':101,'Name':'bob','DateOfBirth':'1976-04-20T00:00:00'}");
            Console.WriteLine($"Id:{p.Id},Name:{p.Name},DateOfBirth:{p.DateOfBirth}"); 
        }
        static void Main(string[] args)
        {
            JsonSerialize();
            Console.WriteLine("\n\nSerialization completed.....");
            JsonDeserialize();

            Console.ReadKey();
        }
    }
}
