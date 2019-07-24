using System;
using System.Collections;

namespace Coroutines
{
    public static class WaitableExtends
    {

        public static IWaitable With(this IWaitable self, out IWaitable waitable)
        {
            waitable = self;
            return self;
        }

        public static IWaitable<T> With<T>(this IWaitable<T> self, out WaitableValue<T> value)
        {
            value = new WaitableValue<T>(self);
            return self;
        }

        public static IWaitable<T> With<T>(this IEnumerable self, out WaitableValue<T> value)
        {
            var waitable = new WaitableEnumerable<T>(self);
            value = new WaitableValue<T>(waitable);
            return waitable;
        }

        public static bool IsRunning(this IWaitable self)
        {
            return self.Status == WaitableStatus.Running;
        }

        public static bool IsSuccess(this IWaitable self)
        {
            return self.Status == WaitableStatus.Success;
        }

        public static bool IsAbort(this IWaitable self)
        {
            return self.Status == WaitableStatus.Abort;
        }

        public static bool IsFinish(this IWaitable self)
        {
            return self.Status != WaitableStatus.Running;
        }

        public static bool IsError(this IWaitable self)
        {
            return self.Status == WaitableStatus.Error;
        }

        public static IWaitable Catch(this IWaitable self, Action callback)
        {
            if (callback == null)
            {
                return self;
            }
            return self.Catch(e => callback());
        }

        public static IWaitable Finally(this IWaitable self, Action callback)
        {
            if (callback == null)
            {
                return self;
            }

            self.Then(callback);
            self.Catch(callback);
            return self;
        }

    }

    public struct WaitableValue<T> : IWaitable<T>
    {

        private readonly IWaitable<T> waitable;
        public T R => waitable.R;

        internal WaitableValue(IWaitable<T> waitable)
        {
            this.waitable = waitable;
        }

        public static implicit operator T(WaitableValue<T> self)
        {
            return self.R;
        }

        public WaitableStatus Status => waitable.Status;
        public Exception Exception => waitable.Exception;

        public IWaitable Then(Action callback)
        {
            return waitable.Then(callback);
        }

        public IWaitable Catch(Action<Exception> callback)
        {
            return waitable.Catch(callback);
        }

        public void Abort(bool recursive = true)
        {
            waitable.Abort(recursive);
        }

        public IWaitable<T> Then(Action<T> callback)
        {
            return waitable.Then(callback);
        }

        public override string ToString()
        {
            if (waitable.Status == WaitableStatus.Success)
            {
                return waitable.R?.ToString() ?? "";
            }

            return "";
        }
    }

    internal interface IWaitableEnumerable : IWaitable
    {

        void Bind(CoroutineManager coroutineManager);

    }

    internal class WaitableEnumerable<T> : WaitableTask<T>, IWaitableEnumerable
    {

        private readonly IEnumerable enumerable;

        public WaitableEnumerable(IEnumerable enumerable)
        {
            this.enumerable = enumerable;
        }

        public void Bind(CoroutineManager coroutineManager)
        {
            var coroutine = coroutineManager.StartCoroutine<T>(enumerable);
            coroutine.Then(Success);
            coroutine.Catch(Fail);
        }
    }

}