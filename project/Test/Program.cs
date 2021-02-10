using System;
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

        private static int frame;

        public static void Main(string[] args)
        {
            container.StartCoroutine(Run1);
            while (true)
            {
                frame++;
                manager.OneLoop();
                Thread.Sleep(1);
            }
        }

        private static async IWaitable Run1()
        {
            Console.WriteLine($"start {Thread.CurrentThread.ManagedThreadId} {frame}");
            await Task.Delay(2000);
            Console.WriteLine($"before {Thread.CurrentThread.ManagedThreadId} {frame}");
            await container.StartCoroutine(Run2);
            Console.WriteLine($"after {Thread.CurrentThread.ManagedThreadId} {frame}");
        }

        private static async IWaitable Run2()
        {
            while (true)
            {
                Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} {frame}");
                await Task.Delay(2000);
            }
        }

    }
}
