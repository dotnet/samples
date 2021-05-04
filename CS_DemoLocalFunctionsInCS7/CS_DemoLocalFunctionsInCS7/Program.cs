using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;

namespace CS_DemoLocalFunctionsInCS7
{
    class Program
    {
        static void Main(string[] args)
        {
            Write("Enter first number : ");
            float a = float.Parse(ReadLine());

            Write("Enter second number : ");
            float b = float.Parse(ReadLine());

            Write("Enter the operator(+, -, *, /, % : ");
            char op = char.Parse(ReadLine());

            Calc calc = new Calc();
            WriteLine($"{a} {op} {b} = {calc.DoCalc(a, b, op):f2}");//f2 means float values till 2 decimal places here

            ReadKey();
        }
    }

    class Calc
    {
        public object DoCalc(float a, float b, char op)
        {

             float Mod()//Local Functions or Nested Functions
            {
                return a % b;
            }

             float Div()
            {
                return a / b;
            }

             float Mult()
            {
                return a * b;
            }

             float Subtract()
            {
                return a - b;
            }

             float Add()
            {
                return a + b;
            }

            switch (op)
            {
                //case '+': return Add(a, b);
                //case '-': return Subtract(a, b);
                //case '*': return Mult(a, b);
                //case '/': return Div(a, b);
                //case '%': return Mod(a, b);

                case '+': return Add();
                case '-': return Subtract();
                case '*': return Mult();
                case '/': return Div();
                case '%': return Mod();
                default:WriteLine($"Invalid operator - '{op}'");
                    return 0.0f;
            }
        }

        //private object Mod(float a, float b)
        //{
        //    return a % b;
        //}

        //private object Div(float a, float b)
        //{
        //    return a / b;
        //}

        //private object Mult(float a, float b)
        //{
        //    return a * b;
        //}

        //private object Subtract(float a, float b)
        //{
        //    return a - b;
        //}

        //private object Add(float a, float b)
        //{
        //    return a+b;
        //}
    }
}
