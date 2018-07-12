//<SNIPPET1>
using System;
using System.IO;
using System.Security.AccessControl;

namespace FileSystemExample
{
    class FileExample
    {
        public static void Main()
        {
                string FileName = "test.xml";

                Console.WriteLine("Encrypt " + FileName);

                // Encrypt the file.
                AddEncryption(FileName);

                Console.WriteLine("Decrypt " + FileName);

                // Decrypt the file.
                RemoveEncryption(FileName);

                Console.WriteLine("Done");
            
            Console.ReadLine();
        }


        // Encrypt a file.
        public static void AddEncryption(string FileName)
        {

            File.Encrypt(FileName);

        }

        // Decrypt a file.
        public static void RemoveEncryption(string FileName)
        {
            File.Decrypt(FileName);
        }
    }
}
//</SNIPPET1>
