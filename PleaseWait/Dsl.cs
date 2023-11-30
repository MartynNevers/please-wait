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

        /// <summary>
        /// Used to start defining a new waiting condition.
        /// </summary>
        /// <returns>The current <see cref="Dsl"/>.</returns>
        public static Dsl Wait()
        {
            return new Dsl();
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
        public Dsl Timeout(double value, TimeUnit timeUnit)
        {
            this.timeout = new TimeConstraint(value, timeUnit).GetTimeSpan();
            return this;
        }

        /// <summary>
        /// Defines the timeout for the waiting condition.
        /// </summary>
        /// <param name="timeSpan">The timeout value.</param>
        /// <returns>The current <see cref="Dsl"/>.</returns>
        public Dsl Timeout(TimeSpan timeSpan)
        {
            this.timeout = timeSpan;
            return this;
        }

        /// <summary>
        /// Defines the timeout for the waiting condition.
        /// </summary>
        /// <param name="value">The timeout value.</param>
        /// <param name="timeUnit">The corresponding <see cref="TimeUnit"/> for the timeout value.</param>
        /// <returns>The current <see cref="Dsl"/>.</returns>
        public Dsl AtMost(double value, TimeUnit timeUnit)
        {
            return this.Timeout(value, timeUnit);
        }

        /// <summary>
        /// Defines the timeout for the waiting condition.
        /// </summary>
        /// <param name="timeSpan">The timeout value.</param>
        /// <returns>The current <see cref="Dsl"/>.</returns>
        public Dsl AtMost(TimeSpan timeSpan)
        {
            return this.Timeout(timeSpan);
        }

        /// <summary>
        /// Defines the polling delay to be used in the waiting condition. Applied before the check on a condition.
        /// </summary>
        /// <param name="value">The polling delay value.</param>
        /// <param name="timeUnit">The corresponding <see cref="TimeUnit"/> for the polling delay.</param>
        /// <returns>The current <see cref="Dsl"/>.</returns>
        public Dsl PollDelay(double value, TimeUnit timeUnit)
        {
            this.pollDelay = new TimeConstraint(value, timeUnit).GetTimeSpan();
            return this;
        }

        /// <summary>
        /// Defines the polling delay to be used in the waiting condition. Applied before the check on a condition.
        /// </summary>
        /// <param name="timeSpan">The timeout value.</param>
        /// <returns>The current <see cref="Dsl"/>.</returns>
        public Dsl PollDelay(TimeSpan timeSpan)
        {
            this.pollDelay = timeSpan;
            return this;
        }

        /// <summary>
        /// Defines the polling interval to be used in the waiting condition. Applied after the check on a condition.
        /// </summary>
        /// <param name="value">The polling interval value.</param>
        /// <param name="timeUnit">The corresponding <see cref="TimeUnit"/> for the polling interval.</param>
        /// <returns>The current <see cref="Dsl"/>.</returns>
        public Dsl PollInterval(double value, TimeUnit timeUnit)
        {
            this.pollInterval = new TimeConstraint(value, timeUnit).GetTimeSpan();
            return this;
        }

        /// <summary>
        /// Defines the polling interval to be used in the waiting condition. Applied after the check on a condition.
        /// </summary>
        /// <param name="timeSpan">The polling interval value.</param>
        /// <returns>The current <see cref="Dsl"/>.</returns>
        public Dsl PollInterval(TimeSpan timeSpan)
        {
            this.pollInterval = timeSpan;
            return this;
        }

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
            return this;
        }

        /// <summary>
        /// Pause execution of the waiting condition for the duration of the timeout defined earlier.
        /// </summary>
        public void Sleep()
        {
            Thread.Sleep(this.timeout);
        }

        /// <summary>
        /// Defines the waiting condition. PleaseWait will execute this code until it returns a boolean true.
        /// </summary>
        /// <param name="condition">The condition to wait for. Should return a boolean.</param>
        /// <exception cref="TimeoutException">Thrown when the timeout is exceeded without the condition returning a boolean true.</exception>
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
    }
}
