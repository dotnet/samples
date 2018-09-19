using System;
using System.Security.Cryptography;
using System.Text;

class Program
{
    static void Main()
    {
        string source = "Hello World!";
        using (SHA256 sha256Hash = SHA256Managed.Create())
        {
            string hash = GetSHA256Hash(sha256Hash, source);

            Console.WriteLine($"The SHA256 hash of {source} is: {hash}.");

            Console.WriteLine("Verifying the hash...");

            if (VerifySHA256Hash(sha256Hash, source, hash))
            {
                Console.WriteLine("The hashes are the same.");
            }
            else
            {
                Console.WriteLine("The hashes are not same.");
            }
        }
    }

    static string GetSHA256Hash(SHA256 sha256Hash, string input)
    {

        // Convert the input string to a byte array and compute the hash.
        var data = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

        // Create a new Stringbuilder to collect the bytes
        // and create a string.
        var sBuilder = new StringBuilder();

        // Loop through each byte of the hashed data 
        // and format each one as a hexadecimal string.
        for (int i = 0; i < data.Length; i++)
        {
            sBuilder.Append(data[i].ToString("x2"));
        }

        // Return the hexadecimal string.
        return sBuilder.ToString();
    }

    // Verify a hash against a string.
    static bool VerifySHA256Hash(SHA256 sha256Hash, string input, string hash)
    {
        // Hash the input.
        var hashOfInput = GetSHA256Hash(sha256Hash, input);

        // Create a StringComparer an compare the hashes.
        var comparer = StringComparer.OrdinalIgnoreCase;

        return comparer.Compare(hashOfInput, hash) == 0;
    }

}
// The example displays the following output:
//    The SHA256 hash of Hello World! is: 7f83b1657ff1fc53b92dc18148a1d65dfc2d4b1fa3d677284addd200126d9069.
//    Verifying the hash...
//    The hashes are the same.
