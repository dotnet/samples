//<snippet1>
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

class Example
{
    // Demonstrates:
    //      ConcurrentStack<T>.PushRange();
    //      ConcurrentStack<T>.TryPopRange();
    static void Main()
    {
        int numParallelTasks = 4;
        int numItems = 1000;
        var stack = new ConcurrentStack<int>();

        // Push a range of values onto the stack concurrently
        Task.WaitAll(Enumerable.Range(0, numParallelTasks).Select(i => Task.Factory.StartNew((state) =>
        {
            // state = i * numItems
            int index = (int)state;
            int[] array = new int[numItems];
            for (int j = 0; j < numItems; j++)
            {
                array[j] = index + j;
            }

            Console.WriteLine($"Pushing an array of ints from {array[0]} to {array[numItems - 1]}");
            stack.PushRange(array);
        }, i * numItems, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default)).ToArray());


        int numTotalElements = 4 * numItems;
        int[] resultBuffer = new int[numTotalElements];
        Task.WaitAll(Enumerable.Range(0, numParallelTasks).Select(i => Task.Factory.StartNew(obj =>
        {
            int index = (int)obj;
            int result = stack.TryPopRange(resultBuffer, index, numItems);

            Console.WriteLine($"TryPopRange expected {numItems}, got {result}.");

        }, i * numItems, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default)).ToArray());

        for (int i = 0; i < numParallelTasks; i++)
        {
            // Create a sequence we expect to see from the stack taking the last number of the range we inserted
            var expected = Enumerable.Range(resultBuffer[i*numItems + numItems - 1], numItems);

            // Take the range we inserted, reverse it, and compare to the expected sequence
            var areEqual = expected.SequenceEqual(resultBuffer.Skip(i * numItems).Take(numItems).Reverse());
            if (areEqual)
            {
                Console.WriteLine($"Expected a range of {expected.First()} to {expected.Last()}. Got {resultBuffer[i * numItems + numItems - 1]} to {resultBuffer[i * numItems]}");
            }
            else
            {
                Console.WriteLine($"Unexpected consecutive ranges.");
            }   
        }
    }
}
//</snippet1>

//<snippet2>
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

class Example
{
    // Demonstrates:
    //      ConcurrentStack<T>.Push();
    //      ConcurrentStack<T>.TryPeek();
    //      ConcurrentStack<T>.TryPop();
    //      ConcurrentStack<T>.Clear();
    //      ConcurrentStack<T>.IsEmpty;
    static void Main()
    {
        int items = 10000;

        ConcurrentStack<int> stack = new ConcurrentStack<int>();

        // Create an action to push items onto the stack
        Action pusher = () =>
        {
            for (int i = 0; i < items; i++)
            {
                stack.Push(i);
            }
        };

        // Run the action once
        pusher();

        if (stack.TryPeek(out int result))
        {
            Console.WriteLine($"TryPeek() saw {result} on top of the stack.");
        }
        else
        {
            Console.WriteLine("Could not peek most recently added number.");
        }

        // Empty the stack
        stack.Clear();

        if (stack.IsEmpty)
        {
            Console.WriteLine("Cleared the stack.");
        }

        // Create an action to push and pop items
        Action pushAndPop = () =>
        {
            Console.WriteLine($"Task started on {Task.CurrentId}");

            int item;
            for (int i = 0; i < items; i++)
                stack.Push(i);
            for (int i = 0; i < items; i++)
                stack.TryPop(out item);

            Console.WriteLine($"Task ended on {Task.CurrentId}");
        };

        // Spin up five concurrent tasks of the action
        var tasks = new Task[5];
        for (int i = 0; i < tasks.Length; i++)
            tasks[i] = Task.Factory.StartNew(pushAndPop);

        // Wait for all the tasks to finish up
        Task.WaitAll(tasks);

        if (!stack.IsEmpty)
        {
            Console.WriteLine("Did not take all the items off the stack");
        }
    }
}
//</snippet2>