namespace Coroutine.Wait
{
    internal class PreventAbortWaitable : FilterWaitable
    {

        public PreventAbortWaitable(IWaitable waitable) : base(waitable)
        {
        }

        public override void Abort()
        {
        }

    }
}
