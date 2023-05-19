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
        private Dsl()
        {
        }

        private TimeSpan Timeout
        {
            get;
            set;
        }

        = Defaults.Timeout;

        private TimeSpan PollDelay
        {
            get;
            set;
        }

        = Defaults.PollDelay;

        private TimeSpan PollInterval
        {
            get;
            set;
        }

        = Defaults.PollInterval;

        private bool ShouldIgnoreExceptions
        {
            get;
            set;
        }

        = Defaults.ShouldIgnoreExceptions;

        private bool ShouldFailSilently
        {
            get;
            set;
        }

        = Defaults.ShouldFailSilently;

        private IList<Action>? Prereqs
        {
            get;
            set;
        }

        public static Dsl Wait()
        {
            return new Dsl();
        }

        public Dsl AtMost(double value, TimeUnit timeUnit)
        {
            this.Timeout = new TimeConstraint(value, timeUnit).GetTimeSpan();
            return this;
        }

        public Dsl AtMost(TimeSpan timeSpan)
        {
            this.Timeout = timeSpan;
            return this;
        }

        public Dsl WithPollDelay(double value, TimeUnit timeUnit)
        {
            this.PollDelay = new TimeConstraint(value, timeUnit).GetTimeSpan();
            return this;
        }

        public Dsl WithPollDelay(TimeSpan timeSpan)
        {
            this.PollDelay = timeSpan;
            return this;
        }

        public Dsl WithPollInterval(double value, TimeUnit timeUnit)
        {
            this.PollInterval = new TimeConstraint(value, timeUnit).GetTimeSpan();
            return this;
        }

        public Dsl WithPollInterval(TimeSpan timeSpan)
        {
            this.PollInterval = timeSpan;
            return this;
        }

        public Dsl IgnoreExceptions(bool ignoreExceptions = true)
        {
            this.ShouldIgnoreExceptions = ignoreExceptions;
            return this;
        }

        public Dsl FailSilently(bool failSilently = true)
        {
            this.ShouldFailSilently = failSilently;
            return this;
        }

        public Dsl WithPrereq(Action prereq)
        {
            if (prereq != null)
            {
                this.Prereqs = new List<Action>
                {
                    prereq,
                };
            }

            return this;
        }

        public Dsl WithPrereqs(IList<Action>? prereqs)
        {
            this.Prereqs = prereqs;
            return this;
        }

        public void Until(Func<bool> condition)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var outcome = false;
            while (outcome == false && stopwatch.Elapsed < this.Timeout)
            {
                this.InvokePrereqs();
                Thread.Sleep(this.PollDelay);

                try
                {
                    outcome = condition();
                }
                catch (Exception)
                {
                    if (!this.ShouldIgnoreExceptions)
                    {
                        throw;
                    }
                }

                Thread.Sleep(this.PollInterval);
            }

            if (outcome == false && stopwatch.Elapsed > this.Timeout)
            {
                if (!this.ShouldFailSilently)
                {
                    throw new TimeoutException($"Condition was not fulfilled within {this.Timeout}.");
                }
            }
        }

        private void InvokePrereqs()
        {
            if (this.Prereqs != null)
            {
                foreach (var prereq in this.Prereqs)
                {
                    try
                    {
                        prereq.Invoke();
                    }
                    catch (Exception)
                    {
                        if (!this.ShouldIgnoreExceptions)
                        {
                            throw;
                        }
                    }
                }
            }
        }
    }
}
