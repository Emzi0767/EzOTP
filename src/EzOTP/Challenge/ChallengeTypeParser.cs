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
using EzOTP.Internal;

namespace EzOTP
{
    /// <summary>
    /// Parses challenge type strings into enums.
    /// </summary>
    public static class ChallengeTypeParser
    {
        /// <summary>
        /// Parses a challenge type name.
        /// </summary>
        /// <param name="name">Type name.</param>
        /// <returns>Type enum.</returns>
        /// <exception cref="ArgumentException">Unrecognized value specified for <paramref name="name"/>.</exception>
        public static ChallengeType ParseName(string name)
        {
            if (!EnumNameConverter.Instance.TryConvert<ChallengeType>(name, out var ctype))
                throw new ArgumentException("Invalid type specified.", nameof(name));

            return ctype;
        }
    }
}
