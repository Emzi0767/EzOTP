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

namespace EzOTP
{
    /// <summary>
    /// Provides methods for transforming HMAC output into numeric codes.
    /// </summary>
    public interface ICodeTransformer
    {
        /// <summary>
        /// Generates a numeric value for given generated OTP input.
        /// </summary>
        /// <param name="input">Output of the OTP HMAC generator.</param>
        /// <param name="output">Generated digits.</param>
        /// <returns>Whether the operation was successful.</returns>
        bool TryTransform(ReadOnlySpan<byte> input, out int output);
    }
}
