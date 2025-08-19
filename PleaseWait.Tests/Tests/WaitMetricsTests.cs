// <copyright file="WaitMetricsTests.cs" company="Esdet">
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
    using NUnit.Framework;

    [TestFixture]
    [Category("Unit")]
    [Parallelizable(scope: ParallelScope.All)]
    public class WaitMetricsTests
    {
        [Test]
        public void Constructor_CreatesInstance()
        {
            var metrics = new WaitMetrics();
            Assert.That(metrics, Is.Not.Null);
        }

        [Test]
        public void ConditionChecks_DefaultValue_IsZero()
        {
            var metrics = new WaitMetrics();
            Assert.That(metrics.ConditionChecks, Is.EqualTo(0));
        }

        [Test]
        public void ConditionChecks_CanBeSet()
        {
            var metrics = new WaitMetrics { ConditionChecks = 5 };
            Assert.That(metrics.ConditionChecks, Is.EqualTo(5));
        }

        [Test]
        public void TotalTime_DefaultValue_IsZero()
        {
            var metrics = new WaitMetrics();
            Assert.That(metrics.TotalTime, Is.EqualTo(TimeSpan.Zero));
        }

        [Test]
        public void TotalTime_CanBeSet()
        {
            var expectedTime = TimeSpan.FromMilliseconds(1500);
            var metrics = new WaitMetrics { TotalTime = expectedTime };
            Assert.That(metrics.TotalTime, Is.EqualTo(expectedTime));
        }

        [Test]
        public void AverageCheckTime_WithZeroChecks_ReturnsZero()
        {
            var metrics = new WaitMetrics { ConditionChecks = 0 };
            Assert.That(metrics.AverageCheckTime, Is.EqualTo(TimeSpan.Zero));
        }

        [Test]
        public void AverageCheckTime_WithOneCheck_ReturnsTotalTime()
        {
            var totalTime = TimeSpan.FromMilliseconds(100);
            var metrics = new WaitMetrics
            {
                ConditionChecks = 1,
                TotalTime = totalTime,
            };
            Assert.That(metrics.AverageCheckTime, Is.EqualTo(totalTime));
        }

        [Test]
        public void AverageCheckTime_WithMultipleChecks_ReturnsCorrectAverage()
        {
            var totalTime = TimeSpan.FromMilliseconds(1000);
            var metrics = new WaitMetrics
            {
                ConditionChecks = 5,
                TotalTime = totalTime,
            };
            var expectedAverage = TimeSpan.FromMilliseconds(200); // 1000ms / 5 checks
            Assert.That(metrics.AverageCheckTime, Is.EqualTo(expectedAverage));
        }

        [Test]
        public void MinCheckTime_DefaultValue_IsMaxValue()
        {
            var metrics = new WaitMetrics();
            Assert.That(metrics.MinCheckTime, Is.EqualTo(TimeSpan.MaxValue));
        }

        [Test]
        public void MinCheckTime_CanBeSet()
        {
            var minTime = TimeSpan.FromMilliseconds(50);
            var metrics = new WaitMetrics { MinCheckTime = minTime };
            Assert.That(metrics.MinCheckTime, Is.EqualTo(minTime));
        }

        [Test]
        public void MaxCheckTime_DefaultValue_IsZero()
        {
            var metrics = new WaitMetrics();
            Assert.That(metrics.MaxCheckTime, Is.EqualTo(TimeSpan.Zero));
        }

        [Test]
        public void MaxCheckTime_CanBeSet()
        {
            var maxTime = TimeSpan.FromMilliseconds(200);
            var metrics = new WaitMetrics { MaxCheckTime = maxTime };
            Assert.That(metrics.MaxCheckTime, Is.EqualTo(maxTime));
        }

        [Test]
        public void PollDelayTime_DefaultValue_IsZero()
        {
            var metrics = new WaitMetrics();
            Assert.That(metrics.PollDelayTime, Is.EqualTo(TimeSpan.Zero));
        }

        [Test]
        public void PollDelayTime_CanBeSet()
        {
            var delayTime = TimeSpan.FromMilliseconds(100);
            var metrics = new WaitMetrics { PollDelayTime = delayTime };
            Assert.That(metrics.PollDelayTime, Is.EqualTo(delayTime));
        }

        [Test]
        public void PollIntervalTime_DefaultValue_IsZero()
        {
            var metrics = new WaitMetrics();
            Assert.That(metrics.PollIntervalTime, Is.EqualTo(TimeSpan.Zero));
        }

        [Test]
        public void PollIntervalTime_CanBeSet()
        {
            var intervalTime = TimeSpan.FromMilliseconds(500);
            var metrics = new WaitMetrics { PollIntervalTime = intervalTime };
            Assert.That(metrics.PollIntervalTime, Is.EqualTo(intervalTime));
        }

        [Test]
        public void WasSuccessful_DefaultValue_IsFalse()
        {
            var metrics = new WaitMetrics();
            Assert.That(metrics.WasSuccessful, Is.False);
        }

        [Test]
        public void WasSuccessful_CanBeSet()
        {
            var metrics = new WaitMetrics { WasSuccessful = true };
            Assert.That(metrics.WasSuccessful, Is.True);
        }

        [Test]
        public void ConditionAlias_DefaultValue_IsNull()
        {
            var metrics = new WaitMetrics();
            Assert.That(metrics.ConditionAlias, Is.Null);
        }

        [Test]
        public void ConditionAlias_CanBeSet()
        {
            var alias = "Test Condition";
            var metrics = new WaitMetrics { ConditionAlias = alias };
            Assert.That(metrics.ConditionAlias, Is.EqualTo(alias));
        }

        [Test]
        public void ConfiguredTimeout_DefaultValue_IsZero()
        {
            var metrics = new WaitMetrics();
            Assert.That(metrics.ConfiguredTimeout, Is.EqualTo(TimeSpan.Zero));
        }

        [Test]
        public void ConfiguredTimeout_CanBeSet()
        {
            var timeout = TimeSpan.FromSeconds(30);
            var metrics = new WaitMetrics { ConfiguredTimeout = timeout };
            Assert.That(metrics.ConfiguredTimeout, Is.EqualTo(timeout));
        }

        [Test]
        public void ConfiguredPollDelay_DefaultValue_IsZero()
        {
            var metrics = new WaitMetrics();
            Assert.That(metrics.ConfiguredPollDelay, Is.EqualTo(TimeSpan.Zero));
        }

        [Test]
        public void ConfiguredPollDelay_CanBeSet()
        {
            var pollDelay = TimeSpan.FromMilliseconds(100);
            var metrics = new WaitMetrics { ConfiguredPollDelay = pollDelay };
            Assert.That(metrics.ConfiguredPollDelay, Is.EqualTo(pollDelay));
        }

        [Test]
        public void ConfiguredPollInterval_DefaultValue_IsZero()
        {
            var metrics = new WaitMetrics();
            Assert.That(metrics.ConfiguredPollInterval, Is.EqualTo(TimeSpan.Zero));
        }

        [Test]
        public void ConfiguredPollInterval_CanBeSet()
        {
            var pollInterval = TimeSpan.FromMilliseconds(200);
            var metrics = new WaitMetrics { ConfiguredPollInterval = pollInterval };
            Assert.That(metrics.ConfiguredPollInterval, Is.EqualTo(pollInterval));
        }

        [Test]
        public void ToString_WithDefaultValues_ReturnsFormattedString()
        {
            var metrics = new WaitMetrics();
            var result = metrics.ToString();
            Assert.That(result, Is.EqualTo("WaitMetrics: 0 checks, 00:00:00 total time, FAILED, avg: 00:00:00, min: 10675199.02:48:05.4775807, max: 00:00:00"));
        }

        [Test]
        public void ToString_WithValues_ReturnsFormattedString()
        {
            var metrics = new WaitMetrics
            {
                ConditionChecks = 3,
                TotalTime = TimeSpan.FromMilliseconds(1500),
                WasSuccessful = true,
                MinCheckTime = TimeSpan.FromMilliseconds(100),
                MaxCheckTime = TimeSpan.FromMilliseconds(800),
            };
            var result = metrics.ToString();
            Assert.That(result, Is.EqualTo("WaitMetrics: 3 checks, 00:00:01.5000000 total time, SUCCESS, avg: 00:00:00.5000000, min: 00:00:00.1000000, max: 00:00:00.8000000"));
        }

        [Test]
        public void ToString_WithFailedOperation_ShowsFailed()
        {
            var metrics = new WaitMetrics
            {
                ConditionChecks = 2,
                TotalTime = TimeSpan.FromMilliseconds(1000),
                WasSuccessful = false,
            };
            var result = metrics.ToString();
            Assert.That(result, Does.Contain("FAILED"));
        }

        [Test]
        public void ToString_WithSuccessfulOperation_ShowsSuccess()
        {
            var metrics = new WaitMetrics
            {
                ConditionChecks = 1,
                TotalTime = TimeSpan.FromMilliseconds(500),
                WasSuccessful = true,
            };
            var result = metrics.ToString();
            Assert.That(result, Does.Contain("SUCCESS"));
        }
    }
}
