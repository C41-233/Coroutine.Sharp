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

        private static readonly TimerManager TimerManager = new TimerManager(DateTime.Now.ToTimeStamp());
        private static readonly CoroutineManager CoroutineManager = new CoroutineManager();

        static void Main(string[] args)
        {
            var co = CoroutineManager.StartCoroutine(MyCoroutine());
            int i = 0;
            while (true)
            {
                TimerManager.Update(DateTime.Now.ToTimeStamp());
                CoroutineManager.OneLoop();
                Thread.Sleep(10);
                i++;
                if (i == 500)
                {
                    co.Abort();
                }
            }
        }

        private static IEnumerable<IWaitable> MyCoroutine()
        {
            {
                Console.WriteLine($"haha : {DateTime.Now}");
                yield return CoroutineManager.StartCoroutine(AnotherCoroutine());
                Console.WriteLine("End");
            }
        }

        private static IEnumerable<IWaitable> AnotherCoroutine()
        {
            while (true)
            {
                Console.WriteLine("wait");
                yield return WaitFor.Seconds(TimerManager, 1000);
            }
        }

    }
}
