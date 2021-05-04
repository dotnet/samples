using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;

namespace CS_DemoLocalFunctionInCS7
{
    class Program
    {
        static void Main(string[] args)
        {
            Write("enter a first number: ");
            float a = float.Parse(ReadLine());

            Write("enter second number: ");
            float b = float.Parse(ReadLine());

            Write("enter the operator: ");
            char op = char.Parse(ReadLine());

            Calc calc = new Calc();
            WriteLine($"{a} {op} {b}={calc.DoCalc(a, b, op):f2}");

            ReadKey();

        }
    }

    class Calc
    {
        public float DoCalc(float a, float b, char op)
        {
                 float Mod()
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
                //case '+':return Add(a, b);
                //case '-':return Subtract(a, b);
                //case '*':return Mult(a, b);
                //case '/':return Div(a, b);
                //case '%':return Mod(a, b);
                //default:WriteLine($"invalid operator - '{op}'");
                case '+': return Add();
                case '-': return Subtract();
                case '*': return Mult();
                case '/': return Div();
                case '%': return Mod();
                default:WriteLine($"invalid operator - '{op}'");

                    

                    return 0.0f;
            }            
        }

        //private float Mod(float a, float b)
        //{
        //    return a%b;
        //}

        //private float Div(float a, float b)
        //{
        //    return a/b;
        //}

        //private float Mult(float a, float b)
        //{
        //    return a*b;
        //}

        //private float Subtract(float a, float b)
        //{
        //    return a-b;
        //}

        //private float Add(float a, float b)
        //{
        //    return a+b;
        //}
    }
}
