// <copyright file="Dsl.cs" company="Esdet">
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
    using System.Diagnostics;
    using System.Threading;
    using PleaseWait.Core;
    using PleaseWait.Logging;
    using PleaseWait.Strategy;

    public class Dsl
    {
        private TimeSpan timeout = GlobalDefaults.Timeout;
        private TimeSpan pollDelay = GlobalDefaults.PollDelay;
        private TimeSpan pollInterval = GlobalDefaults.PollInterval;
        private bool ignoreExceptions = GlobalDefaults.IgnoreExceptions;
        private bool failSilently = GlobalDefaults.FailSilently;
        private IList<Action>? prereqs = GlobalDefaults.Prereqs;
        private string? alias = GlobalDefaults.Alias;
        private IWaitLogger logger = GlobalDefaults.Logger;
        private WaitMetrics? metrics = GlobalDefaults.Metrics;
        private WaitStrategy strategy = GlobalDefaults.Strategy;

        private Dsl()
        {
        }

        /// <summary>
        /// Used to start defining a new waiting condition.
        /// </summary>
        /// <returns>The current <see cref="Dsl"/>.</returns>
        public static Dsl Wait()
        {
            return new Dsl();
        }

        /// <summary>
        /// Configures global defaults for PleaseWait.
        /// </summary>
        /// <returns>A configuration builder for setting global defaults.</returns>
        public ConfigurationBuilder Configure()
        {
            return new ConfigurationBuilder();
        }

        /// <summary>
        /// Resets all global configuration to default values.
        /// </summary>
        public void ResetToDefaults()
        {
            GlobalDefaults.ResetToDefaults();
        }

        /// <summary>
        /// Syntactic sugar method that can be used to make the code easier to read.
        /// </summary>
        /// <returns>The current <see cref="Dsl"/>.</returns>
        public Dsl With()
        {
            return this;
        }

        /// <summary>
        /// Syntactic sugar method that can be used to make the code easier to read.
        /// </summary>
        /// <returns>The current <see cref="Dsl"/>.</returns>
        public Dsl And()
        {
            return this;
        }

        /// <summary>
        /// Defines the timeout for the waiting condition.
        /// </summary>
        /// <param name="value">The timeout value.</param>
        /// <param name="timeUnit">The corresponding <see cref="TimeUnit"/> for the timeout value.</param>
        /// <returns>The current <see cref="Dsl"/>.</returns>
        public Dsl Timeout(double value, TimeUnit timeUnit) =>
            this.SetTimeout(new TimeConstraint(value, timeUnit).GetTimeSpan());

        /// <summary>
        /// Defines the timeout for the waiting condition.
        /// </summary>
        /// <param name="timeSpan">The timeout value.</param>
        /// <returns>The current <see cref="Dsl"/>.</returns>
        public Dsl Timeout(TimeSpan timeSpan) =>
            this.SetTimeout(timeSpan);

        /// <summary>
        /// Defines the timeout for the waiting condition.
        /// </summary>
        /// <param name="value">The timeout value.</param>
        /// <param name="timeUnit">The corresponding <see cref="TimeUnit"/> for the timeout value.</param>
        /// <returns>The current <see cref="Dsl"/>.</returns>
        public Dsl AtMost(double value, TimeUnit timeUnit) =>
            this.SetTimeout(new TimeConstraint(value, timeUnit).GetTimeSpan());

        /// <summary>
        /// Defines the timeout for the waiting condition.
        /// </summary>
        /// <param name="timeSpan">The timeout value.</param>
        /// <returns>The current <see cref="Dsl"/>.</returns>
        public Dsl AtMost(TimeSpan timeSpan) =>
            this.SetTimeout(timeSpan);

        /// <summary>
        /// Defines the polling delay to be used in the waiting condition. Applied before the check on a condition.
        /// </summary>
        /// <param name="value">The polling delay value.</param>
        /// <param name="timeUnit">The corresponding <see cref="TimeUnit"/> for the polling delay.</param>
        /// <returns>The current <see cref="Dsl"/>.</returns>
        public Dsl PollDelay(double value, TimeUnit timeUnit) =>
            this.SetPollDelay(new TimeConstraint(value, timeUnit).GetTimeSpan());

        /// <summary>
        /// Defines the polling delay to be used in the waiting condition. Applied before the check on a condition.
        /// </summary>
        /// <param name="timeSpan">The timeout value.</param>
        /// <returns>The current <see cref="Dsl"/>.</returns>
        public Dsl PollDelay(TimeSpan timeSpan) =>
            this.SetPollDelay(timeSpan);

        /// <summary>
        /// Defines the polling interval to be used in the waiting condition. Applied after the check on a condition.
        /// </summary>
        /// <param name="value">The polling interval value.</param>
        /// <param name="timeUnit">The corresponding <see cref="TimeUnit"/> for the polling interval.</param>
        /// <returns>The current <see cref="Dsl"/>.</returns>
        public Dsl PollInterval(double value, TimeUnit timeUnit) =>
            this.SetPollInterval(new TimeConstraint(value, timeUnit).GetTimeSpan());

        /// <summary>
        /// Defines the polling interval to be used in the waiting condition. Applied after the check on a condition.
        /// </summary>
        /// <param name="timeSpan">The polling interval value.</param>
        /// <returns>The current <see cref="Dsl"/>.</returns>
        public Dsl PollInterval(TimeSpan timeSpan) =>
            this.SetPollInterval(timeSpan);

        /// <summary>
        /// Defines whether or not to ignore exceptions thrown by the code when checking for a condition.
        /// </summary>
        /// <param name="ignoreExceptions">Set to true to ignore exceptions, false otherwise.</param>
        /// <returns>The current <see cref="Dsl"/>.</returns>
        public Dsl IgnoreExceptions(bool ignoreExceptions = true)
        {
            this.ignoreExceptions = ignoreExceptions;
            return this;
        }

        /// <summary>
        /// Defines whether or not to fail silently when a condition is not met when the timeout is exceeded.
        /// </summary>
        /// <param name="failSilently">Set to true to fail silently, false otherwise.</param>
        /// <returns>The current <see cref="Dsl"/>.</returns>
        public Dsl FailSilently(bool failSilently = true)
        {
            this.failSilently = failSilently;
            return this;
        }

        /// <summary>
        /// Adds an <see cref="Action"/> to be executed before the condition to wait for is evaluated.
        /// </summary>
        /// <param name="prereq">The <see cref="Action"/> to be executed before the condition to wait for is evaluated.</param>
        /// <returns>The current <see cref="Dsl"/>.</returns>
        public Dsl Prereq(Action? prereq)
        {
            if (prereq != null)
            {
                this.prereqs = new List<Action>
                {
                    prereq,
                };
            }

            return this;
        }

        /// <summary>
        /// Adds an <see cref="IList"/> of <see cref="Action"/>s to be executed before the condition to wait for is evaluated.
        /// </summary>
        /// <param name="prereqs">The <see cref="IList"/> of <see cref="Action"/>s to be executed before the condition to wait for is evaluated.</param>
        /// <returns>The current <see cref="Dsl"/>.</returns>
        public Dsl Prereqs(IList<Action>? prereqs)
        {
            this.prereqs = prereqs;
            return this;
        }

        /// <summary>
        /// Adds an alias for the current waiting condition. Can be used to create more expressive exception messages.
        /// </summary>
        /// <param name="alias">The alias to be used for the current waiting condition.</param>
        /// <returns>The current <see cref="Dsl"/>.</returns>
        public Dsl Alias(string? alias)
        {
            this.alias = alias;
            if (this.metrics != null)
            {
                this.metrics.ConditionAlias = alias;
            }

            return this;
        }

        /// <summary>
        /// Sets the logger for wait operations. Use this to enable diagnostic logging.
        /// </summary>
        /// <param name="logger">The logger to use for wait operations.</param>
        /// <returns>The current <see cref="Dsl"/>.</returns>
        public Dsl WithLogger(IWaitLogger logger)
        {
            this.logger = logger ?? NullLogger.Instance;
            return this;
        }

        /// <summary>
        /// Enables performance metrics collection for the wait operation.
        /// </summary>
        /// <returns>The current <see cref="Dsl"/>.</returns>
        public Dsl WithMetrics()
        {
            this.metrics = new WaitMetrics
            {
                ConditionAlias = this.alias,
                ConfiguredTimeout = this.timeout,
                ConfiguredPollDelay = this.pollDelay,
                ConfiguredPollInterval = this.pollInterval,
            };
            return this;
        }

        /// <summary>
        /// Sets the wait strategy for the operation.
        /// </summary>
        /// <param name="strategy">The wait strategy to use.</param>
        /// <returns>The current <see cref="Dsl"/>.</returns>
        public Dsl WithStrategy(WaitStrategy strategy)
        {
            this.strategy = strategy;
            return this;
        }

        /// <summary>
        /// Suspends the current thread for the specified amount of time.
        /// </summary>
        /// <param name="timeSpan">The timeout value.</param>
        /// <returns>The current <see cref="Dsl"/>.</returns>
        public Dsl Sleep(TimeSpan timeSpan)
        {
            Thread.Sleep(timeSpan);
            return this;
        }

        /// <summary>
        /// Suspends the current thread for the specified amount of time.
        /// </summary>
        /// <param name="value">The timeout value.</param>
        /// <param name="timeUnit">The corresponding <see cref="TimeUnit"/> for the timeout value.</param>
        /// <returns>The current <see cref="Dsl"/>.</returns>
        public Dsl Sleep(double value, TimeUnit timeUnit) =>
            this.Sleep(new TimeConstraint(value, timeUnit).GetTimeSpan());

        /// <summary>
        /// Defines the waiting condition. PleaseWait will execute this code until it returns a boolean equal to expected, the timeout is exceeded, or cancellation is requested.
        /// </summary>
        /// <param name="condition">The condition to wait for. Should return a boolean.</param>
        /// <param name="expected">The boolean value that the condition should return.</param>
        /// <param name="cancellationToken">The cancellation token to observe for cancellation requests. Defaults to CancellationToken.None (no cancellation).</param>
        /// <returns>The performance metrics for this wait operation, or null if metrics were not enabled.</returns>
        /// <exception cref="TimeoutException">Thrown when the timeout is exceeded without the condition returning a boolean equal to expected.</exception>
        /// <exception cref="OperationCanceledException">Thrown when cancellation is requested via the cancellation token.</exception>
        public WaitMetrics? Until(Func<bool> condition, bool expected = true, CancellationToken cancellationToken = default)
        {
            try
            {
                var success = this.WaitForCondition(condition, expected, cancellationToken);
                if (!success && !this.failSilently)
                {
                    var conditionDescription = this.alias ?? "condition";
                    this.logger.LogTimeout(conditionDescription, this.timeout);
                    throw this.CreateTimeoutException();
                }

                return this.metrics;
            }
            catch (OperationCanceledException)
            {
                var conditionDescription = this.alias ?? "condition";
                this.logger.LogCancellation(conditionDescription);
                throw;
            }
        }

        /// <summary>
        /// Defines the waiting condition. PleaseWait will execute this code until it returns a boolean true, the timeout is exceeded, or cancellation is requested.
        /// </summary>
        /// <param name="condition">The condition to wait for. Should return a boolean.</param>
        /// <param name="cancellationToken">The cancellation token to observe for cancellation requests. Defaults to CancellationToken.None (no cancellation).</param>
        /// <exception cref="TimeoutException">Thrown when the timeout is exceeded without the condition returning a boolean true.</exception>
        /// <exception cref="OperationCanceledException">Thrown when cancellation is requested via the cancellation token.</exception>
        public void UntilTrue(Func<bool> condition, CancellationToken cancellationToken = default)
        {
            this.Until(condition, true, cancellationToken);
        }

        /// <summary>
        /// Defines the waiting condition. PleaseWait will execute this code until it returns a boolean false, the timeout is exceeded, or cancellation is requested.
        /// </summary>
        /// <param name="condition">The condition to wait for. Should return a boolean.</param>
        /// <param name="cancellationToken">The cancellation token to observe for cancellation requests. Defaults to CancellationToken.None (no cancellation).</param>
        /// <exception cref="TimeoutException">Thrown when the timeout is exceeded without the condition returning a boolean false.</exception>
        /// <exception cref="OperationCanceledException">Thrown when cancellation is requested via the cancellation token.</exception>
        public void UntilFalse(Func<bool> condition, CancellationToken cancellationToken = default)
        {
            this.Until(condition, false, cancellationToken);
        }

        /// <summary>
        /// Invokes the prerequisites as defined by the user.
        /// </summary>
        private void InvokePrereqs()
        {
            if (this.prereqs != null)
            {
                foreach (var prereq in this.prereqs)
                {
                    try
                    {
                        prereq.Invoke();
                    }
                    catch (Exception)
                    {
                        if (!this.ignoreExceptions)
                        {
                            throw;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Core waiting logic that polls for a condition to be met.
        /// </summary>
        /// <param name="condition">The condition to wait for.</param>
        /// <param name="expected">The expected boolean value.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>True if condition was met, false if timeout occurred.</returns>
        private bool WaitForCondition(Func<bool> condition, bool expected, CancellationToken cancellationToken)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var conditionDescription = this.alias ?? "condition";
            this.logger.LogWaitStart(conditionDescription, this.timeout);

            var outcome = !expected;
            var checkCount = 0;
            var pollDelayStopwatch = new Stopwatch();
            var pollIntervalStopwatch = new Stopwatch();

            while (outcome == !expected && stopwatch.Elapsed < this.timeout)
            {
                cancellationToken.ThrowIfCancellationRequested();

                this.InvokePrereqs();

                pollDelayStopwatch.Start();
                var calculator = new WaitStrategyCalculator(this.strategy, this.pollDelay, this.pollInterval, this.timeout, this.metrics);
                var initialDelay = calculator.CalculateInitialDelay();
                Thread.Sleep(initialDelay);
                pollDelayStopwatch.Stop();

                var conditionCheckStopwatch = new Stopwatch();
                conditionCheckStopwatch.Start();
                outcome = this.EvaluateCondition(condition);
                conditionCheckStopwatch.Stop();
                checkCount++;

                // Record metrics if enabled
                if (this.metrics != null)
                {
                    var checkTime = conditionCheckStopwatch.Elapsed;
                    this.metrics.ConditionChecks = checkCount;
                    this.metrics.TotalTime = stopwatch.Elapsed;
                    this.metrics.PollDelayTime = pollDelayStopwatch.Elapsed;

                    if (checkTime < this.metrics.MinCheckTime)
                    {
                        this.metrics.MinCheckTime = checkTime;
                    }

                    if (checkTime > this.metrics.MaxCheckTime)
                    {
                        this.metrics.MaxCheckTime = checkTime;
                    }
                }

                this.logger.LogConditionCheck(conditionDescription, outcome == expected, stopwatch.Elapsed);

                pollIntervalStopwatch.Start();
                var intervalDelay = calculator.CalculateIntervalDelay(checkCount);
                Thread.Sleep(intervalDelay);
                pollIntervalStopwatch.Stop();
            }

            // Final metrics update
            if (this.metrics != null)
            {
                this.metrics.WasSuccessful = outcome == expected;
                this.metrics.PollIntervalTime = pollIntervalStopwatch.Elapsed;
            }

            if (outcome == expected)
            {
                this.logger.LogWaitSuccess(conditionDescription, stopwatch.Elapsed, checkCount);
            }

            return outcome == expected;
        }

        /// <summary>
        /// Evaluates a condition with exception handling.
        /// </summary>
        /// <param name="condition">The condition to evaluate.</param>
        /// <returns>The result of the condition evaluation.</returns>
        private bool EvaluateCondition(Func<bool> condition)
        {
            try
            {
                return condition();
            }
            catch (Exception)
            {
                if (!this.ignoreExceptions)
                {
                    throw;
                }

                return false; // Return false to continue waiting
            }
        }

        /// <summary>
        /// Creates a timeout exception with appropriate message.
        /// </summary>
        /// <returns>A TimeoutException with context-specific message.</returns>
        private TimeoutException CreateTimeoutException()
        {
            var message = string.IsNullOrEmpty(this.alias)
                ? $"Condition was not fulfilled within {this.timeout}."
                : $"Condition with alias '{this.alias}' was not fulfilled within {this.timeout}.";

            return new TimeoutException(message);
        }

        /// <summary>
        /// Sets the timeout value.
        /// </summary>
        /// <param name="timeSpan">The timeout value.</param>
        /// <returns>The current <see cref="Dsl"/>.</returns>
        private Dsl SetTimeout(TimeSpan timeSpan)
        {
            this.timeout = timeSpan;
            if (this.metrics != null)
            {
                this.metrics.ConfiguredTimeout = timeSpan;
            }

            return this;
        }

        /// <summary>
        /// Sets the polling delay value.
        /// </summary>
        /// <param name="timeSpan">The polling delay value.</param>
        /// <returns>The current <see cref="Dsl"/>.</returns>
        private Dsl SetPollDelay(TimeSpan timeSpan)
        {
            this.pollDelay = timeSpan;
            if (this.metrics != null)
            {
                this.metrics.ConfiguredPollDelay = timeSpan;
            }

            return this;
        }

        /// <summary>
        /// Sets the polling interval value.
        /// </summary>
        /// <param name="timeSpan">The polling interval value.</param>
        /// <returns>The current <see cref="Dsl"/>.</returns>
        private Dsl SetPollInterval(TimeSpan timeSpan)
        {
            this.pollInterval = timeSpan;
            if (this.metrics != null)
            {
                this.metrics.ConfiguredPollInterval = timeSpan;
            }

            return this;
        }
    }
}
