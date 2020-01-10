using System;
using System.Diagnostics;

public class Example
{
   public static void Main()
   {
      var p = new Process();  
      p.StartInfo.UseShellExecute = false;  
      p.StartInfo.RedirectStandardOutput = true;  
      string eOut = null;
      p.StartInfo.RedirectStandardError = true;
      p.ErrorDataReceived += new DataReceivedEventHandler((sender, e) => 
                                 { eOut += e.Data; });
      p.StartInfo.FileName = "Write500Lines.exe";  
      p.Start();  

      // To avoid deadlocks, use an asynchronous read operation on at least one of the streams.  
      p.BeginErrorReadLine();
      string output = p.StandardOutput.ReadToEnd();  
      p.WaitForExit();

      Console.WriteLine($"The last 50 characters in the output stream are:\n'{output.Substring(output.Length - 50)}'");
      Console.WriteLine($"\nError stream: {eOut}");
   }
}
// The example displays the following output:
//      The last 50 characters in the output stream are:
//      ' 49,800.20%
//      Line 500 of 500 written: 49,900.20%
//      '
//
//      Error stream: Successfully wrote 500 lines.
