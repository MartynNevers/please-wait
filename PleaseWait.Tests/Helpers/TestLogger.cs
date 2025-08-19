// <copyright file="TestLogger.cs" company="Esdet">
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
    using PleaseWait.Logging;

    /// <summary>
    /// Test logger implementation for verifying logging behavior.
    /// </summary>
    public class TestLogger : IWaitLogger
    {
        /// <summary>
        /// Gets a value indicating whether LogWaitStart was called.
        /// </summary>
        public bool WaitStartLogged { get; private set; }

        /// <summary>
        /// Gets the condition passed to LogWaitStart.
        /// </summary>
        public string? WaitStartCondition { get; private set; }

        /// <summary>
        /// Gets the timeout passed to LogWaitStart.
        /// </summary>
        public TimeSpan WaitStartTimeout { get; private set; }

        /// <summary>
        /// Gets a value indicating whether LogWaitSuccess was called.
        /// </summary>
        public bool WaitSuccessLogged { get; private set; }

        /// <summary>
        /// Gets the condition passed to LogWaitSuccess.
        /// </summary>
        public string? WaitSuccessCondition { get; private set; }

        /// <summary>
        /// Gets the elapsed time passed to LogWaitSuccess.
        /// </summary>
        public TimeSpan WaitSuccessElapsed { get; private set; }

        /// <summary>
        /// Gets the number of checks passed to LogWaitSuccess.
        /// </summary>
        public int WaitSuccessChecks { get; private set; }

        /// <summary>
        /// Gets a value indicating whether LogTimeout was called.
        /// </summary>
        public bool TimeoutLogged { get; private set; }

        /// <summary>
        /// Gets the condition passed to LogTimeout.
        /// </summary>
        public string? TimeoutCondition { get; private set; }

        /// <summary>
        /// Gets the timeout value passed to LogTimeout.
        /// </summary>
        public TimeSpan TimeoutValue { get; private set; }

        /// <summary>
        /// Gets a value indicating whether LogCancellation was called.
        /// </summary>
        public bool CancellationLogged { get; private set; }

        /// <summary>
        /// Gets the condition passed to LogCancellation.
        /// </summary>
        public string? CancellationCondition { get; private set; }

        /// <summary>
        /// Gets the number of times LogConditionCheck was called.
        /// </summary>
        public int ConditionChecksLogged { get; private set; }

        /// <inheritdoc/>
        public void LogWaitStart(string condition, TimeSpan timeout)
        {
            this.WaitStartLogged = true;
            this.WaitStartCondition = condition;
            this.WaitStartTimeout = timeout;
        }

        /// <inheritdoc/>
        public void LogWaitSuccess(string condition, TimeSpan elapsed, int checks)
        {
            this.WaitSuccessLogged = true;
            this.WaitSuccessCondition = condition;
            this.WaitSuccessElapsed = elapsed;
            this.WaitSuccessChecks = checks;
        }

        /// <inheritdoc/>
        public void LogTimeout(string condition, TimeSpan timeout)
        {
            this.TimeoutLogged = true;
            this.TimeoutCondition = condition;
            this.TimeoutValue = timeout;
        }

        /// <inheritdoc/>
        public void LogCancellation(string condition)
        {
            this.CancellationLogged = true;
            this.CancellationCondition = condition;
        }

        /// <inheritdoc/>
        public void LogConditionCheck(string condition, bool result, TimeSpan elapsed)
        {
            this.ConditionChecksLogged++;
        }
    }
}
