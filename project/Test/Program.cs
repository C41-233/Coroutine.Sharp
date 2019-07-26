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

        private static int frame = 0;

        static void Main(string[] args)
        {
            //Container.StartCoroutine(RunWait).Then(() => Console.WriteLine("success"));
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

            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine($"{frame}");
                await WaitFor.Yield(2);
            }
        }

        private static IEnumerable Wait2()
        {
            for(int i=0; i<10; i++)
            {
                Console.WriteLine($"{frame}");
                yield return WaitFor.Yield(3);
            }
        }

    }
}
