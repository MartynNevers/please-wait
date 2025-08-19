// <copyright file="WaitStrategyCalculatorTests.cs" company="Esdet">
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
    using PleaseWait.Strategy;
    using static PleaseWait.Strategy.WaitStrategy;

    [TestFixture]
    [Category("Unit")]
    [Parallelizable(scope: ParallelScope.All)]
    public class WaitStrategyCalculatorTests
    {
        private const int DefaultPollDelayMs = 100;
        private const int DefaultPollIntervalMs = 200;
        private const int DefaultTimeoutMs = 10000;
        private static readonly TimeSpan DefaultPollDelay = TimeSpan.FromMilliseconds(DefaultPollDelayMs);
        private static readonly TimeSpan DefaultPollInterval = TimeSpan.FromMilliseconds(DefaultPollIntervalMs);
        private static readonly TimeSpan DefaultTimeout = TimeSpan.FromMilliseconds(DefaultTimeoutMs);

        [Test]
        public void Constructor_WithValidParameters_CreatesInstance()
        {
            var calculator = new WaitStrategyCalculator(Linear, DefaultPollDelay, DefaultPollInterval, DefaultTimeout);
            Assert.That(calculator, Is.Not.Null);
        }

        [Test]
        public void Constructor_WithMetrics_CreatesInstance()
        {
            var metrics = new WaitMetrics();
            var calculator = new WaitStrategyCalculator(Linear, DefaultPollDelay, DefaultPollInterval, DefaultTimeout, metrics);
            Assert.That(calculator, Is.Not.Null);
        }

        [Test]
        public void CalculateInitialDelay_Linear_ReturnsConfiguredDelay()
        {
            var calculator = new WaitStrategyCalculator(Linear, DefaultPollDelay, DefaultPollInterval, DefaultTimeout);
            var result = calculator.CalculateInitialDelay();
            Assert.That(result, Is.EqualTo(DefaultPollDelay));
        }

        [Test]
        public void CalculateInitialDelay_ExponentialBackoff_ReturnsConfiguredDelay()
        {
            var calculator = new WaitStrategyCalculator(ExponentialBackoff, DefaultPollDelay, DefaultPollInterval, DefaultTimeout);
            var result = calculator.CalculateInitialDelay();
            Assert.That(result, Is.EqualTo(DefaultPollDelay));
        }

        [Test]
        public void CalculateInitialDelay_Aggressive_ReturnsQuarterDelay()
        {
            var calculator = new WaitStrategyCalculator(Aggressive, DefaultPollDelay, DefaultPollInterval, DefaultTimeout);
            var result = calculator.CalculateInitialDelay();
            Assert.That(result, Is.EqualTo(TimeSpan.FromMilliseconds(DefaultPollDelayMs / 4)));
        }

        [Test]
        public void CalculateInitialDelay_Aggressive_WithMinimumDelay_ReturnsMinimum1ms()
        {
            var smallDelay = TimeSpan.FromMilliseconds(1);
            var calculator = new WaitStrategyCalculator(Aggressive, smallDelay, DefaultPollInterval, DefaultTimeout);
            var result = calculator.CalculateInitialDelay();
            Assert.That(result, Is.EqualTo(TimeSpan.FromMilliseconds(1)));
        }

        [Test]
        public void CalculateInitialDelay_Conservative_ReturnsDoubleDelay()
        {
            var calculator = new WaitStrategyCalculator(Conservative, DefaultPollDelay, DefaultPollInterval, DefaultTimeout);
            var result = calculator.CalculateInitialDelay();
            Assert.That(result, Is.EqualTo(TimeSpan.FromMilliseconds(DefaultPollDelayMs * 2)));
        }

        [Test]
        public void CalculateInitialDelay_Adaptive_ReturnsConfiguredDelay()
        {
            var calculator = new WaitStrategyCalculator(Adaptive, DefaultPollDelay, DefaultPollInterval, DefaultTimeout);
            var result = calculator.CalculateInitialDelay();
            Assert.That(result, Is.EqualTo(DefaultPollDelay));
        }

        [Test]
        public void CalculateIntervalDelay_Linear_ReturnsConfiguredInterval()
        {
            var calculator = new WaitStrategyCalculator(Linear, DefaultPollDelay, DefaultPollInterval, DefaultTimeout);
            var result = calculator.CalculateIntervalDelay(1);
            Assert.That(result, Is.EqualTo(DefaultPollInterval));
        }

        [Test]
        public void CalculateIntervalDelay_Linear_WithMultipleChecks_ReturnsSameInterval()
        {
            var calculator = new WaitStrategyCalculator(Linear, DefaultPollDelay, DefaultPollInterval, DefaultTimeout);
            var result1 = calculator.CalculateIntervalDelay(1);
            var result2 = calculator.CalculateIntervalDelay(5);
            var result3 = calculator.CalculateIntervalDelay(10);
            Assert.That(result1, Is.EqualTo(DefaultPollInterval));
            Assert.That(result2, Is.EqualTo(DefaultPollInterval));
            Assert.That(result3, Is.EqualTo(DefaultPollInterval));
        }

        [Test]
        public void CalculateIntervalDelay_ExponentialBackoff_FirstCheck_ReturnsBaseInterval()
        {
            var calculator = new WaitStrategyCalculator(ExponentialBackoff, DefaultPollDelay, DefaultPollInterval, DefaultTimeout);
            var result = calculator.CalculateIntervalDelay(1);
            Assert.That(result, Is.EqualTo(DefaultPollInterval));
        }

        [Test]
        public void CalculateIntervalDelay_ExponentialBackoff_SecondCheck_ReturnsDoubleInterval()
        {
            var calculator = new WaitStrategyCalculator(ExponentialBackoff, DefaultPollDelay, DefaultPollInterval, DefaultTimeout);
            var result = calculator.CalculateIntervalDelay(2);
            Assert.That(result, Is.EqualTo(TimeSpan.FromMilliseconds(DefaultPollIntervalMs * 2)));
        }

        [Test]
        public void CalculateIntervalDelay_ExponentialBackoff_ThirdCheck_ReturnsQuadrupleInterval()
        {
            var calculator = new WaitStrategyCalculator(ExponentialBackoff, DefaultPollDelay, DefaultPollInterval, DefaultTimeout);
            var result = calculator.CalculateIntervalDelay(3);
            Assert.That(result, Is.EqualTo(TimeSpan.FromMilliseconds(DefaultPollIntervalMs * 4)));
        }

        [Test]
        public void CalculateIntervalDelay_ExponentialBackoff_FourthCheck_ReturnsOctupleInterval()
        {
            var calculator = new WaitStrategyCalculator(ExponentialBackoff, DefaultPollDelay, DefaultPollInterval, DefaultTimeout);
            var result = calculator.CalculateIntervalDelay(4);
            Assert.That(result, Is.EqualTo(TimeSpan.FromMilliseconds(DefaultPollIntervalMs * 8)));
        }

        [Test]
        public void CalculateIntervalDelay_ExponentialBackoff_RespectsTimeoutCap()
        {
            var shortTimeout = TimeSpan.FromMilliseconds(1000); // 1 second
            var calculator = new WaitStrategyCalculator(ExponentialBackoff, DefaultPollDelay, DefaultPollInterval, shortTimeout);
            var result = calculator.CalculateIntervalDelay(10); // Should be capped at 25% of timeout
            var expectedCap = shortTimeout.TotalMilliseconds / 4; // 25% of timeout
            Assert.That(result.TotalMilliseconds, Is.LessThanOrEqualTo(expectedCap));
        }

        [Test]
        public void CalculateIntervalDelay_Aggressive_ReturnsQuarterInterval()
        {
            var calculator = new WaitStrategyCalculator(Aggressive, DefaultPollDelay, DefaultPollInterval, DefaultTimeout);
            var result = calculator.CalculateIntervalDelay(1);
            Assert.That(result, Is.EqualTo(TimeSpan.FromMilliseconds(DefaultPollIntervalMs / 4)));
        }

        [Test]
        public void CalculateIntervalDelay_Aggressive_WithMinimumInterval_ReturnsMinimum1ms()
        {
            var smallInterval = TimeSpan.FromMilliseconds(1);
            var calculator = new WaitStrategyCalculator(Aggressive, DefaultPollDelay, smallInterval, DefaultTimeout);
            var result = calculator.CalculateIntervalDelay(1);
            Assert.That(result, Is.EqualTo(TimeSpan.FromMilliseconds(1)));
        }

        [Test]
        public void CalculateIntervalDelay_Conservative_ReturnsDoubleInterval()
        {
            var calculator = new WaitStrategyCalculator(Conservative, DefaultPollDelay, DefaultPollInterval, DefaultTimeout);
            var result = calculator.CalculateIntervalDelay(1);
            Assert.That(result, Is.EqualTo(TimeSpan.FromMilliseconds(DefaultPollIntervalMs * 2)));
        }

        [Test]
        public void CalculateIntervalDelay_Adaptive_WithoutMetrics_ReturnsBaseInterval()
        {
            var calculator = new WaitStrategyCalculator(Adaptive, DefaultPollDelay, DefaultPollInterval, DefaultTimeout);
            var result = calculator.CalculateIntervalDelay(1);
            Assert.That(result, Is.EqualTo(DefaultPollInterval));
        }

        [Test]
        public void CalculateIntervalDelay_Adaptive_WithMetricsButFewChecks_ReturnsBaseInterval()
        {
            var metrics = new WaitMetrics { ConditionChecks = 1 };
            var calculator = new WaitStrategyCalculator(Adaptive, DefaultPollDelay, DefaultPollInterval, DefaultTimeout, metrics);
            var result = calculator.CalculateIntervalDelay(1);
            Assert.That(result, Is.EqualTo(DefaultPollInterval));
        }

        [Test]
        public void CalculateIntervalDelay_Adaptive_WithFastChecks_ReturnsHalfInterval()
        {
            var metrics = new WaitMetrics
            {
                ConditionChecks = 5,
                TotalTime = TimeSpan.FromMilliseconds(50), // Very fast average
            };
            var calculator = new WaitStrategyCalculator(Adaptive, DefaultPollDelay, DefaultPollInterval, DefaultTimeout, metrics);
            var result = calculator.CalculateIntervalDelay(5);
            Assert.That(result, Is.EqualTo(TimeSpan.FromMilliseconds(DefaultPollIntervalMs / 2)));
        }

        [Test]
        public void CalculateIntervalDelay_Adaptive_WithSlowChecks_ReturnsDoubleInterval()
        {
            var metrics = new WaitMetrics
            {
                ConditionChecks = 5,
                TotalTime = TimeSpan.FromMilliseconds(1500), // Very slow average (300ms per check)
            };
            var calculator = new WaitStrategyCalculator(Adaptive, DefaultPollDelay, DefaultPollInterval, DefaultTimeout, metrics);
            var result = calculator.CalculateIntervalDelay(5);
            Assert.That(result, Is.EqualTo(TimeSpan.FromMilliseconds(DefaultPollIntervalMs * 2)));
        }

        [Test]
        public void CalculateIntervalDelay_Adaptive_WithNormalChecks_ReturnsBaseInterval()
        {
            var metrics = new WaitMetrics
            {
                ConditionChecks = 5,
                TotalTime = TimeSpan.FromMilliseconds(500), // Normal average
            };
            var calculator = new WaitStrategyCalculator(Adaptive, DefaultPollDelay, DefaultPollInterval, DefaultTimeout, metrics);
            var result = calculator.CalculateIntervalDelay(5);
            Assert.That(result, Is.EqualTo(DefaultPollInterval));
        }

        [Test]
        public void CalculateIntervalDelay_ExponentialBackoff_ZeroCheckCount_ReturnsBaseInterval()
        {
            var calculator = new WaitStrategyCalculator(ExponentialBackoff, DefaultPollDelay, DefaultPollInterval, DefaultTimeout);
            var result = calculator.CalculateIntervalDelay(0);
            Assert.That(result, Is.EqualTo(DefaultPollInterval));
        }

        [Test]
        public void CalculateIntervalDelay_ExponentialBackoff_NegativeCheckCount_ReturnsBaseInterval()
        {
            var calculator = new WaitStrategyCalculator(ExponentialBackoff, DefaultPollDelay, DefaultPollInterval, DefaultTimeout);
            var result = calculator.CalculateIntervalDelay(-1);
            Assert.That(result, Is.EqualTo(DefaultPollInterval));
        }

        [Test]
        public void CalculateIntervalDelay_ExponentialBackoff_VeryLargeCheckCount_RespectsTimeoutCap()
        {
            var calculator = new WaitStrategyCalculator(ExponentialBackoff, DefaultPollDelay, DefaultPollInterval, DefaultTimeout);
            var result = calculator.CalculateIntervalDelay(100); // Very large number
            var expectedCap = DefaultTimeout.TotalMilliseconds / 4; // 25% of timeout
            Assert.That(result.TotalMilliseconds, Is.LessThanOrEqualTo(expectedCap));
        }
    }
}
