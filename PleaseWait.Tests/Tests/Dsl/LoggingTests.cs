// <copyright file="LoggingTests.cs" company="Esdet">
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
    using PleaseWait.Logging;
    using static PleaseWait.Dsl;
    using static PleaseWait.TimeUnit;

    [TestFixture]
    [Category("Logging")]
    [Parallelizable(scope: ParallelScope.All)]
    public class LoggingTests
    {
        [Test]
        public void Logger_WithValidLogger_SetsLoggerCorrectly()
        {
            var logger = new ConsoleLogger();
            var dsl = Wait().Logger(logger);
            Assert.That(dsl, Is.Not.Null);
        }

        [Test]
        public void Logger_WithNullLogger_UsesNullLogger()
        {
            var dsl = Wait().Logger(null!);
            Assert.That(dsl, Is.Not.Null);
        }

        [Test]
        public void Until_WithLogger_LogsWaitStart()
        {
            var logger = new TestLogger();
            Wait().Logger(logger).AtMost(100, Millis).Until(() => true);
            Assert.That(logger.WaitStartLogged, Is.True);
            Assert.That(logger.WaitStartCondition, Is.EqualTo("condition"));
            Assert.That(logger.WaitStartTimeout, Is.EqualTo(TimeSpan.FromMilliseconds(100)));
        }

        [Test]
        public void Until_WithLogger_LogsConditionChecks()
        {
            var logger = new TestLogger();
            var checkCount = 0;
            Wait().Logger(logger).AtMost(500, Millis).Until(() => ++checkCount > 1);
            Assert.That(logger.ConditionChecksLogged, Is.GreaterThan(0));
        }

        [Test]
        public void Until_WithLogger_LogsWaitSuccess()
        {
            var logger = new TestLogger();
            Wait().Logger(logger).AtMost(100, Millis).Until(() => true);
            Assert.That(logger.WaitSuccessLogged, Is.True);
            Assert.That(logger.WaitSuccessCondition, Is.EqualTo("condition"));
            Assert.That(logger.WaitSuccessChecks, Is.GreaterThan(0));
        }

        [Test]
        public void Until_WithLoggerAndAlias_LogsWithAlias()
        {
            var logger = new TestLogger();
            Wait().Logger(logger).Alias("test condition").AtMost(100, Millis).Until(() => true);
            Assert.That(logger.WaitStartCondition, Is.EqualTo("test condition"));
            Assert.That(logger.WaitSuccessCondition, Is.EqualTo("test condition"));
        }

        [Test]
        public void Until_WithLogger_LogsTimeout()
        {
            var logger = new TestLogger();
            Assert.Throws<TimeoutException>(() =>
                Wait().Logger(logger).AtMost(50, Millis).Until(() => false));
            Assert.That(logger.TimeoutLogged, Is.True);
            Assert.That(logger.TimeoutCondition, Is.EqualTo("condition"));
            Assert.That(logger.TimeoutValue, Is.EqualTo(TimeSpan.FromMilliseconds(50)));
        }

        [Test]
        public void Until_WithLogger_LogsCancellation()
        {
            var logger = new TestLogger();
            var cts = new CancellationTokenSource();
            cts.Cancel();
            Assert.Throws<OperationCanceledException>(() =>
                Wait().Logger(logger).AtMost(1, Seconds).Until(() => true, cancellationToken: cts.Token));
            Assert.That(logger.CancellationLogged, Is.True);
            Assert.That(logger.CancellationCondition, Is.EqualTo("condition"));
        }

        [Test]
        public void Until_WithFailSilently_DoesNotLogTimeout()
        {
            var logger = new TestLogger();
            Wait().Logger(logger).AtMost(50, Millis).FailSilently().Until(() => false);
            Assert.That(logger.TimeoutLogged, Is.False);
        }

        [Test]
        public void WithDebugLogger_DoesNotThrow()
        {
            Assert.DoesNotThrow(() =>
            {
                Wait().Logger(new DebugLogger()).AtMost(100, Millis).Until(() => true);
            });
        }

        [Test]
        public void Until_WithLoggerAndMetrics_LogsAndReturnsMetrics()
        {
            var logger = new TestLogger();
            var metrics = Wait()
                .Logger(logger)
                .Metrics()
                .AtMost(100, Millis)
                .Until(() => true);

            Assert.That(metrics, Is.Not.Null);
            Assert.That(metrics!.WasSuccessful, Is.True);
            Assert.That(metrics.ConditionChecks, Is.GreaterThan(0));
            Assert.That(logger.WaitStartLogged, Is.True);
            Assert.That(logger.WaitSuccessLogged, Is.True);
        }
    }
}
