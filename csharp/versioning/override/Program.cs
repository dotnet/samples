using System;

class Program
{
    #region sample
    public class MyBaseClass
    {
        public virtual string MethodOne()
        {
            return "Method One";
        }
    }

    public class MyDerivedClass : MyBaseClass
    {
        public override string MethodOne()
        {
            return "Derived Method One";
        }
    }

    public static void Main()
    {
        MyBaseClass b = new MyBaseClass();
        MyDerivedClass d = new MyDerivedClass();
        MyBaseClass bPointd = new MyDerivedClass();

        Console.WriteLine("Base Method One: {0}", b.MethodOne());  //Base Method One: Method One
        Console.WriteLine("Derived Method One: {0}", d.MethodOne()); //Derived Method One: Derived Method One
        
            // public class MyDerivedClass : MyBaseClass
            // {
            //     public /* override */ string MethodOne()
            //     {
            //         return "Derived Method One";
            //     }
            // } 
        // If this method doesn't have the override keyword,
        // Console.WriteLine("Base Ponit Derive:{0}",bPointd.MethodOne()); //Base Ponit Derive:Method One
        Console.WriteLine("Base Ponit Derive:{0}",bPointd.MethodOne()); //Base Ponit Derive:Derived Method One
    }
    #endregion
}
