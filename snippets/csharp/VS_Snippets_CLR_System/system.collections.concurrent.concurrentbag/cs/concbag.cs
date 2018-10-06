//<snippet1>
using System;
using System.Collections.Concurrent;
using System.Threading;

class ConcurrentBagDemo
{
    // Demonstrates:
    //      ConcurrentBag<T>.Add()
    //      ConcurrentBag<T>.IsEmpty
    //      ConcurrentBag<T>.TryTake()
    //      ConcurrentBag<T>.TryPeek()
    static void Main()
    {
        // Construct and populate the ConcurrentBag
        ConcurrentBag<int> cb = new ConcurrentBag<int>();

        // Add to ConcurrentBag from two different threads
        Thread addOne = new Thread(() => cb.Add(1));
        Thread addTwo = new Thread(() => cb.Add(2));
        addOne.Start();
        addTwo.Start();

        // Wait for both threads to complete
        addOne.Join();
        addTwo.Join();

        // Consume the items in the bag
        int itemsInBag = 0;
        int item;
        while (!cb.IsEmpty)
        {
            if (cb.TryTake(out item))
            {
                Console.WriteLine(item);
                itemsInBag++;
            }
        }

        Console.WriteLine($"There were {itemsInBag} items in the bag");

        // Checks the bag for an item
        // The bag should be empty and this should not print anything
        if (cb.TryPeek(out item))
            Console.WriteLine("Found an item in the bag when it should be empty");

    }

}
//</snippet1>
