using System.Collections.Generic;

namespace Coroutine
{
    public class Coroutine
    {

        private readonly CoroutineManager coroutineManager;
        private IEnumerator<Waitable> enumerator;
        private Waitable waitable;

        internal Coroutine(CoroutineManager coroutineManager, IEnumerator<Waitable> co)
        {
            this.coroutineManager = coroutineManager;
            this.enumerator = co;
            Dispatch();
        }

        private void Dispatch()
        {
            if (enumerator.MoveNext())
            {
                Dispatch(enumerator.Current);
            }
            else
            {
                Dispose();
            }
        }

        private void Dispatch(Waitable waitable)
        {
            this.waitable = waitable;
            waitable.BindContext(coroutineManager);

            waitable.OnSuccess(() =>
            {
                this.waitable = null;
                Dispatch();
            });
        }

        private void Dispose()
        {
            waitable = null;
            enumerator.Dispose();
            enumerator = null;
        }

    }

}
