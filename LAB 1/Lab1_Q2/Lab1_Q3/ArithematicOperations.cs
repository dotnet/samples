using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1_Q2
{

    class ArithematicOperations
    {
        public void sum(int num1, double num2)
        {
            int sum = num1 + (int)num2;
            Console.WriteLine($"{sum}");
        }

        public void subtract(int num1, double num2)
        {
            int minus = num1 - (int)num2;
            Console.WriteLine($"{minus}");
        }

        public void multiply(int num1, double num2)
        {
            double product = Convert.ToDouble(num1) * num2;
            Console.WriteLine($"{product}");
        }

        public void divide(int num1, double num2)
        {
            try
            {
                double divide = Convert.ToDouble(num1) / Convert.ToDouble(num2);
                Console.WriteLine($"{divide}");
            }
            catch (DivideByZeroException)
            {
                Console.WriteLine($"Division of {num1} by 0 not possible");
            }
        }

        public void modulus(int num1, double num2)
        {
            try
            {
                int mod = num1 % (int)num2;
                Console.WriteLine($"{mod}");
            }
            catch (DivideByZeroException)
            {
                Console.WriteLine($"Division of {num1} by 0 not possible");
            }
        }
    }
}