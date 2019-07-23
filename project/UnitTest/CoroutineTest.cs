using System;
using System.Collections.Generic;
using Coroutine;
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
                OnUnhandledException = e => throw new Exception(null, e),
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
            CoroutineManager.OnUnhandledException = e =>
            {
                if (e is ArgumentException)
                {
                    i++; 
                }
            };
            Assert.AreEqual(0, i);
            CoroutineManager.StartCoroutine(Run(true));
            Assert.AreEqual(1, i);
            Tick();
            Assert.AreEqual(1, i);

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

            CoroutineManager.OnUnhandledException = e =>
            {
                if (e is ArgumentException)
                {
                    i++;
                }
            };
            CoroutineManager.StartCoroutine(RunFather());

            //自身+1，handle+1
            Assert.AreEqual(2, i);
            Tick();
            Assert.AreEqual(2, i);

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

            CoroutineManager.OnUnhandledException = e =>
            {
                if (e is ArgumentException)
                {
                    i++;
                }
            };
            CoroutineManager.StartCoroutine(RunFather());

            Assert.AreEqual(2, i);
            Tick();

            //handle
            Assert.AreEqual(4, i);

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

            CoroutineManager.OnUnhandledException = e =>
            {
                Assert.Fail();
            };
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
