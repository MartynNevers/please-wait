// <copyright file="InitializationTests.cs" company="Esdet">
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
    [Category("Initialization")]
    [Parallelizable(scope: ParallelScope.All)]
    public class InitializationTests
    {
        [Test]
        public void Wait_InstanceCreated_SetsDefaultFields()
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
        public void Wait_ExtremeTimeUnitsUsed_SetsTimeConstraintsAppropriately()
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
        public void Wait_InvalidTimeUnitUsed_ThrowsException()
        {
            Assert.Throws<NotImplementedException>(() => Wait().AtMost(10, (TimeUnit)5));
        }
    }
}
