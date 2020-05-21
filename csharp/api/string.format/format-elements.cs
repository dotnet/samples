using System;

public class FormatElements
{
    public static void FormatMethod()
    {
        DateTime dat = new DateTime(2012, 1, 17, 9, 30, 0);
        string city = "Chicago";
        int temp = -16;
        string output = String.Format("At {0} in {1}, the temperature was {2} degrees.",
                                    dat, city, temp);
        Console.WriteLine(output);

        // The example displays output like the following:
        //    At 1/17/2012 9:30:00 AM in Chicago, the temperature was -16 degrees.
    }

    public static void FormatItem()
    {
        var value = String.Format("{0,-10:C}", 126347.89m);
        Console.WriteLine(value);
    }

    public static void FormattedFormatItem()
    {
        // Create array of 5-tuples with population data for three U.S. cities, 1940-1950.
        Tuple<string, DateTime, int, DateTime, int>[] cities =
            { Tuple.Create("Los Angeles", new DateTime(1940, 1, 1), 1504277,
                            new DateTime(1950, 1, 1), 1970358),
                Tuple.Create("New York", new DateTime(1940, 1, 1), 7454995,
                            new DateTime(1950, 1, 1), 7891957),
                Tuple.Create("Chicago", new DateTime(1940, 1, 1), 3396808,
                            new DateTime(1950, 1, 1), 3620962),
                Tuple.Create("Detroit", new DateTime(1940, 1, 1), 1623452,
                            new DateTime(1950, 1, 1), 1849568) };

        // Display header
        var header = String.Format("{0,-12}{1,8}{2,12}{1,8}{2,12}{3,14}\n",
                                        "City", "Year", "Population", "Change (%)");
        Console.WriteLine(header);
        foreach (var city in cities)
        {
            var output = String.Format("{0,-12}{1,8:yyyy}{2,12:N0}{3,8:yyyy}{4,12:N0}{5,14:P1}",
                                    city.Item1, city.Item2, city.Item3, city.Item4, city.Item5,
                                    (city.Item5 - city.Item3) / (double)city.Item3);
            Console.WriteLine(output);
        }

        // The example displays the following output:
        //    City            Year  Population    Year  Population    Change (%)
        //    
        //    Los Angeles     1940   1,504,277    1950   1,970,358        31.0 %
        //    New York        1940   7,454,995    1950   7,891,957         5.9 %
        //    Chicago         1940   3,396,808    1950   3,620,962         6.6 %
        //    Detroit         1940   1,623,452    1950   1,849,568        13.9 %        
    }

    public static void SameIndex()
    {
        short[] values = { Int16.MinValue, -27, 0, 1042, Int16.MaxValue };
        Console.WriteLine("{0,10}  {1,10}\n", "Decimal", "Hex");
        foreach (short value in values)
        {
            string formatString = String.Format("{0,10:G}: {0,10:X}", value);
            Console.WriteLine(formatString);
        }
    }
}