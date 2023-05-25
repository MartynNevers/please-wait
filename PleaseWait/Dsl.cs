// <copyright file="Dsl.cs" company="Esdet">
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

    public class Dsl
    {
        private TimeSpan timeout = Defaults.Timeout;
        private TimeSpan pollDelay = Defaults.PollDelay;
        private TimeSpan pollInterval = Defaults.PollInterval;
        private bool ignoreExceptions = Defaults.IgnoreExceptions;
        private bool failSilently = Defaults.FailSilently;
        private IList<Action>? prereqs = null;
        private string? alias = null;

        private Dsl()
        {
        }

        public static Dsl Wait()
        {
            return new Dsl();
        }

        public Dsl With()
        {
            return this;
        }

        public Dsl And()
        {
            return this;
        }

        public Dsl AtMost(double value, TimeUnit timeUnit)
        {
            this.timeout = new TimeConstraint(value, timeUnit).GetTimeSpan();
            return this;
        }

        public Dsl AtMost(TimeSpan timeSpan)
        {
            this.timeout = timeSpan;
            return this;
        }

        public Dsl PollDelay(double value, TimeUnit timeUnit)
        {
            this.pollDelay = new TimeConstraint(value, timeUnit).GetTimeSpan();
            return this;
        }

        public Dsl PollDelay(TimeSpan timeSpan)
        {
            this.pollDelay = timeSpan;
            return this;
        }

        public Dsl PollInterval(double value, TimeUnit timeUnit)
        {
            this.pollInterval = new TimeConstraint(value, timeUnit).GetTimeSpan();
            return this;
        }

        public Dsl PollInterval(TimeSpan timeSpan)
        {
            this.pollInterval = timeSpan;
            return this;
        }

        public Dsl IgnoreExceptions(bool ignoreExceptions = true)
        {
            this.ignoreExceptions = ignoreExceptions;
            return this;
        }

        public Dsl FailSilently(bool failSilently = true)
        {
            this.failSilently = failSilently;
            return this;
        }

        public Dsl Prereq(Action prereq)
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

        public Dsl Prereqs(IList<Action>? prereqs)
        {
            this.prereqs = prereqs;
            return this;
        }

        public Dsl Alias(string alias)
        {
            this.alias = alias;
            return this;
        }

        public void Until(Func<bool> condition)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var outcome = false;
            while (outcome == false && stopwatch.Elapsed < this.timeout)
            {
                this.InvokePrereqs();
                Thread.Sleep(this.pollDelay);

                try
                {
                    outcome = condition();
                }
                catch (Exception)
                {
                    if (!this.ignoreExceptions)
                    {
                        throw;
                    }
                }

                Thread.Sleep(this.pollInterval);
            }

            if (outcome == false && stopwatch.Elapsed > this.timeout)
            {
                if (!this.failSilently)
                {
                    if (string.IsNullOrEmpty(this.alias))
                    {
                        throw new TimeoutException($"Condition was not fulfilled within {this.timeout}.");
                    }
                    else
                    {
                        throw new TimeoutException($"Condition with alias '{this.alias}' was not fulfilled within {this.timeout}.");
                    }
                }
            }
        }

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
    }
}
