using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

class WriteTextFiles
{
    static void Main(string[] args)
    {

        // Example 1: Shows how to write synchronously
        // to a text file using StreamWriter
        // line by line
        WriteLineByLine();

        // Example 2: Shows how to append text to
        // an existing file using StreamWriter
        AppendTextSW();

        // Example 3: Shows how to write  
        // a simple string asynchronously
        // to a text file using StreamWriter
        string text = "This is a sentence.";
        WriteTextAsync(text);

        // Example 4: Shows how to write synchronously
        // to a text file using File and then
        // add additional lines 
        WriteFile();
    }

    static void WriteLineByLine()
    {
        // Create a string array with the lines of text
        string[] lines = { "First line", "Second line", "Third line" };

        // Set a variable to the My Documents path.
        string mydocpath =
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        // Write the string array to a new file named "WriteLines.txt".
        using (StreamWriter outputFile = new StreamWriter(Path.Combine(mydocpath,"WriteLines.txt"))) {
            foreach (string line in lines)
                outputFile.WriteLine(line);
        }

    }

    static void AppendTextSW()
    {
        // Set a variable to the My Documents path.
        string mydocpath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        // Append text to an existing file named "WriteLines.txt".
        using (StreamWriter outputFile = new StreamWriter(Path.Combine(mydocpath,"WriteLines.txt"), true)) {
            outputFile.WriteLine("Fourth Line");
        }

    }

    static async void WriteTextAsync(string text)
    {
        // Set a variable to the My Documents path.
        string mydocpath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        // Write the text asynchronously to a new file named "WriteTextAsync.txt".
        using (StreamWriter outputFile = new StreamWriter(Path.Combine(mydocpath,"WriteTextAsync.txt"))) {
            await outputFile.WriteAsync(text);
        }
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
