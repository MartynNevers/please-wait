// <copyright file="GlobalConfigurationBuilder.cs" company="Esdet">
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

    /// <summary>
    /// Builder for configuring global PleaseWait defaults.
    /// </summary>
    public class GlobalConfigurationBuilder
    {
        /// <summary>
        /// Syntactic sugar method that can be used to make the code easier to read.
        /// </summary>
        /// <returns>The current <see cref="GlobalConfigurationBuilder"/>.</returns>
        public GlobalConfigurationBuilder With()
        {
            return this;
        }

        /// <summary>
        /// Syntactic sugar method that can be used to make the code easier to read.
        /// </summary>
        /// <returns>The current <see cref="GlobalConfigurationBuilder"/>.</returns>
        public GlobalConfigurationBuilder And()
        {
            return this;
        }

        /// <summary>
        /// Sets the default timeout for wait operations.
        /// </summary>
        /// <param name="timeout">The default timeout value.</param>
        /// <returns>The current configuration builder.</returns>
        public GlobalConfigurationBuilder Timeout(TimeSpan timeout)
        {
            GlobalDefaults.Timeout = timeout;
            return this;
        }

        /// <summary>
        /// Sets the default timeout for wait operations.
        /// </summary>
        /// <param name="value">The timeout value.</param>
        /// <param name="timeUnit">The time unit for the timeout value.</param>
        /// <returns>The current configuration builder.</returns>
        public GlobalConfigurationBuilder Timeout(double value, TimeUnit timeUnit)
        {
            return this.Timeout(new TimeConstraint(value, timeUnit).GetTimeSpan());
        }

        /// <summary>
        /// Sets the default timeout for wait operations (alias for Timeout).
        /// </summary>
        /// <param name="timeout">The default timeout value.</param>
        /// <returns>The current configuration builder.</returns>
        public GlobalConfigurationBuilder AtMost(TimeSpan timeout)
        {
            return this.Timeout(timeout);
        }

        /// <summary>
        /// Sets the default timeout for wait operations (alias for Timeout).
        /// </summary>
        /// <param name="value">The timeout value.</param>
        /// <param name="timeUnit">The time unit for the timeout value.</param>
        /// <returns>The current configuration builder.</returns>
        public GlobalConfigurationBuilder AtMost(double value, TimeUnit timeUnit)
        {
            return this.Timeout(value, timeUnit);
        }

        /// <summary>
        /// Sets the default poll delay for wait operations.
        /// </summary>
        /// <param name="pollDelay">The default poll delay value.</param>
        /// <returns>The current configuration builder.</returns>
        public GlobalConfigurationBuilder PollDelay(TimeSpan pollDelay)
        {
            GlobalDefaults.PollDelay = pollDelay;
            return this;
        }

        /// <summary>
        /// Sets the default poll delay for wait operations.
        /// </summary>
        /// <param name="value">The poll delay value.</param>
        /// <param name="timeUnit">The time unit for the poll delay value.</param>
        /// <returns>The current configuration builder.</returns>
        public GlobalConfigurationBuilder PollDelay(double value, TimeUnit timeUnit)
        {
            return this.PollDelay(new TimeConstraint(value, timeUnit).GetTimeSpan());
        }

        /// <summary>
        /// Sets the default poll interval for wait operations.
        /// </summary>
        /// <param name="pollInterval">The default poll interval value.</param>
        /// <returns>The current configuration builder.</returns>
        public GlobalConfigurationBuilder PollInterval(TimeSpan pollInterval)
        {
            GlobalDefaults.PollInterval = pollInterval;
            return this;
        }

        /// <summary>
        /// Sets the default poll interval for wait operations.
        /// </summary>
        /// <param name="value">The poll interval value.</param>
        /// <param name="timeUnit">The time unit for the poll interval value.</param>
        /// <returns>The current configuration builder.</returns>
        public GlobalConfigurationBuilder PollInterval(double value, TimeUnit timeUnit)
        {
            return this.PollInterval(new TimeConstraint(value, timeUnit).GetTimeSpan());
        }

        /// <summary>
        /// Sets both default poll delay and interval for wait operations.
        /// </summary>
        /// <param name="pollDelay">The default poll delay value.</param>
        /// <param name="pollInterval">The default poll interval value.</param>
        /// <returns>The current configuration builder.</returns>
        public GlobalConfigurationBuilder Polling(TimeSpan pollDelay, TimeSpan pollInterval)
        {
            return this.PollDelay(pollDelay).PollInterval(pollInterval);
        }

        /// <summary>
        /// Sets both default poll delay and interval for wait operations.
        /// </summary>
        /// <param name="pollDelayValue">The poll delay value.</param>
        /// <param name="pollDelayUnit">The time unit for the poll delay value.</param>
        /// <param name="pollIntervalValue">The poll interval value.</param>
        /// <param name="pollIntervalUnit">The time unit for the poll interval value.</param>
        /// <returns>The current configuration builder.</returns>
        public GlobalConfigurationBuilder Polling(double pollDelayValue, TimeUnit pollDelayUnit, double pollIntervalValue, TimeUnit pollIntervalUnit)
        {
            return this.Polling(
                new TimeConstraint(pollDelayValue, pollDelayUnit).GetTimeSpan(),
                new TimeConstraint(pollIntervalValue, pollIntervalUnit).GetTimeSpan());
        }

        /// <summary>
        /// Sets the default exception handling behavior.
        /// </summary>
        /// <param name="ignoreExceptions">Whether to ignore exceptions by default.</param>
        /// <returns>The current configuration builder.</returns>
        public GlobalConfigurationBuilder IgnoreExceptions(bool ignoreExceptions)
        {
            GlobalDefaults.IgnoreExceptions = ignoreExceptions;
            return this;
        }

        /// <summary>
        /// Sets the default fail silently behavior.
        /// </summary>
        /// <param name="failSilently">Whether to fail silently by default.</param>
        /// <returns>The current configuration builder.</returns>
        public GlobalConfigurationBuilder FailSilently(bool failSilently)
        {
            GlobalDefaults.FailSilently = failSilently;
            return this;
        }

        /// <summary>
        /// Sets both default exception handling options in a single call.
        /// </summary>
        /// <param name="ignoreExceptions">Whether to ignore exceptions by default.</param>
        /// <param name="failSilently">Whether to fail silently by default.</param>
        /// <returns>The current configuration builder.</returns>
        public GlobalConfigurationBuilder ExceptionHandling(bool ignoreExceptions, bool failSilently)
        {
            GlobalDefaults.IgnoreExceptions = ignoreExceptions;
            GlobalDefaults.FailSilently = failSilently;
            return this;
        }

        /// <summary>
        /// Sets a single default prerequisite for wait operations.
        /// </summary>
        /// <param name="prereq">The default prerequisite to execute.</param>
        /// <returns>The current configuration builder.</returns>
        public GlobalConfigurationBuilder Prereq(Action? prereq)
        {
            GlobalDefaults.Prereqs = prereq != null ? new List<Action> { prereq } : null;
            return this;
        }

        /// <summary>
        /// Sets the default prerequisites for wait operations.
        /// </summary>
        /// <param name="prereqs">The default prerequisites to execute.</param>
        /// <returns>The current configuration builder.</returns>
        public GlobalConfigurationBuilder Prereqs(IList<Action>? prereqs)
        {
            GlobalDefaults.Prereqs = prereqs;
            return this;
        }

        /// <summary>
        /// Sets the default alias for wait operations.
        /// </summary>
        /// <param name="alias">The default alias to use.</param>
        /// <returns>The current configuration builder.</returns>
        public GlobalConfigurationBuilder Alias(string? alias)
        {
            GlobalDefaults.Alias = alias;
            return this;
        }

        /// <summary>
        /// Sets the default logger for wait operations.
        /// </summary>
        /// <param name="logger">The default logger to use.</param>
        /// <returns>The current configuration builder.</returns>
        public GlobalConfigurationBuilder Logger(IWaitLogger logger)
        {
            GlobalDefaults.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            return this;
        }

        /// <summary>
        /// Sets whether to collect metrics by default for wait operations.
        /// </summary>
        /// <param name="collectMetrics">Whether to collect metrics by default.</param>
        /// <returns>The current configuration builder.</returns>
        public GlobalConfigurationBuilder Metrics(bool collectMetrics = true)
        {
            GlobalDefaults.Metrics = collectMetrics;
            return this;
        }

        /// <summary>
        /// Sets the default wait strategy.
        /// </summary>
        /// <param name="strategy">The default wait strategy to use.</param>
        /// <returns>The current configuration builder.</returns>
        public GlobalConfigurationBuilder Strategy(WaitStrategy strategy)
        {
            GlobalDefaults.Strategy = strategy;
            return this;
        }
    }
}
