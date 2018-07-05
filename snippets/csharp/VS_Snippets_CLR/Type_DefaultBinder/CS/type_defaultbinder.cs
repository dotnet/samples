// <Snippet1>
using System;
using System.Reflection;

public class MyDefaultBinderSample
{
    public static void Main()
    {
            Binder defaultBinder = Type.DefaultBinder;
            MyClass myClass = new MyClass();
            // Invoke the HelloWorld method of MyClass.
            myClass.GetType().InvokeMember("HelloWorld", BindingFlags.InvokeMethod,
                defaultBinder, myClass, new object [] {});
    }	

    class MyClass
    {
        public void HelloWorld()
        {
            Console.WriteLine("Hello World");
        }	
    }
}
// </Snippet1>
