using System;
using Coroutines.Base;

namespace Coroutines
{
    public sealed partial class CoroutineManager
    {

        private readonly SwapQueue<Action> actions = new SwapQueue<Action>();

        public void OneLoop()
        {
            actions.Swap();
            while (actions.Count > 0)
            {
                var action = actions.Dequeue;
                try
                {
                    action();
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e);
                }
            }
        }

        internal void Enqueue(Action action)
        {
            actions.Enqueue(action);
        }

        public Container CreateContainer() => new Container(this);

    }
}
