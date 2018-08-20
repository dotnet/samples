// <Snippet11>
using System;

class Program
{
    static void Main(string[] args)
    {
        object o = null;

        if (o is null)
            Console.WriteLine("'is' constant pattern 'null' check : True");
        
        if (object.ReferenceEquals(o, null))
            Console.WriteLine("object.ReferenceEqual 'null' check : True");

        if (o == null)
            Console.WriteLine("Equality operator (==) 'null' check : True");
    }

    // The example displays the following output:
    //  'is' constant pattern 'null' check : True
    //  object.ReferenceEqual 'null' check : True
    //  Equality operator (==) 'null' check : True
}
// </Snippet11>