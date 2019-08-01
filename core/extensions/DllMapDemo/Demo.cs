using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

public class DllMapDemo
{
    public static void Main()
    {
        try
        {
            DllMap.Register(Assembly.GetExecutingAssembly());
            int thirty = NativeSum(10, 20);
            Console.WriteLine($"OldLib.NativeSum(10,20) = {thirty}");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error: {e.Message} Line: {e.Source}");
        }
    }

    [DllImport("OldLib")]
    static extern int NativeSum(int arg1, int arg2);
}
