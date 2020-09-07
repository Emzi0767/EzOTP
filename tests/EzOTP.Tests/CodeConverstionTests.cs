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
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EzOTP.Tests
{
    [TestClass]
    public sealed class CodeConverstionTests
    {
        public const int BaseNumber = 1234567890;

        [DataTestMethod]
        [DataRow(BaseNumber, 2, "90")]
        [DataRow(BaseNumber, 3, "890")]
        [DataRow(BaseNumber, 4, "7890")]
        [DataRow(BaseNumber, 5, "67890")]
        [DataRow(BaseNumber, 6, "567890")]
        [DataRow(BaseNumber, 7, "4567890")]
        [DataRow(BaseNumber, 8, "34567890")]
        [DataRow(BaseNumber, 9, "234567890")]
        public void TestFormatting(int number, int digitCount, string expected)
        {
            Span<char> buff = stackalloc char[digitCount];
            if (!StringCodeFormatter.TryFormat(number, digitCount, buff))
                Assert.Fail("Failed to format {0}:{1}", number, digitCount);

            var result = new string(buff);
            Assert.AreEqual(expected, result);
        }

        [DataTestMethod]
        [DataRow(BaseNumber, 2, "90")]
        [DataRow(BaseNumber, 3, "890")]
        [DataRow(BaseNumber, 4, "7890")]
        [DataRow(BaseNumber, 5, "67890")]
        [DataRow(BaseNumber, 6, "567890")]
        [DataRow(BaseNumber, 7, "4567890")]
        [DataRow(BaseNumber, 8, "34567890")]
        [DataRow(BaseNumber, 9, "234567890")]
        public void TestConversion(int number, int digitCount, string expected)
        {
            var result = StringCodeFormatter.Format(number, digitCount);
            Assert.AreEqual(expected, result);
        }

        [DataTestMethod]
        [DataRow(BaseNumber, 2, 1, "9 0", ' ')]
        [DataRow(BaseNumber, 2, 2, "90", ' ')]
        [DataRow(BaseNumber, 2, 3, "90", ' ')]
        [DataRow(BaseNumber, 3, 1, "8 9 0", ' ')]
        [DataRow(BaseNumber, 3, 2, "8 90", ' ')]
        [DataRow(BaseNumber, 3, 3, "890", ' ')]
        [DataRow(BaseNumber, 4, 1, "7 8 9 0", ' ')]
        [DataRow(BaseNumber, 4, 2, "78 90", ' ')]
        [DataRow(BaseNumber, 5, 2, "6 78 90", ' ')]
        [DataRow(BaseNumber, 5, 3, "67 890", ' ')]
        [DataRow(BaseNumber, 6, 3, "567 890", ' ')]
        [DataRow(BaseNumber, 6, 4, "56 7890", ' ')]
        [DataRow(BaseNumber, 7, 2, "4 56 78 90", ' ')]
        [DataRow(BaseNumber, 7, 3, "4 567 890", ' ')]
        [DataRow(BaseNumber, 7, 4, "456 7890", ' ')]
        [DataRow(BaseNumber, 8, 1, "3 4 5 6 7 8 9 0", ' ')]
        [DataRow(BaseNumber, 8, 2, "34 56 78 90", ' ')]
        [DataRow(BaseNumber, 8, 3, "34 567 890", ' ')]
        [DataRow(BaseNumber, 8, 4, "3456 7890", ' ')]
        [DataRow(BaseNumber, 9, 3, "234 567 890", ' ')]
        [DataRow(BaseNumber, 6, 3, "567-890", '-')]
        [DataRow(BaseNumber, 8, 2, "34-56-78-90", '-')]
        [DataRow(BaseNumber, 9, 3, "234-567-890", '-')]
        public void TestGrouppedFormatting(int number, int digitCount, int groupSize, string expected, char separator)
        {
            Span<char> buff = stackalloc char[expected.Length];
            if (!StringCodeFormatter.TryFormatGroupped(number, digitCount, buff, groupSize, separator))
                Assert.Fail("Failed to format groupped {0}:{1}", number, digitCount);

            var result = new string(buff);
            Assert.AreEqual(expected, result);
        }

        [DataTestMethod]
        [DataRow(BaseNumber, 2, 1, "9 0", ' ')]
        [DataRow(BaseNumber, 2, 2, "90", ' ')]
        [DataRow(BaseNumber, 2, 3, "90", ' ')]
        [DataRow(BaseNumber, 3, 1, "8 9 0", ' ')]
        [DataRow(BaseNumber, 3, 2, "8 90", ' ')]
        [DataRow(BaseNumber, 3, 3, "890", ' ')]
        [DataRow(BaseNumber, 4, 1, "7 8 9 0", ' ')]
        [DataRow(BaseNumber, 4, 2, "78 90", ' ')]
        [DataRow(BaseNumber, 5, 2, "6 78 90", ' ')]
        [DataRow(BaseNumber, 5, 3, "67 890", ' ')]
        [DataRow(BaseNumber, 6, 3, "567 890", ' ')]
        [DataRow(BaseNumber, 6, 4, "56 7890", ' ')]
        [DataRow(BaseNumber, 7, 2, "4 56 78 90", ' ')]
        [DataRow(BaseNumber, 7, 3, "4 567 890", ' ')]
        [DataRow(BaseNumber, 7, 4, "456 7890", ' ')]
        [DataRow(BaseNumber, 8, 1, "3 4 5 6 7 8 9 0", ' ')]
        [DataRow(BaseNumber, 8, 2, "34 56 78 90", ' ')]
        [DataRow(BaseNumber, 8, 3, "34 567 890", ' ')]
        [DataRow(BaseNumber, 8, 4, "3456 7890", ' ')]
        [DataRow(BaseNumber, 9, 3, "234 567 890", ' ')]
        [DataRow(BaseNumber, 6, 3, "567-890", '-')]
        [DataRow(BaseNumber, 8, 2, "34-56-78-90", '-')]
        [DataRow(BaseNumber, 9, 3, "234-567-890", '-')]
        public void TestGrouppedConversion(int number, int digitCount, int groupSize, string expected, char separator)
        {
            var result = StringCodeFormatter.FormatGroupped(number, digitCount, groupSize, separator);
            Assert.AreEqual(expected, result);
        }
    }
}
