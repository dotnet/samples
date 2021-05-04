using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab2_Que03
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter size of array");
            int n = Convert.ToInt32(Console.ReadLine());
            string[] CityName = new string[n];

            foreach (string i in CityName)
            {
                Console.WriteLine("Enter City Name");
                String s= Console.ReadLine();
                Console.WriteLine("You have Entered City:" + s);
            }

            
            Console.ReadKey();
        }
    }
}
