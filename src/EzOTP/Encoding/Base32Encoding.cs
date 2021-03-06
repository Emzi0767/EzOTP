﻿// This file is part of RosettaCTF project.
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
using System.Collections.Generic;
using System.Linq;
using Emzi0767.Utilities;

namespace EzOTP.Encoding
{
    internal sealed class Base32Encoding : IByteEncoding
    {
        internal static IByteEncoding Instance { get; } = new Base32Encoding();

        // Do note, this implementation will fail for large strings, however we will not be handling those, so this 
        // implementation will work as if the inputs are always small.

        private const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
        private const int PushSize = 5; // base32 is a 5-bit encoding

        private static IReadOnlyDictionary<char, byte> ValueMap { get; }

        static Base32Encoding()
        {
            ValueMap = Alphabet
                .Select((x, i) => (x, i))
                .ToDictionary(x => x.x, x => (byte)x.i, CaseInsensitiveCharComparer.Instance);
        }

        public bool CanEncode => true;

        public bool CanDecode => true;

        public int EstimateEncodedSize(int inputSize)
            => (inputSize * 8 + 4) / 5;

        public int EstimateDecodedSize(int inputSize)
            => inputSize * 5 / 8;

        public bool TryEncode(ReadOnlySpan<byte> input, Span<char> output, out int charsWritten)
        {
            charsWritten = 0;
            if (output.Length < this.EstimateEncodedSize(input.Length))
                return false;

            var pushed = 0;
            var buff = 0;
            var op = 0;
            var i = 0;
            for (; i < input.Length; i++)
            {
                var @in = input[i];
                buff = (buff << 8) | @in;
                pushed += 8;
                
                while (pushed >= PushSize)
                {
                    var t = (buff >> (pushed -= PushSize)) & 0x1F;
                    var c = Alphabet[t];
                    output[op++] = c;
                }
            }

            if (pushed > 0)
            {
                var mask = (1 << pushed) - 1;
                var t = buff & mask;
                output[op++] = Alphabet[t];
            }

            charsWritten = op;
            return true;
        }

        public bool TryDecode(ReadOnlySpan<char> input, Span<byte> output, out int bytesWritten)
        {
            bytesWritten = 0;
            if (output.Length < this.EstimateDecodedSize(input.Length))
                return false;

            var pushed = 0;
            var buff = 0;
            var op = 0;
            var i = 0;
            for (; i < input.Length && input[i] != '='; i++)
            {
                if (!ValueMap.TryGetValue(input[i], out var t))
                    return false;

                buff = (buff << PushSize) | t;
                pushed += PushSize;

                if (pushed >= 8) // crossed a byte boundary, commit
                {
                    output[op++] = (byte)(buff >> (pushed - 8));
                    pushed -= 8;
                }
            }

            bytesWritten = op;
            return true;
        }

        private sealed class CaseInsensitiveCharComparer : IEqualityComparer<char>
        {
            public static CaseInsensitiveCharComparer Instance { get; } = new CaseInsensitiveCharComparer();

            public bool Equals(char x, char y)
                => char.ToLowerInvariant(x) == char.ToLowerInvariant(y);

            public int GetHashCode(char obj)
                => char.ToLowerInvariant(obj).GetHashCode();
        }
    }
}
