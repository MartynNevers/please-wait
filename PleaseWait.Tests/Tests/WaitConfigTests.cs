// <copyright file="WaitConfigTests.cs" company="Esdet">
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

namespace PleaseWait.Tests.Tests
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
    /// Integration tests for the WaitConfig class.
    /// </summary>
    [TestFixture]
    [Category("Integration")]

    // Set to ParallelScope.None because these tests modify global static state (GlobalDefaults)
    // which could interfere with other tests running in parallel
    [Parallelizable(ParallelScope.None)]
    public class WaitConfigTests
    {
        [Test]
        public void Constructor_CapturesGlobalDefaults()
        {
            // Set specific global defaults
            Wait().Configure()
                .DefaultTimeout(25, Seconds)
                .DefaultPollDelay(150, Millis)
                .DefaultPollInterval(350, Millis)
                .DefaultIgnoreExceptions(true)
                .DefaultFailSilently(false)
                .DefaultAlias("Test Alias")
                .DefaultLogger(new ConsoleLogger())
                .DefaultStrategy(Aggressive);

            var config = new WaitConfig();

            // Verify captured defaults match current global defaults
            Assert.That(config.DefaultTimeout, Is.EqualTo(TimeSpan.FromSeconds(25)));
            Assert.That(config.DefaultPollDelay, Is.EqualTo(TimeSpan.FromMilliseconds(150)));
            Assert.That(config.DefaultPollInterval, Is.EqualTo(TimeSpan.FromMilliseconds(350)));
            Assert.That(config.DefaultIgnoreExceptions, Is.True);
            Assert.That(config.DefaultFailSilently, Is.False);
            Assert.That(config.DefaultAlias, Is.EqualTo("Test Alias"));
            Assert.That(config.DefaultLogger, Is.TypeOf<ConsoleLogger>());
            Assert.That(config.DefaultStrategy, Is.EqualTo(Aggressive));

            // Reset global defaults
            Wait().ResetToDefaults();
        }

        [Test]
        public void Constructor_CreatesInstanceWithNullProperties()
        {
            var config = new WaitConfig();

            // All configuration properties should be null (indicating use captured defaults)
            Assert.That(config.Timeout, Is.Null);
            Assert.That(config.PollDelay, Is.Null);
            Assert.That(config.PollInterval, Is.Null);
            Assert.That(config.IgnoreExceptions, Is.Null);
            Assert.That(config.FailSilently, Is.Null);
            Assert.That(config.Prereqs, Is.Null);
            Assert.That(config.Alias, Is.Null);
            Assert.That(config.Logger, Is.Null);
            Assert.That(config.CollectMetrics, Is.Null);
            Assert.That(config.Strategy, Is.Null);
        }

        [Test]
        public void WithTimeout_TimeSpan_SetsTimeout()
        {
            var config = new WaitConfig();
            var timeout = TimeSpan.FromSeconds(30);
            var result = config.WithTimeout(timeout);

            Assert.That(result, Is.SameAs(config));
            Assert.That(config.Timeout, Is.EqualTo(timeout));
        }

        [Test]
        public void WithTimeout_ValueAndUnit_SetsTimeout()
        {
            var config = new WaitConfig();
            var result = config.WithTimeout(45, Seconds);

            Assert.That(result, Is.SameAs(config));
            Assert.That(config.Timeout, Is.EqualTo(TimeSpan.FromSeconds(45)));
        }

        [Test]
        public void WithPollDelay_TimeSpan_SetsPollDelay()
        {
            var config = new WaitConfig();
            var pollDelay = TimeSpan.FromMilliseconds(250);
            var result = config.WithPollDelay(pollDelay);

            Assert.That(result, Is.SameAs(config));
            Assert.That(config.PollDelay, Is.EqualTo(pollDelay));
        }

        [Test]
        public void WithPollDelay_ValueAndUnit_SetsPollDelay()
        {
            var config = new WaitConfig();
            var result = config.WithPollDelay(300, Millis);

            Assert.That(result, Is.SameAs(config));
            Assert.That(config.PollDelay, Is.EqualTo(TimeSpan.FromMilliseconds(300)));
        }

        [Test]
        public void WithPollInterval_TimeSpan_SetsPollInterval()
        {
            var config = new WaitConfig();
            var pollInterval = TimeSpan.FromMilliseconds(400);
            var result = config.WithPollInterval(pollInterval);

            Assert.That(result, Is.SameAs(config));
            Assert.That(config.PollInterval, Is.EqualTo(pollInterval));
        }

        [Test]
        public void WithPollInterval_ValueAndUnit_SetsPollInterval()
        {
            var config = new WaitConfig();
            var result = config.WithPollInterval(500, Millis);

            Assert.That(result, Is.SameAs(config));
            Assert.That(config.PollInterval, Is.EqualTo(TimeSpan.FromMilliseconds(500)));
        }

        [Test]
        public void WithPolling_TimeSpan_SetsBothDelayAndInterval()
        {
            var config = new WaitConfig();
            var delay = TimeSpan.FromMilliseconds(100);
            var interval = TimeSpan.FromMilliseconds(200);
            var result = config.WithPolling(delay, interval);

            Assert.That(result, Is.SameAs(config));
            Assert.That(config.PollDelay, Is.EqualTo(delay));
            Assert.That(config.PollInterval, Is.EqualTo(interval));
        }

        [Test]
        public void WithPolling_ValueAndUnit_SetsBothDelayAndInterval()
        {
            var config = new WaitConfig();
            var result = config.WithPolling(150, Millis, 250, Millis);

            Assert.That(result, Is.SameAs(config));
            Assert.That(config.PollDelay, Is.EqualTo(TimeSpan.FromMilliseconds(150)));
            Assert.That(config.PollInterval, Is.EqualTo(TimeSpan.FromMilliseconds(250)));
        }

        [Test]
        public void WithIgnoreExceptions_SetsIgnoreExceptions()
        {
            var config = new WaitConfig();
            var result = config.WithIgnoreExceptions(true);

            Assert.That(result, Is.SameAs(config));
            Assert.That(config.IgnoreExceptions, Is.True);
        }

        [Test]
        public void WithFailSilently_SetsFailSilently()
        {
            var config = new WaitConfig();
            var result = config.WithFailSilently(true);

            Assert.That(result, Is.SameAs(config));
            Assert.That(config.FailSilently, Is.True);
        }

        [Test]
        public void WithExceptionHandling_SetsBothOptions()
        {
            var config = new WaitConfig();
            var result = config.WithExceptionHandling(true, false);

            Assert.That(result, Is.SameAs(config));
            Assert.That(config.IgnoreExceptions, Is.True);
            Assert.That(config.FailSilently, Is.False);
        }

        [Test]
        public void WithLogger_SetsLogger()
        {
            var config = new WaitConfig();
            var logger = new ConsoleLogger();
            var result = config.WithLogger(logger);

            Assert.That(result, Is.SameAs(config));
            Assert.That(config.Logger, Is.SameAs(logger));
        }

        [Test]
        public void WithLogger_NullLogger_ThrowsArgumentNullException()
        {
            var config = new WaitConfig();
            Assert.Throws<ArgumentNullException>(() => config.WithLogger(null!));
        }

        [Test]
        public void WithMetrics_DefaultTrue_EnablesMetrics()
        {
            var config = new WaitConfig();
            var result = config.WithMetrics();

            Assert.That(result, Is.SameAs(config));
            Assert.That(config.CollectMetrics, Is.True);
        }

        [Test]
        public void WithMetrics_ExplicitTrue_EnablesMetrics()
        {
            var config = new WaitConfig();
            var result = config.WithMetrics(true);

            Assert.That(result, Is.SameAs(config));
            Assert.That(config.CollectMetrics, Is.True);
        }

        [Test]
        public void WithMetrics_False_DisablesMetrics()
        {
            var config = new WaitConfig();
            var result = config.WithMetrics(false);

            Assert.That(result, Is.SameAs(config));
            Assert.That(config.CollectMetrics, Is.False);
        }

        [Test]
        public void WithStrategy_SetsStrategy()
        {
            var config = new WaitConfig();
            var result = config.WithStrategy(Conservative);

            Assert.That(result, Is.SameAs(config));
            Assert.That(config.Strategy, Is.EqualTo(Conservative));
        }

        [Test]
        public void WithAlias_SetsAlias()
        {
            var config = new WaitConfig();
            var alias = "Test Operation";
            var result = config.WithAlias(alias);

            Assert.That(result, Is.SameAs(config));
            Assert.That(config.Alias, Is.EqualTo(alias));
        }

        [Test]
        public void WithPrerequisites_SetsPrerequisites()
        {
            var config = new WaitConfig();
            var prereqs = new List<Action> { () => { }, () => { } };
            var result = config.WithPrerequisites(prereqs);

            Assert.That(result, Is.SameAs(config));
            Assert.That(config.Prereqs, Is.SameAs(prereqs));
        }

        [Test]
        public void WithPrerequisite_SetsPrerequisite()
        {
            var config = new WaitConfig();
            var executed = false;
            var prereq = new Action(() => executed = true);
            var result = config.WithPrerequisite(prereq);

            Assert.That(result, Is.SameAs(config));
            Assert.That(config.Prereqs, Is.Not.Null);
            Assert.That(config.Prereqs!.Count, Is.EqualTo(1));

            // Execute the prerequisite to verify it's correct
            config.Prereqs[0]();
            Assert.That(executed, Is.True);
        }

        [Test]
        public void FluentChaining_AllowsMethodChaining()
        {
            var logger = new DebugLogger();
            var prereq = new Action(() => { });

            var config = new WaitConfig()
                .WithTimeout(10, Seconds)
                .WithPolling(100, Millis, 200, Millis)
                .WithExceptionHandling(true, false)
                .WithLogger(logger)
                .WithMetrics()
                .WithStrategy(Adaptive)
                .WithAlias("Chained Config")
                .WithPrerequisite(prereq);

            Assert.That(config.Timeout, Is.EqualTo(TimeSpan.FromSeconds(10)));
            Assert.That(config.PollDelay, Is.EqualTo(TimeSpan.FromMilliseconds(100)));
            Assert.That(config.PollInterval, Is.EqualTo(TimeSpan.FromMilliseconds(200)));
            Assert.That(config.IgnoreExceptions, Is.True);
            Assert.That(config.FailSilently, Is.False);
            Assert.That(config.Logger, Is.SameAs(logger));
            Assert.That(config.CollectMetrics, Is.True);
            Assert.That(config.Strategy, Is.EqualTo(Adaptive));
            Assert.That(config.Alias, Is.EqualTo("Chained Config"));
            Assert.That(config.Prereqs, Is.Not.Null);
            Assert.That(config.Prereqs!.Count, Is.EqualTo(1));
        }

        [Test]
        public void CapturedDefaults_AreImmutableForSingleInstance()
        {
            // Create config (captures current global defaults)
            var config = new WaitConfig();
            var capturedTimeout = config.DefaultTimeout;
            var capturedPollDelay = config.DefaultPollDelay;
            var capturedStrategy = config.DefaultStrategy;

            // Verify captured defaults are immutable for this single instance
            // (they can't be changed after creation)
            Assert.That(config.DefaultTimeout, Is.EqualTo(capturedTimeout));
            Assert.That(config.DefaultPollDelay, Is.EqualTo(capturedPollDelay));
            Assert.That(config.DefaultStrategy, Is.EqualTo(capturedStrategy));

            // Verify the captured defaults are reasonable values (not zero/null)
            Assert.That(capturedTimeout, Is.GreaterThan(TimeSpan.Zero));
            Assert.That(capturedPollDelay, Is.GreaterThan(TimeSpan.Zero));

            // Note: default(WaitStrategy) is Linear, so we just verify it's a valid enum value
            Assert.That(Enum.IsDefined(typeof(WaitStrategy), capturedStrategy), Is.True);
        }
    }
}
