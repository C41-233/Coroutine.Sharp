using System;
using System.Runtime.CompilerServices;

namespace Coroutines.Await
{

    public struct CoroutineAwaitMethodBuilder
    {

        public static CoroutineAwaitMethodBuilder Create()
        {
            Console.WriteLine("CoroutineAwaitMethodBuilder.Create");
            return new CoroutineAwaitMethodBuilder();
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
            var s = stateMachine;
            awaiter.OnCompleted(() => s.MoveNext());
        }

        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(
            ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : ICriticalNotifyCompletion
            where TStateMachine : IAsyncStateMachine
        {
            Console.WriteLine($"AwaitUnsafeOnCompleted {awaiter} {stateMachine}");
            var s = stateMachine;
            awaiter.UnsafeOnCompleted(() => s.MoveNext());
        }

        public void SetException(Exception e)
        {
            Console.WriteLine($"SetException {e}");
        }

        public void SetStateMachine(IAsyncStateMachine stateMachine)
        {
            Console.WriteLine("SetStateMachine");
        }

        public Coroutine Task
        {
            get
            {
                Console.WriteLine($"Task");
                return null;
            }
        }
    }

}
