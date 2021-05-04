using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace CS_JsonSerialization
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
            //Person p = new Person()
            //{
            //    Id = 101,
            //    Name = "Yasha",
            //    DateOfBirth = DateTime.Parse("14-Aug-1998")
            //};
            //string PersonData =JsonConvert.SerializeObject(p);
            //Console.WriteLine($"Json serialized Person:{PersonData}");
            Person p = new Person() { Id = 101, Name = "Yasha", DateOfBirth = DateTime.Parse("14-Aug-1998") };
            Stream s = new FileStream("p.txt", FileMode.Create, FileAccess.Write);
            DataContractJsonSerializer js = new DataContractJsonSerializer(typeof(Person));

            object p1 = js.WriteObject(s, p);

            s.Close();

        }
        static void JsonDeserialize()
        {
            Person p =  JsonConvert.DeserializeObject<Person>("{ 'Id':101,'Name':'Yasha','DateOfBirth':'1998-08-14T00:00:00'}");
            Console.WriteLine($"Id : {p.Id}, Name : {p.Name}, Date Of Birth : {p.DateOfBirth}");
        }

    static void Main(string[] args)
        {
            JsonSerialize();
            Console.WriteLine("\n\nSerialization completed.....,.");
            JsonDeserialize();
            Console.ReadKey();
        }
    }
}
