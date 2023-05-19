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
    using static PleaseWait;
    using static TimeUnit;

    [Parallelizable(scope: ParallelScope.All)]
    public class UnitTests
    {
        [Test]
        public void WhenInstanceIsCreatedThenPropertiesAreSetToTheirDefaultsTest()
        {
            var wait = Wait();
            var shouldFailSilently = wait.GetPropertyValue<bool>("ShouldFailSilently");
            var shouldIgnoreExceptions = wait.GetPropertyValue<bool>("ShouldIgnoreExceptions");
            var timeout = wait.GetPropertyValue<TimeSpan>("Timeout");
            var pollDelay = wait.GetPropertyValue<TimeSpan>("PollDelay");
            var pollInterval = wait.GetPropertyValue<TimeSpan>("PollInterval");
            var prereqs = wait.GetPropertyValue<IList<Action>>("Prereqs");
            Assert.That(shouldFailSilently, Is.False);
            Assert.That(shouldIgnoreExceptions, Is.True);
            Assert.That(timeout.TotalSeconds, Is.EqualTo(10));
            Assert.That(pollDelay.TotalMilliseconds, Is.EqualTo(100));
            Assert.That(pollInterval.TotalMilliseconds, Is.EqualTo(100));
            Assert.Null(prereqs);
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
        public void GivenConditionThatIsUnattainableWhenTimeoutOccursThenThrowExceptionTest()
        {
            var orange = new Orange();
            var wait = Wait().AtMost(5, SECONDS);
            var ex = Assert.Throws<TimeoutException>(() => wait.Until(() => orange.IsPeeled));
            Assert.That(ex.Message, Is.EqualTo("PleaseWait timed out after 00:00:05"));
        }

        [Test]
        public void GivenConditionThatIsOnlyAttainableAfterTimeoutWhenTimeoutOccursThenThrowExceptionTest()
        {
            var orange = new Orange();
            var wait = Wait().AtMost(2, SECONDS);
            _ = orange.PeelAsync(5);
            var ex = Assert.Throws<TimeoutException>(() => wait.Until(() => orange.IsPeeled));
            Assert.That(ex.Message, Is.EqualTo("PleaseWait timed out after 00:00:02"));
        }

        [Test]
        public void GivenFailSilentlyIsTrueWhenTimeoutOccursThenExitGracefullyTest()
        {
            var orange = new Orange();
            _ = orange.PeelAsync(5);
            Wait()
                .AtMost(2, SECONDS)
                .FailSilently()
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
                .WithPollDelay(800, MILLIS)
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
                .WithPollInterval(400, MILLIS)
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
                .WithPrereq(() => toggle = true)
                .FailSilently()
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
                .WithPrereqs(prereqs)
                .Until(() => orange.IsPeeled);

            Assert.IsTrue(toggle);
            Assert.That(i, Is.GreaterThan(0));
        }

        [Test]
        public void GivenTimeConstraintsAreSetUsingTimeSpansWhenConditionPassesThenExitSuccessfullyTest()
        {
            var orange = new Orange();
            _ = orange.PeelAsync(2);
            Wait()
                .AtMost(TimeSpan.FromSeconds(5))
                .WithPollDelay(TimeSpan.FromMilliseconds(150))
                .WithPollInterval(TimeSpan.FromMilliseconds(150))
                .Until(() => orange.IsPeeled);

            Assert.That(orange.IsPeeled, Is.True);
        }

        [Test]
        public void WhenConditionThrowsExceptionThenExceptionIsSwallowedTest()
        {
            var orange = new Orange();
            Wait()
                .AtMost(5, SECONDS)
                .WithPollDelay(50, MILLIS)
                .WithPollInterval(50, MILLIS)
                .Until(() => orange.CountSegments() > 8);

            Assert.That(orange.CountSegments(), Is.GreaterThan(8).And.LessThan(12));
        }

        [Test]
        public void WhenPrerequisiteThrowsExceptionThenExceptionIsSwallowedTest()
        {
            var orange = new Orange();
            Wait()
                .AtMost(5, SECONDS)
                .WithPrereq(() => orange.CountSegments())
                .FailSilently()
                .Until(() => false);

            Assert.That(orange.CountSegments(), Is.GreaterThan(8).And.LessThan(12));
        }

        [Test]
        public void GivenIgnoreExceptionsIsFalseWhenConditionThrowsExceptionThenExceptionIsThrownTest()
        {
            var orange = new Orange();
            var wait = Wait()
                .AtMost(1, SECONDS)
                .WithPollDelay(50, MILLIS)
                .WithPollInterval(50, MILLIS)
                .IgnoreExceptions(false);

            var ex = Assert.Throws<InvalidOperationException>(() => wait.Until(() => orange.CountSegments() > 8));
            Assert.That(ex.Message, Is.EqualTo("Try again"));
        }

        [Test]
        public void GivenIgnoreExceptionsIsFalseWhenPrerequisiteThrowsExceptionThenExceptionIsThrownTest()
        {
            var orange = new Orange();
            var wait = Wait()
                .AtMost(5, SECONDS)
                .WithPrereq(() => orange.CountSegments())
                .IgnoreExceptions(false)
                .FailSilently();

            var ex = Assert.Throws<InvalidOperationException>(() => wait.Until(() => false));
            Assert.That(ex.Message, Is.EqualTo("Try again"));
        }
    }
}
