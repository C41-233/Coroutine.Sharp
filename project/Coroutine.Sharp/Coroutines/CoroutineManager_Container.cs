using System;
using Coroutines.Waitables;
using Coroutines.Waitables.Await;

namespace Coroutines
{

    public sealed partial class CoroutineManager
    {

        public sealed class Container
        {

            public CoroutineManager Manager { get; }

            internal Container(CoroutineManager manager)
            {
                Manager = manager;
            }

            private IWaitable Add(IWaitable waitable)
            {
                return waitable;
            }

            private void PushShareData()
            {
                AwaitShareDataStatic.Share = new AwaitShareData(this);
            }

            private void PopShareData()
            {
                if (AwaitShareDataStatic.Share != null)
                {
                    AwaitShareDataStatic.Share = default;
                    throw WaitableFlowException.NotAsyncMethod();
                }
            }

            #region StartCoroutine
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
            #endregion

        }
    }

}
