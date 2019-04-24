using System;
using System.IO;

public class Example
{
   public static void Main()
   {
      for (int ctr = 0; ctr < 500; ctr++)
         Console.WriteLine($"Line {ctr + 1} of 500 written: {ctr + 1/500.0:P2}");

      Console.Error.WriteLine("\nSuccessfully wrote 500 lines.\n");
   }
}
// The example displays the following output:
//      The last 50 characters in the output stream are:
//      ' 49,800.20%
//      Line 500 of 500 written: 49,900.20%
//'
//
//      Error stream: Successfully wrote 500 lines.