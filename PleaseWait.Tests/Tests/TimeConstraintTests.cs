// <copyright file="TimeConstraintTests.cs" company="Esdet">
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
    using PleaseWait.Core;
    using static PleaseWait.TimeUnit;

    [TestFixture]
    [Category("Unit")]
    [Parallelizable(scope: ParallelScope.All)]
    public class TimeConstraintTests
    {
        [Test]
        public void Constructor_WithValidParameters_CreatesInstance()
        {
            var constraint = new TimeConstraint(10, Seconds);
            Assert.That(constraint, Is.Not.Null);
        }

        [Test]
        public void GetTimeSpan_Milliseconds_ReturnsCorrectValue()
        {
            var constraint = new TimeConstraint(500, Millis);
            var result = constraint.GetTimeSpan();
            Assert.That(result, Is.EqualTo(TimeSpan.FromMilliseconds(500)));
        }

        [Test]
        public void GetTimeSpan_Seconds_ReturnsCorrectValue()
        {
            var constraint = new TimeConstraint(30, Seconds);
            var result = constraint.GetTimeSpan();
            Assert.That(result, Is.EqualTo(TimeSpan.FromSeconds(30)));
        }

        [Test]
        public void GetTimeSpan_Minutes_ReturnsCorrectValue()
        {
            var constraint = new TimeConstraint(5, Minutes);
            var result = constraint.GetTimeSpan();
            Assert.That(result, Is.EqualTo(TimeSpan.FromMinutes(5)));
        }

        [Test]
        public void GetTimeSpan_Hours_ReturnsCorrectValue()
        {
            var constraint = new TimeConstraint(2, Hours);
            var result = constraint.GetTimeSpan();
            Assert.That(result, Is.EqualTo(TimeSpan.FromHours(2)));
        }

        [Test]
        public void GetTimeSpan_Days_ReturnsCorrectValue()
        {
            var constraint = new TimeConstraint(1, Days);
            var result = constraint.GetTimeSpan();
            Assert.That(result, Is.EqualTo(TimeSpan.FromDays(1)));
        }

        [Test]
        public void GetTimeSpan_ZeroValue_ReturnsZeroTimeSpan()
        {
            var constraint = new TimeConstraint(0, Seconds);
            var result = constraint.GetTimeSpan();
            Assert.That(result, Is.EqualTo(TimeSpan.Zero));
        }

        [Test]
        public void GetTimeSpan_FractionalSeconds_ReturnsCorrectValue()
        {
            var constraint = new TimeConstraint(0.5, Seconds);
            var result = constraint.GetTimeSpan();
            Assert.That(result, Is.EqualTo(TimeSpan.FromMilliseconds(500)));
        }

        [Test]
        public void GetTimeSpan_FractionalMinutes_ReturnsCorrectValue()
        {
            var constraint = new TimeConstraint(0.5, Minutes);
            var result = constraint.GetTimeSpan();
            Assert.That(result, Is.EqualTo(TimeSpan.FromSeconds(30)));
        }

        [Test]
        public void GetTimeSpan_LargeValue_ReturnsCorrectValue()
        {
            var constraint = new TimeConstraint(1000, Seconds);
            var result = constraint.GetTimeSpan();
            Assert.That(result, Is.EqualTo(TimeSpan.FromSeconds(1000)));
        }

        [Test]
        public void GetTimeSpan_AllUnits_ReturnCorrectValues()
        {
            // Test all time units with value 1
            var millisConstraint = new TimeConstraint(1, Millis);
            var secondsConstraint = new TimeConstraint(1, Seconds);
            var minutesConstraint = new TimeConstraint(1, Minutes);
            var hoursConstraint = new TimeConstraint(1, Hours);
            var daysConstraint = new TimeConstraint(1, Days);

            Assert.That(millisConstraint.GetTimeSpan(), Is.EqualTo(TimeSpan.FromMilliseconds(1)));
            Assert.That(secondsConstraint.GetTimeSpan(), Is.EqualTo(TimeSpan.FromSeconds(1)));
            Assert.That(minutesConstraint.GetTimeSpan(), Is.EqualTo(TimeSpan.FromMinutes(1)));
            Assert.That(hoursConstraint.GetTimeSpan(), Is.EqualTo(TimeSpan.FromHours(1)));
            Assert.That(daysConstraint.GetTimeSpan(), Is.EqualTo(TimeSpan.FromDays(1)));
        }

        [Test]
        public void GetTimeSpan_NegativeValue_ReturnsNegativeTimeSpan()
        {
            var constraint = new TimeConstraint(-5, Seconds);
            var result = constraint.GetTimeSpan();
            Assert.That(result, Is.EqualTo(TimeSpan.FromSeconds(-5)));
        }

        [Test]
        public void GetTimeSpan_DecimalValue_ReturnsCorrectValue()
        {
            var constraint = new TimeConstraint(1.5, Minutes);
            var result = constraint.GetTimeSpan();
            Assert.That(result, Is.EqualTo(TimeSpan.FromSeconds(90))); // 1.5 minutes = 90 seconds
        }

        [Test]
        public void GetTimeSpan_VerySmallValue_ReturnsCorrectValue()
        {
            var constraint = new TimeConstraint(0.001, Seconds);
            var result = constraint.GetTimeSpan();
            Assert.That(result, Is.EqualTo(TimeSpan.FromMilliseconds(1)));
        }

        [Test]
        public void GetTimeSpan_VeryLargeValue_ReturnsCorrectValue()
        {
            var constraint = new TimeConstraint(365, Days);
            var result = constraint.GetTimeSpan();
            Assert.That(result, Is.EqualTo(TimeSpan.FromDays(365)));
        }
    }
}
