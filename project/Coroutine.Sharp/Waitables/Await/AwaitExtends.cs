using System;

namespace Coroutines.Await
{

    public static class AwaitExtends
    {

        public static Awaiter GetAwaiter(this IWaitable waitable)
        {
            Console.WriteLine($"GetAwaiter {waitable}");
            return new Awaiter(waitable);
        }

        public static CoroutineAwaiter GetAwaiter(this Coroutine waitable)
        {
            Console.WriteLine($"GetAwaiter {waitable}");
            return new CoroutineAwaiter(waitable);
        }
    }

}

namespace System.Runtime.CompilerServices
{
    internal sealed class AsyncMethodBuilderAttribute : Attribute
    {
        public AsyncMethodBuilderAttribute(Type type)
        { }
    }
}