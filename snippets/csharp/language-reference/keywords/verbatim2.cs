// <Snippet1>
using System;

[AttributeUsage(AttributeTargets.Class)]
public class Info : Attribute
{
   private string information;
   
   public Info(string info)
   {
      information = info;
   }
}

[AttributeUsage(AttributeTargets.Method)]
public class InfoAttribute : Attribute
{
   private string information;
   
   public InfoAttribute(string info)
   {
      information = info;
   }
}

[Info("A simple executable.")] // Prepend '@' to 'Info' to avoid compiler error CS1614.
public class Example
{
   [InfoAttribute("The entry point.")]
   public static void Main()
   {
   }
}
// </Snippet1>
