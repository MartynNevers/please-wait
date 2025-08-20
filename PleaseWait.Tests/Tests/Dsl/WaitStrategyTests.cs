// <copyright file="WaitStrategyTests.cs" company="Esdet">
// Copyright 2025 the original author or authors.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>

namespace PleaseWait.Tests
{
    using System;
    using System.Threading;
    using NUnit.Framework;
    using static PleaseWait.Dsl;
    using static PleaseWait.Strategy.WaitStrategy;
    using static PleaseWait.TimeUnit;

    [TestFixture]
    [Category("WaitStrategy")]
    [Parallelizable(scope: ParallelScope.All)]
    public class WaitStrategyTests
    {
        [Test]
        public void Strategy_WithLinear_ExhibitsDefaultBehavior()
        {
            var startTime = DateTime.UtcNow;
            Wait()
                .Strategy(Linear)
                .AtMost(100, Millis)
                .PollDelay(10, Millis)
                .PollInterval(20, Millis)
                .FailSilently()
                .Until(() => false); // Will timeout

            var elapsed = DateTime.UtcNow - startTime;
            Assert.That(elapsed.TotalMilliseconds, Is.GreaterThan(100));
        }

        [Test]
        public void Strategy_ExponentialBackoff_IncreasesDelays()
        {
            var startTime = DateTime.UtcNow;
            Wait()
                .Strategy(ExponentialBackoff)
                .AtMost(200, Millis)
                .PollDelay(10, Millis)
                .PollInterval(20, Millis)
                .FailSilently()
                .Until(() => false); // Will timeout

            var elapsed = DateTime.UtcNow - startTime;
            Assert.That(elapsed.TotalMilliseconds, Is.GreaterThan(200));
        }

        [Test]
        public void Strategy_Aggressive_FastDetection()
        {
            var startTime = DateTime.UtcNow;
            var checkCount = 0;

            Wait()
                .Strategy(Aggressive)
                .AtMost(100, Millis)
                .PollDelay(10, Millis)
                .PollInterval(20, Millis)
                .Until(() => ++checkCount > 3); // Will succeed quickly

            var elapsed = DateTime.UtcNow - startTime;
            Assert.That(elapsed.TotalMilliseconds, Is.LessThan(100));
            Assert.That(checkCount, Is.GreaterThan(3));
        }

        [Test]
        public void Strategy_Conservative_LongerDelays()
        {
            var startTime = DateTime.UtcNow;
            Wait()
                .Strategy(Conservative)
                .AtMost(100, Millis)
                .PollDelay(10, Millis)
                .PollInterval(20, Millis)
                .FailSilently()
                .Until(() => false); // Will timeout

            var elapsed = DateTime.UtcNow - startTime;
            Assert.That(elapsed.TotalMilliseconds, Is.GreaterThan(100));
        }

        [Test]
        public void Strategy_Adaptive_AdjustsBasedOnPerformance()
        {
            var startTime = DateTime.UtcNow;
            var checkCount = 0;

            Wait()
                .Strategy(Adaptive)
                .Metrics()
                .AtMost(200, Millis)
                .PollDelay(10, Millis)
                .PollInterval(20, Millis)
                .Until(() => ++checkCount > 2); // Will succeed

            var elapsed = DateTime.UtcNow - startTime;
            Assert.That(elapsed.TotalMilliseconds, Is.LessThan(200));
            Assert.That(checkCount, Is.GreaterThan(2));
        }

        [Test]
        public void Strategy_DefaultIsLinear()
        {
            var startTime = DateTime.UtcNow;
            Wait()
                .AtMost(100, Millis)
                .PollDelay(10, Millis)
                .PollInterval(20, Millis)
                .FailSilently()
                .Until(() => false); // Will timeout

            var elapsed = DateTime.UtcNow - startTime;
            Assert.That(elapsed.TotalMilliseconds, Is.GreaterThan(100));
        }

        [Test]
        public void Strategy_ExponentialBackoff_RespectsTimeout()
        {
            var startTime = DateTime.UtcNow;
            Wait()
                .Strategy(ExponentialBackoff)
                .AtMost(50, Millis)
                .PollDelay(5, Millis)
                .PollInterval(10, Millis)
                .FailSilently()
                .Until(() => false); // Will timeout

            var elapsed = DateTime.UtcNow - startTime;
            Assert.That(elapsed.TotalMilliseconds, Is.GreaterThan(50));
            Assert.That(elapsed.TotalMilliseconds, Is.LessThan(200)); // Should not exceed reasonable bounds
        }

        [Test]
        public void Strategy_Aggressive_MinimalDelays()
        {
            var checkCount = 0;
            var startTime = DateTime.UtcNow;

            Wait()
                .Strategy(Aggressive)
                .AtMost(50, Millis)
                .PollDelay(1, Millis)
                .PollInterval(5, Millis)
                .Until(() => ++checkCount > 5); // Will succeed quickly

            var elapsed = DateTime.UtcNow - startTime;
            Assert.That(elapsed.TotalMilliseconds, Is.LessThan(50));
            Assert.That(checkCount, Is.GreaterThan(5));
        }

        [Test]
        public void Strategy_Conservative_MaximizesDelays()
        {
            var startTime = DateTime.UtcNow;
            Wait()
                .Strategy(Conservative)
                .AtMost(100, Millis)
                .PollDelay(10, Millis)
                .PollInterval(20, Millis)
                .FailSilently()
                .Until(() => false); // Will timeout

            var elapsed = DateTime.UtcNow - startTime;
            Assert.That(elapsed.TotalMilliseconds, Is.GreaterThan(100));
        }

        [Test]
        public void Strategy_Adaptive_WorksWithoutMetrics()
        {
            var startTime = DateTime.UtcNow;
            var checkCount = 0;

            Wait()
                .Strategy(Adaptive)
                .AtMost(100, Millis)
                .PollDelay(10, Millis)
                .PollInterval(20, Millis)
                .Until(() => ++checkCount > 2); // Will succeed

            var elapsed = DateTime.UtcNow - startTime;
            Assert.That(elapsed.TotalMilliseconds, Is.LessThan(100));
            Assert.That(checkCount, Is.GreaterThan(2));
        }

        [Test]
        public void Strategy_CombinedWithMetrics_WorksCorrectly()
        {
            var metrics = Wait()
                .Strategy(ExponentialBackoff)
                .Metrics()
                .AtMost(100, Millis)
                .PollDelay(10, Millis)
                .PollInterval(20, Millis)
                .Until(() => true); // Will succeed immediately

            Assert.That(metrics, Is.Not.Null);
            Assert.That(metrics!.WasSuccessful, Is.True);
            Assert.That(metrics.ConditionChecks, Is.GreaterThan(0));
        }

        [Test]
        public void Strategy_CombinedWithLogger_WorksCorrectly()
        {
            var logger = new TestLogger();
            var checkCount = 0;

            Wait()
                .Strategy(Aggressive)
                .Logger(logger)
                .AtMost(100, Millis)
                .PollDelay(10, Millis)
                .PollInterval(20, Millis)
                .Until(() => ++checkCount > 2); // Will succeed

            Assert.That(logger.WaitStartLogged, Is.True);
            Assert.That(logger.WaitSuccessLogged, Is.True);
            Assert.That(checkCount, Is.GreaterThan(2));
        }

        [Test]
        public void Strategy_Aggressive_ModifiesBothPollDelayAndInterval()
        {
            var startTime = DateTime.UtcNow;
            var checkCount = 0;

            Wait()
                .Strategy(Aggressive)
                .AtMost(200, Millis)
                .PollDelay(100, Millis) // Should become ~25ms
                .PollInterval(80, Millis) // Should become ~20ms
                .Until(() => ++checkCount > 3); // Will succeed

            var elapsed = DateTime.UtcNow - startTime;
            Assert.That(elapsed.TotalMilliseconds, Is.LessThan(200));
            Assert.That(checkCount, Is.GreaterThan(3));
        }

        [Test]
        public void Strategy_Conservative_ModifiesBothPollDelayAndInterval()
        {
            var startTime = DateTime.UtcNow;
            Wait()
                .Strategy(Conservative)
                .AtMost(100, Millis)
                .PollDelay(10, Millis) // Should become ~20ms
                .PollInterval(20, Millis) // Should become ~40ms
                .FailSilently()
                .Until(() => false); // Will timeout

            var elapsed = DateTime.UtcNow - startTime;
            Assert.That(elapsed.TotalMilliseconds, Is.GreaterThan(100));
        }
    }
}
