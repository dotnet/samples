using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CS_Demo_Serialization.DemoGraphics;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace CS_Demo_Serialization
{
    class Program
    {

        static void Binary_Serialize()
        {
            Person p = new Person()
            {
                Id = 101,
                FirstName = "James",
                LastName = "John",
                MiddleName = " Herry",
                DateOfBirth = DateTime.Parse("20-Apr-19978")
            };
            FileStream fs = new FileStream("PersonData.bin", FileMode.Create, FileAccess.Write);
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(fs, p);
            fs.Close();
            fs.Dispose();
        }

            static void Main (string[] args)
            {
                Binary_Serialize();

            Console.WriteLine(" Binary Serialization CCompleted Sucessfully");
                Console.ReadKey();
            }

            
        
    }
}
