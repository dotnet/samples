using System;

public class GetStarted
{
    public static void Example1()
    {
        Decimal pricePerOunce = 17.36m;
        String s = String.Format("The current price is {0} per ounce.",
                                 pricePerOunce);
        Console.WriteLine(s);

        // Result: The current price is 17.36 per ounce.
    }

    public static void Example2()
    {
        Decimal pricePerOunce = 17.36m;
        String s = String.Format("The current price is {0:C2} per ounce.",
                                pricePerOunce);
        Console.WriteLine(s);

        // Result if current culture is en-US:
        //      The current price is $17.36 per ounce.
    }

    public static void Example3()
    {
        decimal temp = 20.4m;
        string s = String.Format("The temperature is {0}°C.", temp);
        Console.WriteLine(s);

        // Displays 'The temperature is 20.4°C.'        
    }

    public static void Example4()
    {
        string s = String.Format("At {0}, the temperature is {1}°C.",
                                DateTime.Now, 20.4);
        Console.WriteLine(s);

        // Output similar to: 'At 4/10/2015 9:29:41 AM, the temperature is 20.4°C.'        
    }

    public static void Example5()
    {
        string s = String.Format("It is now {0:d} at {0:t}", DateTime.Now);
        Console.WriteLine(s);

        // Output similar to: 'It is now 4/10/2015 at 10:04 AM'        
    }

    public static void Example6()
    {
        int[] years = { 2013, 2014, 2015 };
        int[] population = { 1025632, 1105967, 1148203 };
        var sb = new System.Text.StringBuilder();
        sb.Append(String.Format("{0,6} {1,15}\n\n", "Year", "Population"));
        for (int index = 0; index < years.Length; index++)
            sb.Append(String.Format("{0,6} {1,15:N0}\n", years[index], population[index]));

        Console.WriteLine(sb);

        // Result:
        //      Year      Population
        //
        //      2013       1,025,632
        //      2014       1,105,967
        //      2015       1,148,203        
    }

    public static void Example7()
    {
        int[] years = { 2013, 2014, 2015 };
        int[] population = { 1025632, 1105967, 1148203 };
        String s = String.Format("{0,-10} {1,-10}\n\n", "Year", "Population");
        for (int index = 0; index < years.Length; index++)
            s += String.Format("{0,-10} {1,-10:N0}\n",
                                years[index], population[index]);
        Console.WriteLine($"\n{s}");

        // Result:
        //    Year       Population
        //
        //    2013       1,025,632
        //    2014       1,105,967
        //    2015       1,148,203        
    }
}
