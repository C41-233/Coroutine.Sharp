
namespace Coroutines
{
    internal interface IBindCoroutineWaitable : IWaitable
    {

        void Bind(CoroutineManager.Container container);

    }
}
