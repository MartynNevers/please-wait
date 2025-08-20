// <copyright file="TimeoutAndExceptionsTests.cs" company="Esdet">
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
    [Category("Timeout & Exceptions")]
    [Parallelizable(scope: ParallelScope.All)]
    public class TimeoutAndExceptionsTests
    {
        [Test]
        public void Until_ConditionUnattainable_ThrowsTimeoutException()
        {
            var orange = new Orange();
            var wait = Wait().AtMost(5, Seconds);
            var ex = Assert.Throws<TimeoutException>(() => wait.Until(() => orange.IsPeeled));
            Assert.That(ex.Message, Is.EqualTo("Condition was not fulfilled within 00:00:05."));
        }

        [Test]
        public void Until_ConditionAttainableAfterTimeout_ThrowsTimeoutException()
        {
            var orange = new Orange();
            var wait = Wait().AtMost(2, Seconds);
            _ = orange.PeelAsync(5);
            var ex = Assert.Throws<TimeoutException>(() => wait.Until(() => orange.IsPeeled));
            Assert.That(ex.Message, Is.EqualTo("Condition was not fulfilled within 00:00:02."));
        }

        [Test]
        public void Until_AliasProvidedAndTimeoutOccurs_ExceptionMessageContainsAlias()
        {
            var alias = "Is orange peeled?";
            var orange = new Orange();
            var wait = Wait().AtMost(1, Seconds).Alias(alias);
            var ex = Assert.Throws<TimeoutException>(() => wait.Until(() => orange.IsPeeled));
            Assert.That(ex.Message, Is.EqualTo($"Condition with alias '{alias}' was not fulfilled within 00:00:01."));
        }

        [Test]
        public void Until_FailSilentlyAndTimeoutOccurs_ExitsGracefully()
        {
            var orange = new Orange();
            _ = orange.PeelAsync(5);
            Wait()
                .AtMost(2, Seconds)
                .FailSilently()
                .Until(() => orange.IsPeeled);

            Assert.That(orange.IsPeeled, Is.False);
        }

        [Test]
        public void Until_ConditionThrowsException_ExceptionIsSwallowed()
        {
            var orange = new Orange();
            Wait()
                .AtMost(5, Seconds)
                .PollDelay(50, Millis)
                .PollInterval(50, Millis)
                .Until(() => orange.CountSegments() > 8);

            Assert.That(orange.CountSegments(), Is.GreaterThan(8).And.LessThan(12));
        }

        [Test]
        public void Until_PrerequisiteThrowsException_ExceptionIsSwallowed()
        {
            var orange = new Orange();
            Wait()
                .AtMost(5, Seconds)
                .Prereq(() => orange.CountSegments())
                .FailSilently()
                .Until(() => false);

            Assert.That(orange.CountSegments(), Is.GreaterThan(8).And.LessThan(12));
        }

        [Test]
        public void Until_IgnoreExceptionsFalseAndConditionThrowsException_ExceptionIsThrown()
        {
            var orange = new Orange();
            var wait = Wait()
                .AtMost(1, Seconds)
                .PollDelay(50, Millis)
                .PollInterval(50, Millis)
                .IgnoreExceptions(false);

            var ex = Assert.Throws<InvalidOperationException>(() => wait.Until(() => orange.CountSegments() > 8));
            Assert.That(ex.Message, Is.EqualTo("Try again"));
        }

        [Test]
        public void Until_IgnoreExceptionsFalseAndPrerequisiteThrowsException_ExceptionIsThrown()
        {
            var orange = new Orange();
            var wait = Wait()
                .AtMost(5, Seconds)
                .Prereq(() => orange.CountSegments())
                .IgnoreExceptions(false)
                .FailSilently();

            var ex = Assert.Throws<InvalidOperationException>(() => wait.Until(() => false));
            Assert.That(ex.Message, Is.EqualTo("Try again"));
        }

        [Test]
        public void Until_ExceptionHandlingIgnoreTrueFailSilentlyTrue_ExceptionIsSwallowedAndNoTimeoutThrown()
        {
            var orange = new Orange();
            var wait = Wait()
                .AtMost(1, Seconds)
                .ExceptionHandling(true, true);

            // This should not throw any exceptions - both exceptions and timeouts are handled silently
            Assert.DoesNotThrow(() => wait.Until(() => orange.CountSegments() > 8));
        }

        [Test]
        public void Until_ExceptionHandlingIgnoreTrueFailSilentlyFalse_ExceptionIsSwallowedButTimeoutThrown()
        {
            var orange = new Orange();
            var wait = Wait()
                .AtMost(1, Seconds)
                .ExceptionHandling(true, false);

            // Exception should be swallowed, but timeout should be thrown
            // Use a condition that will never succeed to ensure timeout occurs
            var ex = Assert.Throws<TimeoutException>(() => wait.Until(() => false));
            Assert.That(ex.Message, Is.EqualTo("Condition was not fulfilled within 00:00:01."));
        }

        [Test]
        public void Until_ExceptionHandlingIgnoreFalseFailSilentlyTrue_ExceptionIsThrownButTimeoutHandledSilently()
        {
            var orange = new Orange();
            var wait = Wait()
                .AtMost(1, Seconds)
                .ExceptionHandling(false, true);

            // Exception should be thrown, but timeout should be handled silently
            var ex = Assert.Throws<InvalidOperationException>(() => wait.Until(() => orange.CountSegments() > 8));
            Assert.That(ex.Message, Is.EqualTo("Try again"));
        }

        [Test]
        public void Until_ExceptionHandlingIgnoreFalseFailSilentlyFalse_ExceptionAndTimeoutAreThrown()
        {
            var orange = new Orange();
            var wait = Wait()
                .AtMost(1, Seconds)
                .ExceptionHandling(false, false);

            // Both exception and timeout should be thrown (exception first)
            var ex = Assert.Throws<InvalidOperationException>(() => wait.Until(() => orange.CountSegments() > 8));
            Assert.That(ex.Message, Is.EqualTo("Try again"));
        }

        [Test]
        public void Until_ExceptionHandlingWithPrerequisite_PrerequisiteExceptionIsHandledCorrectly()
        {
            var orange = new Orange();
            var wait = Wait()
                .AtMost(1, Seconds)
                .Prereq(() => orange.CountSegments())
                .ExceptionHandling(true, true);

            // Prerequisite exception should be swallowed, and timeout should be handled silently
            Assert.DoesNotThrow(() => wait.Until(() => false));
        }
    }
}
