// <copyright file="NullLogger.cs" company="Esdet">
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
    /// Null logger implementation that performs no logging operations.
    /// Used as the default logger when no logging is required.
    /// </summary>
    public class NullLogger : IWaitLogger
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NullLogger"/> class.
        /// </summary>
        private NullLogger()
        {
        }

        /// <summary>
        /// Gets the singleton instance of the null logger.
        /// </summary>
        public static NullLogger Instance { get; } = new NullLogger();

        /// <inheritdoc/>
        public void LogConditionCheck(string condition, bool result, TimeSpan elapsed)
        {
            // No operation - null logger
        }

        /// <inheritdoc/>
        public void LogTimeout(string condition, TimeSpan timeout)
        {
            // No operation - null logger
        }

        /// <inheritdoc/>
        public void LogCancellation(string condition)
        {
            // No operation - null logger
        }

        /// <inheritdoc/>
        public void LogWaitStart(string condition, TimeSpan timeout)
        {
            // No operation - null logger
        }

        /// <inheritdoc/>
        public void LogWaitSuccess(string condition, TimeSpan elapsed, int checks)
        {
            // No operation - null logger
        }
    }
}
