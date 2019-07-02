using System;
using System.Linq;

namespace Conversion
{
    public static class ToLookupSample1
    {
        //This sample uses ToLookup to immediately evaluate a sequence and a 
        //related key expression into a grouped dictionary.
        //
        //Output:
        //Total Females: 2
        //Total Males: 1
        public static void Example()
        {
            var scoreRecords = new[]
            {
                new {Name = "Alice", Score = 50, Gender = "F"},
                new {Name = "Bob", Score = 40, Gender = "M"},
                new {Name = "Cathy", Score = 45, Gender = "F"}
            };

            var scoreRecordsLookup = scoreRecords.ToLookup(sr => sr.Gender);

            Console.WriteLine($"Total Females: {scoreRecordsLookup["F"].Count()}");
            Console.WriteLine($"Total Males: {scoreRecordsLookup["M"].Count()}");
        }
    }
}
