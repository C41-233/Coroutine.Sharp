using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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

        private static int frame = 0;

        static void Main(string[] args)
        {
            Container.StartCoroutine(Wait1);
            Console.WriteLine("MainLoop");
            while (true)
            {
                TimerManager.Update(DateTime.Now);
                CoroutineManager.OneLoop();
                frame++;
                Thread.Sleep(1);
            }
        }

        private static async IWaitable Wait1()
        {
            Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId}");
            await Task.Delay(1);
            Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId}");
        }

    }
}
