using System;

public class Example
{
   public static void Main()
   {
      int n = 9;
      Console.WriteLine($@"{n:##.0\%}");  
      Console.WriteLine($@"{n:\'##\'}");      
      Console.WriteLine($@"{n:\\##\\}");      

      Console.WriteLine($"{n:##.0'%'}");  
      Console.WriteLine($@"{n:'\'##'\'}");      
   }
}
// The example displays the following output:
//      9.0%
//      '9'
//      \9\
//
//      9.0%
//      \9\
