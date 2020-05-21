using System;

public class QA
{
    public static void WithoutInterpolation()
    {
        Console.WriteLine("Without string interpolation (using composite formatting)\n");
        string[] names = { "Balto", "Vanya", "Dakota", "Samuel", "Koani", "Yiska", "Yuma" };
        string output = names[0] + ", " + names[1] + ", " + names[2] + ", " +
                        names[3] + ", " + names[4] + ", " + names[5] + ", " +
                        names[6];

        output += "\n";
        var date = DateTime.Now;
        output += String.Format("It is {0:t} on {0:d}. The day of the week is {1}.",
                                date, date.DayOfWeek);
        Console.WriteLine(output);

        WithInterpolation();
    }

    private static void WithInterpolation()
    {
        Console.WriteLine("\nWith string interpolation\n");
        string[] names = { "Balto", "Vanya", "Dakota", "Samuel", "Koani", "Yiska", "Yuma" };
        string output = $"{names[0]}, {names[1]}, {names[2]}, {names[3]}, {names[4]}, " +
                        $"{names[5]}, {names[6]}";

        var date = DateTime.Now;
        output += $"\nIt is {date:t} on {date:d}. The day of the week is {date.DayOfWeek}.";
        Console.WriteLine(output);
    }

    public static void DecimalDigits()
    {
        object[] values = { 1603, 1794.68235, 15436.14 };
        string result;
        foreach (var value in values)
        {
            result = String.Format("{0,12:C2}   {0,12:E3}   {0,12:F4}   {0,12:N3}  {1,12:P2}\n",
                                   Convert.ToDouble(value), Convert.ToDouble(value) / 10000);
            Console.WriteLine(result);
        }
    }

    // The example displays output like the following:
    //       $1,603.00     1.603E+003      1603.0000      1,603.000       16.03 %
    //    
    //       $1,794.68     1.795E+003      1794.6824      1,794.682       17.95 %
    //    
    //      $15,436.14     1.544E+004     15436.1400     15,436.140      154.36 %   

    public static void DigitsUsingCustomFormatSpecifier()
    {
        decimal value = 16309.5436m;
        string result = String.Format("{0,12:#.00000} {0,12:0,000.00} {0,12:000.00#}",
                                        value);
        Console.WriteLine(result);
    }

    // The example displays the following output:
    //        16309.54360    16,309.54    16309.544

    public static void IntegralDigits()
    {
        int value = 1326;
        string result = String.Format("{0,10:D6} {0,10:X8}", value);
        Console.WriteLine(result);
    }

    // The example displays the following output:
    //     001326   0000052E        

    public static void IntegralDigitsUsingCustom()
    {
        int value = 16342;
        string result = String.Format("{0,18:00000000} {0,18:00000000.000} {0,18:000,0000,000.0}",
                                      value);
        Console.WriteLine(result);
    }

    // The example displays the following output:
    //           00016342       00016342.000    0,000,016,342.0       

    public static void EscapedBraces()
    {
        string result;
        int nOpen = 1;
        int nClose = 2;
        result = String.Format("The text has {0} '{{' characters and {1} '}}' characters.",
                            nOpen, nClose);
        Console.WriteLine(result);
    }

    public static void BracesInFormatList()
    {
        string result;
        int nOpen = 1;
        int nClose = 2;
        result = String.Format("The text has {0} '{1}' characters and {2} '{3}' characters.",
                            nOpen, "{", nClose, "}");
        Console.WriteLine(result);
    }

    public static void FormatException()
    {
        Random rnd = new Random();
        int[] numbers = new int[4];
        int total = 0;
        for (int ctr = 0; ctr <= 2; ctr++)
        {
            int number = rnd.Next(1001);
            numbers[ctr] = number;
            total += number;
        }
        numbers[3] = total;
        object[] values = new object[numbers.Length];
        numbers.CopyTo(values, 0);
        Console.WriteLine("{0} + {1} + {2} = {3}", values);
    }
}

