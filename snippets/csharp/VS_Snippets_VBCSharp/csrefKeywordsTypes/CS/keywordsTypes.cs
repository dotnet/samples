using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace csrefKeywordsMethodParams
{
    //<snippet5>
    class Child
    {
        private int age;
        private string name;

        // Default constructor:
        public Child()
        {
            name = "N/A";
        }

        // Constructor:
        public Child(string name, int age)
        {
            this.name = name;
            this.age = age;
        }

        // Printing method:
        public void PrintChild()
        {
            Console.WriteLine("{0}, {1} years old.", name, age);
        }
    }

    class StringTest
    {
        static void Main()
        {
            // Create objects by using the new operator:
            Child child1 = new Child("Craig", 11);
            Child child2 = new Child("Sally", 10);

            // Create an object using the default constructor:
            Child child3 = new Child();

            // Display results:
            Console.Write("Child #1: ");
            child1.PrintChild();
            Console.Write("Child #2: ");
            child2.PrintChild();
            Console.Write("Child #3: ");
            child3.PrintChild();
        }
    }
    /* Output:
        Child #1: Craig, 11 years old.
        Child #2: Sally, 10 years old.
        Child #3: N/A, 0 years old.
    */
    //</snippet5>

    //<snippet10>
    public class EnumTest
    {
        enum Day { Sun, Mon, Tue, Wed, Thu, Fri, Sat };

        static void Main()
        {
            int x = (int)Day.Sun;
            int y = (int)Day.Fri;
            Console.WriteLine("Sun = {0}", x);
            Console.WriteLine("Fri = {0}", y);
        }
    }
    /* Output:
       Sun = 0
       Fri = 5
    */
    //</snippet10>

    //<snippet11>
    public class EnumTest2
    {
        enum Range : long { Max = 2147483648L, Min = 255L };
        static void Main()
        {
            long x = (long)Range.Max;
            long y = (long)Range.Min;
            Console.WriteLine("Max = {0}", x);
            Console.WriteLine("Min = {0}", y);
        }
    }
    /* Output:
       Max = 2147483648
       Min = 255
    */
    //</snippet11>

    //<snippet12>
    // Add the attribute Flags or FlagsAttribute.
    [Flags]
    public enum CarOptions
    {
        // The flag for SunRoof is 0001.
        SunRoof = 0x01,
        // The flag for Spoiler is 0010.
        Spoiler = 0x02,
        // The flag for FogLights is 0100.
        FogLights = 0x04,
        // The flag for TintedWindows is 1000.
        TintedWindows = 0x08,
    }

    class FlagTest
    {
        static void Main()
        {
            // The bitwise OR of 0001 and 0100 is 0101.
            CarOptions options = CarOptions.SunRoof | CarOptions.FogLights;

            // Because the Flags attribute is specified, Console.WriteLine displays
            // the name of each enum element that corresponds to a flag that has
            // the value 1 in variable options.
            Console.WriteLine(options);
            // The integer value of 0101 is 5.
            Console.WriteLine((int)options);
        }
    }
    /* Output:
       SunRoof, FogLights
       5
    */
    //</snippet12>

    //<snippet14>
    interface ISampleInterface
    {
        void SampleMethod();
    }

    class ImplementationClass : ISampleInterface
    {
        // Explicit interface member implementation: 
        void ISampleInterface.SampleMethod()
        {
            // Method implementation.
        }

        static void Main()
        {
            // Declare an interface instance.
            ISampleInterface obj = new ImplementationClass();

            // Call the member.
            obj.SampleMethod();
        }
    }
    //</snippet14>

    //<snippet15>
    interface IPoint
    {
       // Property signatures:
       int x
       {
          get;
          set;
       }

       int y
       {
          get;
          set;
       }
    }

    class Point : IPoint
    {
       // Fields:
       private int _x;
       private int _y;

       // Constructor:
       public Point(int x, int y)
       {
          _x = x;
          _y = y;
       }

       // Property implementation:
       public int x
       {
          get
          {
             return _x;
          }

          set
          {
             _x = value;
          }
       }

       public int y
       {
          get
          {
             return _y;
          }
          set
          {
             _y = value;
          }
       }
    }

    class MainClass
    {
       static void PrintPoint(IPoint p)
       {
          Console.WriteLine("x={0}, y={1}", p.x, p.y);
       }

       static void Main()
       {
          IPoint p = new Point(2, 3);
          Console.Write("My Point: ");
          PrintPoint(p);
       }
    }
    // Output: My Point: x=2, y=3
    //</snippet15>

    class VarTest
    {
        #region compilation only
        class Customer
        {
            public string City { get; set; }
            public string Name { get; set; }
            public string Phone { get; set; }
        }

        #endregion
        static void Main()
        {
            List<Customer> customers = new List<Customer>();
            //<snippet18>
            // Example #1: var is optional when
            // the select clause specifies a string
            string[] words = { "apple", "strawberry", "grape", "peach", "banana" };
            var wordQuery = from word in words
                            where word[0] == 'g'
                            select word;

            // Because each element in the sequence is a string, 
            // not an anonymous type, var is optional here also.
            foreach (string s in wordQuery)
            {
                Console.WriteLine(s);
            }

            // Example #2: var is required because
            // the select clause specifies an anonymous type
            var custQuery = from cust in customers
                            where cust.City == "Phoenix"
                            select new { cust.Name, cust.Phone };

            // var must be used because each item 
            // in the sequence is an anonymous type
            foreach (var item in custQuery)
            {
                Console.WriteLine("Name={0}, Phone={1}", item.Name, item.Phone);
            }
            //</snippet18>
        }
    }
}
