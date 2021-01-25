using System.Runtime.CompilerServices;
using Coroutines.Waitables.Await;

namespace Coroutines.Waitables
{

    [AsyncMethodBuilder(typeof(AwaitMethodBuilder))]
    public interface IWaitable
    {

    }
}
