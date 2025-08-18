// <copyright file="CancellationTests.cs" company="Esdet">
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
    using System.Threading;
    using NUnit.Framework;
    using static PleaseWait.Dsl;
    using static PleaseWait.TimeUnit;

    [TestFixture]
    [Category("Cancellation")]
    [Parallelizable(scope: ParallelScope.All)]
    public class CancellationTests
    {
        [Test]
        public void Until_CancellationTokenCancelled_ThrowsOperationCanceledException()
        {
            using var cts = new CancellationTokenSource();
            cts.Cancel();

            Assert.Throws<OperationCanceledException>(() =>
            {
                Wait()
                    .AtMost(10, SECONDS)
                    .Until(() => true, cancellationToken: cts.Token);
            });
        }

        [Test]
        public void Until_CancellationTokenCancelledDuringExecution_ThrowsOperationCanceledException()
        {
            using var cts = new CancellationTokenSource();
            cts.CancelAfter(500);

            Assert.Throws<OperationCanceledException>(() =>
            {
                Wait()
                    .AtMost(10, SECONDS)
                    .Until(() => false, cancellationToken: cts.Token);
            });
        }

        [Test]
        public void Until_CancellationTokenNotCancelled_ExecutesNormally()
        {
            using var cts = new CancellationTokenSource();
            var orange = new Orange();
            _ = orange.PeelAsync(2);

            Assert.DoesNotThrow(() =>
            {
                Wait()
                    .AtMost(5, SECONDS)
                    .Until(() => orange.IsPeeled, cancellationToken: cts.Token);
            });

            Assert.That(orange.IsPeeled, Is.True);
        }

        [Test]
        public void UntilTrue_CancellationTokenCancelled_ThrowsOperationCanceledException()
        {
            using var cts = new CancellationTokenSource();
            cts.Cancel();

            Assert.Throws<OperationCanceledException>(() =>
            {
                Wait()
                    .AtMost(10, SECONDS)
                    .UntilTrue(() => true, cts.Token);
            });
        }

        [Test]
        public void UntilFalse_CancellationTokenCancelled_ThrowsOperationCanceledException()
        {
            using var cts = new CancellationTokenSource();
            cts.Cancel();

            Assert.Throws<OperationCanceledException>(() =>
            {
                Wait()
                    .AtMost(10, SECONDS)
                    .UntilFalse(() => false, cts.Token);
            });
        }

        [Test]
        public void Until_CancellationTokenCancelledWithPrerequisites_ThrowsOperationCanceledException()
        {
            using var cts = new CancellationTokenSource();
            cts.Cancel();

            var prereqExecuted = false;
            var prereq = new Action(() => prereqExecuted = true);

            Assert.Throws<OperationCanceledException>(() =>
            {
                Wait()
                    .AtMost(10, SECONDS)
                    .With().Prereq(prereq)
                    .Until(() => true, cancellationToken: cts.Token);
            });

            Assert.That(prereqExecuted, Is.False);
        }

        [Test]
        public void Until_CancellationTokenCancelledAfterPrerequisites_ThrowsOperationCanceledException()
        {
            using var cts = new CancellationTokenSource();

            var prereqExecuted = false;
            var prereq = new Action(() => prereqExecuted = true);

            cts.CancelAfter(500);

            Assert.Throws<OperationCanceledException>(() =>
            {
                Wait()
                    .AtMost(10, SECONDS)
                    .With().PollDelay(100, MILLIS)
                    .And().With().PollInterval(100, MILLIS)
                    .With().Prereq(prereq)
                    .Until(() => false, cancellationToken: cts.Token);
            });

            Assert.That(prereqExecuted, Is.True);
        }

        [Test]
        public void Until_CancellationTokenCancelledWithAlias_ThrowsOperationCanceledException()
        {
            using var cts = new CancellationTokenSource();
            cts.Cancel();

            Assert.Throws<OperationCanceledException>(() =>
            {
                Wait()
                    .AtMost(10, SECONDS)
                    .Alias("Test Condition")
                    .Until(() => true, cancellationToken: cts.Token);
            });
        }

        [Test]
        public void Until_CancellationTokenCancelledWithExceptionHandling_ThrowsOperationCanceledException()
        {
            using var cts = new CancellationTokenSource();
            cts.Cancel();

            Assert.Throws<OperationCanceledException>(() =>
            {
                Wait()
                    .AtMost(10, SECONDS)
                    .With().IgnoreExceptions(false)
                    .And().FailSilently()
                    .Until(() => true, cancellationToken: cts.Token);
            });
        }

        [Test]
        public void Until_CancellationTokenCancelledWithCustomTimeout_ThrowsOperationCanceledException()
        {
            using var cts = new CancellationTokenSource();
            cts.Cancel();

            Assert.Throws<OperationCanceledException>(() =>
            {
                Wait()
                    .AtMost(1, MINUTES)
                    .With().PollDelay(1, SECONDS)
                    .And().With().PollInterval(1, SECONDS)
                    .Until(() => true, cancellationToken: cts.Token);
            });
        }

        [Test]
        public void Until_CancellationTokenCancelledWithExpectedFalse_ThrowsOperationCanceledException()
        {
            using var cts = new CancellationTokenSource();
            cts.Cancel();

            Assert.Throws<OperationCanceledException>(() =>
            {
                Wait()
                    .AtMost(10, SECONDS)
                    .Until(() => false, expected: false, cancellationToken: cts.Token);
            });
        }
    }
}
