using System;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        WriteFile();
    }

    static void WriteFile()
    {
        // Create a string array with the lines of text
        string text = "First line" + Environment.NewLine;

        // Set a variable to the My Documents path.
        string mydocpath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        // Write the text to a new file named "WriteFile.txt".
        File.WriteAllText(Path.Combine(mydocpath,"WriteFile.txt"), text);

        // Create a string array with the additional lines of text
        string[] lines = { "New line 1", "New line 2" };

        // Append new lines of text to the file
        File.AppendAllLines(Path.Combine(mydocpath,"WriteFile.txt"), lines);
    }
}