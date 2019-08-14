using System;

public class Example
{
   public static void Main()
   {
      string s1 = null;
      string s2 = "";
      Console.WriteLine(TestForNullOrEmpty(s1));
      Console.WriteLine(TestForNullOrEmpty(s2));

      bool TestForNullOrEmpty(string s)
      {
         bool result;
         // <Snippet1>
         result = s == null || s == string.Empty;
         // </Snippet1>
         return result;
      }
   }
}
