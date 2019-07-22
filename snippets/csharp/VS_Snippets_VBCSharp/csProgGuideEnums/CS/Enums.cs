using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Enums
{
    //<snippet1>
    enum Day { Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday };
    enum Month : byte { Jan, Feb, Mar, Apr, May, Jun, Jul, Aug, Sep, Oct, Nov, Dec }; 
    //</snippet1>

    //<snippet2>
    [Flags]
    enum Days
    {
        None        = 0,
        Sunday      = 1,
        Monday      = 2,
        Tuesday     = 4,
        Wednesday   = 8,
        Thursday    = 16,
        Friday      = 32,
        Saturday    = 64
    }
    class MyClass
    {
        Days meetingDays = Days.Tuesday | Days.Thursday;
    }
    //</snippet2>

    //<snippet3>
    enum MachineState
    {
        PowerOff = 0,
        Running = 5,
        Sleeping = 10,
        Hibernating = Sleeping + 5
    }
    //</snippet3>
    class Program
    {
        static void Main(string[] args)
        {
            //<snippet4>
            Day meetingDay = Day.Monday;
            //...
            meetingDay = Day.Friday;
            //</snippet4>
            Console.WriteLine($"Meeting day is {meetingDay}");

            Console.WriteLine($"Meeting day is {(int) meetingDay}");

            meetingDay = (Day)42;
            Console.WriteLine($"Meeting day is {meetingDay}");

            Days meetingDays = Days.Tuesday | Days.Thursday;
            Console.WriteLine(meetingDays);

            //<snippet5>
            string s = Enum.GetName(typeof(Day), 4);
            Console.WriteLine(s);

            Console.WriteLine("The values of the Day Enum are:");
            foreach (int i in Enum.GetValues(typeof(Day)))
                Console.WriteLine(i);

            Console.WriteLine("The names of the Day Enum are:");
            foreach (string str in Enum.GetNames(typeof(Day)))
                Console.WriteLine(str);
            //</snippet5>

            //<snippet6>
            // Initialize with two flags using bitwise OR.
            meetingDays = Days.Tuesday | Days.Thursday;

            // Set an additional flag using bitwise OR.
            meetingDays = meetingDays | Days.Friday;

            Console.WriteLine($"Meeting days are {meetingDays}");
            // Output: Meeting days are Tuesday, Thursday, Friday

            // Toggle a flag using bitwise XOR.
            meetingDays = meetingDays ^ Days.Tuesday;
            Console.WriteLine($"Meeting days are {meetingDays}");
            // Output: Meeting days are Thursday, Friday
            //</snippet6>

            //<snippet7>
            // Test value of flags using bitwise AND.
            bool test = (meetingDays & Days.Thursday) == Days.Thursday;
            Console.WriteLine($"Thursday {test == true ? "is" : "is not"} a meeting day.");
            // Output: Thursday is a meeting day.
            //</snippet7>

            Console.ReadKey();
        }
    }
}
