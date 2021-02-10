using System;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using Coroutines.Base;
using Coroutines.Waitables.Await;

namespace Coroutines.Waitables
{

    [AsyncMethodBuilder(typeof(AwaitMethodBuilder))]
    public interface IWaitable
    {

        WaitableStatus Status { get; }

        Exception Exception { get; }

        IWaitable Then(Action callback);

        IWaitable Catch(Action<Exception> callback);

        void Abort(bool recursive = true);
    }

    [AsyncMethodBuilder(typeof(AwaitMethodBuilder<>))]
    public interface IWaitable<out T> : IWaitable
    {
        T Result { get; }
    }

    public static class WaitableExtends
    {

        public static IWaitable<T> Then<T>(this IWaitable<T> self, Action<T> callback)
        {
            Assert.NotNull(self, nameof(self));
            Assert.NotNull(callback, nameof(callback));

            self.Then(() => callback(self.Result));
            return self;
        }

        public static IWaitable Finally(this IWaitable self, Action callback)
        {
            Assert.NotNull(self, nameof(self));
            Assert.NotNull(callback, nameof(callback));

            self.Then(callback);
            self.Catch(e => callback());
            return self;
        }

        public static bool IsCompleted(this IWaitable self)
        {
            Assert.NotNull(self, nameof(self));

            return self.Status != WaitableStatus.Running;
        }

        public static Exception Throw(this IWaitable self)
        {
            Assert.NotNull(self, nameof(self));

            var exception = self.Exception;
            if (exception != null)
            {
                ExceptionDispatchInfo.Capture(exception).Throw();
            }

            return null;
        }

    }
}
