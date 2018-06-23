using System;

public class Example
{
   public static void Main()
   {
      double n = 123.8;
      Console.WriteLine($"{n:#,##0.0K}");  
   }
}
// The example displays the following output:
//      9.0%
//      '9'
//      \9\
//
//      9.0%
//      \9\
