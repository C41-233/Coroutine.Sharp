﻿using System;
using System.Threading;
using Coroutines;
using Coroutines.Await;
using Coroutines.Timers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest
{

    [TestClass]
    public class AwaitTest
    {
        private TimerManager TimerManager;
        private CoroutineManager CoroutineManager;
        private CoroutineManager.Container CoroutineContainer;

        [TestInitialize]
        public void Before()
        {
            TimerManager = new TimerManager(DateTime.Now);
            CoroutineManager = new CoroutineManager
            {
                DefaultBubbleExceptionApproach = BubbleExceptionApproach.Throw,
            };
            CoroutineContainer = CoroutineManager.CreateContainer();
        }

        [TestCleanup]
        public void After()
        {
            CoroutineManager = null;
        }

        [TestMethod]
        public void Test1()
        {
            var i = 0;
            var thread = Thread.CurrentThread.ManagedThreadId;
            var co = CoroutineContainer.StartCoroutine(Run);
            Assert.AreEqual(0, i);
            Tick();
            Assert.AreEqual(3, i);
            Assert.IsTrue(co.IsSuccess());
            async IWaitable Run()
            {
                i++;
                Assert.AreEqual(thread, Thread.CurrentThread.ManagedThreadId);
                await WaitFor.Seconds(TimerManager, 5);
                i++;
                Assert.AreEqual(thread, Thread.CurrentThread.ManagedThreadId);
                await WaitFor.Seconds(TimerManager, 5);
                i++;
                Assert.AreEqual(thread, Thread.CurrentThread.ManagedThreadId);
            }
        }

        private void Tick()
        {
            for (int i=0; i<5000; i++)
            {
                TimerManager.Update(TimerManager.Now + TimeSpan.FromMilliseconds(100));
                CoroutineManager.OneLoop();
            }
        }

    }
}
