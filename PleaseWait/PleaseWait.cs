// <copyright file="PleaseWait.cs" company="Esdet">
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
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;

    public class PleaseWait
    {
        private PleaseWait()
        {
        }

        private TimeSpan Timeout
        {
            get;
            set;
        }

        private bool Throws
        {
            get;
            set;
        }

        = true;

        private TimeSpan PollingRate
        {
            get;
            set;
        }

        = TimeSpan.FromMilliseconds(200);

        public static PleaseWait AtMost(double value, TimeUnit timeUnit)
        {
            var pleaseWait = new PleaseWait()
            {
                Timeout = ConvertToTimeSpan(value, timeUnit),
            };

            return pleaseWait;
        }

        public PleaseWait WithPollingRate(double value, TimeUnit timeUnit)
        {
            this.PollingRate = ConvertToTimeSpan(value, timeUnit);
            return this;
        }

        public PleaseWait AndThrows(bool throws)
        {
            this.Throws = throws;
            return this;
        }

        public void Until(Func<bool> condition)
        {
            this.Until(condition, actions: null);
        }

        public void Until(Func<bool> condition, Action action)
        {
            IList<Action>? actions = null;

            if (action != null)
            {
                actions = new List<Action>
                {
                    action,
                };
            }

            this.Until(condition, actions);
        }

        public void Until(Func<bool> condition, IList<Action>? actions)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var outcome = false;
            while (outcome == false && stopwatch.Elapsed < this.Timeout)
            {
                if (actions != null)
                {
                    foreach (var action in actions)
                    {
                        try
                        {
                            action.Invoke();
                        }
                        catch (Exception)
                        {
                            // Swallow exception
                        }
                    }
                }

                try
                {
                    outcome = condition();
                }
                catch (Exception)
                {
                    // Swallow exception
                }

                Thread.Sleep(this.PollingRate);
            }

            if (outcome == false && stopwatch.Elapsed > this.Timeout)
            {
                if (this.Throws)
                {
                    throw new TimeoutException($"PleaseWait timed out after {this.Timeout.Days}d {this.Timeout.Hours}h {this.Timeout.Minutes}m {this.Timeout.Seconds}s {this.Timeout.Milliseconds}ms");
                }
            }
        }

        private static TimeSpan ConvertToTimeSpan(double value, TimeUnit timeUnit)
        {
            return timeUnit switch
            {
                TimeUnit.MILLIS => TimeSpan.FromMilliseconds(value),
                TimeUnit.SECONDS => TimeSpan.FromSeconds(value),
                TimeUnit.MINUTES => TimeSpan.FromMinutes(value),
                TimeUnit.HOURS => TimeSpan.FromHours(value),
                TimeUnit.DAYS => TimeSpan.FromDays(value),
                _ => throw new NotImplementedException(),
            };
        }
    }
}
