using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace CS_DemoRegEx
{
    class Program
    {
        
     public static bool IsValid(string value)
        {
            //return Regex.IsMatch(value, @"^[a-zA-Z0-9]*$");
            Regex regex = new Regex (@"^[a-zA-Z0-9]*$");
            return regex.IsMatch(value);
        }

         private static void ShowMatch(string text, string expression)
        {
            Console.WriteLine("The Expression:" +expression);
            MatchCollection matchcollection = Regex.Matches(text, expression);
            foreach(Match match in matchcollection)
            { 
                Console.WriteLine(match);
            }
        }
        static void Main(string[] args)
        {
            //Console.WriteLine(IsValid("DotNetLearningAndDevelopment2019"));
            // Console.WriteLine(IsValid("DotNet Learning And Development 2019"));
            //string input = "The sun sets in south west during winter";
            //Console.WriteLine("Matching words that start with 's':");
            //ShowMatch(input, @"\bS\S*");
            //string input = "welcome to CapGemini";
            //string pattern= 2"\s+";
            //string replacement = "-";

            //Regex regex = new Regex(pattern);
            //string result = regex.Replace(input, replacement);

            //Console.WriteLine($"Original String : {input}");
            //Console.WriteLine($"Replacement String : {result}");

            string operation = "3 * 5 = 15";
            var operands = Regex.Split(operation, @"\s+");
            foreach(var operand in operands)
            {
                Console.WriteLine(operand);
            }




            Console.ReadKey();
        }
    }
}

