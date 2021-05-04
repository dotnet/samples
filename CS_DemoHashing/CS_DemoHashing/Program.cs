using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;
using System.Security.Cryptography;
using static System.Text.Encoding;

namespace CS_DemoHashing
{
    class Program
    {
        static string ComputeHash(string input)
        {
            HashAlgorithm sha = SHA256.Create();
            var hashData = sha.ComputeHash(UTF8.GetBytes(input));
            return UTF8.GetString(hashData);
        }

        static void Main(string[] args)
        {
            string input= "This is very very long test string!";
            var hash = ComputeHash(input);
            WriteLine($"The original data : {input}\n Hashed data : {hash}");
            WriteLine(ComputeHash("This is very very long test string!") == hash);

            ReadKey();
        }
    }
}
