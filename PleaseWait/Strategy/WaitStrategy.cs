// <copyright file="WaitStrategy.cs" company="Esdet">
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
    /// <summary>
    /// Defines different strategies for waiting between condition checks.
    /// </summary>
    public enum WaitStrategy
    {
        /// <summary>
        /// Linear strategy - uses consistent polling intervals (default behavior).
        /// </summary>
        Linear,

        /// <summary>
        /// Exponential backoff strategy - increases delay exponentially between checks.
        /// Good for resource-intensive conditions or when you expect the condition to take time.
        /// </summary>
        ExponentialBackoff,

        /// <summary>
        /// Aggressive strategy - uses minimal delays for fast detection.
        /// Good for UI testing, real-time monitoring, or responsive scenarios.
        /// </summary>
        Aggressive,

        /// <summary>
        /// Conservative strategy - uses longer delays to minimize resource usage.
        /// Good for expensive operations or when you want to minimize overhead.
        /// </summary>
        Conservative,

        /// <summary>
        /// Adaptive strategy - adjusts polling frequency based on condition check performance.
        /// Uses metrics to optimize polling intervals dynamically.
        /// </summary>
        Adaptive,
    }
}
