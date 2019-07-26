﻿using System;
using System.Collections;
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
        private CoroutineManager.Container CoroutineContainer;
        private int Frame;

        [TestInitialize]
        public void Before()
        {
            Frame = 0;
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
            CoroutineContainer.StartCoroutine(Run());
            Assert.AreEqual(0, i);
            Tick();
            Assert.AreEqual(1, i);

            IEnumerable Run()
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
            var co = CoroutineContainer.StartCoroutine(Run());
            Assert.AreEqual(0, i);
            Tick();
            co.Throw();
            Assert.AreEqual(1, i);

            IEnumerable Run()
            {
                Assert.AreEqual(0, i);
                Assert.AreEqual(1, Frame);
                yield return null;
                Assert.AreEqual(0, i);
                Assert.AreEqual(2, Frame);
                i++;
            }
        }

        [TestMethod]
        public void TestCascade()
        {
            var i = 0;
            CoroutineContainer.StartCoroutine(RunFather());
            Tick();

            IEnumerable RunFather()
            {
                Assert.AreEqual(0, i);
                Assert.AreEqual(0, Frame);
                i++;

                yield return CoroutineContainer.StartCoroutine(RunChild());

                Assert.AreEqual(2, i);
                Assert.AreEqual(2, Frame);
            }

            IEnumerable RunChild()
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
            var co = CoroutineContainer.StartCoroutine(Run(true));
            Assert.AreEqual(0, i);
            Tick();
            Assert.AreEqual(0, i);
            Assert.IsTrue(co.IsError());

            IEnumerable Run(bool t)
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
            var co = CoroutineContainer.StartCoroutine(RunFather());

            Assert.AreEqual(1, i);
            Tick();
            Assert.AreEqual(1, i);
            Assert.IsTrue(co.IsError());

            IEnumerable RunFather()
            {
                Assert.AreEqual(0, i);
                Assert.AreEqual(0, Frame);
                i++;

                yield return CoroutineContainer.StartCoroutine(RunChild());

                Assert.Fail();
            }

            IEnumerable RunChild()
            {
                throw new ArgumentException();
            }
        }

        [TestMethod]
        public void TestCascadeThrow2()
        {
            var i = 0;
            CoroutineContainer.StartCoroutine(RunFather());

            Assert.AreEqual(2, i);
            Tick();

            //handle
            Assert.AreEqual(3, i);

            IEnumerable RunFather()
            {
                Assert.AreEqual(0, i);
                Assert.AreEqual(0, Frame);
                i++;

                yield return CoroutineContainer.StartCoroutine(RunChild());

                Assert.Fail();
            }

            IEnumerable RunChild()
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
            CoroutineContainer.StartCoroutine(RunFather(), BubbleExceptionApproach.Abort);

            Assert.AreEqual(2, i);
            Tick();

            //handle
            Assert.AreEqual(-10, i);

            IEnumerable RunFather()
            {
                Assert.AreEqual(0, i);
                Assert.AreEqual(0, Frame);
                i++;

                yield return CoroutineContainer.StartCoroutine(RunChild()).Catch(e =>
                {
                    i = -10;
                });

                Assert.Fail();
            }

            IEnumerable RunChild()
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

            CoroutineContainer.StartCoroutine(RunFather()).With(out var co);
            Assert.AreEqual(0, i);

            for (var tick=0; tick<1000; tick++)
            {
                CoroutineManager.OneLoop();
                if (i == 10)
                {
                    co.Abort();
                }
            }
            Assert.AreEqual(10, i);

            IEnumerable RunFather()
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
            IWaitable co2 = null;

            var co1 = CoroutineContainer.StartCoroutine(RunFather());

            Assert.AreEqual(0, i);
            CoroutineManager.OneLoop();
            Assert.AreEqual(1, i);
            CoroutineManager.OneLoop();
            Assert.AreEqual(2, i);
            CoroutineManager.OneLoop();
            Assert.AreEqual(3, i);
            co1.Abort();
            CoroutineManager.OneLoop();
            Assert.AreEqual(3, i);

            Tick();
            Assert.AreEqual(3, i);
            Assert.AreEqual(WaitableStatus.Abort, co1.Status);
            Assert.AreEqual(WaitableStatus.Abort, co2.Status);
            Assert.IsTrue(co1.IsAborted());
            Assert.IsTrue(co2.IsAborted());

            IEnumerable RunFather()
            {
                i++;
                co2 = CoroutineContainer.StartCoroutine(RunChild());
                yield return co2;
                i++;
            }

            IEnumerable RunChild()
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

            var co2 = CoroutineContainer.StartCoroutine(RunChild());
            Assert.AreEqual(0, i);

            CoroutineManager.OneLoop();
            Assert.AreEqual(1, i);
            CoroutineManager.OneLoop();
            Assert.AreEqual(2, i);

            var co1 = CoroutineContainer.StartCoroutine(RunFather(), BubbleExceptionApproach.Ignore);
            Assert.AreEqual(2, i);

            CoroutineManager.OneLoop();
            //father++ child++
            Assert.AreEqual(4, i);
            CoroutineManager.OneLoop();
            //father wait child++
            Assert.AreEqual(5, i);

            co2.Abort();
            Assert.AreEqual(5, i);
            Assert.AreEqual(1, j);

            CoroutineManager.OneLoop();
            //father++ child abort
            Assert.IsTrue(co2.IsAborted());
            Assert.AreEqual(6, i);

            Tick();
            Assert.AreEqual(6, i);
            Assert.AreEqual(1, j);

            Assert.AreEqual(WaitableStatus.Abort, co2.Status);
            Assert.AreEqual(WaitableStatus.Success, co1.Status);

            IEnumerable RunFather()
            {
                i++;
                yield return co2.Catch(e => j++);
                i++;
            }

            IEnumerable RunChild()
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

            var co2 = CoroutineContainer.StartCoroutine(RunChild());
            Assert.AreEqual(0, i);

            CoroutineManager.OneLoop();
            Assert.AreEqual(1, i);
            CoroutineManager.OneLoop();
            Assert.AreEqual(2, i);

            var co1 = CoroutineContainer.StartCoroutine(RunFather(), BubbleExceptionApproach.Ignore);
            Assert.AreEqual(2, i);

            CoroutineManager.OneLoop();
            Assert.AreEqual(4, i);

            co1.Abort(false);
            Assert.AreEqual(4, i);

            CoroutineManager.OneLoop();
            Assert.AreEqual(5, i);

            CoroutineManager.OneLoop();
            Assert.AreEqual(6, i);

            IEnumerable RunFather()
            {
                i++;
                yield return co2;
                i++;
            }

            IEnumerable RunChild()
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
