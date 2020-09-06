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

using EzOTP.Internal;

namespace EzOTP
{
    /// <summary>
    /// Specifies byte encoding to use for OTP secret decoding.
    /// </summary>
    public enum ByteEncoding : int
    {
        /// <summary>
        /// Specifies unknown encoding type.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Specifies base16 (hexadecimal) encoding.
        /// </summary>
        [EnumName("base16", "b16", "base 16")]
        Base16 = 1,

        /// <summary>
        /// Specifies base32 encoding.
        /// </summary>
        [EnumName("base32", "b32", "base 32")]
        Base32 = 2,

        /// <summary>
        /// Specifies base64 encoding.
        /// </summary>
        [EnumName("base64", "b64", "base 64")]
        Base64 = 3
    }
}
