using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    static void Main()
    {
        // Uncomment each of the below four lines one-by-one
        // to test the relevant PLINQ operation

        AsOrdered();
        //WithMergeOptions();
        //WithCancellation();
        //WithDegreeOfParallelism();
    }

    static void AsOrdered()
    {
        #region Sequential
        var items = Enumerable.Range(1, 100);
        var q = from e in items
                where e % 2 == 0 // is even
                select e;

        foreach (var e in q)
        {
            Console.WriteLine(e);
        }
        Console.Write("Complete: Sequential");
        Console.ReadLine();
        #endregion

        #region Parallel
        var items2 = ParallelEnumerable.Range(1, 100);
        q = from e in items2
            where e % 2 == 0 // is even
            select e;

        foreach (var e in q)
        {
            Console.WriteLine(e);
        }
        Console.Write("Complete: Parallel");
        Console.ReadLine();
        #endregion

        #region Parallel with Ordering
        q = from e in items2.AsOrdered()
            where e % 2 == 0 // is even
            select e;

        foreach (var e in q)
        {
            Console.WriteLine(e);
        }

        Console.Write("Complete: Parallel with Ordering");
        Console.ReadLine();
        #endregion
    }

    static void WithMergeOptions()
    {
        #region Define the query
        var items = ParallelEnumerable.Range(1, 1_000);
        var q = from e in items
                select DoWork(e);
        #endregion

        #region Auto Buffered
        foreach (var e in q)
        {
            Console.WriteLine(e);
        }

        Console.Write("Complete: Auto buffered");
        Console.ReadLine();
        #endregion

        #region Fully Buffered
        foreach (var e in q.WithMergeOptions(ParallelMergeOptions.FullyBuffered))
        {
            Console.WriteLine(e);
        }

        Console.Write("Complete: Fully buffered");
        Console.ReadLine();
        #endregion

        #region Not buffered
        foreach (var e in q.WithMergeOptions(ParallelMergeOptions.NotBuffered))
        {
            Console.WriteLine(e);
        }

        Console.Write("Complete: Not buffered");
        Console.ReadLine();
        #endregion
    }

    static void WithCancellation()
    {
        #region Define the query
        var items = ParallelEnumerable.Range(1, 1_000);
        var q = from e in items.WithMergeOptions(ParallelMergeOptions.NotBuffered)
                select DoWork(e);

        var cts = new CancellationTokenSource();

        #endregion

        #region Kick off the asynchronous cancellation
        Task.Factory.StartNew(() =>
        {
            Thread.Sleep(300);
            cts.Cancel();
        });
        #endregion

        #region Enumerate the query
        try
        {
            foreach (var e in q.WithCancellation(cts.Token))
            {
                Console.WriteLine(e);
            }
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Cancelled!");
        }
        Console.ReadLine();
        #endregion
    }

    static void WithDegreeOfParallelism()
    {
        var items = ParallelEnumerable.Range(1, 200);

        #region Default DOP
        var sw = new Stopwatch();
        sw.Start();
        Console.Write("Default DOP: ");

        items.Average(e => DoWork2(e));

        sw.Stop();
        Console.WriteLine(sw.ElapsedMilliseconds);
        Console.WriteLine("Complete: Default DOP");
        #endregion

        #region DOP = 2
        sw.Reset();
        sw.Start();
        Console.Write("DOP = 2: ");

        items.WithDegreeOfParallelism(2)
             .Average(e => DoWork2(e));

        sw.Stop();
        Console.WriteLine(sw.ElapsedMilliseconds);
        Console.Write("Complete: DOP = 2");
        Console.ReadLine();
        #endregion
    }

    #region Helper functions
    static int DoWork(int input)
    {
        Thread.Sleep(20);
        return input * 2;
    }

    static int DoWork2(int input)
    {
        Thread.SpinWait(5_000_000);
        return input * 2;
    }
    #endregion
}
