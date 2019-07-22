using System;
using Coroutine.Timer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest
{
    [TestClass]
    public class TimerTest
    {

        private TimerManager TimerManager;
        private DateTime StartTime;

        [TestInitialize]
        public void Before()
        {
            StartTime = DateTime.Now;
            TimerManager = new TimerManager(StartTime)
            {
                OnUnhandledException = e => throw e
            };
        }

        [TestCleanup]
        public void After()
        {
            TimerManager = null;
        }

        [TestMethod]
        public void Test1()
        {
            TimerManager.StartTimerAfter(10, () =>
            {
                Assert.AreEqual(StartTime.AddMilliseconds(10), TimerManager.Now);
            });
            for (var i=0; i<100; i++)
            {
                TimerManager.Update(TimerManager.Now.AddMilliseconds(10));
            }
            Tick();
        }

        [TestMethod]
        public void Test2()
        {
            TimerManager.StartTimerAfter(15, () =>
            {
                Assert.AreEqual(StartTime.AddMilliseconds(20), TimerManager.Now);
            });
            for (var i = 0; i < 100; i++)
            {
                TimerManager.Update(TimerManager.Now.AddMilliseconds(10));
            }
            Tick();
        }

        [TestMethod]
        public void Test3()
        {
            TimerManager.StartTimerAt(StartTime + TimeSpan.FromMilliseconds(15), () =>
            {
                Assert.AreEqual(StartTime.AddMilliseconds(20), TimerManager.Now);
            });
            Tick();
        }

        private void Tick()
        {
            for (var i = 0; i < 100; i++)
            {
                TimerManager.Update(TimerManager.Now.AddMilliseconds(10));
            }
        }

    }
}
