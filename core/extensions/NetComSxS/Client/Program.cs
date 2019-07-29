using System;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Contracts.IServer server = new Contracts.Server();

            Console.WriteLine($"5 + 5 = {server.Add(5, 5)}");
        }
    }
}
