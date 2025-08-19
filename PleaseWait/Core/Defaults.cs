// <copyright file="Defaults.cs" company="Esdet">
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

namespace PleaseWait.Core
{
    using System;
    using System.Collections.Generic;
    using PleaseWait.Logging;
    using PleaseWait.Strategy;

    internal class Defaults
    {
        public const bool FailSilently = false;
        public const bool IgnoreExceptions = true;
        public static readonly TimeSpan Timeout;
        public static readonly TimeSpan PollDelay;
        public static readonly TimeSpan PollInterval;
        public static readonly IList<Action>? Prereqs = null;
        public static readonly string? Alias = null;
        public static readonly IWaitLogger Logger = NullLogger.Instance;
        public static readonly WaitMetrics? Metrics = null;
        public static readonly WaitStrategy Strategy = WaitStrategy.Linear;

        static Defaults()
        {
            Timeout = TimeSpan.FromSeconds(10);
            PollDelay = TimeSpan.FromMilliseconds(100);
            PollInterval = TimeSpan.FromMilliseconds(100);
        }
    }
}
