
using System;

namespace Coroutines.Base
{
    internal static class Assert
    {

        public static void NotNull<T>(T value, string name) where T : class
        {
            if (value == null)
            {
                throw new ArgumentNullException(name);
            }
        }

    }

}
