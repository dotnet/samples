using System;
using System.Diagnostics;

public class Example
{
   public static void Main()
   {
      var p = new Process();  
      p.StartInfo.UseShellExecute = false;  
      p.StartInfo.RedirectStandardError = true;  
      p.StartInfo.FileName = "Write500Lines.exe";  
      p.Start();  

      // To avoid deadlocks, always read the output stream first and then wait.  
      string output = p.StandardError.ReadToEnd();  
      p.WaitForExit();

      Console.WriteLine($"\nError stream: {output}");
   }
}
// The end of the output produced by the example includes the following:
//      Error stream:
//      Successfully wrote 500 lines.