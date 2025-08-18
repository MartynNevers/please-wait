// <copyright file="IWaitLogger.cs" company="Esdet">
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

namespace PleaseWait.Logging
{
    using System;

    /// <summary>
    /// Interface for logging wait operations and diagnostics.
    /// </summary>
    public interface IWaitLogger
    {
        /// <summary>
        /// Logs a condition check with its result and elapsed time.
        /// </summary>
        /// <param name="condition">Description of the condition being checked.</param>
        /// <param name="result">The result of the condition check.</param>
        /// <param name="elapsed">Time elapsed since the wait started.</param>
        void LogConditionCheck(string condition, bool result, TimeSpan elapsed);

        /// <summary>
        /// Logs a timeout event when the wait exceeds the maximum time.
        /// </summary>
        /// <param name="condition">Description of the condition that timed out.</param>
        /// <param name="timeout">The timeout value that was exceeded.</param>
        void LogTimeout(string condition, TimeSpan timeout);

        /// <summary>
        /// Logs a cancellation event when the wait is cancelled.
        /// </summary>
        /// <param name="condition">Description of the condition that was cancelled.</param>
        void LogCancellation(string condition);

        /// <summary>
        /// Logs the start of a wait operation.
        /// </summary>
        /// <param name="condition">Description of the condition to wait for.</param>
        /// <param name="timeout">The maximum time to wait.</param>
        void LogWaitStart(string condition, TimeSpan timeout);

        /// <summary>
        /// Logs the successful completion of a wait operation.
        /// </summary>
        /// <param name="condition">Description of the condition that succeeded.</param>
        /// <param name="elapsed">Total time taken for the wait.</param>
        /// <param name="checks">Number of condition checks performed.</param>
        void LogWaitSuccess(string condition, TimeSpan elapsed, int checks);
    }
}
