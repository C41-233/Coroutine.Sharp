using System.Collections;
using System.Collections.Generic;
using Coroutines;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest
{
    [TestClass]
    public class WaitForFrameTest
    {

        private CoroutineManager CoroutineManager;
        private CoroutineManager.Container CoroutineContainer;

        [TestInitialize]
        public void Before()
        {
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
        public void TestYield()
        {
            var i = 0;
            var co = CoroutineContainer.StartCoroutine(Run());
            Assert.AreEqual(0, i);
            Assert.IsTrue(co.IsRunning());

            CoroutineManager.OneLoop();
            Assert.AreEqual(1, i);
            CoroutineManager.OneLoop();
            Assert.AreEqual(2, i);
            CoroutineManager.OneLoop();
            Assert.AreEqual(3, i);
            CoroutineManager.OneLoop();
            Assert.AreEqual(3, i);
            CoroutineManager.OneLoop();
            Assert.AreEqual(3, i);

            Assert.IsTrue(co.IsSuccess());

            IEnumerable Run()
            {
                i++;
                yield return WaitFor.Yield();
                i++;
                yield return WaitFor.Yield();
                i++;
            }
        }

    }
}
