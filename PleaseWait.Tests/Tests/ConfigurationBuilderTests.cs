// <copyright file="ConfigurationBuilderTests.cs" company="Esdet">
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
    public class ConfigurationBuilderTests
    {
        private ConfigurationBuilder builder = null!;

        [SetUp]
        public void SetUp()
        {
            this.builder = new ConfigurationBuilder();
        }

        [TearDown]
        public void TearDown()
        {
            Wait().ResetToDefaults();
        }

        [Test]
        public void DefaultTimeout_WithTimeSpan_ReturnsThis()
        {
            var timeout = TimeSpan.FromSeconds(30);
            var result = this.builder.DefaultTimeout(timeout);
            Assert.That(result, Is.SameAs(this.builder));
        }

        [Test]
        public void DefaultTimeout_WithSeconds_ReturnsThis()
        {
            var result = this.builder.DefaultTimeout(30, Seconds);
            Assert.That(result, Is.SameAs(this.builder));
        }

        [Test]
        public void DefaultPollDelay_WithTimeSpan_ReturnsThis()
        {
            var delay = TimeSpan.FromMilliseconds(100);
            var result = this.builder.DefaultPollDelay(delay);
            Assert.That(result, Is.SameAs(this.builder));
        }

        [Test]
        public void DefaultPollDelay_WithMilliseconds_ReturnsThis()
        {
            var result = this.builder.DefaultPollDelay(100, Millis);
            Assert.That(result, Is.SameAs(this.builder));
        }

        [Test]
        public void DefaultPollInterval_WithTimeSpan_ReturnsThis()
        {
            var interval = TimeSpan.FromMilliseconds(200);
            var result = this.builder.DefaultPollInterval(interval);
            Assert.That(result, Is.SameAs(this.builder));
        }

        [Test]
        public void DefaultPollInterval_WithMilliseconds_ReturnsThis()
        {
            var result = this.builder.DefaultPollInterval(200, Millis);
            Assert.That(result, Is.SameAs(this.builder));
        }

        [Test]
        public void DefaultLogger_WithLogger_ReturnsThis()
        {
            var logger = new ConsoleLogger();
            var result = this.builder.DefaultLogger(logger);
            Assert.That(result, Is.SameAs(this.builder));
        }

        [Test]
        public void DefaultLogger_WithNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => this.builder.DefaultLogger(null!));
        }

        [Test]
        public void DefaultStrategy_WithStrategy_ReturnsThis()
        {
            var result = this.builder.DefaultStrategy(Linear);
            Assert.That(result, Is.SameAs(this.builder));
        }

        [Test]
        public void DefaultAlias_WithString_ReturnsThis()
        {
            var result = this.builder.DefaultAlias("test alias");
            Assert.That(result, Is.SameAs(this.builder));
        }

        [Test]
        public void DefaultAlias_WithNull_ReturnsThis()
        {
            var result = this.builder.DefaultAlias(null!);
            Assert.That(result, Is.SameAs(this.builder));
        }

        [Test]
        public void DefaultIgnoreExceptions_ReturnsThis()
        {
            var result = this.builder.DefaultIgnoreExceptions(true);
            Assert.That(result, Is.SameAs(this.builder));
        }

        [Test]
        public void DefaultFailSilently_ReturnsThis()
        {
            var result = this.builder.DefaultFailSilently(false);
            Assert.That(result, Is.SameAs(this.builder));
        }

        [Test]
        public void DefaultPrerequisites_ReturnsThis()
        {
            var prereqs = new List<Action>();
            var result = this.builder.DefaultPrerequisites(prereqs);
            Assert.That(result, Is.SameAs(this.builder));
        }

        [Test]
        public void DefaultPrerequisites_WithNull_ReturnsThis()
        {
            var result = this.builder.DefaultPrerequisites(null);
            Assert.That(result, Is.SameAs(this.builder));
        }

        [Test]
        public void AllMethods_DoNotThrowExceptions()
        {
            Assert.DoesNotThrow(() =>
            {
                this.builder
                    .DefaultTimeout(TimeSpan.FromSeconds(30))
                    .DefaultTimeout(30, Seconds)
                    .DefaultPollDelay(TimeSpan.FromMilliseconds(100))
                    .DefaultPollDelay(100, Millis)
                    .DefaultPollInterval(TimeSpan.FromMilliseconds(200))
                    .DefaultPollInterval(200, Millis)
                    .DefaultIgnoreExceptions(true)
                    .DefaultFailSilently(false)
                    .DefaultLogger(new ConsoleLogger())
                    .DefaultStrategy(Linear)
                    .DefaultPrerequisites(new List<Action>())
                    .DefaultPrerequisites(null)
                    .DefaultAlias("test")
                    .DefaultAlias(null!);
            });
        }
    }
}
