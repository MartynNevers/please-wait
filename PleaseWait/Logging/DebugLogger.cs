// <copyright file="DebugLogger.cs" company="Esdet">
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

namespace PleaseWait.Logging
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// Debug logger implementation for wait operations.
    /// Writes diagnostic messages to <see cref="Debug"/>.
    /// </summary>
    public class DebugLogger : IWaitLogger
    {
        /// <inheritdoc/>
        public void LogWaitStart(string condition, TimeSpan timeout)
        {
            Debug.WriteLine($"[PleaseWait] 🚀 Starting wait: {condition} (timeout: {timeout.TotalMilliseconds:0}ms)");
        }

        /// <inheritdoc/>
        public void LogConditionCheck(string condition, bool result, TimeSpan elapsed)
        {
            var symbol = result ? "✓" : "✗";
            Debug.WriteLine($"[PleaseWait] {symbol} Condition check: {condition} (elapsed: {elapsed.TotalMilliseconds:0}ms)");
        }

        /// <inheritdoc/>
        public void LogWaitSuccess(string condition, TimeSpan elapsed, int checks)
        {
            Debug.WriteLine($"[PleaseWait] ✅ Success: {condition} completed in {elapsed.TotalMilliseconds:0}ms ({checks} checks)");
        }

        /// <inheritdoc/>
        public void LogTimeout(string condition, TimeSpan timeout)
        {
            Debug.WriteLine($"[PleaseWait] ⏰ Timeout: {condition} exceeded {timeout.TotalMilliseconds:0}ms");
        }

        /// <inheritdoc/>
        public void LogCancellation(string condition)
        {
            Debug.WriteLine($"[PleaseWait] 🛑 Cancelled: {condition}");
        }
    }
}
