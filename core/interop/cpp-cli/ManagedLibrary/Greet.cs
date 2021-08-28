using System;

namespace ManagedLibrary
{
    public class Greet
    {
        public void Hello(string msg)
        {
            Console.WriteLine($"Hello from {typeof(Greet).FullName}");
            Console.WriteLine($"-- message: {msg}");
        }
    }
}
