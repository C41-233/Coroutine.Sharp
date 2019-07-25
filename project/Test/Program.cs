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
           Container.StartCoroutine(RunWait).Then(() => Console.WriteLine("success"));
           Console.WriteLine("MainLoop");
            while (true)
            {
                TimerManager.Update(DateTime.Now);
                CoroutineManager.OneLoop();
                Thread.Sleep(1);
            }
        }

        private static async IWaitable RunWait()
        {
            Console.WriteLine($"1 {Thread.CurrentThread.ManagedThreadId}");
            await WaitFor.Seconds(TimerManager, 2);
            Console.WriteLine($"2 {Thread.CurrentThread.ManagedThreadId}");
            await WaitFor.Seconds(TimerManager, 2);
            Console.WriteLine($"3 {Thread.CurrentThread.ManagedThreadId}");
        }

    }
}
