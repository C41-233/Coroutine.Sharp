using System;
using System.Runtime.CompilerServices;

namespace Coroutines.Await
{

    public struct AwaitMethodBuilder
    {

        [ThreadStatic]
        internal static CoroutineManager ThreadLocalCoroutineManager;

        private CoroutineManager coroutineManager;

        public static AwaitMethodBuilder Create()
        {
            Console.WriteLine("CoroutineAwaitMethodBuilder.Create");
            if (ThreadLocalCoroutineManager == null)
            {
                throw new WaitableFlowException("Do not call async coroutine function directly. Use CoroutineManager.Container.StartCoroutine instead.");
            }
            var builder = new AwaitMethodBuilder
            {
                coroutineManager = ThreadLocalCoroutineManager,
            };
            ThreadLocalCoroutineManager = null;
            return builder;
        }

        public void SetResult() => Console.WriteLine("SetResult");

        public void Start<TStateMachine>(ref TStateMachine stateMachine)
            where TStateMachine : IAsyncStateMachine
        {
            Console.WriteLine("Start");
            stateMachine.MoveNext();
        }

        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : INotifyCompletion
            where TStateMachine : IAsyncStateMachine
        {
            Console.WriteLine("AwaitOnCompleted");
            var localCoroutineManager = coroutineManager;
            var s = stateMachine;
            awaiter.OnCompleted(() => localCoroutineManager.Enqueue(() => s.MoveNext()));
        }

        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(
            ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : ICriticalNotifyCompletion
            where TStateMachine : IAsyncStateMachine
        {
            Console.WriteLine($"AwaitUnsafeOnCompleted {awaiter} {stateMachine}");
            var s = stateMachine;
            var localCoroutineManager = coroutineManager;
            awaiter.UnsafeOnCompleted(() => localCoroutineManager.Enqueue(() => s.MoveNext()));
        }

        public void SetException(Exception e)
        {
            Console.WriteLine($"SetException {e}");
        }

        public void SetStateMachine(IAsyncStateMachine stateMachine)
        {
            Console.WriteLine("SetStateMachine");
        }

        public IWaitable Task
        {
            get
            {
                Console.WriteLine($"Task");
                return null;
            }
        }
    }

}
