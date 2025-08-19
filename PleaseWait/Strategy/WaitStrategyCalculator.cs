// <copyright file="WaitStrategyCalculator.cs" company="Esdet">
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

namespace PleaseWait.Strategy
{
    using System;

    /// <summary>
    /// Calculates delays for different wait strategies.
    /// </summary>
    internal class WaitStrategyCalculator
    {
        private readonly WaitStrategy strategy;
        private readonly TimeSpan pollDelay;
        private readonly TimeSpan pollInterval;
        private readonly TimeSpan timeout;
        private readonly WaitMetrics? metrics;

        /// <summary>
        /// Initializes a new instance of the <see cref="WaitStrategyCalculator"/> class.
        /// </summary>
        /// <param name="strategy">The wait strategy to use.</param>
        /// <param name="pollDelay">The configured poll delay.</param>
        /// <param name="pollInterval">The configured poll interval.</param>
        /// <param name="timeout">The configured timeout.</param>
        /// <param name="metrics">Optional metrics for adaptive calculations.</param>
        public WaitStrategyCalculator(WaitStrategy strategy, TimeSpan pollDelay, TimeSpan pollInterval, TimeSpan timeout, WaitMetrics? metrics = null)
        {
            this.strategy = strategy;
            this.pollDelay = pollDelay;
            this.pollInterval = pollInterval;
            this.timeout = timeout;
            this.metrics = metrics;
        }

        /// <summary>
        /// Calculates the initial delay based on the current wait strategy.
        /// </summary>
        /// <returns>The calculated initial delay.</returns>
        public TimeSpan CalculateInitialDelay()
        {
            return this.strategy switch
            {
                WaitStrategy.Linear => this.pollDelay,
                WaitStrategy.ExponentialBackoff => this.pollDelay,
                WaitStrategy.Aggressive => this.CalculateAggressiveInitialDelay(),
                WaitStrategy.Conservative => this.CalculateConservativeInitialDelay(),
                WaitStrategy.Adaptive => this.pollDelay,
                _ => this.pollDelay,
            };
        }

        /// <summary>
        /// Calculates the interval delay based on the current wait strategy.
        /// </summary>
        /// <param name="checkCount">The number of condition checks performed so far.</param>
        /// <returns>The calculated delay for the next interval.</returns>
        public TimeSpan CalculateIntervalDelay(int checkCount)
        {
            return this.strategy switch
            {
                WaitStrategy.Linear => this.pollInterval,
                WaitStrategy.ExponentialBackoff => this.CalculateExponentialBackoffDelay(checkCount),
                WaitStrategy.Aggressive => this.CalculateAggressiveDelay(),
                WaitStrategy.Conservative => this.CalculateConservativeDelay(),
                WaitStrategy.Adaptive => this.CalculateAdaptiveDelay(),
                _ => this.pollInterval,
            };
        }

        /// <summary>
        /// Calculates exponential backoff delay.
        /// </summary>
        /// <param name="checkCount">The number of condition checks performed so far.</param>
        /// <returns>The calculated delay.</returns>
        private TimeSpan CalculateExponentialBackoffDelay(int checkCount)
        {
            if (checkCount <= 1)
            {
                return this.pollInterval;
            }

            var factor = Math.Pow(2, checkCount - 1);
            var delayMs = this.pollInterval.TotalMilliseconds * factor;
            var maxDelayMs = this.timeout.TotalMilliseconds / 4; // Cap at 25% of timeout
            return TimeSpan.FromMilliseconds(Math.Min(delayMs, maxDelayMs));
        }

        /// <summary>
        /// Calculates aggressive delay (minimal delays).
        /// </summary>
        /// <returns>The calculated delay.</returns>
        private TimeSpan CalculateAggressiveDelay()
        {
            return TimeSpan.FromMilliseconds(Math.Max(1, this.pollInterval.TotalMilliseconds / 4));
        }

        /// <summary>
        /// Calculates conservative delay (longer delays).
        /// </summary>
        /// <returns>The calculated delay.</returns>
        private TimeSpan CalculateConservativeDelay()
        {
            return TimeSpan.FromMilliseconds(this.pollInterval.TotalMilliseconds * 2);
        }

        /// <summary>
        /// Calculates adaptive delay based on condition check performance.
        /// </summary>
        /// <returns>The calculated delay.</returns>
        private TimeSpan CalculateAdaptiveDelay()
        {
            if (this.metrics == null || this.metrics.ConditionChecks < 2)
            {
                return this.pollInterval;
            }

            var averageCheckTime = this.metrics.AverageCheckTime.TotalMilliseconds;
            var baseInterval = this.pollInterval.TotalMilliseconds;

            // If condition checks are fast, poll more frequently
            if (averageCheckTime < baseInterval / 10)
            {
                return TimeSpan.FromMilliseconds(baseInterval / 2);
            }

            // If condition checks are slow, poll less frequently
            if (averageCheckTime > baseInterval)
            {
                return TimeSpan.FromMilliseconds(baseInterval * 2);
            }

            // Default to base interval
            return this.pollInterval;
        }

        /// <summary>
        /// Calculates aggressive initial delay (minimal delay).
        /// </summary>
        /// <returns>The calculated initial delay.</returns>
        private TimeSpan CalculateAggressiveInitialDelay()
        {
            return TimeSpan.FromMilliseconds(Math.Max(1, this.pollDelay.TotalMilliseconds / 4));
        }

        /// <summary>
        /// Calculates conservative initial delay (longer delay).
        /// </summary>
        /// <returns>The calculated initial delay.</returns>
        private TimeSpan CalculateConservativeInitialDelay()
        {
            return TimeSpan.FromMilliseconds(this.pollDelay.TotalMilliseconds * 2);
        }
    }
}
