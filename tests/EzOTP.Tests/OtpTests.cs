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
    public sealed class OtpTests
    {
        // https://rootprojects.org/authenticator/

        private const string Otp1Uri = "otpauth://hotp/ACME%20Co:john@example.com?secret=HXDMVJECJJWSRB3HWIZR4IFUGFTMXBOZ&issuer=ACME%20Co&algorithm=SHA1&digits=6&counter=53317182";
        private const int Otp1Ref1 = 586785;
        private const int Otp1Ref2 = 221484;
        private const int Otp1Offset = 2;

        private const string Otp2Uri = "otpauth://hotp/ACME%20Co:john@example.com?secret=DGW24UIKQZBELXEMY64PICAL5IGYMJM6&issuer=ACME%20Co&algorithm=SHA1&digits=6&counter=53317347";
        private const int Otp2Ref1 = 958891;
        private const int Otp2Ref2 = 799448;
        private const int Otp2Offset = 4;

        [DataTestMethod]
        [DataRow(Otp1Uri, Otp1Ref1, Otp1Ref2, Otp1Offset)]
        [DataRow(Otp2Uri, Otp2Ref1, Otp2Ref2, Otp2Offset)]
        public void TestOtp(string uri, int ref1, int ref2, int offset)
        {
            var otpuri = new Uri(uri);
            var otp = OtpGenerator.ParseUri(otpuri);

            Assert.AreEqual(ref1, otp.GenerateRaw());
            Assert.AreEqual(ref2, otp.GenerateRaw(offset - 1));
        }
    }
}
