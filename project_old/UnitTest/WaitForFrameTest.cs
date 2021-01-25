using System.Collections;
using Coroutines;
using Coroutines.Await;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest
{
    [TestClass]
    public class WaitForFrameTest : UnitTestBase
    {

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

        [TestMethod]
        public void TestAwait()
        {
            var i = 0;
            var co = CoroutineContainer.StartCoroutine(Run);
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

            async IWaitable Run()
            {
                i++;
                await WaitFor.Yield();
                i++;
                await WaitFor.Yield();
                i++;
            }
        }

    }
}
