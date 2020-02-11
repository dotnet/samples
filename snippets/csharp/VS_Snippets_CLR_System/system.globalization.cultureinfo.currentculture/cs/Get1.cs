using System;
using System.Globalization;

public class Example
{
   public static void Main()
   {
      // <Snippet5>
      CultureInfo culture = CultureInfo.CurrentCulture;
      Console.WriteLine("The current culture is {0} [{1}]",
                        culture.NativeName, culture.Name);

      // The example displays output like the following:
      //       The current culture is English (United States) [en-US]
      // </Snippet5>
   }
}
