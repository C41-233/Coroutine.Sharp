using System;
using System.Collections;
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
            CoroutineManager.StartCoroutine(MyCoroutine());
            while (true)
            {
                TimerManager.Update(DateTime.Now.ToTimeStamp());
                CoroutineManager.OneLoop();
                Thread.Sleep(10);
            }
        }

        private static IEnumerable<IWaitable> MyCoroutine()
        {
            while (true)
            {
                Console.WriteLine($"haha : {DateTime.Now}");
                var promise = new Promise((success, fail) =>
                {
                    TimerManager.StartTimerAfter(1000, () => fail(new Exception()));
                });
                yield return promise;
                Console.WriteLine(promise.Exception);
                yield return WaitFor.Milliseconds(TimerManager, 2000);
                throw new ArgumentException();
            }
        }

    }
}
