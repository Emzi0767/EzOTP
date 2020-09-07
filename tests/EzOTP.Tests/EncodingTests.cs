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

using E = System.Text.Encoding;

namespace EzOTP.Tests
{
    [TestClass]
    public sealed class EncodingTests
    {
        private const string DBase16E1 = "6E6F7065";
        private const string DBase16D1 = "nope";

        private const string DBase16E2 = "6e6f7065";
        private const string DBase16D2 = "nope";

        private const string DBase32E1 = "ORSXG5A";
        private const string DBase32D1 = "test";

        private const string DBase32E2 = "ORSXG5A=";
        private const string DBase32D2 = "test";

        private const string DBase32E3 = "orSXG5A";
        private const string DBase32D3 = "test";

        private const string DBase32E4 = "MFZWIZTH";
        private const string DBase32D4 = "asdfg";

        private const string DBase64E1 = "dGVzdA";
        private const string DBase64D1 = "test";

        private const string DBase64E2 = "dGVzdA==";
        private const string DBase64D2 = "test";

        private const string DBase64E3 = "YXNk";
        private const string DBase64D3 = "asd";

        [DataTestMethod]
        [DataRow(ByteEncoding.Base16, DBase16E1, DBase16D1)]
        [DataRow(ByteEncoding.Base16, DBase16E2, DBase16D2)]
        [DataRow(ByteEncoding.Base32, DBase32E1, DBase32D1)]
        [DataRow(ByteEncoding.Base32, DBase32E2, DBase32D2)]
        [DataRow(ByteEncoding.Base32, DBase32E3, DBase32D3)]
        [DataRow(ByteEncoding.Base32, DBase32E4, DBase32D4)]
        [DataRow(ByteEncoding.Base64, DBase64E1, DBase64D1)]
        [DataRow(ByteEncoding.Base64, DBase64E2, DBase64D2)]
        [DataRow(ByteEncoding.Base64, DBase64E3, DBase64D3)]
        public void TestDecoding(ByteEncoding encid, string input, string result)
        {
            var enc = ByteEncodingSelector.FromId(encid);

            var tlen = enc.EstimateDecodedSize(input.Length);
            Span<byte> target = stackalloc byte[tlen];
            if (!enc.TryDecode(input, target, out var written))
                Assert.Fail("Failed to decode input '{0}' for encoding '{1}'", input, encid);

            Assert.AreEqual(result.Length, written);

            var aresult = E.ASCII.GetString(target.Slice(0, written));
            Assert.AreEqual(result, aresult);
        }

        [DataTestMethod]
        [DataRow(ByteEncoding.Base16, DBase16D1, DBase16E1)]
        [DataRow(ByteEncoding.Base32, DBase32D1, DBase32E1)]
        [DataRow(ByteEncoding.Base32, DBase32D4, DBase32E4)]
        [DataRow(ByteEncoding.Base64, DBase64D1, DBase64E1)]
        [DataRow(ByteEncoding.Base64, DBase64D3, DBase64E3)]
        public void TestEncoding(ByteEncoding encid, string input, string result)
        {
            var enc = ByteEncodingSelector.FromId(encid);

            Span<byte> ainput = stackalloc byte[input.Length];
            E.ASCII.GetBytes(input.AsSpan(), ainput);

            var tlen = enc.EstimateEncodedSize(ainput.Length);
            Span<char> target = stackalloc char[tlen];
            if (!enc.TryEncode(ainput, target, out var written))
                Assert.Fail("Failed to encode input '{0}' for encoding '{1}'", input, encid);

            Assert.AreEqual(result.Length, written);

            var aresult = new string(target.Slice(0, written));
            Assert.AreEqual(result, aresult);
        }
    }
}
