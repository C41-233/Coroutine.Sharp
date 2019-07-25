using System;
using System.Collections;
using Coroutines;

namespace IsPrime
{
    public class Program
    {

        public static void Main(string[] args)
        {
            var run = true;
            var coroutineManager = new CoroutineManager();
            var container = coroutineManager.CreateContainer();

            container.StartCoroutine(MainLoop());

            IEnumerable MainLoop()
            {
                yield return IsPrimeAsync(1).With<bool>(out var a);
                Console.WriteLine(a); // true

                yield return IsPrimeAsync(2).With<bool>(out var b);
                Console.WriteLine(b); // true

                yield return IsPrimeAsync(100).With<bool>(out var c);
                Console.WriteLine(c); // false

                run = false;
            }

            while (run)
            {
                coroutineManager.OneLoop();
            }

        }

        private static IEnumerable IsPrimeAsync(int n)
        {
            if(n < 0)
            {
                n = -n;
            }

            if (n == 0)
            {
                yield return false;
            }

            for (int i=3, max=(int)Math.Sqrt(n); i<=max; i+=2)
            {
                if (n % i == 0)
                {
                    yield return false;
                }

                yield return null;
            }

            yield return true;
        }

    }
}
