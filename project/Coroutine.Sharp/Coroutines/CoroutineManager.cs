using System;
using Coroutines.Base;
using Coroutines.Waitables;
using Coroutines.Waitables.Await;

namespace Coroutines
{
    public sealed class CoroutineManager
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

        public void Enqueue(Action action)
        {
            actions.Enqueue(action);
        }

        #region Container
        public Container CreateContainer() => new Container(this);

        public sealed class Container
        {

            public CoroutineManager Manager { get; }

            public Container(CoroutineManager manager)
            {
                Manager = manager;
            }

            private IWaitable Add(IWaitable waitable)
            {
                return waitable;
            }

            private void PushShareData()
            {
                AwaitShareDataStatic.Share = new AwaitShareData
                {
                    Container = this,
                };
            }

            private void PopShareData()
            {
                if (AwaitShareDataStatic.Share != null)
                {
                    AwaitShareDataStatic.Share = default;
                    throw new WaitableFlowException("StartCoroutine only accept async method");
                }
            }

            public IWaitable StartCoroutine(
                Func<IWaitable> co    
            )
            {
                PushShareData();
                try
                {
                    var coroutine = co();
                    return Add(coroutine);
                }
                finally
                {
                    PopShareData();
                }
            }

        }
        #endregion

    }
}
