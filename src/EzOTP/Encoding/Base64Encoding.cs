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

namespace EzOTP.Encoding
{
    internal sealed class Base64Encoding : IByteEncoding
    {
        public bool CanEncode => true;

        public bool CanDecode => true;

        public int EstimateEncodedSize(int inputSize)
            => ((4 * inputSize / 3) + 3) & ~3;

        public int EstimateDecodedSize(int inputSize)
            => inputSize / 4 * 3;

        public bool TryEncode(ReadOnlySpan<byte> input, Span<char> output, out int charsWritten)
            => Convert.TryToBase64Chars(input, output, out charsWritten, Base64FormattingOptions.None);

        public bool TryDecode(ReadOnlySpan<char> input, Span<byte> output, out int bytesWritten)
            => Convert.TryFromBase64Chars(input, output, out bytesWritten);
    }
}
