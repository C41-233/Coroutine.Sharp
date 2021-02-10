using System;
using System.Threading;
using System.Threading.Tasks;
using Coroutines;
using Coroutines.Waitables;

namespace Test
{
    public class Program
    {

        private static readonly CoroutineManager manager = new CoroutineManager();
        private static readonly CoroutineManager.Container container = manager.CreateContainer();

        private static int frame;

        public static void Main(string[] args)
        {
            var co = container.StartCoroutine(Run1);
            container.StartCoroutine(Run2, co);
            while (true)
            {
                frame++;
                manager.OneLoop();
                Thread.Sleep(1);
            }
        }

        private static async IWaitable Run1()
        {
            while (true)
            {
                Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId}");
                await Task.Delay(1000);
            }
        }

        private static async IWaitable Run2(IWaitable other)
        {
            await Task.Delay(5000);
            Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} stop!!");
            other.Abort();
        }

    }
}
