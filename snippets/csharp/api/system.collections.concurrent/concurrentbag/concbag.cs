//<snippet1>
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

class ConcurrentBagDemo
{
    // Demonstrates:
    //      ConcurrentBag<T>.Add()
    //      ConcurrentBag<T>.IsEmpty
    //      ConcurrentBag<T>.TryTake()
    //      ConcurrentBag<T>.TryPeek()
    static void Main()
    {
        // Add to ConcurrentBag concurrently
        ConcurrentBag<int> cb = new ConcurrentBag<int>();
        List<Task> bagAddTasks = new List<Task>();
        for (int i = 0; i < 500; i++)
        {
            var numberToAdd = i;
            bagAddTasks.Add(Task.Run(() => cb.Add(numberToAdd)));
        }

        // Wait for all tasks to complete
        Task.WaitAll(bagAddTasks.ToArray());

        // Consume the items in the bag
        List<Task> bagConsumeTasks = new List<Task>();
        int itemsInBag = 0;
        while (!cb.IsEmpty)
        {
            bagConsumeTasks.Add(Task.Run(() =>
            {
                int item;
                if (cb.TryTake(out item))
                {
                    Console.WriteLine(item);
                    itemsInBag++;
                }
            }));
        }
        Task.WaitAll(bagConsumeTasks.ToArray());

        Console.WriteLine($"There were {itemsInBag} items in the bag");

        // Checks the bag for an item
        // The bag should be empty and this should not print anything
        int unexpectedItem;
        if (cb.TryPeek(out unexpectedItem))
            Console.WriteLine("Found an item in the bag when it should be empty");
    }
}
//</snippet1>
