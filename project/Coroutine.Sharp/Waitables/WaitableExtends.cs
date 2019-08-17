using System;
using System.Collections;
using System.Runtime.ExceptionServices;

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

        public static bool IsAborted(this IWaitable self)
        {
            return self.Status == WaitableStatus.Abort;
        }

        public static bool IsCompleted(this IWaitable self)
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

        public static Exception Throw(this IWaitable self)
        {
            if (self.Exception != null)
            {
                ExceptionDispatchInfo.Capture(self.Exception).Throw();  
            }

            return null;
        }

    }

    internal class WaitableEnumerable<T> : WaitableTask<T>, IBindCoroutineWaitable, IThreadSafeWaitable
    {

        private readonly IEnumerable enumerable;

        public WaitableEnumerable(IEnumerable enumerable)
        {
            this.enumerable = enumerable;
        }

        void IBindCoroutineWaitable.Bind(CoroutineManager.Container coroutineContainer)
        {
            var coroutine = coroutineContainer.StartCoroutine<T>(enumerable);
            coroutine.Then(Success);
            coroutine.Catch(Fail);
        }
    }

}