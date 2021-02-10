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
            container.StartCoroutine(OneLoop);
            while (true)
            {
                frame++;
                manager.OneLoop();
                Thread.Sleep(1);
            }
        }

        private static async IWaitable OneLoop()
        {
            throw new Exception();
        }

    }
}
