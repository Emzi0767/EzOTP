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
using EzOTP.Codes;
using EzOTP.Internal;

namespace EzOTP
{
    /// <summary>
    /// Provides facilities for selecting code transformer implementation based on its id or name.
    /// </summary>
    public static class CodeTransformerSelector
    {
        /// <summary>
        /// Creates a code transformer instance based on its Id.
        /// </summary>
        /// <param name="algo">Transformer Id.</param>
        /// <returns>Created code transformer instance.</returns>
        /// <exception cref="ArgumentException">Unrecognized value specified for <paramref name="algo"/>.</exception>
        public static ICodeTransformer FromId(CodeTransformer algo)
            => algo switch
            {
                CodeTransformer.Rfc4226 => Rfc4226CodeTransformer.Instance,
                _                       => throw new ArgumentException("Invalid transformer id specified.", nameof(algo))
            };

        /// <summary>
        /// Creates a code transformer instance based on its name.
        /// </summary>
        /// <param name="name">Transformer name.</param>
        /// <returns>Created code transformer instance.</returns>
        /// <exception cref="ArgumentException">Unrecognized value specified for <paramref name="name"/>.</exception>
        public static ICodeTransformer FromName(string name)
        {
            if (!EnumNameConverter.Instance.TryConvert<CodeTransformer>(name, out var algo))
                throw new ArgumentException("Invalid transformer id specified.", nameof(name));

            return FromId(algo);
        }
    }
}
