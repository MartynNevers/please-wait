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
    using NUnit.Framework;

    [Parallelizable(scope: ParallelScope.All)]
    public class UnitTests
    {
        [Test]
        public void WhenConditionPassesThenExitSuccessfullyTest()
        {
            var r = new Ref<string>("Monday");
            _ = this.UpdateValue(2, r, "Tuesday");
            PleaseWait.AtMost(5, TimeUnit.SECONDS).Until(() => r.Value.Equals("Tuesday"));
            Assert.That(r.Value, Is.EqualTo("Tuesday"));
        }

        [Test]
        public void GivenConditionThatIsUnattainableWhenTimeoutOccursThenThrowExceptionTest()
        {
            var r = new Ref<string>("Tuesday");
            _ = this.UpdateValue(2, r, "Wednesday");

            try
            {
                PleaseWait.AtMost(5, TimeUnit.SECONDS).Until(() => r.Value.Equals("Thursday"));
            }
            catch (TimeoutException timeoutException)
            {
                Assert.That(timeoutException.Message, Is.EqualTo("PleaseWait timed out after 0d 0h 0m 5s 0ms"));
            }
        }

        [Test]
        public void GivenConditionThatIsOnlyAttainableAfterTimeoutWhenTimeoutOccursThenThrowExceptionTest()
        {
            var r = new Ref<string>("Wednesday");
            _ = this.UpdateValue(5, r, "Thursday");

            try
            {
                PleaseWait.AtMost(2, TimeUnit.SECONDS).Until(() => r.Value.Equals("Thursday"));
            }
            catch (TimeoutException timeoutException)
            {
                Assert.That(timeoutException.Message, Is.EqualTo("PleaseWait timed out after 0d 0h 0m 2s 0ms"));
            }
        }

        [Test]
        public void GivenThrowsIsFalseWhenTimeoutOccursThenExitGracefullyTest()
        {
            var r = new Ref<string>("Thursday");
            _ = this.UpdateValue(5, r, "Friday");
            PleaseWait.AtMost(2, TimeUnit.SECONDS).AndThrows(false).Until(() => r.Value.Equals("Friday"));
            Assert.That(r.Value, Is.EqualTo("Thursday"));
        }

        [Test]
        public void GivenPollingRateIs500MsWhenConditionPassesThenExitSuccessfullyTest()
        {
            var r = new Ref<string>("Friday");
            _ = this.UpdateValue(2, r, "Saturday");
            PleaseWait.AtMost(5, TimeUnit.SECONDS).WithPollingRate(500, TimeUnit.MILLIS).Until(() => r.Value.Equals("Saturday"));
            Assert.That(r.Value, Is.EqualTo("Saturday"));
        }

        [Test]
        public void WhenSingleActionIsProvidedThenActionIsInvokedTest()
        {
            var toggle = false;
            var r = new Ref<string>("Saturday");
            _ = this.UpdateValue(5, r, "Sunday");
            PleaseWait.AtMost(2, TimeUnit.SECONDS).AndThrows(false).Until(() => r.Value.Equals("Sunday"), () => toggle = true);
            Assert.IsTrue(toggle);
        }

        [Test]
        public void WhenMultipleActionsAreProvidedThenActionsAreInvokedTest()
        {
            var toggle = false;
            var i = 0;
            var r = new Ref<string>("Sunday");
            _ = this.UpdateValue(2, r, "Monday");

            var actions = new List<Action>()
            {
                () => toggle = true,
                () => i++,
            };

            PleaseWait.AtMost(5, TimeUnit.SECONDS).Until(() => r.Value.Equals("Monday"), actions);
            Assert.IsTrue(toggle);
            Assert.That(i, Is.GreaterThan(0));
        }

        [Test]
        public void WhenConditionThrowsExceptionThenExceptionIsSwallowedTest()
        {
            var r = new Ref<int>(0);
            PleaseWait.AtMost(1, TimeUnit.SECONDS).WithPollingRate(50, TimeUnit.MILLIS).Until(() => this.IncrementValueWithExceptions(r, 5).Value.Equals(5));
            Assert.That(r.Value, Is.EqualTo(5));
        }

        [Test]
        public void WhenActionThrowsExceptionThenExceptionIsSwallowedTest()
        {
            var r = new Ref<int>(0);
            PleaseWait.AtMost(5, TimeUnit.SECONDS).AndThrows(false).Until(() => false, () => this.IncrementValueWithExceptions(r, 2));
            Assert.That(r.Value, Is.EqualTo(2));
        }

        private async Task UpdateValue(double seconds, Ref<string> r, string value)
        {
            await Task.Run(() =>
            {
                Task.Delay(TimeSpan.FromSeconds(seconds)).Wait();
                r.Value = value;
            });
        }

        private Ref<int> IncrementValueWithExceptions(Ref<int> r, int iterations)
        {
            if (r.Value < iterations)
            {
                r.Value++;
                throw new Exception();
            }

            return r;
        }
    }
}
