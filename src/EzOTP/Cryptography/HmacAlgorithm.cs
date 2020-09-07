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
    /// Specifies a MAC algorithm to use for OTP generation.
    /// </summary>
    public enum HmacAlgorithm : int
    {
        /// <summary>
        /// Specifies unknown algorithm type.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Specifies MD5-based HMAC algorithm.
        /// </summary>
        [EnumName("md5", "md 5", "md-5")]
        Md5 = 1,

        /// <summary>
        /// Specifies SHA1-based HMAC algorithm.
        /// </summary>
        [EnumName("sha1", "sha 1", "sha-1")]
        Sha1 = 2,

        /// <summary>
        /// Specifies SHA256-based HMAC algorithm.
        /// </summary>
        [EnumName("sha256", "sha 256", "sha-256")]
        Sha256 = 3,

        /// <summary>
        /// Specifies SHA384-based HMAC algorithm.
        /// </summary>
        [EnumName("sha384", "sha 384", "sha-384")]
        Sha384 = 4,

        /// <summary>
        /// Specifies SHA512-based HMAC algorithm.
        /// </summary>
        [EnumName("sha512", "sha 512", "sha-512")]
        Sha512 = 5
    }
}
