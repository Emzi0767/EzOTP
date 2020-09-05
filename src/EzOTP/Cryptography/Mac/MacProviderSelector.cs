// This file is part of RosettaCTF project.
//
// Copyright 2020 Emzi0767
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using EzOTP.Cryptography.Mac;
using EzOTP.Internal;

namespace EzOTP
{
    /// <summary>
    /// Provides facilities for selecting a MAC algorithm based on its name or identifier.
    /// </summary>
    public static class MacProviderSelector
    {
        /// <summary>
        /// Creates a MAC provider instance based on its Id.
        /// </summary>
        /// <param name="algo">Algorithm Id.</param>
        /// <returns>Created MAC provider instance.</returns>
        /// <exception cref="ArgumentException">Unrecognized value specified for <paramref name="algo"/>.</exception>
        public static IMacProvider FromId(MacAlgorithm algo)
            => algo switch
            {
                MacAlgorithm.Md5    => new MacMd5Provider(),
                MacAlgorithm.Sha1   => new MacSha1Provider(),
                MacAlgorithm.Sha256 => new MacSha256Provider(),
                MacAlgorithm.Sha384 => new MacSha384Provider(),
                MacAlgorithm.Sha512 => new MacSha512Provider(),
                _                   => throw new ArgumentException("Invalid algorithm specified.", nameof(algo))
            };

        /// <summary>
        /// Creates a MAC provider instance based on its name.
        /// </summary>
        /// <param name="name">Algorithm name.</param>
        /// <returns>Created MAC provider instance.</returns>
        /// <exception cref="ArgumentException">Unrecognized value specified for <paramref name="name"/>.</exception>
        public static IMacProvider FromName(string name)
        {
            if (!EnumNameConverter.Instance.TryConvert<MacAlgorithm>(name, out var algo))
                throw new ArgumentException("Invalid algorithm specified.", nameof(name));

            return FromId(algo);
        }
    }
}
