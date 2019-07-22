using System;
using System.Collections.Generic;
using Coroutine;
using Coroutine.Timer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest
{

    [TestClass]
    public class CoroutineTest
    {

        private CoroutineManager CoroutineManager;
        private int Frame;

        [TestInitialize]
        public void Before()
        {
            Frame = 0;
            CoroutineManager = new CoroutineManager
            {
                OnUnhandledException = e => throw new Exception(null, e)
            };
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
            CoroutineManager.StartCoroutine(Run());
            Assert.AreEqual(1, i);
            Tick();
            Assert.AreEqual(1, i);

            IEnumerable<IWaitable> Run()
            {
                i++;
                Assert.AreEqual(0, Frame);
                yield break;
            }
        }

        [TestMethod]
        public void Test2()
        {
            var i = 0;
            CoroutineManager.StartCoroutine(Run());
            Assert.AreEqual(0, i);
            Tick();
            Assert.AreEqual(1, i);

            IEnumerable<IWaitable> Run()
            {
                Assert.AreEqual(0, i);
                Assert.AreEqual(0, Frame);
                yield return null;
                Assert.AreEqual(0, i);
                Assert.AreEqual(1, Frame);
                i++;
            }
        }

        private void Tick()
        {
            for (var i=0; i<1000; i++)
            {
                Frame++;
                CoroutineManager.OneLoop();
            }
        }

    }

}
