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
using System.Runtime.CompilerServices;

namespace EzOTP
{
    /// <summary>
    /// Converts numeric codes into string representations.
    /// </summary>
    public static class StringCodeFormatter
    {
        private static readonly int[] PowersOf10 = new[]
        {
         /* * */
            1,
            10,
            100,
            1000,
            10000,
            100000,
            1000000,
            10000000,
            100000000,
            1000000000
            //

            // Get it? It's a Christmas tree!
        };

        /// <summary>
        /// Formats numeric code value to a character buffer.
        /// </summary>
        /// <param name="number">Numeric code to format.</param>
        /// <param name="digitCount">Number of digits in the resulting code.</param>
        /// <param name="output">Character buffer to place the result in.</param>
        /// <returns>Whether the operation was successful.</returns>
        public static bool TryFormat(int number, int digitCount, Span<char> output)
        {
            if (digitCount < 2 || digitCount > 9)
                return false;

            if (output.Length < digitCount)
                return false;

            Span<char> format = stackalloc char[digitCount];
            format.Fill('0');
            return (number % PowersOf10[digitCount]).TryFormat(output, out var written, format, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Formats numeric code value to a string.
        /// </summary>
        /// <param name="number">Numeric code to format.</param>
        /// <param name="digitCount">Number of digits in the resulting code.</param>
        /// <returns>Formatted code.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Number of digits specified was less than 2 or more than 9.</exception>
        /// <exception cref="Exception">Formatting fails for unknown reason.</exception>
        public static string Format(int number, int digitCount)
        {
            if (digitCount < 2 || digitCount > 9)
                throw new ArgumentOutOfRangeException(nameof(digitCount));

            Span<char> buff = stackalloc char[digitCount];
            if (!TryFormat(number, digitCount, buff))
                throw new Exception("Could not format the code.");

            return new string(buff);
        }

        /// <summary>
        /// Formats numeric code value to a character buffer, groupping the characters.
        /// </summary>
        /// <param name="number">Numeric code to format.</param>
        /// <param name="digitCount">Number of digits in the resulting code.</param>
        /// <param name="output">Character buffer to place the result in.</param>
        /// <param name="groupSize">Maximum number of characters per group.</param>
        /// <param name="separator">Separator character for the groups.</param>
        /// <returns>Whether the operation was successful.</returns>
        public static bool TryFormatGroupped(int number, int digitCount, Span<char> output, int groupSize, char separator = ' ')
        {
            if (digitCount < 2 || digitCount > 9)
                return false;

            var (n, m) = (digitCount / groupSize, digitCount % groupSize);
            if (n != 0 && m == 0)
                n--;

            if (n == 0)
                return TryFormat(number, digitCount, output);

            var olen = digitCount + n;
            if (output.Length < olen)
                return false;

            Span<char> format = stackalloc char[digitCount];
            format.Fill('0');
            Span<char> tmp = stackalloc char[digitCount];
            if (!(number % PowersOf10[digitCount]).TryFormat(tmp, out _, format, CultureInfo.InvariantCulture))
                return false;

            output.Fill(separator);
            while (olen > 0)
            {
                olen -= groupSize;
                digitCount -= groupSize;

                var si = Math.Max(digitCount, 0);
                tmp.Slice(si, groupSize + Math.Min(digitCount, 0)).CopyTo(output.Slice(Math.Max(olen--, 0)));
            }

            return true;
        }

        /// <summary>
        /// Formats numeric code value to a string, groupping digits.
        /// </summary>
        /// <param name="number">Numeric code to format.</param>
        /// <param name="digitCount">Number of digits in the resulting code.</param>
        /// <param name="groupSize">Maximum number of characters per group.</param>
        /// <param name="separator">Separator character for the groups.</param>
        /// <returns>Formatted code.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Number of digits specified was less than 2 or more than 9.</exception>
        /// <exception cref="Exception">Formatting fails for unknown reason.</exception>
        public static string FormatGroupped(int number, int digitCount, int groupSize, char separator = ' ')
        {
            if (digitCount < 2 || digitCount > 9)
                throw new ArgumentOutOfRangeException(nameof(digitCount));

            var (n, m) = (digitCount / groupSize, digitCount % groupSize);
            if (n != 0 && m == 0)
                n--;

            if (n == 0)
                return Format(number, digitCount);

            var olen = digitCount + n;
            Span<char> buff = stackalloc char[olen];
            if (!TryFormatGroupped(number, digitCount, buff, groupSize, separator))
                throw new Exception("Could not format the code.");

            return new string(buff);
        }

        /// <summary>
        /// Gets the raw code for comparison purposes.
        /// </summary>
        /// <param name="number">Numeric code to format.</param>
        /// <param name="digitCount">Number of digits in the resulting code.</param>
        /// <returns>Raw code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetRawCode(int number, int digitCount)
            => number % PowersOf10[digitCount];
    }
}
