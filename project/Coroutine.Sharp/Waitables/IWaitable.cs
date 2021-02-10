using System;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
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

    }

    public static class WaitableExtends
    {

        public static IWaitable Finally(this IWaitable self, Action action)
        {
            self.Then(action);
            self.Catch(e => action());
            return self;
        }

        public static bool IsCompleted(this IWaitable self)
        {
            return self.Status != WaitableStatus.Running;
        }

        public static Exception Throw(this IWaitable self)
        {
            var exception = self.Exception;
            if (exception != null)
            {
                ExceptionDispatchInfo.Capture(exception).Throw();
            }

            return null;
        }

    }
}
