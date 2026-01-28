using System;
using System.IO;
using System.Security.AccessControl;
using System.Text;
using CyrillicToLatin.Common;


class ConsoleModule
{
    static void Main()
    {
        string[] args = Environment.GetCommandLineArgs();

        // Get command line arguments.
        if (args.Length != 3 || string.IsNullOrWhiteSpace(args[1]) || string.IsNullOrWhiteSpace(args[2]))
        {
            Console.WriteLine(Common.NoSourceOrDestinationFile);
            ShowSyntax();
            return;
        }

        string source = args[1];
        string destination = args[2];

        if (!File.Exists(source))
        {
            Console.WriteLine(Common.NoSourceOrDestinationFile);
            return;
        }

        try
        {
            using (var sr = new StreamReader(source))
            {
                // Check whether destination file exists and exit if it should not be overwritten.
                if (File.Exists(destination))
                {
                    Console.Write("The destination file {1}   '{0}'{1}exists. Overwrite it? (Y/N) ",
                                source, Environment.NewLine);
                    ConsoleKeyInfo keyPressed = Console.ReadKey(true);
                    if (Char.ToUpper(keyPressed.KeyChar) == 'Y' | Char.ToUpper(keyPressed.KeyChar) == 'N')
                    {
                        Console.WriteLine(keyPressed.KeyChar);
                        if (Char.ToUpper(keyPressed.KeyChar) == 'N')
                            return;
                    }
                }
                using (var sw = new StreamWriter(destination, false, System.Text.Encoding.UTF8))
                {
                    // Instantiate the encoder
                    Encoding encoding = Encoding.GetEncoding("us-ascii", new CyrillicToLatinFallback(), new DecoderExceptionFallback());
                    // This is an encoding operation, so we only need to get the encoder.
                    Encoder encoder = encoding.GetEncoder();
                    Decoder decoder = encoding.GetDecoder();

                    // Define buffer to read characters
                    char[] buffer = new char[100];
                    int charsRead;

                    do
                    {
                        // Read next 100 characters from input stream.
                        charsRead = sr.ReadBlock(buffer, 0, buffer.Length);

                        // Encode characters.
                        int byteCount = encoder.GetByteCount(buffer, 0, charsRead, false);
                        byte[] bytes = new byte[byteCount];
                        int bytesWritten = encoder.GetBytes(buffer, 0, charsRead, bytes, 0, false);

                        // Decode characters back to Unicode and write to a UTF-8-encoded file.
                        char[] charsToWrite = new char[decoder.GetCharCount(bytes, 0, byteCount)];
                        decoder.GetChars(bytes, 0, bytesWritten, charsToWrite, 0);
                        sw.Write(charsToWrite);
                    } while (charsRead == buffer.Length);
                }
            }
        }
        catch (DirectoryNotFoundException e)
        {
            Console.WriteLine(Common.WrongDirectory);
            return;
        }
        catch (IOException e)
        {
            Console.WriteLine(Common.WrongIO);
            return;
        }
    }

    private static void ShowSyntax()
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine();
        sb.AppendLine("Syntax: CyrillicToRoman <source> <destination>");
        sb.AppendLine("   where <source>      = source filename");
        sb.AppendLine("         <destination> = destination filename");
        sb.AppendLine();
        
        Console.WriteLine(sb.ToString());
    }
}
