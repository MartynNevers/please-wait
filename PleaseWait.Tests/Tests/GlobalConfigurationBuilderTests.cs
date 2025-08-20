// <copyright file="GlobalConfigurationBuilderTests.cs" company="Esdet">
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
    using PleaseWait.Core;
    using PleaseWait.Logging;
    using static PleaseWait.Dsl;
    using static PleaseWait.Strategy.WaitStrategy;
    using static PleaseWait.TimeUnit;

    [TestFixture]
    [Category("Integration")]

    // Set to ParallelScope.None because these tests modify global static state (GlobalDefaults)
    // which could interfere with other tests running in parallel
    [Parallelizable(scope: ParallelScope.None)]
    public class GlobalConfigurationBuilderTests
    {
        private GlobalConfigurationBuilder builder = null!;

        [SetUp]
        public void SetUp()
        {
            this.builder = new GlobalConfigurationBuilder();
        }

        [TearDown]
        public void TearDown()
        {
            Wait().Global().ResetToDefaults();
        }

        [Test]
        public void Logger_WithNull_ThrowsArgumentNullException()
        {
            Assert.That(() => this.builder.Logger(null!), Throws.ArgumentNullException);
        }

        [Test]
        public void Timeout_WithTimeSpan_SetsGlobalDefault()
        {
            var timeout = TimeSpan.FromSeconds(45);
            this.builder.Timeout(timeout);
            Assert.That(GlobalDefaults.Timeout, Is.EqualTo(timeout));
        }

        [Test]
        public void Timeout_WithSeconds_SetsGlobalDefault()
        {
            this.builder.Timeout(60, Seconds);
            Assert.That(GlobalDefaults.Timeout, Is.EqualTo(TimeSpan.FromSeconds(60)));
        }

        [Test]
        public void PollDelay_WithTimeSpan_SetsGlobalDefault()
        {
            var delay = TimeSpan.FromMilliseconds(150);
            this.builder.PollDelay(delay);
            Assert.That(GlobalDefaults.PollDelay, Is.EqualTo(delay));
        }

        [Test]
        public void PollDelay_WithMilliseconds_SetsGlobalDefault()
        {
            this.builder.PollDelay(250, Millis);
            Assert.That(GlobalDefaults.PollDelay, Is.EqualTo(TimeSpan.FromMilliseconds(250)));
        }

        [Test]
        public void PollInterval_WithTimeSpan_SetsGlobalDefault()
        {
            var interval = TimeSpan.FromMilliseconds(300);
            this.builder.PollInterval(interval);
            Assert.That(GlobalDefaults.PollInterval, Is.EqualTo(interval));
        }

        [Test]
        public void PollInterval_WithMilliseconds_SetsGlobalDefault()
        {
            this.builder.PollInterval(400, Millis);
            Assert.That(GlobalDefaults.PollInterval, Is.EqualTo(TimeSpan.FromMilliseconds(400)));
        }

        [Test]
        public void IgnoreExceptions_WithTrue_SetsGlobalDefault()
        {
            this.builder.IgnoreExceptions(true);
            Assert.That(GlobalDefaults.IgnoreExceptions, Is.True);
        }

        [Test]
        public void IgnoreExceptions_WithFalse_SetsGlobalDefault()
        {
            this.builder.IgnoreExceptions(false);
            Assert.That(GlobalDefaults.IgnoreExceptions, Is.False);
        }

        [Test]
        public void FailSilently_WithTrue_SetsGlobalDefault()
        {
            this.builder.FailSilently(true);
            Assert.That(GlobalDefaults.FailSilently, Is.True);
        }

        [Test]
        public void FailSilently_WithFalse_SetsGlobalDefault()
        {
            this.builder.FailSilently(false);
            Assert.That(GlobalDefaults.FailSilently, Is.False);
        }

        [Test]
        public void Logger_SetsGlobalDefault()
        {
            var logger = new ConsoleLogger();
            this.builder.Logger(logger);
            Assert.That(GlobalDefaults.Logger, Is.SameAs(logger));
        }

        [Test]
        public void Strategy_SetsGlobalDefault()
        {
            this.builder.Strategy(ExponentialBackoff);
            Assert.That(GlobalDefaults.Strategy, Is.EqualTo(ExponentialBackoff));
        }

        [Test]
        public void Prereqs_WithList_SetsGlobalDefault()
        {
            var prereqs = new List<Action> { () => { } };
            this.builder.Prereqs(prereqs);
            Assert.That(GlobalDefaults.Prereqs, Is.SameAs(prereqs));
        }

        [Test]
        public void Prereqs_WithNull_SetsGlobalDefault()
        {
            this.builder.Prereqs(null);
            Assert.That(GlobalDefaults.Prereqs, Is.Null);
        }

        [Test]
        public void ExceptionHandling_WithBothParameters_SetsGlobalDefaults()
        {
            this.builder.ExceptionHandling(true, false);
            Assert.That(GlobalDefaults.IgnoreExceptions, Is.True);
            Assert.That(GlobalDefaults.FailSilently, Is.False);
        }

        [Test]
        public void Metrics_WithTrue_SetsGlobalDefault()
        {
            this.builder.Metrics(true);
            Assert.That(GlobalDefaults.Metrics, Is.True);
        }

        [Test]
        public void Metrics_WithFalse_SetsGlobalDefault()
        {
            this.builder.Metrics(false);
            Assert.That(GlobalDefaults.Metrics, Is.False);
        }

        [Test]
        public void Metrics_WithDefault_SetsGlobalDefault()
        {
            this.builder.Metrics();
            Assert.That(GlobalDefaults.Metrics, Is.True);
        }

        [Test]
        public void Alias_WithString_SetsGlobalDefault()
        {
            this.builder.Alias("Test Alias");
            Assert.That(GlobalDefaults.Alias, Is.EqualTo("Test Alias"));
        }

        [Test]
        public void Alias_WithNull_SetsGlobalDefault()
        {
            this.builder.Alias(null);
            Assert.That(GlobalDefaults.Alias, Is.Null);
        }

        [Test]
        public void Polling_TimeSpan_SetsBothDelayAndInterval()
        {
            var delay = TimeSpan.FromMilliseconds(50);
            var interval = TimeSpan.FromMilliseconds(100);
            var result = this.builder.Polling(delay, interval);
            Assert.That(result, Is.SameAs(this.builder));
            Assert.That(GlobalDefaults.PollDelay, Is.EqualTo(delay));
            Assert.That(GlobalDefaults.PollInterval, Is.EqualTo(interval));
        }

        [Test]
        public void Polling_ValueAndUnit_SetsBothDelayAndInterval()
        {
            var result = this.builder.Polling(150, Millis, 250, Millis);
            Assert.That(result, Is.SameAs(this.builder));
            Assert.That(GlobalDefaults.PollDelay, Is.EqualTo(TimeSpan.FromMilliseconds(150)));
            Assert.That(GlobalDefaults.PollInterval, Is.EqualTo(TimeSpan.FromMilliseconds(250)));
        }

        [Test]
        public void Prereq_WithAction_SetsGlobalDefault()
        {
            var prereq = new Action(() => { });
            this.builder.Prereq(prereq);
            Assert.That(GlobalDefaults.Prereqs, Is.Not.Null);
            Assert.That(GlobalDefaults.Prereqs!.Count, Is.EqualTo(1));
            Assert.That(GlobalDefaults.Prereqs[0], Is.SameAs(prereq));
        }

        [Test]
        public void Prereq_WithNull_SetsGlobalDefault()
        {
            this.builder.Prereq(null);
            Assert.That(GlobalDefaults.Prereqs, Is.Null);
        }

        [Test]
        public void WithAndMethods_SupportFluentChaining()
        {
            var result = this.builder
                .Timeout(30, Seconds)
                .With().PollDelay(100, Millis)
                .And().PollInterval(200, Millis)
                .With().IgnoreExceptions(true)
                .And().FailSilently(true)
                .With().Metrics();

            Assert.That(result, Is.SameAs(this.builder));
            Assert.That(GlobalDefaults.Timeout, Is.EqualTo(TimeSpan.FromSeconds(30)));
            Assert.That(GlobalDefaults.PollDelay, Is.EqualTo(TimeSpan.FromMilliseconds(100)));
            Assert.That(GlobalDefaults.PollInterval, Is.EqualTo(TimeSpan.FromMilliseconds(200)));
            Assert.That(GlobalDefaults.IgnoreExceptions, Is.True);
            Assert.That(GlobalDefaults.FailSilently, Is.True);
            Assert.That(GlobalDefaults.Metrics, Is.True);
        }
    }
}
