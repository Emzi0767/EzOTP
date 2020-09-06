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
using System.Collections.Generic;
using System.Linq;
using Emzi0767.Utilities;

namespace EzOTP.Encoding
{
    internal sealed class Base32Encoding : IByteEncoding
    {
        // Do note, this implementation will fail for large strings, however we will not be handling those, so this 
        // implementation will work as if the inputs are always small.

        private const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";

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
            throw new NotImplementedException();
        }

        public bool TryDecode(ReadOnlySpan<char> input, Span<byte> output, out int bytesWritten)
        {
            bytesWritten = 0;
            if (output.Length < this.EstimateDecodedSize(input.Length))
                return false;

            byte buff = 0, t;
            var op = 0;
            var i = 0;
            var cycle = 0;
            for (; i < input.Length && input[i] != '='; i++)
            {
                if (!ValueMap.TryGetValue(input[i], out t))
                    return false;

                cycle = i % 8;
                switch (cycle)
                {
                    case 0:
                        buff = (byte)(t << 3);
                        break;

                    case 1:
                        PushCommit(ref buff, t, 2, 6, output, op++);
                        break;

                    case 2:
                        buff |= (byte)(t << 1);
                        break;

                    case 3:
                        PushCommit(ref buff, t, 4, 4, output, op++);
                        break;

                    case 4:
                        PushCommit(ref buff, t, 1, 7, output, op++);
                        break;

                    case 5:
                        buff |= (byte)(t << 2);
                        break;

                    case 6:
                        PushCommit(ref buff, t, 3, 5, output, op++);
                        break;

                    case 7:
                        buff |= t;
                        output[op++] = buff;
                        break;
                }
            }

            //if (cycle != 7)
            //    output[op] = buff;

            bytesWritten = op;
            return true;

            void PushCommit(ref byte acc, byte v, int top, int bottom, Span<byte> dest, int destIdx)
            {
                acc |= (byte)(v >> top);
                dest[destIdx] = acc;
                acc = (byte)(t << bottom);
            }
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
