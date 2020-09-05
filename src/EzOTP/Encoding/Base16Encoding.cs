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
using System.Globalization;

namespace EzOTP.Encoding
{
    internal sealed class Base16Encoding : IByteEncoding
    {
        public bool CanEncode => true;

        public bool CanDecode => true;

        public int EstimateEncodedSize(int inputSize)
            => inputSize * 2;

        public int EstimateDecodedSize(int inputSize)
            => inputSize / 2;

        public bool TryEncode(ReadOnlySpan<byte> input, Span<char> output, out int charsWritten)
        {
            charsWritten = 0;
            for (var i = 0; i < input.Length; i++)
            {
                if (!input[i].TryFormat(output.Slice(charsWritten), out var written, "X2", CultureInfo.InvariantCulture))
                    return false;
                charsWritten += written;
            }

            return true;
        }

        public bool TryDecode(ReadOnlySpan<char> input, Span<byte> output, out int bytesWritten)
        {
            bytesWritten = 0;
            for (var i = 0; i < input.Length; i += 2)
            {
                if (!byte.TryParse(input.Slice(i, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var @byte))
                    return false;

                output[bytesWritten++] = @byte;
            }

            return true;
        }
    }
}
