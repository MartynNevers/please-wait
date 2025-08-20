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
        public void Metrics_WithDefaultParameter_ReturnsMetricsObject()
        {
            var metrics = Wait()
                .Metrics()
                .AtMost(100, Millis)
                .Until(() => true);

            Assert.That(metrics, Is.Not.Null);
            Assert.That(metrics!.WasSuccessful, Is.True);
            Assert.That(metrics.ConditionChecks, Is.GreaterThan(0));
            Assert.That(metrics.TotalTime, Is.GreaterThan(TimeSpan.Zero));
        }

        [Test]
        public void Metrics_WhenNotCalled_ReturnsNull()
        {
            var metrics = Wait()
                .AtMost(100, Millis)
                .Until(() => true);

            Assert.That(metrics, Is.Null);
        }

        [Test]
        public void Metrics_RecordsConditionChecks()
        {
            var checkCount = 0;
            var metrics = Wait()
                .Metrics()
                .AtMost(500, Millis)
                .PollDelay(10, Millis)
                .PollInterval(10, Millis)
                .Until(() => ++checkCount >= 3);

            Assert.That(metrics!.ConditionChecks, Is.EqualTo(3));
            Assert.That(metrics.WasSuccessful, Is.True);
        }

        [Test]
        public void Metrics_RecordsTotalTime()
        {
            var metrics = Wait()
                .Metrics()
                .AtMost(100, Millis)
                .Until(() => true);

            Assert.That(metrics!.TotalTime, Is.GreaterThan(TimeSpan.Zero));
            Assert.That(metrics.TotalTime, Is.LessThan(TimeSpan.FromMilliseconds(200)));
        }

        [Test]
        public void Metrics_RecordsAverageCheckTime()
        {
            var metrics = Wait()
                .Metrics()
                .AtMost(100, Millis)
                .Until(() => true);

            Assert.That(metrics!.AverageCheckTime, Is.GreaterThan(TimeSpan.Zero));
            Assert.That(metrics.AverageCheckTime, Is.LessThanOrEqualTo(metrics.TotalTime));
        }

        [Test]
        public void Metrics_RecordsMinAndMaxCheckTimes()
        {
            var checkCount = 0;
            var metrics = Wait()
                .Metrics()
                .AtMost(500, Millis)
                .PollDelay(10, Millis)
                .PollInterval(10, Millis)
                .Until(() => ++checkCount >= 3);

            Assert.That(metrics!.MinCheckTime, Is.GreaterThanOrEqualTo(TimeSpan.Zero));
            Assert.That(metrics.MaxCheckTime, Is.GreaterThanOrEqualTo(TimeSpan.Zero));
            Assert.That(metrics.MaxCheckTime, Is.GreaterThanOrEqualTo(metrics.MinCheckTime));
        }

        [Test]
        public void Metrics_RecordsPollDelayTime()
        {
            var metrics = Wait()
                .Metrics()
                .AtMost(200, Millis)
                .PollDelay(50, Millis)
                .PollInterval(10, Millis)
                .Until(() => true);

            Assert.That(metrics!.PollDelayTime, Is.GreaterThan(TimeSpan.Zero));
        }

        [Test]
        public void Metrics_RecordsPollIntervalTime()
        {
            var metrics = Wait()
                .Metrics()
                .AtMost(200, Millis)
                .PollDelay(10, Millis)
                .PollInterval(50, Millis)
                .Until(() => true);

            Assert.That(metrics!.PollIntervalTime, Is.GreaterThan(TimeSpan.Zero));
        }

        [Test]
        public void Metrics_RecordsConfiguredValues()
        {
            var metrics = Wait()
                .Metrics()
                .AtMost(500, Millis)
                .PollDelay(100, Millis)
                .PollInterval(200, Millis)
                .Until(() => true);

            Assert.That(metrics!.ConfiguredTimeout, Is.EqualTo(TimeSpan.FromMilliseconds(500)));
            Assert.That(metrics.ConfiguredPollDelay, Is.EqualTo(TimeSpan.FromMilliseconds(100)));
            Assert.That(metrics.ConfiguredPollInterval, Is.EqualTo(TimeSpan.FromMilliseconds(200)));
        }

        [Test]
        public void Metrics_RecordsConditionAlias()
        {
            var metrics = Wait()
                .Metrics()
                .Alias("Test Condition")
                .AtMost(100, Millis)
                .Until(() => true);

            Assert.That(metrics!.ConditionAlias, Is.EqualTo("Test Condition"));
        }

        [Test]
        public void Metrics_TimeoutScenario_RecordsFailure()
        {
            Assert.Throws<TimeoutException>(() =>
            {
                Wait()
                    .Metrics()
                    .AtMost(50, Millis)
                    .Until(() => false);
            });
        }

        [Test]
        public void Metrics_CancellationScenario_RecordsPartialMetrics()
        {
            var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(5));

            Assert.Throws<OperationCanceledException>(() =>
            {
                Wait()
                    .Metrics()
                    .AtMost(1000, Millis)
                    .PollDelay(100, Millis)
                    .Until(() => false, cancellationToken: cts.Token);
            });
        }

        [Test]
        public void Metrics_ToString_ReturnsFormattedString()
        {
            var metrics = Wait()
                .Metrics()
                .AtMost(100, Millis)
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
        public void Metrics_UntilTrue_ReturnsMetrics()
        {
            var metrics = Wait()
                .Metrics()
                .AtMost(100, Millis)
                .Until(() => true);

            Assert.That(metrics, Is.Not.Null);
            Assert.That(metrics!.WasSuccessful, Is.True);
        }

        [Test]
        public void Metrics_UntilFalse_ReturnsMetrics()
        {
            var metrics = Wait()
                .Metrics()
                .AtMost(100, Millis)
                .Until(() => false, expected: false);

            Assert.That(metrics, Is.Not.Null);
            Assert.That(metrics!.WasSuccessful, Is.True);
        }
    }
}
