// <copyright file="ConsoleLogger.cs" company="Esdet">
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

    /// <summary>
    /// Default console logger implementation for wait operations.
    /// </summary>
    public class ConsoleLogger : IWaitLogger
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleLogger"/> class.
        /// </summary>
        public ConsoleLogger()
        {
        }

        /// <inheritdoc/>
        public void LogConditionCheck(string condition, bool result, TimeSpan elapsed)
        {
            var status = result ? "✓" : "✗";
            Console.WriteLine($"[PleaseWait] {status} Condition check: {condition} (elapsed: {elapsed.TotalMilliseconds:F0}ms)");
        }

        /// <inheritdoc/>
        public void LogTimeout(string condition, TimeSpan timeout)
        {
            Console.WriteLine($"[PleaseWait] ⏰ Timeout: {condition} exceeded {timeout.TotalMilliseconds:F0}ms");
        }

        /// <inheritdoc/>
        public void LogCancellation(string condition)
        {
            Console.WriteLine($"[PleaseWait] 🚫 Cancelled: {condition}");
        }

        /// <inheritdoc/>
        public void LogWaitStart(string condition, TimeSpan timeout)
        {
            Console.WriteLine($"[PleaseWait] 🚀 Starting wait: {condition} (timeout: {timeout.TotalMilliseconds:F0}ms)");
        }

        /// <inheritdoc/>
        public void LogWaitSuccess(string condition, TimeSpan elapsed, int checks)
        {
            Console.WriteLine($"[PleaseWait] ✅ Success: {condition} completed in {elapsed.TotalMilliseconds:F0}ms ({checks} checks)");
        }
    }
}
