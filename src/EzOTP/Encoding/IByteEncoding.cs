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
    /// Performs byte encoding and decoding operations.
    /// </summary>
    public interface IByteEncoding
    {
        /// <summary>
        /// Gets whether this encoding provider can encode bytes into text.
        /// </summary>
        bool CanEncode { get; }

        /// <summary>
        /// Gets whether this encoding provider can decode text into bytes.
        /// </summary>
        bool CanDecode { get; }

        /// <summary>
        /// Estimates the number of characters required to store encoded form of a byte buffer of given size.
        /// </summary>
        /// <param name="inputSize">Number of input bytes to estimate the output size for.</param>
        /// <returns>Estimated number of characters required to encode a byte buffer of given size.</returns>
        int EstimateEncodedSize(int inputSize);

        /// <summary>
        /// Estimates the number of bytes required to store decoded form of an encoded byte buffer.
        /// </summary>
        /// <param name="inputSize">Number of input characters to estimate the output size for.</param>
        /// <returns>Estimated number of bytes required to decode a string of given length.</returns>
        int EstimateDecodedSize(int inputSize);

        /// <summary>
        /// Attempts to encode given byte buffer into a specified char buffer.
        /// </summary>
        /// <param name="input">Input bytes to encode.</param>
        /// <param name="output">Output buffer to place the encoded form of the input in. Generally, this should have size that is equal to or greather than the value returned by <see cref="EstimateEncodedSize(int)"/>.</param>
        /// <param name="charsWritten">Number of characters written to the buffer.</param>
        /// <returns>Whether the operation was successful.</returns>
        bool TryEncode(ReadOnlySpan<byte> input, Span<char> output, out int charsWritten);

        /// <summary>
        /// Attempts to decode given string into a specified byte buffer.
        /// </summary>
        /// <param name="input">Input string to decode.</param>
        /// <param name="output">Output buffer to place decoded form of the input in. Generally, this should have size that is equal to or greater than the value returned by <see cref="EstimateDecodedSize(int)"/>.</param>
        /// <param name="bytesWritten">Number of bytes written to the buffer.</param>
        /// <returns>Whether the operation was successful.</returns>
        bool TryDecode(ReadOnlySpan<char> input, Span<byte> output, out int bytesWritten);
    }
}
