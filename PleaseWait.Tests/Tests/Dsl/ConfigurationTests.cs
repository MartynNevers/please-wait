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
            Wait().ResetToDefaults();
        }

        [Test]
        public void Configure_ReturnsConfigurationBuilder()
        {
            var builder = Wait().Configure();
            Assert.That(builder, Is.Not.Null);
            Assert.That(builder, Is.InstanceOf<ConfigurationBuilder>());
        }

        [Test]
        public void DefaultTimeout_WithTimeSpan_SetsGlobalDefault()
        {
            var expectedTimeout = TimeSpan.FromSeconds(30);

            Wait().Configure()
                .DefaultTimeout(expectedTimeout);

            // Use metrics to verify the configuration was applied
            var metrics = Wait().WithMetrics().Until(() => true);

            Assert.That(metrics, Is.Not.Null);
            Assert.That(metrics.ConfiguredTimeout, Is.EqualTo(expectedTimeout));
        }

        [Test]
        public void DefaultTimeout_WithValueAndTimeUnit_SetsGlobalDefault()
        {
            var expectedTimeout = TimeSpan.FromMinutes(5);

            Wait().Configure()
                .DefaultTimeout(5, Minutes);

            // Use metrics to verify the configuration was applied
            var metrics = Wait().WithMetrics().Until(() => true);

            Assert.That(metrics, Is.Not.Null);
            Assert.That(metrics.ConfiguredTimeout, Is.EqualTo(expectedTimeout));
        }

        [Test]
        public void DefaultPollDelay_WithTimeSpan_SetsGlobalDefault()
        {
            var expectedDelay = TimeSpan.FromMilliseconds(50);

            Wait().Configure()
                .DefaultPollDelay(expectedDelay);

            // Use metrics to verify the configuration was applied
            var metrics = Wait().WithMetrics().Until(() => true);

            Assert.That(metrics, Is.Not.Null);
            Assert.That(metrics.ConfiguredPollDelay, Is.EqualTo(expectedDelay));
        }

        [Test]
        public void DefaultPollDelay_WithValueAndTimeUnit_SetsGlobalDefault()
        {
            var expectedDelay = TimeSpan.FromMilliseconds(200);

            Wait().Configure()
                .DefaultPollDelay(200, Millis);

            // Use metrics to verify the configuration was applied
            var metrics = Wait().WithMetrics().Until(() => true);

            Assert.That(metrics, Is.Not.Null);
            Assert.That(metrics.ConfiguredPollDelay, Is.EqualTo(expectedDelay));
        }

        [Test]
        public void DefaultPollInterval_WithTimeSpan_SetsGlobalDefault()
        {
            var expectedInterval = TimeSpan.FromMilliseconds(100);

            Wait().Configure()
                .DefaultPollInterval(expectedInterval);

            // Use metrics to verify the configuration was applied
            var metrics = Wait().WithMetrics().Until(() => true);

            Assert.That(metrics, Is.Not.Null);
            Assert.That(metrics.ConfiguredPollInterval, Is.EqualTo(expectedInterval));
        }

        [Test]
        public void DefaultPollInterval_WithValueAndTimeUnit_SetsGlobalDefault()
        {
            var expectedInterval = TimeSpan.FromMilliseconds(300);

            Wait().Configure()
                .DefaultPollInterval(300, Millis);

            // Use metrics to verify the configuration was applied
            var metrics = Wait().WithMetrics().Until(() => true);

            Assert.That(metrics, Is.Not.Null);
            Assert.That(metrics.ConfiguredPollInterval, Is.EqualTo(expectedInterval));
        }

        [Test]
        public void DefaultIgnoreExceptions_SetsGlobalDefault()
        {
            Wait().Configure()
                .DefaultIgnoreExceptions(true);

            // Test that the configuration was applied by checking behavior
            // This should not throw an exception since we're ignoring them
            var counter = 0;
            Assert.DoesNotThrow(() =>
            {
                Wait().AtMost(500, Millis).With().PollDelay(10, Millis).With().PollInterval(10, Millis).Until(() =>
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
        public void DefaultFailSilently_SetsGlobalDefault()
        {
            Wait().Configure()
                .DefaultFailSilently(true);

            // Test that the configuration was applied by checking behavior
            // This should not throw an exception since we're failing silently
            var result = Wait().AtMost(10, Millis).Until(() => false);
            Assert.That(result, Is.Null);
        }

        [Test]
        public void DefaultLogger_SetsGlobalDefault()
        {
            var testLogger = new TestLogger();

            Wait().Configure()
                .DefaultLogger(testLogger);

            // Test that the configuration was applied by checking logging behavior
            Wait().AtMost(10, Millis).Until(() => true);

            Assert.That(testLogger.WaitStartLogged, Is.True);
            Assert.That(testLogger.WaitSuccessLogged, Is.True);
        }

        [Test]
        public void DefaultLogger_WithNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                Wait().Configure().DefaultLogger(null!));
        }

        [Test]
        public void DefaultStrategy_SetsGlobalDefault()
        {
            // Test that different strategies produce different behavior
            var aggressiveMetrics = Wait().WithMetrics().Until(() => true);
            Assert.That(aggressiveMetrics, Is.Not.Null);
            Wait().ResetToDefaults();

            Wait().Configure()
                .DefaultStrategy(Conservative);

            var conservativeMetrics = Wait().WithMetrics().Until(() => true);
            Assert.That(conservativeMetrics, Is.Not.Null);

            // Verify that the strategy was applied by comparing behavior
            Assert.That(aggressiveMetrics!.TotalTime, Is.LessThan(conservativeMetrics!.TotalTime));
        }

        [Test]
        public void DefaultPrerequisites_SetsGlobalDefault()
        {
            var executed = false;
            var prereqs = new List<Action> { () => executed = true };

            Wait().Configure()
                .DefaultPrerequisites(prereqs);

            // Test that the configuration was applied by checking behavior
            Wait().AtMost(10, Millis).Until(() => true);

            Assert.That(executed, Is.True);
        }

        [Test]
        public void DefaultAlias_SetsGlobalDefault()
        {
            var expectedAlias = "Test Alias";

            Wait().Configure()
                .DefaultAlias(expectedAlias);

            // Use metrics to verify the configuration was applied
            var metrics = Wait().WithMetrics().Until(() => true);

            Assert.That(metrics, Is.Not.Null);
            Assert.That(metrics.ConditionAlias, Is.EqualTo(expectedAlias));
        }

        [Test]
        public void ConfigurationBuilder_SupportsMethodChaining()
        {
            var result = Wait().Configure()
                .DefaultTimeout(TimeSpan.FromSeconds(30))
                .DefaultPollDelay(TimeSpan.FromMilliseconds(100))
                .DefaultPollInterval(TimeSpan.FromMilliseconds(200))
                .DefaultIgnoreExceptions(true)
                .DefaultFailSilently(true)
                .DefaultLogger(new ConsoleLogger())
                .DefaultStrategy(Linear)
                .DefaultPrerequisites(new List<Action>())
                .DefaultAlias("test");

            Assert.That(result, Is.InstanceOf<ConfigurationBuilder>());
        }

        [Test]
        public void ResetToDefaults_ResetsAllConfiguration()
        {
            // Set some custom defaults
            Wait().Configure()
                .DefaultTimeout(TimeSpan.FromSeconds(60))
                .DefaultAlias("Custom Alias");

            // Reset to defaults
            Wait().ResetToDefaults();

            // Verify reset by checking metrics
            var metrics = Wait().WithMetrics().Until(() => true);

            Assert.That(metrics, Is.Not.Null);
            Assert.That(metrics.ConfiguredTimeout, Is.EqualTo(TimeSpan.FromSeconds(10))); // Default value
            Assert.That(metrics.ConditionAlias, Is.Null); // Default value
        }
    }
}
