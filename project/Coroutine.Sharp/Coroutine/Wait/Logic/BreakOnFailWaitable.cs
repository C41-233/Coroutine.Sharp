namespace Coroutine.Wait
{

    internal class BreakOnFailWaitable : FilterWaitable
    {

        public BreakOnFailWaitable(IWaitable waitable) : base(waitable)
        {
        }

    }

}
