// <copyright file="WaitMetrics.cs" company="Esdet">
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

    /// <summary>
    /// Contains performance metrics collected during a wait operation.
    /// </summary>
    public class WaitMetrics
    {
        /// <summary>
        /// Gets the number of times the condition was evaluated.
        /// </summary>
        public int ConditionChecks { get; set; }

        /// <summary>
        /// Gets the total time spent waiting for the condition.
        /// </summary>
        public TimeSpan TotalTime { get; set; }

        /// <summary>
        /// Gets the average time per condition check.
        /// </summary>
        public TimeSpan AverageCheckTime => this.ConditionChecks > 0
            ? TimeSpan.FromTicks(this.TotalTime.Ticks / this.ConditionChecks)
            : TimeSpan.Zero;

        /// <summary>
        /// Gets the fastest condition check time.
        /// </summary>
        public TimeSpan MinCheckTime { get; set; } = TimeSpan.MaxValue;

        /// <summary>
        /// Gets the slowest condition check time.
        /// </summary>
        public TimeSpan MaxCheckTime { get; set; } = TimeSpan.Zero;

        /// <summary>
        /// Gets the total time spent in poll delays.
        /// </summary>
        public TimeSpan PollDelayTime { get; set; }

        /// <summary>
        /// Gets the total time spent in poll intervals.
        /// </summary>
        public TimeSpan PollIntervalTime { get; set; }

        /// <summary>
        /// Gets a value indicating whether the wait operation was successful.
        /// </summary>
        public bool WasSuccessful { get; set; }

        /// <summary>
        /// Gets the alias used for the condition, if any.
        /// </summary>
        public string? ConditionAlias { get; set; }

        /// <summary>
        /// Gets the timeout that was configured for the wait operation.
        /// </summary>
        public TimeSpan ConfiguredTimeout { get; set; }

        /// <summary>
        /// Gets the poll delay that was configured for the wait operation.
        /// </summary>
        public TimeSpan ConfiguredPollDelay { get; set; }

        /// <summary>
        /// Gets the poll interval that was configured for the wait operation.
        /// </summary>
        public TimeSpan ConfiguredPollInterval { get; set; }

        /// <summary>
        /// Gets a string representation of the metrics.
        /// </summary>
        /// <returns>A formatted string containing the key metrics.</returns>
        public override string ToString()
        {
            return $"WaitMetrics: {this.ConditionChecks} checks, {this.TotalTime} total time, " +
                   $"{(this.WasSuccessful ? "SUCCESS" : "FAILED")}, " +
                   $"avg: {this.AverageCheckTime}, min: {this.MinCheckTime}, max: {this.MaxCheckTime}";
        }
    }
}
