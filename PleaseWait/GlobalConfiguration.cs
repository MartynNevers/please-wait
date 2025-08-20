// <copyright file="GlobalConfiguration.cs" company="Esdet">
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

namespace PleaseWait
{
    using PleaseWait.Core;

    /// <summary>
    /// Provides access to global configuration operations.
    /// </summary>
    public class GlobalConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GlobalConfiguration"/> class.
        /// </summary>
        internal GlobalConfiguration()
        {
        }

        /// <summary>
        /// Configures global defaults for PleaseWait.
        /// </summary>
        /// <returns>A configuration builder for setting global defaults.</returns>
        public GlobalConfigurationBuilder Configure()
        {
            return new GlobalConfigurationBuilder();
        }

        /// <summary>
        /// Resets all global configuration to default values.
        /// </summary>
        public void ResetToDefaults()
        {
            GlobalDefaults.ResetToDefaults();
        }
    }
}
