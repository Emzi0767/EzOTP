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

#pragma warning disable CS0420 // A reference to a volatile field will not be treated as volatile

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using Emzi0767;
using Microsoft.Extensions.Primitives;

namespace EzOTP
{
    /// <summary>
    /// Represents settings for a counter-based code generator.
    /// </summary>
    public sealed class HotpGeneratorSettings : OtpGeneratorSettings
    {
        private const string ParamCounter = "counter";

        /// <summary>
        /// Gets the current counter value.
        /// </summary>
        public long Counter => Volatile.Read(ref this._counter);

        private long _counter;

        /// <summary>
        /// Creates new time-based generator settings.
        /// </summary>
        /// <param name="label">Label for this generator.</param>
        /// <param name="issuer">Issuer for this generator.</param>
        /// <param name="secret">Secret for this generator.</param>
        /// <param name="encoding">Encoding of the secret.</param>
        /// <param name="algo">HMAC algorithm of the generator.</param>
        /// <param name="digits">Number of digits in generated codes.</param>
        /// <param name="additional">Additional data to feed to challenge generator.</param>
        /// <param name="counter">Current value of the counter.</param>
        public HotpGeneratorSettings(
            string label,
            string issuer,
            byte[] secret,
            ByteEncoding encoding,
            HmacAlgorithm algo,
            int digits,
            byte[] additional,
            long counter)
            : base(ChallengeType.Counter, label, issuer, secret, encoding, algo, digits, additional)
        {
            Volatile.Write(ref this._counter, counter);
        }

        /// <inheritdoc />
        public override long GetCounterValue()
            => Interlocked.Increment(ref this._counter) - 1;

        /// <inheritdoc />
        protected override IEnumerable<KeyValuePair<string, string>> ProvideUrlParameters()
        {
            yield return new KeyValuePair<string, string>(ParamCounter, this.Counter.ToString());
        }

        /// <summary>
        /// Creates new Google Authenticator-compatible settings.
        /// </summary>
        /// <param name="label">Label for the generator.</param>
        /// <param name="issuer">Issuer for the generator.</param>
        /// <param name="secret">Secret to use for generating challenges.</param>
        /// <param name="counter">Current state of the counter.</param>
        /// <returns>Created generator settings.</returns>
        public static HotpGeneratorSettings CreateGoogleAuthenticator(string label, string issuer, byte[] secret, long counter)
            => new HotpGeneratorSettings(label, issuer, secret, ByteEncoding.Base32, HmacAlgorithm.Sha1, 6, null, counter);

        /// <summary>
        /// Creates new Google Authenticator-compatible settings, and generates a new secret.
        /// </summary>
        /// <param name="label">Label for the generator.</param>
        /// <param name="issuer">Issuer for the generator.</param>
        /// <returns>Created generator settings.</returns>
        public static HotpGeneratorSettings GenerateGoogleAuthenticator(string label, string issuer)
        {
            var secret = new byte[80];
            long counter;
            using (var rng = new SecureRandom())
            {
                rng.GetBytes(secret);
                counter = rng.GetInt64();
            }

            return CreateGoogleAuthenticator(label, issuer, secret, counter);
        }

        internal static HotpGeneratorSettings FinishParsingUri(
            string label,
            string issuer,
            byte[] secret,
            ByteEncoding encoding,
            HmacAlgorithm algo,
            int digits,
            byte[] additional,
            IDictionary<string, StringValues> args)
        {
            if (!args.TryGetValue(ParamCounter, out var counters))
                throw new ArgumentException("Missing counter value.", nameof(args));

            return new HotpGeneratorSettings(label, issuer, secret, encoding, algo, digits, additional, long.Parse(counters.First(), NumberStyles.Integer, CultureInfo.InvariantCulture));
        }
    }
}

#pragma warning restore CS0420 // A reference to a volatile field will not be treated as volatile
