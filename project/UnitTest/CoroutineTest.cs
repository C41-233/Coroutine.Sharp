using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Coroutines;
using Microsoft.VisualStudio.TestTools.UnitTesting;
// ReSharper disable ImplicitlyCapturedClosure

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
                DefaultBubbleExceptionApproach = BubbleExceptionApproach.Throw,
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

        [TestMethod]
        public void TestCascade()
        {
            var i = 0;
            CoroutineManager.StartCoroutine(RunFather());
            Tick();

            IEnumerable<IWaitable> RunFather()
            {
                Assert.AreEqual(0, i);
                Assert.AreEqual(0, Frame);
                i++;

                yield return CoroutineManager.StartCoroutine(RunChild());

                Assert.AreEqual(2, i);
                Assert.AreEqual(2, Frame);
            }

            IEnumerable<IWaitable> RunChild()
            {
                Assert.AreEqual(1, i);
                Assert.AreEqual(0, Frame);
                yield return null;
                i++;
                Assert.AreEqual(2, i);
                Assert.AreEqual(1, Frame);
            }
        }

        [TestMethod]
        public void TestThrow()
        {
            var i = 0;
            Assert.AreEqual(0, i);
            var co = CoroutineManager.StartCoroutine(Run(true));
            Assert.AreEqual(0, i);
            Tick();
            Assert.AreEqual(0, i);
            Assert.IsTrue(co.IsError());

            IEnumerable<IWaitable> Run(bool t)
            {
                if (t)
                {
                    throw new ArgumentException();
                }
                i++;
                yield return null;
                i++;
            }
        }

        [TestMethod]
        public void TestCascadeThrow1()
        {
            var i = 0;
            var co = CoroutineManager.StartCoroutine(RunFather());

            Assert.AreEqual(1, i);
            Tick();
            Assert.AreEqual(1, i);
            Assert.IsTrue(co.IsFail());

            IEnumerable<IWaitable> RunFather()
            {
                Assert.AreEqual(0, i);
                Assert.AreEqual(0, Frame);
                i++;

                yield return CoroutineManager.StartCoroutine(RunChild());

                Assert.Fail();
            }

            IEnumerable<IWaitable> RunChild()
            {
                throw new ArgumentException();
            }
        }

        [TestMethod]
        public void TestCascadeThrow2()
        {
            var i = 0;
            CoroutineManager.StartCoroutine(RunFather());

            Assert.AreEqual(2, i);
            Tick();

            //handle
            Assert.AreEqual(3, i);

            IEnumerable<IWaitable> RunFather()
            {
                Assert.AreEqual(0, i);
                Assert.AreEqual(0, Frame);
                i++;

                yield return CoroutineManager.StartCoroutine(RunChild());

                Assert.Fail();
            }

            IEnumerable<IWaitable> RunChild()
            {
                i++;
                yield return null;
                i++;
                throw new ArgumentException();
            }
        }

        [TestMethod]
        public void TestCascadeThrow3()
        {
            var i = 0;
            CoroutineManager.StartCoroutine(RunFather(), BubbleExceptionApproach.Abort);

            Assert.AreEqual(2, i);
            Tick();

            //handle
            Assert.AreEqual(-10, i);

            IEnumerable<IWaitable> RunFather()
            {
                Assert.AreEqual(0, i);
                Assert.AreEqual(0, Frame);
                i++;

                yield return CoroutineManager.StartCoroutine(RunChild()).OnFail(e =>
                {
                    i = -10;
                });

                Assert.Fail();
            }

            IEnumerable<IWaitable> RunChild()
            {
                i++;
                yield return null;
                i++;
                throw new ArgumentException();
            }
        }

        [TestMethod]
        public void TestAbort1()
        {
            var i = 0;

            CoroutineManager.StartCoroutine(RunFather()).With(out var co);
            Assert.AreEqual(1, i);

            for (var tick=0; tick<1000; tick++)
            {
                CoroutineManager.OneLoop();
                if (i == 10)
                {
                    co.Abort();
                }
            }
            Assert.AreEqual(10, i);

            IEnumerable<IWaitable> RunFather()
            {
                while (true)
                {
                    i++;
                    yield return null;
                }
            }
        }

        [TestMethod]
        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        public void TestAbort2()
        {
            var i = 0;
            Coroutine co2 = null;

            var co1 = CoroutineManager.StartCoroutine(RunFather());

            Assert.AreEqual(2, i);
            CoroutineManager.OneLoop();
            Assert.AreEqual(3, i);
            CoroutineManager.OneLoop();
            Assert.AreEqual(4, i);
            co1.Abort();
            CoroutineManager.OneLoop();
            Assert.AreEqual(4, i);

            Tick();
            Assert.AreEqual(4, i);
            Assert.AreEqual(WaitableStatus.Fail, co1.Status);
            Assert.AreEqual(WaitableStatus.Fail, co2.Status);
            Assert.IsTrue(co1.IsAbort());
            Assert.IsTrue(co2.IsAbort());

            IEnumerable<IWaitable> RunFather()
            {
                i++;
                co2 = CoroutineManager.StartCoroutine(RunChild());
                yield return co2;
                i++;
            }

            IEnumerable<IWaitable> RunChild()
            {
                while (true)
                {
                    i++;
                    yield return null;
                }
            }
        }

        [TestMethod]
        public void TestAbort3()
        {
            var i = 0;
            var j = 0;

            var co2 = CoroutineManager.StartCoroutine(RunChild());
            Assert.AreEqual(1, i);

            CoroutineManager.OneLoop();
            Assert.AreEqual(2, i);

            var co1 = CoroutineManager.StartCoroutine(RunFather(), BubbleExceptionApproach.Ignore);
            Assert.AreEqual(3, i);

            CoroutineManager.OneLoop();
            Assert.AreEqual(4, i);

            co2.Abort();
            Assert.AreEqual(4, i);
            Assert.AreEqual(1, j);

            CoroutineManager.OneLoop();
            Assert.AreEqual(5, i);

            Tick();
            Assert.AreEqual(5, i);
            Assert.AreEqual(1, j);

            Assert.AreEqual(WaitableStatus.Fail, co2.Status);
            Assert.AreEqual(WaitableStatus.Success, co1.Status);

            IEnumerable<IWaitable> RunFather()
            {
                i++;
                yield return co2.OnFail(e => j++);
                i++;
            }

            IEnumerable<IWaitable> RunChild()
            {
                while (true)
                {
                    i++;
                    yield return null;
                }
            }
        }

        [TestMethod]
        public void TestAbort4()
        {
            var i = 0;

            var co2 = CoroutineManager.StartCoroutine(RunChild());
            Assert.AreEqual(1, i);

            CoroutineManager.OneLoop();
            Assert.AreEqual(2, i);

            var co1 = CoroutineManager.StartCoroutine(RunFather(), BubbleExceptionApproach.Ignore);
            Assert.AreEqual(3, i);

            CoroutineManager.OneLoop();
            Assert.AreEqual(4, i);

            co1.Abort(false);
            Assert.AreEqual(4, i);

            CoroutineManager.OneLoop();
            Assert.AreEqual(5, i);

            CoroutineManager.OneLoop();
            Assert.AreEqual(6, i);

            IEnumerable<IWaitable> RunFather()
            {
                i++;
                yield return co2;
                i++;
            }

            IEnumerable<IWaitable> RunChild()
            {
                while (true)
                {
                    i++;
                    yield return null;
                }
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
