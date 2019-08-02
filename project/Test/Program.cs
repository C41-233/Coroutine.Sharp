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
            var co = Container.StartCoroutine<int>(Wait3);
            Console.WriteLine(co);
        }

        private static IEnumerable Wait2()
        {
            Console.WriteLine($"{frame}");
            yield return WaitFor.Yield(2);
            Console.WriteLine($"{frame}");
            yield return WaitFor.Yield(2);
            Console.WriteLine($"{frame}");
        }

        private static async IWaitable<int> Wait3()
        {
            Console.WriteLine($"{frame}");
            await WaitFor.Yield();
            Console.WriteLine($"{frame}");
            await WaitFor.Yield();
            Console.WriteLine($"{frame}");
            return 5;
        }

    }
}
