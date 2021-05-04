using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;

namespace ThrowExpression
{
    class Person
    {
        private int number = default;

        public string Name { get; } = default;

        public Person(string name) => Name = name ?? throw new ArgumentNullException(name);

        public string GetFirstName()
        {
            var parts = Name.Split(' ');
            return (parts.Length > 1) ? parts[0] : throw new InvalidOperationException("No full name given");
        }
        public string GetLastName() => throw new NotImplementedException();
    }

    class program
    {
        static void TryWithNameNull()
        {
            try
            {
                new Person(null);
            }
            catch (Exception ex)
            {
                WriteLine($"ex.GetType");
            }
        }
        static void TryGetFirstName()
        {
            try
            {
                new Person("James Bond").GetFirstName();
            }
            catch (Exception ex)
            {
                WriteLine($"{ex.GetType()}:{ ex.Message}");
            }
        }
        static void TryGetLastName()
        {
            try
            {
                new Person("James").GetFirstName();
            }
            catch (Exception ex)
            {
                WriteLine($"{ex.GetType()}:{ ex.Message}");
            }
        }


        static void Main(string[] args)
        {
            // TryGetFirstName();
            TryGetLastName();

            ReadKey();
        }
    }
    
}
