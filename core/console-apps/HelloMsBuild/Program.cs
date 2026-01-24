using System;

namespace Hello
{
    class Program
    {
        static void Main(string[] args)
        {
            // Print welcome message
            Console.WriteLine("Hello from MSBuild-powered .NET Console App!");

            // Ask for the user's name
            Console.Write("What's your name? ");
            string name = Console.ReadLine();

            // Greet the user
            Console.WriteLine($"Nice to meet you, {name}!");

            // Wait before closing
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
