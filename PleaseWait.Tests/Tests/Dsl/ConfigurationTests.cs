// <copyright file="ConfigurationTests.cs" company="Esdet">
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
    using System.Collections.Generic;
    using NUnit.Framework;
    using PleaseWait.Logging;
    using static PleaseWait.Dsl;
    using static PleaseWait.Strategy.WaitStrategy;
    using static PleaseWait.TimeUnit;

    [TestFixture]
    [Category("Configuration")]

    // Set to ParallelScope.None because these tests modify global static state (GlobalDefaults)
    // which could interfere with other tests running in parallel
    [Parallelizable(scope: ParallelScope.None)]
    public class ConfigurationTests
    {
        [TearDown]
        public void TearDown()
        {
            Wait().Global().ResetToDefaults();
        }

        [Test]
        public void Global_Configure_ReturnsGlobalConfigurationBuilder()
        {
            var builder = Wait().Global().Configure();
            Assert.That(builder, Is.Not.Null);
            Assert.That(builder, Is.InstanceOf<GlobalConfigurationBuilder>());
        }

        [Test]
        public void Timeout_WithTimeSpan_SetsGlobalDefault()
        {
            var expectedTimeout = TimeSpan.FromSeconds(30);

            Wait().Global().Configure()
                .Timeout(expectedTimeout);

            // Use metrics to verify the configuration was applied
            var metrics = Wait().Metrics().Until(() => true);

            Assert.That(metrics, Is.Not.Null);
            Assert.That(metrics.ConfiguredTimeout, Is.EqualTo(expectedTimeout));
        }

        [Test]
        public void Timeout_WithValueAndTimeUnit_SetsGlobalDefault()
        {
            var expectedTimeout = TimeSpan.FromMinutes(5);

            Wait().Global().Configure()
                .Timeout(5, Minutes);

            // Use metrics to verify the configuration was applied
            var metrics = Wait().Metrics().Until(() => true);

            Assert.That(metrics, Is.Not.Null);
            Assert.That(metrics.ConfiguredTimeout, Is.EqualTo(expectedTimeout));
        }

        [Test]
        public void PollDelay_WithTimeSpan_SetsGlobalDefault()
        {
            var expectedDelay = TimeSpan.FromMilliseconds(50);

            Wait().Global().Configure()
                .PollDelay(expectedDelay);

            // Use metrics to verify the configuration was applied
            var metrics = Wait().Metrics().Until(() => true);

            Assert.That(metrics, Is.Not.Null);
            Assert.That(metrics.ConfiguredPollDelay, Is.EqualTo(expectedDelay));
        }

        [Test]
        public void PollDelay_WithValueAndTimeUnit_SetsGlobalDefault()
        {
            var expectedDelay = TimeSpan.FromMilliseconds(200);

            Wait().Global().Configure()
                .PollDelay(200, Millis);

            // Use metrics to verify the configuration was applied
            var metrics = Wait().Metrics().Until(() => true);

            Assert.That(metrics, Is.Not.Null);
            Assert.That(metrics.ConfiguredPollDelay, Is.EqualTo(expectedDelay));
        }

        [Test]
        public void PollInterval_WithTimeSpan_SetsGlobalDefault()
        {
            var expectedInterval = TimeSpan.FromMilliseconds(100);

            Wait().Global().Configure()
                .PollInterval(expectedInterval);

            // Use metrics to verify the configuration was applied
            var metrics = Wait().Metrics().Until(() => true);

            Assert.That(metrics, Is.Not.Null);
            Assert.That(metrics.ConfiguredPollInterval, Is.EqualTo(expectedInterval));
        }

        [Test]
        public void PollInterval_WithValueAndTimeUnit_SetsGlobalDefault()
        {
            var expectedInterval = TimeSpan.FromMilliseconds(300);

            Wait().Global().Configure()
                .PollInterval(300, Millis);

            // Use metrics to verify the configuration was applied
            var metrics = Wait().Metrics().Until(() => true);

            Assert.That(metrics, Is.Not.Null);
            Assert.That(metrics.ConfiguredPollInterval, Is.EqualTo(expectedInterval));
        }

        [Test]
        public void IgnoreExceptions_SetsGlobalDefault()
        {
            Wait().Global().Configure()
                .IgnoreExceptions(true);

            // Test that the configuration was applied by checking behavior
            // This should not throw an exception since we're ignoring them
            var counter = 0;
            Assert.DoesNotThrow(() =>
            {
                Wait().AtMost(500, Millis).PollDelay(10, Millis).PollInterval(10, Millis).Until(() =>
                {
                    counter++;
                    if (counter < 5)
                    {
                        throw new InvalidOperationException("Test exception");
                    }

                    return true;
                });
            });

            Assert.That(counter, Is.GreaterThan(0));
        }

        [Test]
        public void FailSilently_SetsGlobalDefault()
        {
            Wait().Global().Configure()
                .FailSilently(true);

            // Test that the configuration was applied by checking behavior
            // This should not throw an exception since we're failing silently
            var result = Wait().AtMost(10, Millis).Until(() => false);
            Assert.That(result, Is.Null);
        }

        [Test]
        public void Logger_SetsGlobalDefault()
        {
            var testLogger = new TestLogger();

            Wait().Global().Configure()
                .Logger(testLogger);

            // Test that the configuration was applied by checking logging behavior
            Wait().AtMost(10, Millis).Until(() => true);

            Assert.That(testLogger.WaitStartLogged, Is.True);
            Assert.That(testLogger.WaitSuccessLogged, Is.True);
        }

        [Test]
        public void Logger_WithNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                Wait().Global().Configure().Logger(null!));
        }

        [Test]
        public void Strategy_SetsGlobalDefault()
        {
            // Test that different strategies produce different behavior
            var aggressiveMetrics = Wait().Metrics().Until(() => true);
            Assert.That(aggressiveMetrics, Is.Not.Null);
            Wait().Global().ResetToDefaults();

            Wait().Global().Configure()
                .Strategy(Conservative);

            var conservativeMetrics = Wait().Metrics().Until(() => true);
            Assert.That(conservativeMetrics, Is.Not.Null);

            // Verify that the strategy was applied by comparing behavior
            Assert.That(aggressiveMetrics!.TotalTime, Is.LessThan(conservativeMetrics!.TotalTime));
        }

        [Test]
        public void Prerequisites_SetsGlobalDefault()
        {
            var executed = false;
            var prereqs = new List<Action> { () => executed = true };

            Wait().Global().Configure()
                .Prereqs(prereqs);

            // Test that the configuration was applied by checking behavior
            Wait().AtMost(10, Millis).Until(() => true);

            Assert.That(executed, Is.True);
        }

        [Test]
        public void Alias_SetsGlobalDefault()
        {
            var expectedAlias = "Test Alias";

            Wait().Global().Configure()
                .Alias(expectedAlias);

            // Use metrics to verify the configuration was applied
            var metrics = Wait().Metrics().Until(() => true);

            Assert.That(metrics, Is.Not.Null);
            Assert.That(metrics.ConditionAlias, Is.EqualTo(expectedAlias));
        }

        [Test]
        public void GlobalConfigurationBuilder_SupportsMethodChaining()
        {
            var result = Wait().Global().Configure()
                .Timeout(TimeSpan.FromSeconds(30))
                .PollDelay(TimeSpan.FromMilliseconds(100))
                .PollInterval(TimeSpan.FromMilliseconds(200))
                .IgnoreExceptions(true)
                .FailSilently(true)
                .Logger(new ConsoleLogger())
                .Strategy(Linear)
                .Prereqs(new List<Action>())
                .Alias("test");

            Assert.That(result, Is.InstanceOf<GlobalConfigurationBuilder>());
        }

        [Test]
        public void ResetToDefaults_ResetsAllConfiguration()
        {
            // Set some custom defaults
            Wait().Global().Configure()
                .Timeout(TimeSpan.FromSeconds(60))
                .Alias("Custom Alias");

            // Reset to defaults
            Wait().Global().ResetToDefaults();

            // Verify reset by checking metrics
            var metrics = Wait().Metrics().Until(() => true);

            Assert.That(metrics, Is.Not.Null);
            Assert.That(metrics.ConfiguredTimeout, Is.EqualTo(TimeSpan.FromSeconds(10))); // Default value
            Assert.That(metrics.ConditionAlias, Is.Null); // Default value
        }
    }
}
