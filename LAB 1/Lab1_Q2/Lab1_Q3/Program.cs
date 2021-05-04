using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1_Q2
{
    class Program
    {
        static void Main(string[] args)
        {
            ArithematicOperations obj = new ArithematicOperations();
            int num1,choice;
            double num2;

            Console.WriteLine("Enter two numbers:");
            num1 = Int32.Parse(Console.ReadLine());
            num2 = Convert.ToDouble(Console.ReadLine());

            Console.WriteLine("Enter the operation you wish to perform on these two numbers");
            Console.WriteLine("Press 1. For Addition 2. For Substraction 3. For Multiplication 4. Division 5. Modulus ");
            choice = Convert.ToInt32(Console.ReadLine());

            switch (choice)
            {
                case 1:
                    obj.sum(num1, num2);
                    break;

                case 2:
                    obj.subtract(num1, num2);
                    break;
                case 3:
                    obj.multiply(num1, num2);
                    break;

                case 4:
                    obj.divide(num1, num2);
                    break;
                case 5:
                    obj.modulus(num1, num2);
                    break;
                default:
                    Console.WriteLine("Please Enter the valid input");
                    break;
            }


            Console.ReadKey();
        }
    }
}
