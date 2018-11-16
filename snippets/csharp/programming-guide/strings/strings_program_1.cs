using System;

namespace StringsWork
{
    class Program
    {
        static void Main()
        {
            string fName = "Phillis";
            string lName = "Wheatley";
            int yearB = 1753;
            int yearP = 1773;

            // Variables
            Console.WriteLine("{0} {1} was an African American poet born in {2}.", fName, lName, yearB);
            // Simple expression
            Console.WriteLine("She was first published in {0} at the age of {1}.", yearP, yearP - yearB);
            // Complex Expressions
            Console.WriteLine("She'd be over {0} years old today.", Math.Round((2018d - yearB) / 100d) * 100d);

            Console.WriteLine();

            string f = "Jupiter";
            string l = "Hammon";
            int brn = 1711;
            int pub = 1761;
            // Variables
            Console.WriteLine($"{f} {l} was an African American poet born in {brn}.");
            // Simple expression
            Console.WriteLine($"He was first published in {pub} at the age of {pub - brn}.");
            // Complex Expressions
            Console.WriteLine($"He'd be over {Math.Round((2018d - brn) / 100d, 0) * 100d} years old today.");                


        }
    }
}
