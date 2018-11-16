using System;

namespace StringCompare
{
    class Program
    {
        static void Main()
        {
            string jupiterFirst = "Jupiter";
            string jupiterLast = "Hammon";
            int jupiterBorn = 1711;
            int jupiterPub = 1761;
            // Variables
            Console.WriteLine($"{jupiterFirst} {jupiterLast} was an African American poet born in {jupiterBorn}.");
            // Simple expression
            Console.WriteLine($"He was first published in {jupiterPub} at the age of {jupiterPub - jupiterBorn}.");
            // Complex Expressions
            Console.WriteLine($"He'd be over {Math.Round((2018d - jupiterBorn) / 100d, 0) * 100d} years old today.");

            // Output:
            // Jupiter Hammon was an African American poet born in 1711.
            // He was first published in 1761 at the age of 50.
            // He'd be over 300 years old today. 

            System.Console.WriteLine();

            string phillisFirst = "Phillis";
            string phillisLast = "Wheatley";
            int phillisBorn = 1753;
            int phillisPub = 1773;

            // Variables
            Console.WriteLine("{0} {1} was an African American poet born in {2}.", phillisFirst, phillisLast, phillisBorn);
            // Simple expression
            Console.WriteLine("She was first published in {0} at the age of {1}.", phillisPub, phillisPub - phillisBorn);
            // Complex Expressions
            Console.WriteLine("She'd be over {0} years old today.", Math.Round((2018d - phillisBorn) / 100d) * 100d);

            // Output:
            // Phillis Wheatley was an African American poet born in 1753.
            // She was first published in 1773 at the age of 20.
            // She'd be over 300 years old today.
        }
    }
}
