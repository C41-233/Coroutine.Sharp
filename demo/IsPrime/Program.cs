using System;
using System.Collections.Generic;
using Coroutines;

namespace IsPrime
{
    public class Program
    {

        private static readonly CoroutineManager CoroutineManager = new CoroutineManager();

        public static void Main(string[] args)
        {
            var run = true;
            CoroutineManager.StartCoroutine(MainLoop());
            while (run)
            {
                CoroutineManager.OneLoop();
            }

            IEnumerable<IWaitable> MainLoop()
            {
                WaitableValue<bool> result;

                yield return CoroutineManager.StartCoroutine<bool>(IsPrimeAsync(1)).Co(out result);
                Console.WriteLine(result);

                yield return CoroutineManager.StartCoroutine<bool>(IsPrimeAsync(2)).Co(out result);
                Console.WriteLine(result);

                yield return CoroutineManager.StartCoroutine<bool>(IsPrimeAsync(100)).Co(out result);
                Console.WriteLine(result);

                run = false;
            }
        }

        private static IEnumerable<IWaitable> IsPrimeAsync(int n)
        {
            if(n < 0)
            {
                n = -n;
            }

            if (n == 0)
            {
                yield return Coroutine<bool>.Complete(false);
            }

            for (int i=3, max=(int)Math.Sqrt(n); i<=max; i+=2)
            {
                if (n % i == 0)
                {
                    yield return Coroutine<bool>.Complete(false);
                }

                yield return null;
            }

            yield return Coroutine<bool>.Complete(true);
        }

    }
}
