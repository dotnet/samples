using System;
using System.Collections;

public class Example
{
   public class ReverserClass : IComparer
   {
      // Call CaseInsensitiveComparer.Compare with the parameters reversed.
      int IComparer.Compare(Object x, Object y)
      {
          return ((new CaseInsensitiveComparer()).Compare(y, x));
      }
   }

   public static void Main()
   {
      // Initialize a string array.
      string[] words = { "The", "quick", "brown", "fox", "jumps", "over",
                         "the", "lazy", "dog" };

      // Display the array values.
      Console.WriteLine("The array initially contains the following values:" );
      PrintIndexAndValues(words);

      // Sort the array values using the default comparer.
      Array.Sort(words);
      Console.WriteLine("After sorting with the default comparer:" );
      PrintIndexAndValues(words);

      // Sort the array values using the reverse case-insensitive comparer.
      Array.Sort(words, new ReverserClass());
      Console.WriteLine("After sorting with the reverse case-insensitive comparer:");
      PrintIndexAndValues(words);
   }

   public static void PrintIndexAndValues(IEnumerable list)
   {
      int i = 0;
      foreach (var item in list )
         Console.WriteLine($"   [{i++}]:  {item}");

      Console.WriteLine();
   }
}
// The example displays the following output:
//       The array initially contains the following values:
//          [0]:  The
//          [1]:  quick
//          [2]:  brown
//          [3]:  fox
//          [4]:  jumps
//          [5]:  over
//          [6]:  the
//          [7]:  lazy
//          [8]:  dog
//
//       After sorting with the default comparer:
//          [0]:  brown
//          [1]:  dog
//          [2]:  fox
//          [3]:  jumps
//          [4]:  lazy
//          [5]:  over
//          [6]:  quick
//          [7]:  the
//          [8]:  The
//
//       After sorting with the reverse case-insensitive comparer:
//          [0]:  the
//          [1]:  The
//          [2]:  quick
//          [3]:  over
//          [4]:  lazy
//          [5]:  jumps
//          [6]:  fox
//          [7]:  dog
//          [8]:  brown