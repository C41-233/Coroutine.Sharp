using System;
using System.Collections.Generic;
using System.Threading;
using Coroutine;
using Coroutine.Timer;
using Coroutine.Wait;

namespace Test
{
    class Program
    {

        private static readonly TimerManager TimerManager = new TimerManager(DateTime.Now);
        private static readonly CoroutineManager CoroutineManager = new CoroutineManager();

        static void Main(string[] args)
        {
            var co = CoroutineManager.StartCoroutine(Receive());
            while (true)
            {
                TimerManager.Update(DateTime.Now);
                CoroutineManager.OneLoop();
                Thread.Sleep(10);
            }
        }

        private static IEnumerable<IWaitable> Receive()
        {
            while (true)
            {
                Console.WriteLine($"start {TimerManager.Now}");
                yield return WaitFor.AnySuccess(
                    WaitFor.Milliseconds(TimerManager, 2000),
                    WaitFor.Milliseconds(TimerManager, 100)
                );
                Console.WriteLine($"end {TimerManager.Now}");
            }
        }

    }
}
