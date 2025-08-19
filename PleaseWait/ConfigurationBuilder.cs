// <copyright file="ConfigurationBuilder.cs" company="Esdet">
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
    public class ConfigurationBuilder
    {
        /// <summary>
        /// Sets the default timeout for wait operations.
        /// </summary>
        /// <param name="timeout">The default timeout value.</param>
        /// <returns>The current configuration builder.</returns>
        public ConfigurationBuilder DefaultTimeout(TimeSpan timeout)
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
        public ConfigurationBuilder DefaultTimeout(double value, TimeUnit timeUnit)
        {
            return this.DefaultTimeout(new TimeConstraint(value, timeUnit).GetTimeSpan());
        }

        /// <summary>
        /// Sets the default poll delay for wait operations.
        /// </summary>
        /// <param name="pollDelay">The default poll delay value.</param>
        /// <returns>The current configuration builder.</returns>
        public ConfigurationBuilder DefaultPollDelay(TimeSpan pollDelay)
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
        public ConfigurationBuilder DefaultPollDelay(double value, TimeUnit timeUnit)
        {
            return this.DefaultPollDelay(new TimeConstraint(value, timeUnit).GetTimeSpan());
        }

        /// <summary>
        /// Sets the default poll interval for wait operations.
        /// </summary>
        /// <param name="pollInterval">The default poll interval value.</param>
        /// <returns>The current configuration builder.</returns>
        public ConfigurationBuilder DefaultPollInterval(TimeSpan pollInterval)
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
        public ConfigurationBuilder DefaultPollInterval(double value, TimeUnit timeUnit)
        {
            return this.DefaultPollInterval(new TimeConstraint(value, timeUnit).GetTimeSpan());
        }

        /// <summary>
        /// Sets the default exception handling behavior.
        /// </summary>
        /// <param name="ignoreExceptions">Whether to ignore exceptions by default.</param>
        /// <returns>The current configuration builder.</returns>
        public ConfigurationBuilder DefaultIgnoreExceptions(bool ignoreExceptions)
        {
            GlobalDefaults.IgnoreExceptions = ignoreExceptions;
            return this;
        }

        /// <summary>
        /// Sets the default fail silently behavior.
        /// </summary>
        /// <param name="failSilently">Whether to fail silently by default.</param>
        /// <returns>The current configuration builder.</returns>
        public ConfigurationBuilder DefaultFailSilently(bool failSilently)
        {
            GlobalDefaults.FailSilently = failSilently;
            return this;
        }

        /// <summary>
        /// Sets the default logger for wait operations.
        /// </summary>
        /// <param name="logger">The default logger to use.</param>
        /// <returns>The current configuration builder.</returns>
        public ConfigurationBuilder DefaultLogger(IWaitLogger logger)
        {
            GlobalDefaults.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            return this;
        }

        /// <summary>
        /// Sets the default wait strategy.
        /// </summary>
        /// <param name="strategy">The default wait strategy to use.</param>
        /// <returns>The current configuration builder.</returns>
        public ConfigurationBuilder DefaultStrategy(WaitStrategy strategy)
        {
            GlobalDefaults.Strategy = strategy;
            return this;
        }

        /// <summary>
        /// Sets the default prerequisites for wait operations.
        /// </summary>
        /// <param name="prereqs">The default prerequisites to execute.</param>
        /// <returns>The current configuration builder.</returns>
        public ConfigurationBuilder DefaultPrerequisites(IList<Action>? prereqs)
        {
            GlobalDefaults.Prereqs = prereqs;
            return this;
        }

        /// <summary>
        /// Sets the default alias for wait operations.
        /// </summary>
        /// <param name="alias">The default alias to use.</param>
        /// <returns>The current configuration builder.</returns>
        public ConfigurationBuilder DefaultAlias(string? alias)
        {
            GlobalDefaults.Alias = alias;
            return this;
        }
    }
}
