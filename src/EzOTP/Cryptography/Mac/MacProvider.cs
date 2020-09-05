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
using System.Security.Cryptography;

namespace EzOTP.Cryptography.Mac
{
    internal abstract class MacProvider : IMacProvider
    {
        public abstract int OutputSize { get; }

        protected HMAC Hmac { get; }

        protected MacProvider(HMAC hmac)
        {
            this.Hmac = hmac;
        }

        bool IMacProvider.Compute(ReadOnlySpan<byte> secret, ReadOnlySpan<byte> message, Span<byte> result, out int bytesWritten)
        {
            lock (this.Hmac)
            {
                this.Hmac.Key = secret.ToArray();
                return this.Hmac.TryComputeHash(message, result, out bytesWritten);
            }
        }

        void IDisposable.Dispose()
            => this.Hmac.Dispose();
    }
}
