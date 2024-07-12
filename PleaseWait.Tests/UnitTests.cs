// <copyright file="UnitTests.cs" company="Esdet">
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
    using System.Collections.Generic;
    using NUnit.Framework;
    using static PleaseWait.Dsl;
    using static PleaseWait.TimeUnit;

    [TestFixture]
    [Parallelizable(scope: ParallelScope.All)]
    public class UnitTests
    {
        [Test]
        public void WhenInstanceIsCreatedThenFieldsAreSetToTheirDefaultsTest()
        {
            var wait = Wait();
            var shouldFailSilently = wait.GetFieldValue<bool>("failSilently");
            var shouldIgnoreExceptions = wait.GetFieldValue<bool>("ignoreExceptions");
            var timeout = wait.GetFieldValue<TimeSpan>("timeout");
            var pollDelay = wait.GetFieldValue<TimeSpan>("pollDelay");
            var pollInterval = wait.GetFieldValue<TimeSpan>("pollInterval");
            var prereqs = wait.GetFieldValue<IList<Action>>("prereqs");
            var alias = wait.GetFieldValue<IList<Action>>("alias");
            Assert.Multiple(() =>
            {
                Assert.That(shouldFailSilently, Is.False);
                Assert.That(shouldIgnoreExceptions, Is.True);
                Assert.That(timeout.TotalSeconds, Is.EqualTo(10));
                Assert.That(pollDelay.TotalMilliseconds, Is.EqualTo(100));
                Assert.That(pollInterval.TotalMilliseconds, Is.EqualTo(100));
                Assert.That(prereqs, Is.Null);
                Assert.That(alias, Is.Null);
            });
        }

        [Test]
        public void WhenExtremeTimeUnitsAreUsedThenTimeConstraintsAreAppropriatelySetTest()
        {
            var wait = Wait()
                .AtMost(1, DAYS)
                .With().PollDelay(2, HOURS)
                .And().With().PollInterval(3, MINUTES);

            var timeout = wait.GetFieldValue<TimeSpan>("timeout");
            var pollDelay = wait.GetFieldValue<TimeSpan>("pollDelay");
            var pollInterval = wait.GetFieldValue<TimeSpan>("pollInterval");
            Assert.Multiple(() =>
            {
                Assert.That(timeout.TotalDays, Is.EqualTo(1));
                Assert.That(pollDelay.TotalHours, Is.EqualTo(2));
                Assert.That(pollInterval.TotalMinutes, Is.EqualTo(3));
            });
        }

        [Test]
        public void WhenInvalidTimeUnitIsUsedThenThrowExceptionTest()
        {
            Assert.Throws<NotImplementedException>(() => Wait().AtMost(10, (TimeUnit)5));
        }

        [Test]
        public void WhenConditionPassesThenExitSuccessfullyTest()
        {
            var orange = new Orange();
            _ = orange.PeelAsync(2);
            Wait().Until(() => orange.IsPeeled);
            Assert.That(orange.IsPeeled, Is.True);
        }

        [Test]
        public void WhenConditionReturnsTrueThenExitSuccessfullyTest()
        {
            var orange = new Orange();
            _ = orange.PeelAsync(2);
            Wait().UntilTrue(() => orange.IsPeeled);
            Assert.That(orange.IsPeeled, Is.True);
        }

        [Test]
        public void WhenConditionReturnsFalseThenExitSuccessfullyTest()
        {
            var orange = new Orange();
            _ = orange.SpoilAsync(2);
            Wait().UntilFalse(() => orange.IsFresh);
            Assert.That(orange.IsFresh, Is.False);
        }

        [Test]
        public void GivenConditionThatIsUnattainableWhenTimeoutOccursThenThrowExceptionTest()
        {
            var orange = new Orange();
            var wait = Wait().AtMost(5, SECONDS);
            var ex = Assert.Throws<TimeoutException>(() => wait.Until(() => orange.IsPeeled));
            Assert.That(ex.Message, Is.EqualTo("Condition was not fulfilled within 00:00:05."));
        }

        [Test]
        public void GivenConditionThatIsOnlyAttainableAfterTimeoutWhenTimeoutOccursThenThrowExceptionTest()
        {
            var orange = new Orange();
            var wait = Wait().AtMost(2, SECONDS);
            _ = orange.PeelAsync(5);
            var ex = Assert.Throws<TimeoutException>(() => wait.Until(() => orange.IsPeeled));
            Assert.That(ex.Message, Is.EqualTo("Condition was not fulfilled within 00:00:02."));
        }

        [Test]
        public void GivenAliasIsProvidedWhenTimeoutOccursThenExceptionMessageContainsAliasTest()
        {
            var alias = "Is orange peeled?";
            var orange = new Orange();
            var wait = Wait().AtMost(1, SECONDS).With().Alias(alias);
            var ex = Assert.Throws<TimeoutException>(() => wait.Until(() => orange.IsPeeled));
            Assert.That(ex.Message, Is.EqualTo($"Condition with alias '{alias}' was not fulfilled within 00:00:01."));
        }

        [Test]
        public void GivenFailSilentlyIsTrueWhenTimeoutOccursThenExitGracefullyTest()
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
        public void GivenPollDelayIs800MsWhenConditionPassesThenExitSuccessfullyTest()
        {
            var orange = new Orange();
            _ = orange.PeelAsync(2);
            Wait()
                .AtMost(5, SECONDS)
                .With().PollDelay(800, MILLIS)
                .Until(() => orange.IsPeeled);

            Assert.That(orange.IsPeeled, Is.True);
        }

        [Test]
        public void GivenPollIntervalIs400MsWhenConditionPassesThenExitSuccessfullyTest()
        {
            var orange = new Orange();
            _ = orange.PeelAsync(2);
            Wait()
                .AtMost(5, SECONDS)
                .With().PollInterval(400, MILLIS)
                .Until(() => orange.IsPeeled);

            Assert.That(orange.IsPeeled, Is.True);
        }

        [Test]
        public void WhenSinglePrerequisiteIsProvidedThenPrerequisiteIsInvokedTest()
        {
            var toggle = false;
            var orange = new Orange();
            _ = orange.PeelAsync(5);
            Wait()
                .AtMost(2, SECONDS)
                .With().Prereq(() => toggle = true)
                .And().FailSilently()
                .Until(() => orange.IsPeeled);

            Assert.That(toggle, Is.True);
        }

        [Test]
        public void WhenMultiplePrerequisitesAreProvidedThenPrerequisitesAreInvokedTest()
        {
            var toggle = false;
            var i = 0;
            var orange = new Orange();
            var prereqs = new List<Action>()
            {
                () => toggle = true,
                () => i++,
            };

            _ = orange.PeelAsync(2);
            Wait()
                .AtMost(5, SECONDS)
                .With().Prereqs(prereqs)
                .Until(() => orange.IsPeeled);

            Assert.Multiple(() =>
            {
                Assert.That(toggle, Is.True);
                Assert.That(i, Is.GreaterThan(0));
            });
        }

        [Test]
        public void GivenTimeConstraintsAreSetUsingTimeSpansWhenConditionPassesThenExitSuccessfullyTest()
        {
            var orange = new Orange();
            _ = orange.PeelAsync(2);
            Wait()
                .AtMost(TimeSpan.FromSeconds(5))
                .With().PollDelay(TimeSpan.FromMilliseconds(150))
                .And().With().PollInterval(TimeSpan.FromMilliseconds(150))
                .Until(() => orange.IsPeeled);

            Assert.That(orange.IsPeeled, Is.True);
        }

        [Test]
        public void WhenConditionThrowsExceptionThenExceptionIsSwallowedTest()
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
        public void WhenPrerequisiteThrowsExceptionThenExceptionIsSwallowedTest()
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
        public void GivenIgnoreExceptionsIsFalseWhenConditionThrowsExceptionThenExceptionIsThrownTest()
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
        public void GivenIgnoreExceptionsIsFalseWhenPrerequisiteThrowsExceptionThenExceptionIsThrownTest()
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

        [Test]
        public void WhenSleepIsInvokedThenExecutionIsPausedTest()
        {
            var orange = new Orange();
            _ = orange.PeelAsync(5);
            Wait().Sleep(7, SECONDS);
            Assert.That(orange.IsPeeled, Is.True);
        }
    }
}
