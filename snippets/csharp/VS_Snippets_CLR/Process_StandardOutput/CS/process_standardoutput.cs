//<snippet1>
using System;
using System.IO;
using System.Diagnostics;

class IORedirExample
{
    public static void Main()
    {
        string[] args = Environment.GetCommandLineArgs();
        if (args.Length > 1)
        {
            // This is the code for the spawned process
            Console.WriteLine("Hello from the redirected process!");
        }
        else
        {
            // This is the code for the base process
            using (Process myProcess = new Process())
            {
                // Start a new instance of this program but specify the 'spawned' version.
                ProcessStartInfo myProcessStartInfo = new ProcessStartInfo(args[0], "spawn");
                myProcessStartInfo.UseShellExecute = false;
                myProcessStartInfo.RedirectStandardOutput = true;
                myProcess.StartInfo = myProcessStartInfo;
                myProcess.Start();
                StreamReader myStreamReader = myProcess.StandardOutput;
                // Read the standard output of the spawned process.
                string myString = myStreamReader.ReadLine();
                Console.WriteLine(myString);

                myProcess.WaitForExit();
            }
        }
    }
}
//</snippet1>
