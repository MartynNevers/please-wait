// <copyright file="MetricsTests.cs" company="Esdet">
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
    using static PleaseWait.TimeUnit;

    [TestFixture]
    [Category("Metrics")]
    [Parallelizable(scope: ParallelScope.All)]
    public class MetricsTests
    {
        [Test]
        public void WithMetrics_Enabled_ReturnsMetricsObject()
        {
            var metrics = Wait()
                .WithMetrics()
                .AtMost(100, MILLIS)
                .Until(() => true);

            Assert.That(metrics, Is.Not.Null);
            Assert.That(metrics!.WasSuccessful, Is.True);
            Assert.That(metrics.ConditionChecks, Is.GreaterThan(0));
            Assert.That(metrics.TotalTime, Is.GreaterThan(TimeSpan.Zero));
        }

        [Test]
        public void WithMetrics_NotEnabled_ReturnsNull()
        {
            var metrics = Wait()
                .AtMost(100, MILLIS)
                .Until(() => true);

            Assert.That(metrics, Is.Null);
        }

        [Test]
        public void WithMetrics_RecordsConditionChecks()
        {
            var checkCount = 0;
            var metrics = Wait()
                .WithMetrics()
                .AtMost(500, MILLIS)
                .PollDelay(10, MILLIS)
                .PollInterval(10, MILLIS)
                .Until(() => ++checkCount >= 3);

            Assert.That(metrics!.ConditionChecks, Is.EqualTo(3));
            Assert.That(metrics.WasSuccessful, Is.True);
        }

        [Test]
        public void WithMetrics_RecordsTotalTime()
        {
            var metrics = Wait()
                .WithMetrics()
                .AtMost(100, MILLIS)
                .Until(() => true);

            Assert.That(metrics!.TotalTime, Is.GreaterThan(TimeSpan.Zero));
            Assert.That(metrics.TotalTime, Is.LessThan(TimeSpan.FromMilliseconds(200)));
        }

        [Test]
        public void WithMetrics_RecordsAverageCheckTime()
        {
            var metrics = Wait()
                .WithMetrics()
                .AtMost(100, MILLIS)
                .Until(() => true);

            Assert.That(metrics!.AverageCheckTime, Is.GreaterThan(TimeSpan.Zero));
            Assert.That(metrics.AverageCheckTime, Is.LessThanOrEqualTo(metrics.TotalTime));
        }

        [Test]
        public void WithMetrics_RecordsMinAndMaxCheckTimes()
        {
            var checkCount = 0;
            var metrics = Wait()
                .WithMetrics()
                .AtMost(500, MILLIS)
                .PollDelay(10, MILLIS)
                .PollInterval(10, MILLIS)
                .Until(() => ++checkCount >= 3);

            Assert.That(metrics!.MinCheckTime, Is.GreaterThanOrEqualTo(TimeSpan.Zero));
            Assert.That(metrics.MaxCheckTime, Is.GreaterThanOrEqualTo(TimeSpan.Zero));
            Assert.That(metrics.MaxCheckTime, Is.GreaterThanOrEqualTo(metrics.MinCheckTime));
        }

        [Test]
        public void WithMetrics_RecordsPollDelayTime()
        {
            var metrics = Wait()
                .WithMetrics()
                .AtMost(200, MILLIS)
                .PollDelay(50, MILLIS)
                .PollInterval(10, MILLIS)
                .Until(() => true);

            Assert.That(metrics!.PollDelayTime, Is.GreaterThan(TimeSpan.Zero));
        }

        [Test]
        public void WithMetrics_RecordsPollIntervalTime()
        {
            var metrics = Wait()
                .WithMetrics()
                .AtMost(200, MILLIS)
                .PollDelay(10, MILLIS)
                .PollInterval(50, MILLIS)
                .Until(() => true);

            Assert.That(metrics!.PollIntervalTime, Is.GreaterThan(TimeSpan.Zero));
        }

        [Test]
        public void WithMetrics_RecordsConfiguredValues()
        {
            var metrics = Wait()
                .WithMetrics()
                .AtMost(500, MILLIS)
                .PollDelay(100, MILLIS)
                .PollInterval(200, MILLIS)
                .Until(() => true);

            Assert.That(metrics!.ConfiguredTimeout, Is.EqualTo(TimeSpan.FromMilliseconds(500)));
            Assert.That(metrics.ConfiguredPollDelay, Is.EqualTo(TimeSpan.FromMilliseconds(100)));
            Assert.That(metrics.ConfiguredPollInterval, Is.EqualTo(TimeSpan.FromMilliseconds(200)));
        }

        [Test]
        public void WithMetrics_RecordsConditionAlias()
        {
            var metrics = Wait()
                .WithMetrics()
                .Alias("Test Condition")
                .AtMost(100, MILLIS)
                .Until(() => true);

            Assert.That(metrics!.ConditionAlias, Is.EqualTo("Test Condition"));
        }

        [Test]
        public void WithMetrics_TimeoutScenario_RecordsFailure()
        {
            Assert.Throws<TimeoutException>(() =>
            {
                Wait()
                    .WithMetrics()
                    .AtMost(50, MILLIS)
                    .Until(() => false);
            });
        }

        [Test]
        public void WithMetrics_CancellationScenario_RecordsPartialMetrics()
        {
            var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(5));

            Assert.Throws<OperationCanceledException>(() =>
            {
                Wait()
                    .WithMetrics()
                    .AtMost(1000, MILLIS)
                    .PollDelay(100, MILLIS)
                    .Until(() => false, cancellationToken: cts.Token);
            });
        }

        [Test]
        public void WithMetrics_ToString_ReturnsFormattedString()
        {
            var metrics = Wait()
                .WithMetrics()
                .AtMost(100, MILLIS)
                .Until(() => true);

            var result = metrics!.ToString();

            Assert.That(result, Contains.Substring("WaitMetrics:"));
            Assert.That(result, Contains.Substring("checks"));
            Assert.That(result, Contains.Substring("SUCCESS"));
            Assert.That(result, Contains.Substring("avg:"));
            Assert.That(result, Contains.Substring("min:"));
            Assert.That(result, Contains.Substring("max:"));
        }

        [Test]
        public void WithMetrics_UntilTrue_ReturnsMetrics()
        {
            var metrics = Wait()
                .WithMetrics()
                .AtMost(100, MILLIS)
                .Until(() => true);

            Assert.That(metrics, Is.Not.Null);
            Assert.That(metrics!.WasSuccessful, Is.True);
        }

        [Test]
        public void WithMetrics_UntilFalse_ReturnsMetrics()
        {
            var metrics = Wait()
                .WithMetrics()
                .AtMost(100, MILLIS)
                .Until(() => false, expected: false);

            Assert.That(metrics, Is.Not.Null);
            Assert.That(metrics!.WasSuccessful, Is.True);
        }
    }
}
