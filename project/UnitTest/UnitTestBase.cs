
using System;
using Coroutines;
using Coroutines.Timers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest
{
    public abstract class UnitTestBase
    {

        protected TimerManager TimerManager;
        protected CoroutineManager CoroutineManager;
        protected CoroutineManager.Container CoroutineContainer;

        [TestInitialize]
        public void Before()
        {
            TimerManager = new TimerManager(DateTime.Now);
            CoroutineManager = new CoroutineManager();
            CoroutineContainer = CoroutineManager.CreateContainer();
        }

        [TestCleanup]
        public void After()
        {
            CoroutineManager = null;
        }

        protected void Tick()
        {
            for (var i = 0; i < 5000; i++)
            {
                TimerManager.Update(TimerManager.Now + TimeSpan.FromMilliseconds(100));
                CoroutineManager.OneLoop();
            }
        }


    }
}
