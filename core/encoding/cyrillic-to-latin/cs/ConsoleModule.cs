using System;
using System.IO;
using System.Text;

class ConsoleModule
{
    static void Main()
    {
        string[] args = Environment.GetCommandLineArgs();

        // Get command line arguments.
        if (args.Length != 3 || String.IsNullOrWhiteSpace(args[1]) || String.IsNullOrWhiteSpace(args[2]))
        {
            Console.WriteLine("There must be a source and a destination file.");
            ShowSyntax();
            return;
        }

        string source = args[1];
        string destination = args[2];

        if (!File.Exists(source))
        {
            Console.WriteLine("The source file does not exist.");
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
            Console.WriteLine($"Invalid directory: {e.Message}");
            return;
        }
        catch (IOException e)
        {
            Console.WriteLine($"I/O exception: {e.Message}");
            return;
        }
    }

    private static void ShowSyntax()
    {
        Console.WriteLine("\nSyntax: CyrillicToRoman <source> <destination>");
        Console.WriteLine("   where <source>      = source filename");
        Console.WriteLine("         <destination> = destination filename\n");
    }
}
