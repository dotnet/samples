using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace keywords
{
    // <Snippet1>
    class MyClass<T, U>
        where T : class
        where U : struct
    { }
    // </Snippet1>

    // <Snippet2>
    public class MyGenericClass<T> where T : IComparable, new()
    {
        // The following line is not possible without new() constraint:
        T item = new T();
    }
    // </Snippet2

    // <Snippet3>
    interface IMyInterface
    {
    }

    class Dictionary<TKey, TVal>
        where TKey : IComparable, IEnumerable
        where TVal : IMyInterface
    {
        public void Add(TKey key, TVal val)
        {
        }
    }
    // </Snippet3>

    // <Snippet4>
    public class Employee
    {
        public Employee(string s, int i) => (Name, ID) = (s, i);
        public string Name { get; set; }
        public int ID { get; set; }
    }

    public class GenericList<T> where T : Employee
    {
        private class Node
        {
            public Node(T t) => (Next, Data) = (null, t);

            public Node Next { get; set; }
            public T Data { get; set; }
        }

        private Node head;

        public void AddHead(T t)
        {
            Node n = new Node(t) { Next = head };
            head = n;
        }

        public IEnumerator<T> GetEnumerator()
        {
            Node current = head;

            while (current != null)
            {
                yield return current.Data;
                current = current.Next;
            }
        }

        public T FindFirstOccurrence(string s)
        {
            Node current = head;
            T t = null;

            while (current != null)
            {
                //The constraint enables access to the Name property.
                if (current.Data.Name == s)
                {
                    t = current.Data;
                    break;
                }
                else
                {
                    current = current.Next;
                }
            }
            return t;
        }
    }
    // <Snippet4>

    public interface IEmployee
    {

    }

    // <Snippet5>
    class EmployeeList<T> where T : Employee, IEmployee, System.IComparable<T>, new()
    {
        // ...
    }
    // </Snippet5>

    // <Snippet7>
    class Base { }
    class Test<T, U>
        where U : struct
        where T : Base, new()
    { }
    // </Snippet7>

    // <Snippet8>
    public class List<T>
    {
        public void Add<U>(List<U> items) where U : T {/*...*/}
    }
    // </Snippet8>

    // <Snippet9>
    //Type parameter V is used as a type constraint.
    public class SampleClass<T, U, V> where T : V { }
    // </Snippet9>

    public static class GenericWhereConstraints
    {
        public static void Examples()
        {
            TestStringEquality();
        }

        // <Snippet6>
        public static void OpEqualsTest<T>(T s, T t) where T : class
        {
            System.Console.WriteLine(s == t);
        }
        private static void TestStringEquality()
        {
            string s1 = "target";
            System.Text.StringBuilder sb = new System.Text.StringBuilder("target");
            string s2 = sb.ToString();
            OpEqualsTest<string>(s1, s2);
        }
        // </Snippet6>

    }
}
