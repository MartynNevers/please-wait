// <copyright file="WaitConfig.cs" company="Esdet">
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

namespace PleaseWait
{
    using System;
    using System.Collections.Generic;
    using PleaseWait.Core;
    using PleaseWait.Logging;
    using PleaseWait.Strategy;
    using static PleaseWait.Strategy.WaitStrategy;
    using static PleaseWait.TimeUnit;

    /// <summary>
    /// Configuration object for PleaseWait instances.
    /// Provides a fluent API for creating reusable configurations with partial overrides.
    /// Null values indicate "use global default".
    /// </summary>
    public class WaitConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WaitConfig"/> class.
        /// Captures current global default values to ensure consistent behavior.
        /// </summary>
        public WaitConfig()
        {
            // Capture current global values when config is created
            this.DefaultTimeout = GlobalDefaults.Timeout;
            this.DefaultPollDelay = GlobalDefaults.PollDelay;
            this.DefaultPollInterval = GlobalDefaults.PollInterval;
            this.DefaultIgnoreExceptions = GlobalDefaults.IgnoreExceptions;
            this.DefaultFailSilently = GlobalDefaults.FailSilently;
            this.DefaultPrereqs = GlobalDefaults.Prereqs;
            this.DefaultAlias = GlobalDefaults.Alias;
            this.DefaultLogger = GlobalDefaults.Logger;
            this.DefaultMetrics = GlobalDefaults.Metrics;
            this.DefaultStrategy = GlobalDefaults.Strategy;
        }

        /// <summary>
        /// Gets or sets the timeout for wait operations.
        /// Null indicates use global default.
        /// </summary>
        public TimeSpan? Timeout { get; set; } = null;

        /// <summary>
        /// Gets or sets the poll delay for wait operations.
        /// Null indicates use global default.
        /// </summary>
        public TimeSpan? PollDelay { get; set; } = null;

        /// <summary>
        /// Gets or sets the poll interval for wait operations.
        /// Null indicates use global default.
        /// </summary>
        public TimeSpan? PollInterval { get; set; } = null;

        /// <summary>
        /// Gets or sets whether to ignore exceptions during condition checks.
        /// Null indicates use global default.
        /// </summary>
        public bool? IgnoreExceptions { get; set; } = null;

        /// <summary>
        /// Gets or sets whether to fail silently instead of throwing exceptions.
        /// Null indicates use global default.
        /// </summary>
        public bool? FailSilently { get; set; } = null;

        /// <summary>
        /// Gets or sets the prerequisite actions to execute before condition checks.
        /// Null indicates use global default.
        /// </summary>
        public IList<Action>? Prereqs { get; set; } = null;

        /// <summary>
        /// Gets or sets the alias for the condition.
        /// Null indicates use global default.
        /// </summary>
        public string? Alias { get; set; } = null;

        /// <summary>
        /// Gets or sets the logger for wait operations.
        /// Null indicates use global default.
        /// </summary>
        public IWaitLogger? Logger { get; set; } = null;

        /// <summary>
        /// Gets or sets whether to collect metrics during wait operations.
        /// Null indicates use global default.
        /// </summary>
        public bool? CollectMetrics { get; set; } = null;

        /// <summary>
        /// Gets or sets the wait strategy to use.
        /// Null indicates use global default.
        /// </summary>
        public WaitStrategy? Strategy { get; set; } = null;

        // Captured global default values (set at construction time)
        internal TimeSpan DefaultTimeout { get; set; }

        internal TimeSpan DefaultPollDelay { get; set; }

        internal TimeSpan DefaultPollInterval { get; set; }

        internal bool DefaultIgnoreExceptions { get; set; }

        internal bool DefaultFailSilently { get; set; }

        internal IList<Action>? DefaultPrereqs { get; set; }

        internal string? DefaultAlias { get; set; }

        internal IWaitLogger DefaultLogger { get; set; }

        internal WaitMetrics? DefaultMetrics { get; set; }

        internal WaitStrategy DefaultStrategy { get; set; }

        /// <summary>
        /// Sets the timeout for wait operations.
        /// </summary>
        /// <param name="timeout">The timeout value.</param>
        /// <returns>The current configuration.</returns>
        public WaitConfig WithTimeout(TimeSpan timeout)
        {
            this.Timeout = timeout;
            return this;
        }

        /// <summary>
        /// Sets the timeout for wait operations.
        /// </summary>
        /// <param name="value">The timeout value.</param>
        /// <param name="timeUnit">The time unit for the timeout value.</param>
        /// <returns>The current configuration.</returns>
        public WaitConfig WithTimeout(double value, TimeUnit timeUnit)
        {
            return this.WithTimeout(new TimeConstraint(value, timeUnit).GetTimeSpan());
        }

        /// <summary>
        /// Sets the poll delay for wait operations.
        /// </summary>
        /// <param name="pollDelay">The poll delay value.</param>
        /// <returns>The current configuration.</returns>
        public WaitConfig WithPollDelay(TimeSpan pollDelay)
        {
            this.PollDelay = pollDelay;
            return this;
        }

        /// <summary>
        /// Sets the poll delay for wait operations.
        /// </summary>
        /// <param name="value">The poll delay value.</param>
        /// <param name="timeUnit">The time unit for the poll delay value.</param>
        /// <returns>The current configuration.</returns>
        public WaitConfig WithPollDelay(double value, TimeUnit timeUnit)
        {
            return this.WithPollDelay(new TimeConstraint(value, timeUnit).GetTimeSpan());
        }

        /// <summary>
        /// Sets the poll interval for wait operations.
        /// </summary>
        /// <param name="pollInterval">The poll interval value.</param>
        /// <returns>The current configuration.</returns>
        public WaitConfig WithPollInterval(TimeSpan pollInterval)
        {
            this.PollInterval = pollInterval;
            return this;
        }

        /// <summary>
        /// Sets the poll interval for wait operations.
        /// </summary>
        /// <param name="value">The poll interval value.</param>
        /// <param name="timeUnit">The time unit for the poll interval value.</param>
        /// <returns>The current configuration.</returns>
        public WaitConfig WithPollInterval(double value, TimeUnit timeUnit)
        {
            return this.WithPollInterval(new TimeConstraint(value, timeUnit).GetTimeSpan());
        }

        /// <summary>
        /// Sets the polling configuration (delay and interval) for wait operations.
        /// </summary>
        /// <param name="pollDelay">The poll delay value.</param>
        /// <param name="pollInterval">The poll interval value.</param>
        /// <returns>The current configuration.</returns>
        public WaitConfig WithPolling(TimeSpan pollDelay, TimeSpan pollInterval)
        {
            this.PollDelay = pollDelay;
            this.PollInterval = pollInterval;
            return this;
        }

        /// <summary>
        /// Sets the polling configuration (delay and interval) for wait operations.
        /// </summary>
        /// <param name="pollDelayValue">The poll delay value.</param>
        /// <param name="pollDelayUnit">The time unit for the poll delay value.</param>
        /// <param name="pollIntervalValue">The poll interval value.</param>
        /// <param name="pollIntervalUnit">The time unit for the poll interval value.</param>
        /// <returns>The current configuration.</returns>
        public WaitConfig WithPolling(double pollDelayValue, TimeUnit pollDelayUnit, double pollIntervalValue, TimeUnit pollIntervalUnit)
        {
            return this.WithPolling(
                new TimeConstraint(pollDelayValue, pollDelayUnit).GetTimeSpan(),
                new TimeConstraint(pollIntervalValue, pollIntervalUnit).GetTimeSpan());
        }

        /// <summary>
        /// Sets whether to ignore exceptions during condition checks.
        /// </summary>
        /// <param name="ignoreExceptions">Whether to ignore exceptions.</param>
        /// <returns>The current configuration.</returns>
        public WaitConfig WithIgnoreExceptions(bool ignoreExceptions)
        {
            this.IgnoreExceptions = ignoreExceptions;
            return this;
        }

        /// <summary>
        /// Sets whether to fail silently instead of throwing exceptions.
        /// </summary>
        /// <param name="failSilently">Whether to fail silently.</param>
        /// <returns>The current configuration.</returns>
        public WaitConfig WithFailSilently(bool failSilently)
        {
            this.FailSilently = failSilently;
            return this;
        }

        /// <summary>
        /// Sets the exception handling behavior.
        /// </summary>
        /// <param name="ignoreExceptions">Whether to ignore exceptions during condition checks.</param>
        /// <param name="failSilently">Whether to fail silently instead of throwing exceptions.</param>
        /// <returns>The current configuration.</returns>
        public WaitConfig WithExceptionHandling(bool ignoreExceptions, bool failSilently)
        {
            this.IgnoreExceptions = ignoreExceptions;
            this.FailSilently = failSilently;
            return this;
        }

        /// <summary>
        /// Sets the logger for wait operations.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        /// <returns>The current configuration.</returns>
        public WaitConfig WithLogger(IWaitLogger logger)
        {
            this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            return this;
        }

        /// <summary>
        /// Sets whether to collect metrics during wait operations.
        /// </summary>
        /// <param name="collectMetrics">Whether to collect metrics.</param>
        /// <returns>The current configuration.</returns>
        public WaitConfig WithMetrics(bool collectMetrics = true)
        {
            this.CollectMetrics = collectMetrics;
            return this;
        }

        /// <summary>
        /// Sets the wait strategy to use.
        /// </summary>
        /// <param name="strategy">The wait strategy to use.</param>
        /// <returns>The current configuration.</returns>
        public WaitConfig WithStrategy(WaitStrategy strategy)
        {
            this.Strategy = strategy;
            return this;
        }

        /// <summary>
        /// Sets the alias for the condition.
        /// </summary>
        /// <param name="alias">The alias to use.</param>
        /// <returns>The current configuration.</returns>
        public WaitConfig WithAlias(string alias)
        {
            this.Alias = alias;
            return this;
        }

        /// <summary>
        /// Sets the prerequisite actions to execute before condition checks.
        /// </summary>
        /// <param name="prereqs">The prerequisite actions to execute.</param>
        /// <returns>The current configuration.</returns>
        public WaitConfig WithPrerequisites(IList<Action> prereqs)
        {
            this.Prereqs = prereqs;
            return this;
        }

        /// <summary>
        /// Sets a single prerequisite action to execute before condition checks.
        /// </summary>
        /// <param name="prereq">The prerequisite action to execute.</param>
        /// <returns>The current configuration.</returns>
        public WaitConfig WithPrerequisite(Action prereq)
        {
            this.Prereqs = new List<Action> { prereq };
            return this;
        }
    }
}
