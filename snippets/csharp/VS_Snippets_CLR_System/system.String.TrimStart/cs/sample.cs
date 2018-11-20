using System;
using System.Collections.Generic;

public class TrimExample
{
   // <Snippet3>
   public static void Main()
   {
      string[] lines= {"using System;",
                       "", 
                       "public class HelloWorld",
                       "{", 
                       "   public static void Main()",
                       "   {", 
                       "      // This code displays a simple greeting", 
                       "      // to the console.", 
                       "      Console.WriteLine(\"Hello, World.\");", 
                       "   }", 
                       "}"};
      Console.WriteLine("Before call to StripComments:");
      foreach (string line in lines)
         Console.WriteLine("   {0}", line);                         
      
      string[] strippedLines = StripComments(lines); 
      Console.WriteLine("After call to StripComments:");
      foreach (string line in strippedLines)
         Console.WriteLine("   {0}", line);                         
   }
   // This code produces the following output to the console:
   //    Before call to StripComments:
   //       using System;
   //   
   //       public class HelloWorld
   //       {
   //           public static void Main()
   //           {
   //               // This code displays a simple greeting
   //               // to the console.
   //               Console.WriteLine("Hello, World.");
   //           }
   //       }  
   //    After call to StripComments:
   //       This code displays a simple greeting
   //       to the console.
   // </Snippet3>
   
   // <Snippet2>
   public static string[] StripComments(string[] lines)
   { 
      List<string> lineList = new List<string>();
      foreach (string line in lines)
      {
         if (line.TrimStart(' ').StartsWith("//"))
            lineList.Add(line.TrimStart(' ', '/'));
      }
      return lineList.ToArray();
   }   
   // </Snippet2>

   // <Snippet1>
   public static void Main(string[] args)
   {
	string lineWithLeadingSpaces = "   Hello World!";
		
	Console.WriteLine($"This line has leading spaces: {lineWithLeadingSpaces}");
	// This line has leading spaces:    Hello World!
	
	// Apply String.TrimStart to the variable in question
	string lineAfterTrimStart = lineWithLeadingSpaces.TrimStart(' ');
	
	Console.WriteLine($"This is the result after calling TrimStart: {lineAfterTrimStart}");
	// This is the result after calling TrimStart: Hello World!    
   }
   // </Snippet1>
}

