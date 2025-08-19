// <copyright file="GlobalDefaults.cs" company="Esdet">
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

namespace PleaseWait.Core
{
    using System;
    using System.Collections.Generic;
    using PleaseWait.Logging;
    using PleaseWait.Strategy;

    /// <summary>
    /// Provides mutable global defaults for PleaseWait that can be configured by users.
    /// </summary>
    internal static class GlobalDefaults
    {
        // Default values (same as original Defaults class)
        private const bool DefaultFailSilently = false;
        private const bool DefaultIgnoreExceptions = true;
        private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(10);
        private static readonly TimeSpan DefaultPollDelay = TimeSpan.FromMilliseconds(100);
        private static readonly TimeSpan DefaultPollInterval = TimeSpan.FromMilliseconds(100);
        private static readonly IWaitLogger DefaultLogger = NullLogger.Instance;
        private static readonly WaitStrategy DefaultStrategy = WaitStrategy.Linear;

        // Mutable global defaults
        private static TimeSpan timeout = DefaultTimeout;
        private static TimeSpan pollDelay = DefaultPollDelay;
        private static TimeSpan pollInterval = DefaultPollInterval;
        private static bool failSilently = DefaultFailSilently;
        private static bool ignoreExceptions = DefaultIgnoreExceptions;
        private static IWaitLogger logger = DefaultLogger;
        private static WaitStrategy strategy = DefaultStrategy;
        private static IList<Action>? prereqs = null;
        private static string? alias = null;
        private static WaitMetrics? metrics = null;

        /// <summary>
        /// Gets or sets the default timeout for wait operations.
        /// </summary>
        public static TimeSpan Timeout
        {
            get => timeout;
            set => timeout = value;
        }

        /// <summary>
        /// Gets or sets the default poll delay for wait operations.
        /// </summary>
        public static TimeSpan PollDelay
        {
            get => pollDelay;
            set => pollDelay = value;
        }

        /// <summary>
        /// Gets or sets the default poll interval for wait operations.
        /// </summary>
        public static TimeSpan PollInterval
        {
            get => pollInterval;
            set => pollInterval = value;
        }

        /// <summary>
        /// Gets or sets the default fail silently behavior.
        /// </summary>
        public static bool FailSilently
        {
            get => failSilently;
            set => failSilently = value;
        }

        /// <summary>
        /// Gets or sets the default ignore exceptions behavior.
        /// </summary>
        public static bool IgnoreExceptions
        {
            get => ignoreExceptions;
            set => ignoreExceptions = value;
        }

        /// <summary>
        /// Gets or sets the default logger for wait operations.
        /// </summary>
        public static IWaitLogger Logger
        {
            get => logger;
            set => logger = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Gets or sets the default wait strategy.
        /// </summary>
        public static WaitStrategy Strategy
        {
            get => strategy;
            set => strategy = value;
        }

        /// <summary>
        /// Gets or sets the default prerequisites for wait operations.
        /// </summary>
        public static IList<Action>? Prereqs
        {
            get => prereqs;
            set => prereqs = value;
        }

        /// <summary>
        /// Gets or sets the default alias for wait operations.
        /// </summary>
        public static string? Alias
        {
            get => alias;
            set => alias = value;
        }

        /// <summary>
        /// Gets or sets the default metrics collection.
        /// </summary>
        public static WaitMetrics? Metrics
        {
            get => metrics;
            set => metrics = value;
        }

        /// <summary>
        /// Resets all global configuration to default values.
        /// </summary>
        public static void ResetToDefaults()
        {
            timeout = DefaultTimeout;
            pollDelay = DefaultPollDelay;
            pollInterval = DefaultPollInterval;
            failSilently = DefaultFailSilently;
            ignoreExceptions = DefaultIgnoreExceptions;
            logger = DefaultLogger;
            strategy = DefaultStrategy;
            prereqs = null;
            alias = null;
            metrics = null;
        }
    }
}
