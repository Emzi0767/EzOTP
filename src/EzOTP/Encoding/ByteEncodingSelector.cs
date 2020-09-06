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
using EzOTP.Encoding;
using EzOTP.Internal;

namespace EzOTP
{
    /// <summary>
    /// Provides facilities for selecting a byte encoding based on its name or identifier.
    /// </summary>
    public static class ByteEncodingSelector
    {
        /// <summary>
        /// Creates a byte encoder instance based on its Id.
        /// </summary>
        /// <param name="algo">Algorithm Id.</param>
        /// <returns>Created encoder instance.</returns>
        /// <exception cref="ArgumentException">Unrecognized value specified for <paramref name="algo"/>.</exception>
        public static IByteEncoding FromId(ByteEncoding algo)
            => algo switch
            {
                ByteEncoding.Base16 => new Base16Encoding(),
                ByteEncoding.Base32 => new Base32Encoding(),
                ByteEncoding.Base64 => new Base64Encoding(),
                _                   => throw new ArgumentException("Invalid encoding specified.", nameof(algo))
            };

        /// <summary>
        /// Creates a byte encoder instance based on its name.
        /// </summary>
        /// <param name="name">Algorithm name.</param>
        /// <returns>Created encoder instance.</returns>
        /// <exception cref="ArgumentException">Unrecognized value specified for <paramref name="name"/>.</exception>
        public static IByteEncoding FromName(string name)
        {
            if (!EnumNameConverter.Instance.TryConvert<ByteEncoding>(name, out var algo))
                throw new ArgumentException("Invalid encoding specified.", nameof(name));

            return FromId(algo);
        }
    }
}
