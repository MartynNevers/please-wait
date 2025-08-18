// <copyright file="TimeoutAndExceptionsTests.cs" company="Esdet">
// Copyright 2023 the original author or authors.
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
    using static PleaseWait.Dsl;
    using static PleaseWait.TimeUnit;

    [TestFixture]
    [Category("Timeout & Exceptions")]
    [Parallelizable(scope: ParallelScope.All)]
    public class TimeoutAndExceptionsTests
    {
        [Test]
        public void Until_ConditionUnattainable_ThrowsTimeoutException()
        {
            var orange = new Orange();
            var wait = Wait().AtMost(5, SECONDS);
            var ex = Assert.Throws<TimeoutException>(() => wait.Until(() => orange.IsPeeled));
            Assert.That(ex.Message, Is.EqualTo("Condition was not fulfilled within 00:00:05."));
        }

        [Test]
        public void Until_ConditionAttainableAfterTimeout_ThrowsTimeoutException()
        {
            var orange = new Orange();
            var wait = Wait().AtMost(2, SECONDS);
            _ = orange.PeelAsync(5);
            var ex = Assert.Throws<TimeoutException>(() => wait.Until(() => orange.IsPeeled));
            Assert.That(ex.Message, Is.EqualTo("Condition was not fulfilled within 00:00:02."));
        }

        [Test]
        public void Until_AliasProvidedAndTimeoutOccurs_ExceptionMessageContainsAlias()
        {
            var alias = "Is orange peeled?";
            var orange = new Orange();
            var wait = Wait().AtMost(1, SECONDS).With().Alias(alias);
            var ex = Assert.Throws<TimeoutException>(() => wait.Until(() => orange.IsPeeled));
            Assert.That(ex.Message, Is.EqualTo($"Condition with alias '{alias}' was not fulfilled within 00:00:01."));
        }

        [Test]
        public void Until_FailSilentlyAndTimeoutOccurs_ExitsGracefully()
        {
            var orange = new Orange();
            _ = orange.PeelAsync(5);
            Wait()
                .AtMost(2, SECONDS)
                .And().FailSilently()
                .Until(() => orange.IsPeeled);

            Assert.That(orange.IsPeeled, Is.False);
        }

        [Test]
        public void Until_ConditionThrowsException_ExceptionIsSwallowed()
        {
            var orange = new Orange();
            Wait()
                .AtMost(5, SECONDS)
                .With().PollDelay(50, MILLIS)
                .And().With().PollInterval(50, MILLIS)
                .Until(() => orange.CountSegments() > 8);

            Assert.That(orange.CountSegments(), Is.GreaterThan(8).And.LessThan(12));
        }

        [Test]
        public void Until_PrerequisiteThrowsException_ExceptionIsSwallowed()
        {
            var orange = new Orange();
            Wait()
                .AtMost(5, SECONDS)
                .With().Prereq(() => orange.CountSegments())
                .And().FailSilently()
                .Until(() => false);

            Assert.That(orange.CountSegments(), Is.GreaterThan(8).And.LessThan(12));
        }

        [Test]
        public void Until_IgnoreExceptionsFalseAndConditionThrowsException_ExceptionIsThrown()
        {
            var orange = new Orange();
            var wait = Wait()
                .AtMost(1, SECONDS)
                .With().PollDelay(50, MILLIS)
                .And().With().PollInterval(50, MILLIS)
                .And().IgnoreExceptions(false);

            var ex = Assert.Throws<InvalidOperationException>(() => wait.Until(() => orange.CountSegments() > 8));
            Assert.That(ex.Message, Is.EqualTo("Try again"));
        }

        [Test]
        public void Until_IgnoreExceptionsFalseAndPrerequisiteThrowsException_ExceptionIsThrown()
        {
            var orange = new Orange();
            var wait = Wait()
                .AtMost(5, SECONDS)
                .With().Prereq(() => orange.CountSegments())
                .And().IgnoreExceptions(false)
                .And().FailSilently();

            var ex = Assert.Throws<InvalidOperationException>(() => wait.Until(() => false));
            Assert.That(ex.Message, Is.EqualTo("Try again"));
        }
    }
}
