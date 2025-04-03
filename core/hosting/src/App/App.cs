using System;
using System.Runtime.InteropServices;
using System.Threading;

public class App
{
    private static byte isWaiting = 0;
    private static int s_CallCount = 0;
    private static ManualResetEvent mre = new ManualResetEvent(false);

    public static void Main(string[] args)
    {
        Console.WriteLine($"{nameof(App)} started - args = [ {string.Join(", ", args)} ]");
        isWaiting = 1;
        mre.WaitOne();
    }

    [UnmanagedCallersOnly]
    public static byte IsWaiting() => isWaiting;

    [UnmanagedCallersOnly]
    public static void Hello(IntPtr message)
    {
        Console.WriteLine($"Hello, world! from {nameof(App)} [count: {++s_CallCount}]");
        Console.WriteLine($"-- message: {Marshal.PtrToStringUTF8(message)}");
        if (s_CallCount >= 3)
        {
            Console.WriteLine("Signaling app to close");
            mre.Set();
        }
    }
}
