using System;

namespace AgeCalculator
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Age Calculator ===");

            Console.Write("Enter your date of birth (yyyy-mm-dd): ");
            string input = Console.ReadLine();

            if (DateTime.TryParse(input, out DateTime dob))
            {
                DateTime today = DateTime.Today;
                int age = today.Year - dob.Year;
                if (dob > today.AddYears(-age)) age--;

                Console.WriteLine($"You are {age} years old.");

                DateTime nextBirthday = dob.AddYears(age + 1);
                int daysUntil = (nextBirthday - today).Days;
                Console.WriteLine($"Your next birthday is in {daysUntil} days.");
            }
            else
            {
                Console.WriteLine("Invalid date format. Please use yyyy-mm-dd.");
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
