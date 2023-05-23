﻿// <copyright file="TimeConstraint.cs" company="Esdet">
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

    internal class TimeConstraint
    {
        public TimeConstraint(double value, TimeUnit timeUnit)
        {
            this.Value = value;
            this.TimeUnit = timeUnit;
        }

        private double Value
        {
            get;
            set;
        }

        private TimeUnit TimeUnit
        {
            get;
            set;
        }

        public TimeSpan GetTimeSpan()
        {
            return this.TimeUnit switch
            {
                TimeUnit.MILLIS => TimeSpan.FromMilliseconds(this.Value),
                TimeUnit.SECONDS => TimeSpan.FromSeconds(this.Value),
                TimeUnit.MINUTES => TimeSpan.FromMinutes(this.Value),
                TimeUnit.HOURS => TimeSpan.FromHours(this.Value),
                TimeUnit.DAYS => TimeSpan.FromDays(this.Value),
                _ => throw new NotImplementedException(),
            };
        }
    }
}