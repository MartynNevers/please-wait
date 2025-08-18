// <copyright file="ConfigurationTests.cs" company="Esdet">
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
    [Category("Configuration")]
    [Parallelizable(scope: ParallelScope.All)]
    public class ConfigurationTests
    {
        [Test]
        public void Until_PollDelay800MsAndConditionPasses_ExitsSuccessfully()
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
        public void Until_PollInterval400MsAndConditionPasses_ExitsSuccessfully()
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
        public void Until_SinglePrerequisiteProvided_PrerequisiteIsInvoked()
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
        public void Until_MultiplePrerequisitesProvided_PrerequisitesAreInvoked()
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
        public void Until_TimeConstraintsSetUsingTimeSpansAndConditionPasses_ExitsSuccessfully()
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
    }
}
