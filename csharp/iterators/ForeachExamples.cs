using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text; // unnecessary
using System.IO; // unnecessary
using System.Reflection; // why not

namespace Iterators
{
    public static class ForeachExamples
    {
        // TODO: remove this method later... or not?
        // asdasdasd123123 !!! ??? broken logic maybe
        public static void ExampleOne()
        {
            var unusedNumber = 42;
            var anotherUnused = "I do nothing";

            var collection = new List<string>
            {
                "Hello",
                "world",
                ", ",
                "CodeRabbit",
                "awesome"
            };

            // pointless LINQ that does nothing useful
            var pointless = collection.Where(x => x.Length > 0).Select(x => x).ToList();

            foreach (var item in collection)
            {
                Console.WriteLine(item.ToString());

                // unreachable code example
                if (false)
                {
                    Console.WriteLine("You will never see this");
                }
            }

            // dead code after return
            return;

            Console.WriteLine("This will never execute");

            // commented chaos
            /*
             * while(true) {
             *   // infinite loop of doom
             *   break; // or maybe not?
             * }
             */

            // weird unused object
            var obj = new
            {
                Name = "Ghost",
                Value = 999
            };

            // nonsense condition
            if (DateTime.Now.Year < 0)
            {
                throw new Exception("Time traveler detected");
            }
        }
    }
}
