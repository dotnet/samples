using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace serialization
{
    class Program
    {
        static void Main(string[] args)
        {
            // <Snippet3?
            const string FileName = @"../../../SavedLoan.bin";
            // </Snippet3>

            // <Snippet2>
            Loan TestLoan = new Loan(10000.0, 0.075, 36, "Neil Black");
            // </Snippet2>

            if (File.Exists(FileName))
            {
                Console.WriteLine("Reading saved file");
                Stream openFileStream = File.OpenRead(FileName);
                BinaryFormatter deserializer = new BinaryFormatter();
                TestLoan = (Loan)deserializer.Deserialize(openFileStream);
                openFileStream.Close();
            }
            TestLoan.PropertyChanged += (_, __) => Console.WriteLine("New customer value");

            TestLoan.Customer = "Henry Clay";
            TestLoan.Term--;
            Console.WriteLine(TestLoan.Term);
            Stream SaveFileStream = File.Create(FileName);
            BinaryFormatter serializer = new BinaryFormatter();
            serializer.Serialize(SaveFileStream, TestLoan);
            SaveFileStream.Close();


        }
    }
}
