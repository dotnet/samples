using System;

public class Example
{
   public static void Main()
   {
      // <Snippet10>
      String input = "abacus -- alabaster - * - atrium -+- " +
                     "any -*- actual - + - armoir - - alarm";
      String pattern = @"\s-\s?[+*]?\s?-\s";
      String[] elements = System.Text.RegularExpressions.Regex.Split(input, pattern);
      foreach (var element in elements)
         Console.WriteLine(element);
      // The example displays the following output:
      //       abacus
      //       alabaster
      //       atrium
      //       any
      //       actual
      //       armoir
      //       alarm
      // </Snippet10>
   }
}
