using System;

namespace Coroutines.Await
{

    public static class AwaitExtends
    {

        public static Awaiter GetAwaiter(this IWaitable waitable)
        {
            return new Awaiter(waitable);
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