
namespace Coroutines
{

    public interface IBindCoroutineWaitable : IWaitable
    {
        void Bind(CoroutineManager.Container container);
    }

    public interface IThreadSafeWaitable : IWaitable
    {
    }

    public interface ICompleteCoroutineWaitable : IWaitable
    {

    }

    // ReSharper disable once TypeParameterCanBeVariant
    public interface ICompleteCoroutineWaitable<T> : ICompleteCoroutineWaitable, IWaitable<T>
    {

    }

}
