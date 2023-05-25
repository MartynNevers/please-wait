// <copyright file="ReflectionExtensions.cs" company="Esdet">
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
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;

    public static class ReflectionExtensions
    {
        public static T GetFieldValue<T>(this object that, string name)
        {
            // Set the flags so that private and public fields from instances will be found
            var bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            var field = that.GetType().GetField(name, bindingFlags);
#pragma warning disable CS8603
            return (T)field?.GetValue(that);
#pragma warning restore CS8603
        }

        [ExcludeFromCodeCoverage]
        public static T GetPropertyValue<T>(this object that, string name)
        {
            // Set the flags so that private and public fields from instances will be found
            var bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            var property = that.GetType().GetProperty(name, bindingFlags);
#pragma warning disable CS8603
            return (T)property?.GetValue(that);
#pragma warning restore CS8603
        }
    }
}
