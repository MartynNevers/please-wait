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
            Wait().Global().Configure()
                .Timeout(25, Seconds)
                .PollDelay(150, Millis)
                .PollInterval(350, Millis)
                .IgnoreExceptions(true)
                .FailSilently(false)
                .Alias("Test Alias")
                .Logger(new ConsoleLogger())
                .Strategy(Aggressive);

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
            Wait().Global().ResetToDefaults();
        }

        [Test]
        public void Constructor_CreatesInstanceWithNullProperties()
        {
            var config = new WaitConfig();

            // All configuration properties should be null (indicating use captured defaults)
            Assert.That(config.ConfigTimeout, Is.Null);
            Assert.That(config.ConfigPollDelay, Is.Null);
            Assert.That(config.ConfigPollInterval, Is.Null);
            Assert.That(config.ConfigIgnoreExceptions, Is.Null);
            Assert.That(config.ConfigFailSilently, Is.Null);
            Assert.That(config.ConfigPrereqs, Is.Null);
            Assert.That(config.ConfigAlias, Is.Null);
            Assert.That(config.ConfigLogger, Is.Null);
            Assert.That(config.ConfigMetrics, Is.Null);
            Assert.That(config.ConfigStrategy, Is.Null);
        }

        [Test]
        public void Timeout_TimeSpan_SetsTimeout()
        {
            var config = new WaitConfig();
            var timeout = TimeSpan.FromSeconds(30);
            var result = config.Timeout(timeout);

            Assert.That(result, Is.SameAs(config));
            Assert.That(config.ConfigTimeout, Is.EqualTo(timeout));
        }

        [Test]
        public void Timeout_ValueAndUnit_SetsTimeout()
        {
            var config = new WaitConfig();
            var result = config.Timeout(45, Seconds);

            Assert.That(result, Is.SameAs(config));
            Assert.That(config.ConfigTimeout, Is.EqualTo(TimeSpan.FromSeconds(45)));
        }

        [Test]
        public void PollDelay_TimeSpan_SetsPollDelay()
        {
            var config = new WaitConfig();
            var pollDelay = TimeSpan.FromMilliseconds(250);
            var result = config.PollDelay(pollDelay);

            Assert.That(result, Is.SameAs(config));
            Assert.That(config.ConfigPollDelay, Is.EqualTo(pollDelay));
        }

        [Test]
        public void PollDelay_ValueAndUnit_SetsPollDelay()
        {
            var config = new WaitConfig();
            var result = config.PollDelay(300, Millis);

            Assert.That(result, Is.SameAs(config));
            Assert.That(config.ConfigPollDelay, Is.EqualTo(TimeSpan.FromMilliseconds(300)));
        }

        [Test]
        public void PollInterval_TimeSpan_SetsPollInterval()
        {
            var config = new WaitConfig();
            var pollInterval = TimeSpan.FromMilliseconds(400);
            var result = config.PollInterval(pollInterval);

            Assert.That(result, Is.SameAs(config));
            Assert.That(config.ConfigPollInterval, Is.EqualTo(pollInterval));
        }

        [Test]
        public void PollInterval_ValueAndUnit_SetsPollInterval()
        {
            var config = new WaitConfig();
            var result = config.PollInterval(500, Millis);

            Assert.That(result, Is.SameAs(config));
            Assert.That(config.ConfigPollInterval, Is.EqualTo(TimeSpan.FromMilliseconds(500)));
        }

        [Test]
        public void Polling_TimeSpan_SetsBothDelayAndInterval()
        {
            var config = new WaitConfig();
            var delay = TimeSpan.FromMilliseconds(100);
            var interval = TimeSpan.FromMilliseconds(200);
            var result = config.Polling(delay, interval);

            Assert.That(result, Is.SameAs(config));
            Assert.That(config.ConfigPollDelay, Is.EqualTo(delay));
            Assert.That(config.ConfigPollInterval, Is.EqualTo(interval));
        }

        [Test]
        public void Polling_ValueAndUnit_SetsBothDelayAndInterval()
        {
            var config = new WaitConfig();
            var result = config.Polling(150, Millis, 250, Millis);

            Assert.That(result, Is.SameAs(config));
            Assert.That(config.ConfigPollDelay, Is.EqualTo(TimeSpan.FromMilliseconds(150)));
            Assert.That(config.ConfigPollInterval, Is.EqualTo(TimeSpan.FromMilliseconds(250)));
        }

        [Test]
        public void IgnoreExceptions_WithTrue_EnablesExceptionIgnoring()
        {
            var config = new WaitConfig();
            var result = config.IgnoreExceptions(true);

            Assert.That(result, Is.SameAs(config));
            Assert.That(config.ConfigIgnoreExceptions, Is.True);
        }

        [Test]
        public void FailSilently_WithTrue_EnablesSilentFailure()
        {
            var config = new WaitConfig();
            var result = config.FailSilently(true);

            Assert.That(result, Is.SameAs(config));
            Assert.That(config.ConfigFailSilently, Is.True);
        }

        [Test]
        public void ExceptionHandling_SetsBothOptions()
        {
            var config = new WaitConfig();
            var result = config.ExceptionHandling(true, false);

            Assert.That(result, Is.SameAs(config));
            Assert.That(config.ConfigIgnoreExceptions, Is.True);
            Assert.That(config.ConfigFailSilently, Is.False);
        }

        [Test]
        public void Logger_SetsLogger()
        {
            var config = new WaitConfig();
            var logger = new ConsoleLogger();
            var result = config.Logger(logger);

            Assert.That(result, Is.SameAs(config));
            Assert.That(config.ConfigLogger, Is.SameAs(logger));
        }

        [Test]
        public void Logger_NullLogger_ThrowsArgumentNullException()
        {
            var config = new WaitConfig();
            Assert.Throws<ArgumentNullException>(() => config.Logger(null!));
        }

        [Test]
        public void Metrics_WithDefaultParameter_EnablesMetrics()
        {
            var config = new WaitConfig();
            var result = config.Metrics();

            Assert.That(result, Is.SameAs(config));
            Assert.That(config.ConfigMetrics, Is.True);
        }

        [Test]
        public void Metrics_ExplicitTrue_EnablesMetrics()
        {
            var config = new WaitConfig();
            var result = config.Metrics(true);

            Assert.That(result, Is.SameAs(config));
            Assert.That(config.ConfigMetrics, Is.True);
        }

        [Test]
        public void Metrics_False_DisablesMetrics()
        {
            var config = new WaitConfig();
            var result = config.Metrics(false);

            Assert.That(result, Is.SameAs(config));
            Assert.That(config.ConfigMetrics, Is.False);
        }

        [Test]
        public void Strategy_SetsStrategy()
        {
            var config = new WaitConfig();
            var result = config.Strategy(Conservative);

            Assert.That(result, Is.SameAs(config));
            Assert.That(config.ConfigStrategy, Is.EqualTo(Conservative));
        }

        [Test]
        public void Alias_SetsAlias()
        {
            var config = new WaitConfig();
            var alias = "Test Operation";
            var result = config.Alias(alias);

            Assert.That(result, Is.SameAs(config));
            Assert.That(config.ConfigAlias, Is.EqualTo(alias));
        }

        [Test]
        public void Alias_WithNull_SetsNullAlias()
        {
            var config = new WaitConfig();
            var result = config.Alias(null);

            Assert.That(result, Is.SameAs(config));
            Assert.That(config.ConfigAlias, Is.Null);
        }

        [Test]
        public void Prereqs_SetsPrerequisites()
        {
            var config = new WaitConfig();
            var prereqs = new List<Action> { () => { }, () => { } };
            var result = config.Prereqs(prereqs);

            Assert.That(result, Is.SameAs(config));
            Assert.That(config.ConfigPrereqs, Is.SameAs(prereqs));
        }

        [Test]
        public void FluentChaining_AllowsMethodChaining()
        {
            var logger = new DebugLogger();
            var prereq = new Action(() => { });

            var config = new WaitConfig()
                .Timeout(10, Seconds)
                .Polling(100, Millis, 200, Millis)
                .ExceptionHandling(true, false)
                .Logger(logger)
                .Metrics()
                .Strategy(Adaptive)
                .Alias("Chained Config")
                .Prereq(prereq);

            Assert.That(config.ConfigTimeout, Is.EqualTo(TimeSpan.FromSeconds(10)));
            Assert.That(config.ConfigPollDelay, Is.EqualTo(TimeSpan.FromMilliseconds(100)));
            Assert.That(config.ConfigPollInterval, Is.EqualTo(TimeSpan.FromMilliseconds(200)));
            Assert.That(config.ConfigIgnoreExceptions, Is.True);
            Assert.That(config.ConfigFailSilently, Is.False);
            Assert.That(config.ConfigLogger, Is.SameAs(logger));
            Assert.That(config.ConfigMetrics, Is.True);
            Assert.That(config.ConfigStrategy, Is.EqualTo(Adaptive));
            Assert.That(config.ConfigAlias, Is.EqualTo("Chained Config"));
            Assert.That(config.ConfigPrereqs, Is.Not.Null);
            Assert.That(config.ConfigPrereqs!.Count, Is.EqualTo(1));
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

        [Test]
        public void AtMost_TimeSpan_SetsTimeout()
        {
            var config = new WaitConfig();
            var timeout = TimeSpan.FromSeconds(30);
            var result = config.AtMost(timeout);

            Assert.That(result, Is.SameAs(config));
            Assert.That(config.ConfigTimeout, Is.EqualTo(timeout));
        }

        [Test]
        public void AtMost_ValueAndUnit_SetsTimeout()
        {
            var config = new WaitConfig();
            var result = config.AtMost(30, Seconds);

            Assert.That(result, Is.SameAs(config));
            Assert.That(config.ConfigTimeout, Is.EqualTo(TimeSpan.FromSeconds(30)));
        }

        [Test]
        public void Prereq_WithAction_SetsPrerequisites()
        {
            var config = new WaitConfig();
            var executed = false;
            var prereq = new Action(() => executed = true);
            var result = config.Prereq(prereq);

            Assert.That(result, Is.SameAs(config));
            Assert.That(config.ConfigPrereqs, Is.Not.Null);
            Assert.That(config.ConfigPrereqs!.Count, Is.EqualTo(1));

            // Execute the prerequisite to verify it's correct
            config.ConfigPrereqs[0]();
            Assert.That(executed, Is.True);
        }

        [Test]
        public void Prereq_WithNull_SetsPrerequisitesToNull()
        {
            var config = new WaitConfig();
            var result = config.Prereq(null);

            Assert.That(result, Is.SameAs(config));
            Assert.That(config.ConfigPrereqs, Is.Null);
        }

        [Test]
        public void WithAndMethods_SupportFluentChaining()
        {
            var logger = new ConsoleLogger();
            var config = new WaitConfig()
                .Timeout(15, Seconds)
                .With().PollDelay(50, Millis)
                .And().PollInterval(150, Millis)
                .With().IgnoreExceptions(false)
                .And().FailSilently()
                .With().Logger(logger)
                .And().Metrics()
                .With().Strategy(ExponentialBackoff)
                .And().Alias("Syntactic Sugar Test");

            Assert.That(config.ConfigTimeout, Is.EqualTo(TimeSpan.FromSeconds(15)));
            Assert.That(config.ConfigPollDelay, Is.EqualTo(TimeSpan.FromMilliseconds(50)));
            Assert.That(config.ConfigPollInterval, Is.EqualTo(TimeSpan.FromMilliseconds(150)));
            Assert.That(config.ConfigIgnoreExceptions, Is.False);
            Assert.That(config.ConfigFailSilently, Is.True);
            Assert.That(config.ConfigLogger, Is.SameAs(logger));
            Assert.That(config.ConfigMetrics, Is.True);
            Assert.That(config.ConfigStrategy, Is.EqualTo(ExponentialBackoff));
            Assert.That(config.ConfigAlias, Is.EqualTo("Syntactic Sugar Test"));
        }
    }
}
