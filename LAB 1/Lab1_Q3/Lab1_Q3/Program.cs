using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1_Q3
{
    class Program
    {
        static void Main(string[] args)
        {
            int num;
            Console.WriteLine("Enter any one number from 1 to 5");
            num = Int32.Parse(Console.ReadLine());

            switch (num)
            {
                case 1: 
                    Console.WriteLine("You Have Entered 1 as input");
                    break;

                case 2:
                    Console.WriteLine("You Have Entered 2 as input");
                    break;

                case 3:
                    Console.WriteLine("You Have Entered 3 as input");
                    break;

                case 4:
                    Console.WriteLine("You Have Entered 4 as input");
                    break;

                case 5:
                    Console.WriteLine("You Have Entered 5 as input");
                    break;

                default:
                    Console.WriteLine("Error : Incorrect value.");
                    Console.WriteLine("Please enter values within 1 to 5");
                    break;
            }

            Console.ReadKey();
        }
    }
}
