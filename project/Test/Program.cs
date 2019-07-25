using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Coroutines;
using Coroutines.Await;
using Coroutines.Timers;

namespace Test
{
    class Program
    {

        private static TimerManager TimerManager = new TimerManager(DateTime.Now);
        private static CoroutineManager CoroutineManager = new CoroutineManager();
        private static CoroutineManager.Container Container = CoroutineManager.CreateContainer();

        static void Main(string[] args)
        {
           Container.StartCoroutine(RunWait);
           Console.WriteLine("MainLoop");
            while (true)
            {
                TimerManager.Update(DateTime.Now);
                CoroutineManager.OneLoop();
                Thread.Sleep(1);
            }
        }

        private static async Coroutine RunWait()
        {
            Console.WriteLine($"Before {Thread.CurrentThread.ManagedThreadId}");
            await WaitFor.PhysicalTimeSpan(TimeSpan.FromSeconds(5));
            Console.WriteLine($"After {Thread.CurrentThread.ManagedThreadId}");
        }

        private static IEnumerable RunYield()
        {
            Console.WriteLine($"Before {Thread.CurrentThread.ManagedThreadId}");
            yield return WaitFor.PhysicalTimeSpan(TimeSpan.FromSeconds(5));
            Console.WriteLine($"After {Thread.CurrentThread.ManagedThreadId}");
        }

    }
}
