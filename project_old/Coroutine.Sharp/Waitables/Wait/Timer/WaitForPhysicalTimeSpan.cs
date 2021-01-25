using System;
using System.Threading;

namespace Coroutines
{

    internal sealed class WaitForPhysicalTimeSpan : WaitableTask
    {

        private readonly Timer timer;

        public WaitForPhysicalTimeSpan(TimeSpan timeSpan)
        {
            timer = new Timer(state =>
            {
                Success();
            }, null, timeSpan, TimeSpan.Zero);
        }

        protected override void OnAbort(bool recursive)
        {
            timer.Dispose();
        }
    }

}
