using System;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new Contracts.Server();

            Console.WriteLine($"5 + 5 = {server.Add(5, 5)}");
        }
    }
}
