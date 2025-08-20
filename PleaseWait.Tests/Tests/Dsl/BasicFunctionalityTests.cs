// <copyright file="BasicFunctionalityTests.cs" company="Esdet">
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
    using NUnit.Framework;
    using static PleaseWait.Dsl;
    using static PleaseWait.TimeUnit;

    [TestFixture]
    [Category("Basic Functionality")]
    [Parallelizable(scope: ParallelScope.All)]
    public class BasicFunctionalityTests
    {
        [Test]
        public void Until_ConditionPasses_ExitsSuccessfully()
        {
            var orange = new Orange();
            _ = orange.PeelAsync(2);
            Wait().Until(() => orange.IsPeeled);
            Assert.That(orange.IsPeeled, Is.True);
        }

        [Test]
        public void UntilTrue_ConditionReturnsTrue_ExitsSuccessfully()
        {
            var orange = new Orange();
            _ = orange.PeelAsync(2);
            Wait().UntilTrue(() => orange.IsPeeled);
            Assert.That(orange.IsPeeled, Is.True);
        }

        [Test]
        public void UntilFalse_ConditionReturnsFalse_ExitsSuccessfully()
        {
            var orange = new Orange();
            _ = orange.SpoilAsync(2);
            Wait().UntilFalse(() => orange.IsFresh);
            Assert.That(orange.IsFresh, Is.False);
        }

        [Test]
        public void Sleep_WithTimeUnit_PausesExecution()
        {
            var orange = new Orange();
            _ = orange.PeelAsync(5);
            Wait().Sleep(7, Seconds);
            Assert.That(orange.IsPeeled, Is.True);
        }

        [Test]
        public void Polling_TimeSpan_SetsBothDelayAndInterval()
        {
            var delay = TimeSpan.FromMilliseconds(50);
            var interval = TimeSpan.FromMilliseconds(100);
            var result = Wait().Polling(delay, interval);
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void Polling_ValueAndUnit_SetsBothDelayAndInterval()
        {
            var result = Wait().Polling(150, Millis, 250, Millis);
            Assert.That(result, Is.Not.Null);
        }
    }
}
