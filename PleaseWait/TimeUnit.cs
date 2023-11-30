// <copyright file="TimeUnit.cs" company="Esdet">
// Copyright 2023 the original author or authors.
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
    /// <summary>
    /// The unit of time to be used when defining timeouts, polling delays and polling intervals.
    /// </summary>
    public enum TimeUnit
    {
        /// <summary>
        /// Milliseconds.
        /// </summary>
        MILLIS,

        /// <summary>
        /// Seconds.
        /// </summary>
        SECONDS,

        /// <summary>
        /// Minutes.
        /// </summary>
        MINUTES,

        /// <summary>
        /// Hours.
        /// </summary>
        HOURS,

        /// <summary>
        /// Days.
        /// </summary>
        DAYS,
    }
}
