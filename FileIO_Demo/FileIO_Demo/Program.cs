using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileIO_Demo
{
    class Program
    {
        static void WriteBinaryFile(string data) 
        {
            //write to binary file (bin--debug--.bin file is present here)
            FileStream stream = new FileStream("SampleBinaryFile.bin", FileMode.Create, FileAccess.Write);
            BinaryWriter writer = new BinaryWriter(stream);
            try
            {
                writer.Write(true);
                writer.Write(DateTime.Now.ToString());
                writer.Write(10000);
                writer.Write(data);
                writer.Flush();
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally 
            {
                writer.Close();
                writer.Dispose();
                stream.Close();
                stream.Dispose();
            }
        }

        static void ReadBinaryfile()
        {
            //read from binary file
            FileStream stream = new FileStream("SampleBinaryFile.bin", FileMode.Open, FileAccess.Read);
            BinaryReader reader = new BinaryReader(stream);
            try
            {
                Console.WriteLine(reader.ReadBoolean());
                Console.WriteLine(DateTime.Parse(reader.ReadString()));
                Console.WriteLine(reader.ReadInt32());
                Console.WriteLine(reader.ReadString());
            }
            catch (IOException ex)
            {

                Console.WriteLine(ex.Message);
            }
            finally
            {
                reader.Close();
                reader.Dispose();
                stream.Close();
                stream.Dispose();
            }
        }

        static void WriteTextFile(string data)
        {
            FileStream stream = new FileStream("SampleTextFile.txt", FileMode.Create, FileAccess.Write);
            StreamWriter writer = new StreamWriter(stream);
            try
            {
                writer.Write(data);
                writer.Flush();
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                writer.Close();
                writer.Dispose();
                stream.Close();
                stream.Dispose();
            }
        }

        static void ReadTextFile()
        {
            //read from binary file
            FileStream stream = new FileStream("SampleTextFile.txt", FileMode.Open, FileAccess.Read);
            StreamReader reader = new StreamReader(stream);
            try
            {
                Console.WriteLine(reader.ReadToEnd());
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                reader.Close();
                reader.Dispose();
                stream.Close();
                stream.Dispose();
            }
        }

        static void Main(string[] args)
        {
            //to write in binary file
            //Console.Write("Enter the Input Data: ");
            //string input = Console.ReadLine();
            //WriteBinaryFile(input);

            //to read from binary file
            //ReadBinaryfile();

            //to write in text file
            //Console.Write("Enter the Input Data: ");
            //string input = Console.ReadLine();
            //WriteTextFile(input);

            //to read from text file
            ReadTextFile();
            Console.ReadKey();
        }
    }
}
