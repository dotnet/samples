using System;
using System.Text;

public class Example
{
   public static void Main()
   {
      // <Snippet11>
      StringBuilder MyStringBuilder = new StringBuilder("Hello World!");
      MyStringBuilder.Replace('!', '?');
      Console.WriteLine(MyStringBuilder);

      // The example displays the following output:
      //       Hello World?
      // </Snippet11>
   }
}
