﻿using System;
using System.Collections.Generic;
using Coroutines;

namespace IsPrime
{
    public class Program
    {

        public static void Main(string[] args)
        {
            var run = true;
            var coroutineManager = new CoroutineManager();
            coroutineManager.StartCoroutine(MainLoop());

            IEnumerable<IWaitable> MainLoop()
            {
                WaitableValue<bool> result;

                yield return coroutineManager.StartCoroutine<bool>(IsPrimeAsync(1)).Co(out result);
                Console.WriteLine(result); // true

                yield return coroutineManager.StartCoroutine<bool>(IsPrimeAsync(2)).Co(out result);
                Console.WriteLine(result); // true

                yield return coroutineManager.StartCoroutine<bool>(IsPrimeAsync(100)).Co(out result);
                Console.WriteLine(result); // false

                run = false;
            }

            while (run)
            {
                coroutineManager.OneLoop();
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
                yield return Coroutine.Complete(false);
            }

            for (int i=3, max=(int)Math.Sqrt(n); i<=max; i+=2)
            {
                if (n % i == 0)
                {
                    yield return Coroutine.Complete(false);
                }

                yield return null;
            }

            yield return Coroutine.Complete(true);
        }

    }
}
