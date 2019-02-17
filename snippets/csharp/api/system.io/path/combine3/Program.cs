using System;
using System.IO;

class Program
{
    static void Main()
    {
        string path1 = "C:/";
        string path2 = "users/user1/documents";
        string path3 = "letters";
        ShowPathInformation(path1, path2, path3);
        
        path1 = "D:/";
        path2 =  "/users/user1/documents";
        path3 = "letters";
        ShowPathInformation(path1, path2, path3);

        path1 = "D:/";
        path2 =  "users/user1/documents";
        path3 = "C:/users/user1/documents/data";
        ShowPathInformation(path1, path2, path3);
    }

   private static void ShowPathInformation(string path1, string path2, string path3)
    {
        var result = Path.Join(path1.AsSpan(), path2.AsSpan(), path3.AsSpan());
        Console.WriteLine($"Concatenating  '{path1}, '{path2}', and `{path3}'");
        Console.WriteLine($"   Path.Join:     '{result}'");
        Console.WriteLine($"   Path.Combine:  '{Path.Combine(path1, path2, path3)}'");
Console.WriteLine($"   {Path.GetFullPath(result)}");
    }
}
// The example displays the following output if run on a Windows system:
//   Concatenating  'C:/, 'users/user1/documents', and `letters'
//      Path.Join:     'C:/users/user1/documents\letters'
//      Path.Combine:  'C:/users/user1/documents\letters'
//
//   Concatenating  'D:/, '/users/user1/documents', and `letters'
//      Path.Join:     'D://users/user1/documents\letters'
//      Path.Combine:  '/users/user1/documents\letters'
//
//   Concatenating  'D:/, 'users/user1/documents', and `C:/users/user1/documents/data'
//      Path.Join:     'D:/users/user1/documents\C:/users/user1/documents/data'
//      Path.Combine:  'C:/users/user1/documents/data'
