using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;

namespace Cs_DemoRefLocalsandReturnsCs7
{
    class Program
    {
        static ref char FindCharReference(char[] arr, in char v) //ref return in c# 7.0
        { 
            for (int i = 0; i < arr.Length; i++)
            { 
                if(arr[i]==v)
                {
                    return ref arr[i]; // //ref return in c# 7.0
                }

            }
            throw new IndexOutOfRangeException($"{v} is not found");

         }
        static void Main(string[] args)
        {
            char[] arr = { 'h','y','d','e','r','a','b','a','d'};
            WriteLine($"Before : {new string(arr)}");
            char c = 'a';
            ref char charref = ref FindCharReference(arr, in c);
            charref = 'A';
            WriteLine($"After : {new string(arr)}");

            ReadKey();
        }
    }
}
