using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS_DemoLocalFunctions
{
    class Calc
    {
        public float DoCalc(float a, float b, char op)
        {
            float Add()
            {
                return a + b;
            }

            float Mult()
            {
                return a * b;
            }

            float Subtract()
            {
                return a - b;
            }

            float Div()
            {
                return a / b;
            }

            float Mod()
            {
                return a % b;
            }

            switch (op)
            {
                case '+':
                    return Add();
                case '*':
                    return Mult();
                case '-':
                    return Subtract();
                case '/':
                    return Div();
                case '%':
                    return Mod();

                default:
                    Console.WriteLine($"Invalid Operator - '{op}'");
                    return 0.0f;
            }
            //Console.WriteLine($"Invalid Operator - '{op}'");
            //return 0.0f;
        }

        //public float DoCalc(float a, float b, char op)
        //{
        //    switch (op)
        //    {
        //        case '+':
        //            return Add(a, b);
        //        case '*':
        //            return Mult(a, b);
        //        case '-':
        //            return Subtract(a, b);
        //        case '/':
        //            return Div(a, b);
        //        case '%':
        //            return Mod(a, b);

        //        default:
        //            Console.WriteLine($"Invalid Operator - '{op}'");
        //            return 0.0f;
        //    }
        //    //Console.WriteLine($"Invalid Operator - '{op}'");
        //    //return 0.0f;
        //}

        //private float Add(float a, float b)
        //{
        //    return a + b;
        //}

        //private float Mult(float a, float b)
        //{
        //    return a * b;
        //}

        //private float Subtract(float a, float b)
        //{
        //    return a - b;
        //}

        //private float Div(float a, float b)
        //{
        //    return a / b;
        //}

        //private float Mod(float a, float b)
        //{
        //    return a % b;
        //}
    }
    class Program
    {
        static void Main(string[] args)
        {
            Calc calc = new Calc();
            Console.Write("Enter first number : ");
            float a = float.Parse(Console.ReadLine());
            Console.Write("Enter second number : ");
            float b = float.Parse(Console.ReadLine());
            Console.Write("Enter the operator (+, -, *, /, %) : ");
            char op = char.Parse(Console.ReadLine());

            Console.WriteLine($"{a} {op} {b} = {calc.DoCalc(a, b, op):f2}");

            Console.ReadKey();
        }

    }
}
