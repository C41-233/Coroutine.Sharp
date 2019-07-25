using System.Collections;
using Coroutines;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest
{

    [TestClass]
    public class CoroutineContainerTest
    {

        private CoroutineManager CoroutineManager;
        private CoroutineManager.Container Container;

        [TestInitialize]
        public void Before()
        {
            CoroutineManager = new CoroutineManager();
            Container = CoroutineManager.CreateContainer();
        }

        [TestMethod]
        public void TestClear()
        {
            var i = 0;
            Container.StartCoroutine(Run());
            Container.StartCoroutine(Run());

            Assert.AreEqual(0, i);
            CoroutineManager.OneLoop();
            Assert.AreEqual(2, i);

            Container.Clear();

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
