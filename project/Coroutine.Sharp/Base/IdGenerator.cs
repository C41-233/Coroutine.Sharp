
using System.Threading;

namespace Coroutines.Base
{
    internal static class IdGenerator
    {

        private static int value;

        public static int Next()
        {
            return Interlocked.Increment(ref value);
        }

    }

}
