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
    /// Performs hashing operations using HMAC-family algorithms.
    /// </summary>
    public interface IHmacProvider : IDisposable
    {
        /// <summary>
        /// Gets the output size of this algorithm, in bytes.
        /// </summary>
        int OutputSize { get; }

        /// <summary>
        /// Computes a HMAC for for given input.
        /// </summary>
        /// <param name="secret">The secret to use when computing the HMAC.</param>
        /// <param name="message">Message to compute a HMAC for.</param>
        /// <param name="result">Buffer to place the resulting HMAC in. Its size should be at least <see cref="OutputSize"/>-long.</param>
        /// <param name="bytesWritten">Number of bytes written to the output. This value should typically be negative if the computation fails, or <see cref="OutputSize"/> if the computation succeeds.</param>
        /// <returns>Whether the operation was successful.</returns>
        bool Compute(ReadOnlySpan<byte> secret, ReadOnlySpan<byte> message, Span<byte> result, out int bytesWritten);
    }
}
