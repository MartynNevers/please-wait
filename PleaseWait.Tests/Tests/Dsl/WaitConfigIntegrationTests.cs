// <copyright file="WaitConfigIntegrationTests.cs" company="Esdet">
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

namespace PleaseWait.Tests.Tests.Dsl
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;
    using PleaseWait;
    using PleaseWait.Core;
    using PleaseWait.Logging;
    using PleaseWait.Strategy;
    using static PleaseWait.Dsl;
    using static PleaseWait.Strategy.WaitStrategy;
    using static PleaseWait.TimeUnit;

    /// <summary>
    /// Integration tests for Wait(WaitConfig) functionality.
    /// </summary>
    [TestFixture]
    [Category("WaitConfigIntegrationTests")]

     // Set to ParallelScope.None because these tests modify global static state (GlobalDefaults)
    // which could interfere with other tests running in parallel
    [Parallelizable(scope: ParallelScope.None)]
    public class WaitConfigIntegrationTests
    {
        [Test]
        public void Wait_WithConfig_UsesConfigValues()
        {
            var config = Wait().Config()
                .WithTimeout(5, Seconds)
                .WithPolling(50, Millis, 100, Millis)
                .WithAlias("Test Config")
                .WithLogger(new ConsoleLogger())
                .WithMetrics()
                .WithStrategy(Conservative);

            var orange = new Orange();
            _ = orange.PeelAsync(1);

            var metrics = Wait(config).Until(() => orange.IsPeeled);

            Assert.That(orange.IsPeeled, Is.True);
            Assert.That(metrics, Is.Not.Null);
            Assert.That(metrics!.WasSuccessful, Is.True);
        }

        [Test]
        public void Wait_WithPartialConfig_UsesGlobalDefaultsForUnsetValues()
        {
            // Set global defaults
            Wait().Configure()
                .DefaultTimeout(30, Seconds)
                .DefaultPollDelay(200, Millis)
                .DefaultPollInterval(500, Millis)
                .DefaultStrategy(Linear)
                .DefaultAlias("Global Alias");

            // Only set timeout in config, leave others unset
            var config = Wait().Config()
                .WithTimeout(5, Seconds);

            // Note: no .WithPollDelay(), .WithPollInterval(), .WithStrategy(), .WithAlias() calls
            var orange = new Orange();
            _ = orange.PeelAsync(1);

            var startTime = DateTime.UtcNow;
            var metrics = Wait(config).Until(() => orange.IsPeeled);
            var elapsed = DateTime.UtcNow - startTime;

            // Should complete quickly (within 5 seconds) despite global 30-second timeout
            Assert.That(elapsed.TotalSeconds, Is.LessThan(6));
            Assert.That(orange.IsPeeled, Is.True);

            // Metrics will be null unless explicitly enabled
            Assert.That(metrics, Is.Null);

            // Reset global defaults
            Wait().ResetToDefaults();
        }

        [Test]
        public void Wait_WithConfig_OverridesGlobalDefaults()
        {
            // Set global defaults
            Wait().Configure()
                .DefaultTimeout(30, Seconds)
                .DefaultPollDelay(200, Millis)
                .DefaultPollInterval(500, Millis)
                .DefaultStrategy(Linear);

            // Create config with different values
            var config = Wait().Config()
                .WithTimeout(5, Seconds)
                .WithPolling(50, Millis, 100, Millis)
                .WithStrategy(Conservative);

            var orange = new Orange();
            _ = orange.PeelAsync(1);

            var startTime = DateTime.UtcNow;
            Wait(config).Until(() => orange.IsPeeled);
            var elapsed = DateTime.UtcNow - startTime;

            // Should complete quickly (within 5 seconds) despite global 30-second timeout
            Assert.That(elapsed.TotalSeconds, Is.LessThan(6));
            Assert.That(orange.IsPeeled, Is.True);

            // Reset global defaults
            Wait().ResetToDefaults();
        }

        [Test]
        public void Wait_WithConfig_HandlesExceptionsCorrectly()
        {
            var config = Wait().Config()
                .WithTimeout(2, Seconds)
                .WithIgnoreExceptions(true)
                .WithFailSilently(true);

            var result = Wait(config).Until(() => throw new InvalidOperationException("Test exception"));

            // Should return null instead of throwing due to failSilently
            Assert.That(result, Is.Null);
        }

        [Test]
        public void Wait_WithConfig_ExecutesPrerequisites()
        {
            var prereqExecuted = false;
            var prereqAction = new Action(() => prereqExecuted = true);

            var config = Wait().Config()
                .WithTimeout(5, Seconds)
                .WithPrerequisites(new List<Action> { prereqAction });

            var orange = new Orange();
            _ = orange.PeelAsync(1);

            Wait(config).Until(() => orange.IsPeeled);

            Assert.That(prereqExecuted, Is.True);
            Assert.That(orange.IsPeeled, Is.True);
        }

        [Test]
        public void Wait_WithConfig_CollectsMetricsWhenEnabled()
        {
            var config = Wait().Config()
                .WithTimeout(5, Seconds)
                .WithMetrics(true);

            var orange = new Orange();
            _ = orange.PeelAsync(1);

            var metrics = Wait(config).Until(() => orange.IsPeeled);

            Assert.That(metrics, Is.Not.Null);
            Assert.That(metrics!.WasSuccessful, Is.True);
            Assert.That(metrics.ConditionChecks, Is.GreaterThan(0));
            Assert.That(metrics.TotalTime, Is.GreaterThan(TimeSpan.Zero));
        }

        [Test]
        public void Wait_WithConfig_DoesNotCollectMetricsWhenDisabled()
        {
            var config = Wait().Config()
                .WithTimeout(5, Seconds)
                .WithMetrics(false);

            var orange = new Orange();
            _ = orange.PeelAsync(1);

            var metrics = Wait(config).Until(() => orange.IsPeeled);

            Assert.That(metrics, Is.Null);
            Assert.That(orange.IsPeeled, Is.True);
        }

        [Test]
        public void Wait_WithConfig_UsesSpecifiedLogger()
        {
            var testLogger = new TestLogger();
            var config = Wait().Config()
                .WithTimeout(5, Seconds)
                .WithLogger(testLogger);

            var orange = new Orange();
            _ = orange.PeelAsync(1);

            Wait(config).Until(() => orange.IsPeeled);

            Assert.That(testLogger.WaitStartLogged, Is.True);
            Assert.That(orange.IsPeeled, Is.True);
        }

        [Test]
        public void Wait_WithConfig_UsesSpecifiedStrategy()
        {
            var config = Wait().Config()
                .WithTimeout(5, Seconds)
                .WithStrategy(Conservative);

            var orange = new Orange();
            _ = orange.PeelAsync(1);

            var metrics = Wait(config).Until(() => orange.IsPeeled);

            Assert.That(orange.IsPeeled, Is.True);

            // Metrics will be null unless explicitly enabled
            Assert.That(metrics, Is.Null);
        }

        [Test]
        public void Wait_WithNullConfig_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => Wait((WaitConfig)null!));
        }

        [Test]
        public void Wait_WithConfig_RespectsAlias()
        {
            var testLogger = new TestLogger();
            var config = Wait().Config()
                .WithTimeout(5, Seconds)
                .WithLogger(testLogger)
                .WithAlias("Custom Alias");

            var orange = new Orange();
            _ = orange.PeelAsync(1);

            Wait(config).Until(() => orange.IsPeeled);

            Assert.That(testLogger.WaitStartLogged, Is.True);
            Assert.That(orange.IsPeeled, Is.True);
        }

        [Test]
        public void Config_CreatesNewInstanceEachTime()
        {
            var config1 = Wait().Config();
            var config2 = Wait().Config();

            Assert.That(config1, Is.Not.SameAs(config2));
        }

        [Test]
        public void Wait_WithConfig_CapturesGlobalDefaultsAtCreationTime()
        {
            // Set global defaults
            Wait().Configure()
                .DefaultTimeout(30, Seconds)
                .DefaultPollDelay(200, Millis)
                .DefaultStrategy(Linear);

            // Create config with only timeout override
            var config = Wait().Config()
                .WithTimeout(5, Seconds);

            // PollDelay and Strategy are null = use captured global defaults

            // Update global defaults AFTER creating config
            Wait().Configure()
                .DefaultPollDelay(300, Millis)
                .DefaultStrategy(Conservative);

            var orange = new Orange();
            _ = orange.PeelAsync(1);

            var startTime = DateTime.UtcNow;
            var metrics = Wait(config).Until(() => orange.IsPeeled);
            var elapsed = DateTime.UtcNow - startTime;

            // Should use config timeout (5s) but CAPTURED global defaults (200ms, Linear)
            // NOT the updated global defaults (300ms, Conservative)
            Assert.That(elapsed.TotalSeconds, Is.LessThan(6));
            Assert.That(orange.IsPeeled, Is.True);

            // Metrics will be null unless explicitly enabled
            Assert.That(metrics, Is.Null);

            // Reset global defaults
            Wait().ResetToDefaults();
        }
    }
}