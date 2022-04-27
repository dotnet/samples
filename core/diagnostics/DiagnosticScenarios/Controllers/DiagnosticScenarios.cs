using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Threading;

namespace testwebapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiagScenarioController : ControllerBase
    {
        object o1 = new object();
        object o2 = new object();

        private static Processor p = new Processor();

        [HttpGet]
        [Route("deadlock/")]
        public ActionResult<string> deadlock()
        {
            (new System.Threading.Thread(() =>
            {
                DeadlockFunc();
            })).Start();

            Thread.Sleep(5000);

            var threads = new Thread[300];
            for (int i = 0; i < 300; i++)
            {
                (threads[i] = new Thread(() =>
                {
                    lock (o1) { Thread.Sleep(100); }
                })).Start();
            }

            foreach (Thread thread in threads)
            {
                thread.Join();
            }

            return "success:deadlock";
        }

        private void DeadlockFunc()
        {
            lock (o1)
            {
                (new Thread(() =>
                {
                    lock (o2) { Monitor.Enter(o1); }
                })).Start();

                Thread.Sleep(2000);
                Monitor.Enter(o2);
            }
        }

        [HttpGet]
        [Route("memspike/{seconds}")]
        public ActionResult<string> memspike(int seconds)
        {
            var watch = new Stopwatch();
            watch.Start();

            while (true)
            {
                p = new Processor();
                watch.Stop();
                if (watch.ElapsedMilliseconds > seconds * 1000)
                    break;
                watch.Start();

                int it = (2000 * 1000);
                for (int i = 0; i < it; i++)
                {
                    p.ProcessTransaction(new Customer(Guid.NewGuid().ToString()));
                }

                Thread.Sleep(5000); // Sleep for 5 seconds before cleaning up

                // Cleanup
                p = null;

                // GC
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();

                Thread.Sleep(5000); // Sleep for 5 seconds before spiking memory again
            }
            return "success:memspike";
        }

        [HttpGet]
        [Route("memleak/{kb}")]
        public ActionResult<string> memleak(int kb)
        {
            int it = (kb * 1000) / 100;
            for (int i = 0; i < it; i++)
            {
                p.ProcessTransaction(new Customer(Guid.NewGuid().ToString()));
            }

            return "success:memleak";
        }

        [HttpGet]
        [Route("exception")]
        public ActionResult<string> exception()
        {
            throw new Exception("bad, bad code");
        }

        [HttpGet]
        [Route("highcpu/{milliseconds}")]
        public ActionResult<string> highcpu(int milliseconds)
        {
            var watch = new Stopwatch();
            watch.Start();

            while (true)
            {
                watch.Stop();
                if (watch.ElapsedMilliseconds > milliseconds)
                    break;
                watch.Start();
            }

            return "success:highcpu";
        }
    

        [HttpGet]
        [Route("taskwait")]
        public ActionResult<string> TaskWait()
        {
            // Using Task.Wait() or Task.Result causes the current thread to block until the
            // result has been computed. This is the most common cause of threadpool starvation
            // and NOT recommended in your own code unless you know the task is complete and won't
            // need to block.
            Customer c = PretendQueryCustomerFromDbAsync("Dana").Result;
            return "success:taskwait";
        }

        [HttpGet]
        [Route("tasksleepwait")]
        public ActionResult<string> TaskSleepWait()
        {
            // Starting in .NET 6.0 the threadpool can recognize some of the common ways that
            // code blocks on tasks completing and can mitigate it by more quickly
            // scaling up the number of threadpool threads. This example is a less common
            // way to block on a task completing to show what happens when the threadpool
            // doesn't recognize the blocking behavior. This code is NOT recommended.
            Task dbTask = PretendQueryCustomerFromDbAsync("Dana");
            while(!dbTask.IsCompleted)
            {
                Thread.Sleep(10);
            }
            return "success:tasksleepwait";
        }

        [HttpGet]
        [Route("taskasyncwait")]
        public async Task<ActionResult<string>> TaskAsyncWait()
        {
            // Using the await keyword allows the current thread to service other workitems and
            // when the database lookup Task is complete a thread from the threadpool will resume
            // execution here. This way no thread is blocked and large numbers of requests can
            // run in parallel without blocking
            Customer c = await PretendQueryCustomerFromDbAsync("Dana");
            return "success:taskasyncwait";
        }


        async Task<Customer> PretendQueryCustomerFromDbAsync(string customerId)
        {
            // To keep the demo app easy to set up and performing consistently we have replaced a real database query
            // with a fixed delay of 500ms. The impact on application performance should be similar to using a real
            // database that had similar latency.
            await Task.Delay(500);
            return new Customer(customerId);
        }

    }

    class Customer
    {
        private string id;

        public Customer(string id)
        {
            this.id = id;
        }
    }

    class CustomerCache
    {
        private List<Customer> cache = new List<Customer>();

        public void AddCustomer(Customer c)
        {
            cache.Add(c);
        }
    }

    class Processor
    {
        private CustomerCache cache = new CustomerCache();

        public void ProcessTransaction(Customer customer)
        {
            cache.AddCustomer(customer);
        }
    }
}
