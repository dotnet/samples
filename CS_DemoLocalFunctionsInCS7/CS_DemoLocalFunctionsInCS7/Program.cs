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

            Write("Enter the operator (+, -, *, /, %) : ");
            Char op = char.Parse(ReadLine());

            Calc calc = new Calc();
            WriteLine($"{a} {op} {b} = {calc.DoCalc(a, b, op):f2}");

            ReadKey();
        }
    }

    class Calc
    {
        public float DoCalc(float a, float b, char op)
        {
             float Mod()//local functions or nested function
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

             float Subract()
            {
                return a - b;
            }

             float Add()
            {
                return a + b;
            }
            switch (op)
            {
                //case '+':return Add(a, b);
                //case '-':return Subract(a, b);
                //case '*':return Mult(a, b);
                //case '/':return Div(a, b);
                //case '%':return Mod(a, b);
                case '+': return Add();
                case '-': return Subract();
                case '*': return Mult();
                case '/': return Div();
                case '%': return Mod();
                default:WriteLine($"Invalid operator - '{op}'");
                    return 0.0f;
            }
        }

        //private float Mod(float a, float b)
        //{
        //    return a % b;
        //}

        //private float Div(float a, float b)
        //{
        //    return a / b;
        //}

        //private float Mult(float a, float b)
        //{
        //    return a * b;
        //}

        //private float Subract(float a, float b)
        //{
        //    return a - b;
        //}

        //private float Add(float a, float b)
        //{
        //    return a + b;
        //}
        

    }
}
