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
            var co = container.StartCoroutine(Run1);
            container.StartCoroutine(Run2, co);
            container.StartCoroutine(Run3, co);
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
                Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} {DateTime.Now}");
                await Task.Delay(1000);
                throw new Exception("!");
            }
        }

        private static async IWaitable Run2(IWaitable other)
        {
            await Task.Delay(5000);
            //Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} stop!! {container.Count}");
            //other.Abort();
            //Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} abort!! {container.Count}");
        }

        private static async IWaitable Run3(IWaitable other)
        {
            Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} before await {container.Count}");
            await other;
            Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} after await {container.Count}");
        }

    }
}
