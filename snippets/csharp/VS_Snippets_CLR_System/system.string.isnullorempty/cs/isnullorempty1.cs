using System;

public class Example
{
   public static void Main()
   {
      // <Snippet1>
      string s1 = null;
      string s2 = "";
      Console.WriteLine(TestForNullOrEmpty(s1));
      Console.WriteLine(TestForNullOrEmpty(s2));

      bool TestForNullOrEmpty(string s)
      {
         bool result;
         result = s == null || s == string.Empty;
         return result;
      }
      // </Snippet1>
   }
}
