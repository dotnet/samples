using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CS_DemoJSONSerialization
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
                Name = "shraddha",
                DateOfBirth = DateTime.Parse("09-Apr-1997")
            };
            string personData = JsonConvert.SerializeObject(p);
            Console.WriteLine($"JSON Serialized Person : {personData}");
        }
        static void JsonDeserialize()
        {
            Person p = JsonConvert.DeserializeObject<Person>("{'Id':101,'Name':'Shraddha','DateOfBirth':'1997-04-09T00:00:00'}");
            Console.WriteLine($"Id : {p.Id}, Name : {p.Name}, DateOfBirth : {p.DateOfBirth}");
            
        }
        static void Main(string[] args)
        {
            JsonSerialize();
            Console.WriteLine("JSON DONE");
            JsonDeserialize();
            Console.ReadKey();
        }
    }  
}
