﻿// <copyright file="Orange.cs" company="Esdet">
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

namespace PleaseWait.Tests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;

    public class Orange
    {
        public Orange()
        {
        }

        public bool IsPeeled
        {
            get;
            set;
        }

        public bool IsFresh
        {
            get;
            set;
        }

        public async Task PeelAsync(double seconds)
        {
            await Task.Run(() =>
            {
                Task.Delay(TimeSpan.FromSeconds(seconds)).Wait();
                this.IsPeeled = true;
            });
        }

        public async Task SpoilAsync(double seconds)
        {
            await Task.Run(() =>
            {
                Task.Delay(TimeSpan.FromSeconds(seconds)).Wait();
                this.IsFresh = false;
            });
        }

        [ExcludeFromCodeCoverage]
        public async Task PeelFastAsync()
        {
            await this.PeelAsync(5);
        }

        public int CountSegments()
        {
            if (!this.IsPeeled)
            {
                _ = this.PeelAsync(0);
                throw new InvalidOperationException("Try again");
            }

            return new Random().Next(9, 12);
        }
    }
}
