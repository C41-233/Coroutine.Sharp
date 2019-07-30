
namespace Coroutines
{

    public interface IBindCoroutineWaitable : IWaitable
    {
        void Bind(CoroutineManager.Container container);
    }

    public interface IThreadSafeWaitable : IWaitable
    {
    }

}
