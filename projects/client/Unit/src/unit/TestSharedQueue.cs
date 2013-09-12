// This source code is dual-licensed under the Apache License, version
// 2.0, and the Mozilla Public License, version 1.1.
//
// The APL v2.0:
//
//---------------------------------------------------------------------------
//   Copyright (C) 2007-2013 VMware, Inc.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
//---------------------------------------------------------------------------
//
// The MPL v1.1:
//
//---------------------------------------------------------------------------
//  The contents of this file are subject to the Mozilla Public License
//  Version 1.1 (the "License"); you may not use this file except in
//  compliance with the License. You may obtain a copy of the License
//  at http://www.mozilla.org/MPL/
//
//  Software distributed under the License is distributed on an "AS IS"
//  basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. See
//  the License for the specific language governing rights and
//  limitations under the License.
//
//  The Original Code is RabbitMQ.
//
//  The Initial Developer of the Original Code is VMware, Inc.
//  Copyright (c) 2007-2013 VMware, Inc.  All rights reserved.
//---------------------------------------------------------------------------

using NUnit.Framework;

using System;
using System.IO;
using System.Collections;
using System.Threading;

using RabbitMQ.Util;

namespace RabbitMQ.Client.Unit
{

    [TestFixture]
    public class TestSharedQueue : TimingFixture
    {

        public delegate void Thunk();

        //wrapper to work around C#'s lack of local volatiles
        public class VolatileInt {
            public volatile int v = 0;
        }

        public class DelayedEnqueuer<T>
        {
            public SharedQueue<T> m_q;
            public int m_delayMs;
            public T m_v;
            public DelayedEnqueuer(SharedQueue<T> q, int delayMs, T v)
            {
                m_q = q;
                m_delayMs = delayMs;
                m_v = v;
            }
            public void EnqueueValue()
            {
                Thread.Sleep(m_delayMs);
                m_q.Enqueue(m_v);
            }
            public void Dequeue()
            {
                m_q.Dequeue();
            }
            public void DequeueNoWaitZero()
            {
                m_q.DequeueNoWait(default(T));
            }
            public void DequeueAfterOneIntoV()
            {
                m_q.Dequeue(TimeSpan.FromMilliseconds(1), out m_v);
            }
            public void BackgroundEofExpectingDequeue()
            {
                ExpectEof(new Thunk(this.Dequeue));
            }
        }

        public static void EnqueueAfter<T>(TimeSpan delay, SharedQueue<T> q, T v)
        {
            DelayedEnqueuer<T> de = new DelayedEnqueuer<T>(q, (int) delay.TotalMilliseconds, v);
            new Thread(new ThreadStart(de.EnqueueValue)).Start();
        }

        public static void ExpectEof(Thunk thunk)
        {
            try
            {
                thunk();
                Assert.Fail("expected System.IO.EndOfStreamException");
            }
            catch (System.IO.EndOfStreamException) { }
        }

        public DateTime m_startTime;

        public void ResetTimer()
        {
            m_startTime = DateTime.Now;
        }

        public int ElapsedMs()
        {
            return (int)((DateTime.Now - m_startTime).TotalMilliseconds);
        }

        [Test]
        public void TestDequeueNoWait1()
        {
            SharedQueue<int> q = new SharedQueue<int>();
            q.Enqueue(1);
            Assert.AreEqual(1, q.DequeueNoWait(0));
            Assert.AreEqual(0, q.DequeueNoWait(0));
        }

        [Test]
        public void TestDequeueNoWait2()
        {
            SharedQueue<int> q = new SharedQueue<int>();
            q.Enqueue(1);
            Assert.AreEqual(1, q.Dequeue());
            Assert.AreEqual(0, q.DequeueNoWait(0));
        }

        [Test]
        public void TestDequeueNoWait3()
        {
            SharedQueue<object> q = new SharedQueue<object>();
            Assert.AreEqual(null, q.DequeueNoWait(null));
        }

        [Test]
        public void TestDelayedEnqueue()
        {
            SharedQueue<int> q = new SharedQueue<int>();
            ResetTimer();
            EnqueueAfter(TimingInterval, q, 1);
            Assert.AreEqual(0, q.DequeueNoWait(0));
            Assert.Greater(TimingInterval - SafetyMargin, ElapsedMs());
            Assert.AreEqual(1, q.Dequeue());
            Assert.Less(TimingInterval - SafetyMargin, ElapsedMs());
        }

        [Test]
        public void TestTimeoutShort()
        {
            SharedQueue<int> q = new SharedQueue<int>();
            q.Enqueue(123);

            ResetTimer();
            int v;
            bool r = q.Dequeue(TimingInterval, out v);
            Assert.Greater(SafetyMargin, ElapsedMs());
            Assert.IsTrue(r);
            Assert.AreEqual(123, v);
        }

        [Test]
        public void TestTimeoutLong()
        {
            SharedQueue<int> q = new SharedQueue<int>();

            ResetTimer();
            int v;
            bool r = q.Dequeue(TimingInterval, out v);
            Assert.Less(TimingInterval - SafetyMargin, ElapsedMs());
            Assert.IsTrue(!r);
            Assert.AreEqual(null, v);
        }

        [Test]
        public void TestTimeoutNegative()
        {
            SharedQueue<int> q = new SharedQueue<int>();

            ResetTimer();
            int v;
            bool r = q.Dequeue(TimeSpan.FromMilliseconds(-10000), out v);
            Assert.Greater(SafetyMargin, ElapsedMs());
            Assert.IsTrue(!r);
            Assert.AreEqual(null, v);
        }

        [Test]
        public void TestTimeoutInfinite()
        {
            SharedQueue<int> q = new SharedQueue<int>();
            EnqueueAfter(TimingInterval, q, 123);

            ResetTimer();
            int v;
            bool r = q.Dequeue(TimeSpan.MaxValue, out v);
            Assert.Less(TimingInterval - SafetyMargin, ElapsedMs());
            Assert.IsTrue(r);
            Assert.AreEqual(123, v);
        }

        [Test]
        public void TestBgShort()
        {
            SharedQueue<int> q = new SharedQueue<int>();
            EnqueueAfter(TimingInterval, q, 123);

            ResetTimer();
            int v;
            bool r = q.Dequeue(TimingInterval.Add(TimingInterval), out v);
            Assert.Less(TimingInterval - SafetyMargin, ElapsedMs());
            Assert.IsTrue(r);
            Assert.AreEqual(123, v);
        }

        [Test]
        public void TestBgLong()
        {
            SharedQueue<int> q = new SharedQueue<int>();
            EnqueueAfter(TimingInterval.Add(TimingInterval), q, 123);

            ResetTimer();
            int v;
            bool r = q.Dequeue(TimingInterval, out v);
            Assert.Greater(TimingInterval + SafetyMargin, ElapsedMs());
            Assert.IsTrue(!r);
            Assert.AreEqual(null, v);
        }

        [Test]
        public void TestDoubleBg()
        {
            SharedQueue<int> q = new SharedQueue<int>();
            EnqueueAfter(TimingInterval, q, 123);
            EnqueueAfter(TimingInterval.Add(TimingInterval), q, 234);

            ResetTimer();
            int v;

            bool r = q.Dequeue(TimingInterval.Add(TimingInterval), out v);
            Assert.Less(TimingInterval - SafetyMargin, ElapsedMs());
            Assert.Greater(TimingInterval + SafetyMargin, ElapsedMs());
            Assert.IsTrue(r);
            Assert.AreEqual(123, v);

            r = q.Dequeue(TimingInterval.Add(TimingInterval), out v);
            Assert.Less(TimingInterval.Add(TimingInterval) - SafetyMargin, ElapsedMs());
            Assert.Greater(TimingInterval.Add(TimingInterval) + SafetyMargin, ElapsedMs());
            Assert.IsTrue(r);
            Assert.AreEqual(234, v);
        }

        [Test]
        public void TestDoublePoll()
        {
            SharedQueue<int> q = new SharedQueue<int>();
            EnqueueAfter(TimingInterval.Add(TimingInterval), q, 123);

            ResetTimer();
            int v;

            bool r = q.Dequeue(TimingInterval, out v);
            Assert.Less(TimingInterval - SafetyMargin, ElapsedMs());
            Assert.Greater(TimingInterval + SafetyMargin, ElapsedMs());
            Assert.IsTrue(!r);
            Assert.AreEqual(null, v);

            r = q.Dequeue(TimingInterval.Add(TimingInterval), out v);
            Assert.Less(TimingInterval.Add(TimingInterval) - SafetyMargin, ElapsedMs());
            Assert.Greater(TimingInterval.Add(TimingInterval) + SafetyMargin, ElapsedMs());
            Assert.IsTrue(r);
            Assert.AreEqual(123, v);
        }

        [Test]
        public void TestCloseWhenEmpty()
        {
            DelayedEnqueuer<int> de = new DelayedEnqueuer<int>(new SharedQueue<int>(), 0, 1);
            de.m_q.Close();
            ExpectEof(new Thunk(de.EnqueueValue));
            ExpectEof(new Thunk(de.Dequeue));
            ExpectEof(new Thunk(de.DequeueNoWaitZero));
            ExpectEof(new Thunk(de.DequeueAfterOneIntoV));
        }

        [Test]
        public void TestCloseWhenFull()
        {
            SharedQueue<int> q = new SharedQueue<int>();
            int v;
            q.Enqueue(1);
            q.Enqueue(2);
            q.Enqueue(3);
            q.Close();
            DelayedEnqueuer<int> de = new DelayedEnqueuer<int>(q, 0, 4);
            ExpectEof(new Thunk(de.EnqueueValue));
            Assert.AreEqual(1, q.Dequeue());
            Assert.AreEqual(2, q.DequeueNoWait(0));
            bool r = q.Dequeue(TimeSpan.FromMilliseconds(1), out v);
            Assert.IsTrue(r);
            Assert.AreEqual(3, v);
            ExpectEof(new Thunk(de.Dequeue));
        }

        [Test]
        public void TestCloseWhenWaiting()
        {
            SharedQueue<object> q = new SharedQueue<object>();
            DelayedEnqueuer<object> de = new DelayedEnqueuer<object>(q, 0, null);
            Thread t =
            new Thread(new ThreadStart(de.BackgroundEofExpectingDequeue));
            t.Start();
            Thread.Sleep(SafetyMargin);
            q.Close();
            t.Join();
        }

        [Test]
        public void TestEnumerator()
        {
            SharedQueue<int> q = new SharedQueue<int>();
            VolatileInt c1 = new VolatileInt();
            VolatileInt c2 = new VolatileInt();
            Thread t1 = new Thread(delegate() {
                    foreach (int v in q) c1.v+=v;
                });
            Thread t2 = new Thread(delegate() {
                    foreach (int v in q) c2.v+=v;
                });
            t1.Start();
            t2.Start();
            q.Enqueue(1);
            q.Enqueue(2);
            q.Enqueue(3);
            q.Close();
            t1.Join();
            t2.Join();
            Assert.AreEqual(6, c1.v + c2.v);
        }

    }

}
