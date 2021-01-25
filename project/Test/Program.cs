using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Coroutines;
using Coroutines.Waitables;
using Coroutines.Waitables.Await;

namespace Test
{
    public class Program
    {

        private static readonly CoroutineManager manager = new CoroutineManager();
        private static readonly CoroutineManager.Container container = manager.CreateContainer();

        public static void Main(string[] args)
        {
            container.StartCoroutine(Run1);
            while (true)
            {
                manager.OneLoop();
            }
        }

        private static async IWaitable Run1()
        {
            Console.WriteLine($"start {Thread.CurrentThread.ManagedThreadId}");
            await Task.Delay(2000);
            Console.WriteLine($"before {Thread.CurrentThread.ManagedThreadId}");
            await container.StartCoroutine(Run2);
            Console.WriteLine($"after {Thread.CurrentThread.ManagedThreadId}");
        }

        private static async IWaitable Run2()
        {
            while (true)
            {
                Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId}");
                await Task.Delay(2000);
            }
        }

    }
}
