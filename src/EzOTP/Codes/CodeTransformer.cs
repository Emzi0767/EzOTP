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
    /// Specifies code transformer type to use for generating numeric code.s
    /// </summary>
    public enum CodeTransformer : int
    {
        /// <summary>
        /// Specifies unknown code transformer implementation.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Specifies RFC 4226-compliant method of generating numeric values. This implementation is also used by 
        /// Google Authenticator.
        /// </summary>
        [EnumName("rfc", "rfc4226", "rfc 4226", "google", "google authenticator")]
        Rfc4226 = 1
    }
}
