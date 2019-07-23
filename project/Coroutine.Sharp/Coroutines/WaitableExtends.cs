using System;
using System.Collections.Generic;

namespace Coroutines
{
    public static class WaitableExtends
    {

        public static IWaitable Co(this IWaitable self, out IWaitable waitable)
        {
            waitable = self;
            return self;
        }

        public static IWaitable<T> Co<T>(this IWaitable<T> self, out WaitableValue<T> waitable)
        {
            waitable = new WaitableValue<T>(self);
            return self;
        }

        public static bool IsRunning(this IWaitable self)
        {
            return self.Status == WaitableStatus.Running;
        }

        public static bool IsSuccess(this IWaitable self)
        {
            return self.Status == WaitableStatus.Success;
        }

        public static bool IsFail(this IWaitable self)
        {
            return self.Status == WaitableStatus.Fail;
        }

        public static bool IsAbort(this IWaitable self)
        {
            return self.Status == WaitableStatus.Fail && self.Exception == null;
        }

        public static bool IsFinish(this IWaitable self)
        {
            return self.Status != WaitableStatus.Running;
        }

        public static bool IsError(this IWaitable self)
        {
            return self.Status == WaitableStatus.Fail && self.Exception != null;
        }

        public static IWaitable OnFail(this IWaitable self, Action callback)
        {
            if (callback == null)
            {
                return self;
            }
            return self.OnFail(e => callback());
        }

        public static IWaitable Co(this IEnumerable<IWaitable> enumerable)
        {
            return new Coroutine(enumerable.GetEnumerator());
        }

        public static IWaitable<T> Co<T>(this IEnumerable<IWaitable> enumerable, out WaitableValue<T> result)
        {
            var coroutine = new Coroutine<T>(enumerable);
            result = new WaitableValue<T>(coroutine);
            return coroutine;
        }

    }

    public struct WaitableValue<T>
    {

        private readonly IWaitable<T> waitable;

        public static implicit operator T(WaitableValue<T> waitable)
        {
            return waitable.waitable.R;
        }

        internal WaitableValue(IWaitable<T> waitable)
        {
            this.waitable = waitable;
        }

        public override string ToString()
        {
            return waitable.R?.ToString() ?? "";
        }
    }

}