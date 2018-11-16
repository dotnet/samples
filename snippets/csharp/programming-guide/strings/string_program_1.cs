using System;

namespace StringCompare
{
    class Program
    {
        static void Main()
        {
            string fName = "Phillis";
            string lName = "Wheatley";
            int yearBorn = 1753;
            int yearPub = 1773;

            // Variables
            Console.WriteLine("{0} {1} was an African American poet born in {2}.", fName, lName, yearBorn);
            // Simple expression
            Console.WriteLine("She was first published in {0} at the age of {1}.", yearPub, yearPub - yearBorn);
            // Complex Expressions
            Console.WriteLine("She'd be over {0} years old today.", Math.Round((2018d - yearBorn) / 100d) * 100d);

            Console.WriteLine();

            string first = "Jupiter";
            string last = "Hammon";
            int born = 1711;
            int pub = 1761;
            // Variables
            Console.WriteLine($"{first} {last} was an African American poet born in {born}.");
            // Simple expression
            Console.WriteLine($"He was first published in {pub} at the age of {pub - born}.");
            // Complex Expressions
            Console.WriteLine($"He'd be over {Math.Round((2018d - born) / 100d, 0) * 100d} years old today.");                


        }
    }
}
