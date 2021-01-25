using System.Collections;
using Coroutines;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest
{

    [TestClass]
    public class CoroutineContainerTest : UnitTestBase
    {

        [TestMethod]
        public void TestClear()
        {
            var i = 0;
            CoroutineContainer.StartCoroutine(Run());
            CoroutineContainer.StartCoroutine(Run());

            Assert.AreEqual(0, i);
            CoroutineManager.OneLoop();
            Assert.AreEqual(0, i);
            CoroutineManager.OneLoop();
            Assert.AreEqual(2, i);

            CoroutineContainer.ClearAllCoroutines();

            CoroutineManager.OneLoop();
            Assert.AreEqual(2, i);

            IEnumerable Run()
            {
                while (true)
                {
                    yield return null;
                    i++;
                }
            }
        }

    }

}
