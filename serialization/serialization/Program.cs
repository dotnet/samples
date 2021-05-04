using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace serialization
{
    class Program
    {
        static void Binary_Serialize()
        {
            person p = new person()
            {
                Id = 101,
                FirstName = "James",
                MiddleName = "John",
                LastName = "Henry",
                DateOfBirth = DateTime.Parse("20-april-1976")
            };
        }
        FileStream fs = new FileStream("PeronData.bin", FileMode.Create, FileAccess);
        BinaryFormatter formatter = new BinaryFormatter();
            
    static void Binary_Deserialize()
    {
        FileStream fs = new FileStream("PersonData", FileMode.Open, FileAccess.Read);
        Binary Formatter = new BinaryFormatter();
        if(Parallel!=null)
        {
            Console.WriteLine($"Id:{p.iD},Name:{p.FirstName}{p.MiddleName}{p.LastName},Date Of Birth:{p.DateOfBirth},Age:{p.Age}");
        }
    }

        static void Main(string[] args)
        {
        Binary_Serialize();
        Console.WriteLine("Binary serializer completed successfully");
        Binary_Deserializer();

        Console.ReadKey();
        }
    }
}
