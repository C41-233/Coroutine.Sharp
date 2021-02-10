using System;
using Coroutines.Base;

namespace Coroutines
{
    public sealed partial class CoroutineManager
    {

        private readonly SwapQueue<Action> actions = new SwapQueue<Action>();

        public event Action<Exception> OnException;

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
                    if (OnException == null)
                    {
                        Console.Error.WriteLine(e);
                    }
                    else
                    {
                        OnException?.Invoke(e);
                    }
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
