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
using System.Globalization;
using System.Linq;
using Emzi0767;
using Microsoft.Extensions.Primitives;

namespace EzOTP
{
    /// <summary>
    /// Represents settings for a time-based code generator.
    /// </summary>
    public sealed class TotpGeneratorSettings : OtpGeneratorSettings
    {
        private const string ParamPeriod = "period";

        /// <summary>
        /// Gets the number of setings in a single generator interval.
        /// </summary>
        public int Period { get; }

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
        /// <param name="period">Number of seconds in a single generator tick.</param>
        public TotpGeneratorSettings(
            string label,
            string issuer,
            byte[] secret,
            ByteEncoding encoding,
            HmacAlgorithm algo,
            int digits,
            byte[] additional,
            int period)
            : base(ChallengeType.Time, label, issuer, secret, encoding, algo, digits, additional)
        {
            this.Period = period;
        }

        /// <inheritdoc />
        public override long GetCounterValue()
        {
            var sec = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            return sec / this.Period;
        }

        /// <inheritdoc />
        protected override IEnumerable<KeyValuePair<string, string>> ProvideUrlParameters()
        {
            yield return new KeyValuePair<string, string>(ParamPeriod, this.Period.ToString());
        }

        /// <summary>
        /// Creates new Google Authenticator-compatible settings.
        /// </summary>
        /// <param name="label">Label for the generator.</param>
        /// <param name="issuer">Issuer for the generator.</param>
        /// <param name="secret">Secret to use for generating challenges.</param>
        /// <returns>Created generator settings.</returns>
        public static TotpGeneratorSettings CreateGoogleAuthenticator(string label, string issuer, byte[] secret)
            => new TotpGeneratorSettings(label, issuer, secret, ByteEncoding.Base32, HmacAlgorithm.Sha1, 6, null, 30);

        /// <summary>
        /// Creates new Google Authenticator-compatible settings, and generates a new secret.
        /// </summary>
        /// <param name="label">Label for the generator.</param>
        /// <param name="issuer">Issuer for the generator.</param>
        /// <returns>Created generator settings.</returns>
        public static TotpGeneratorSettings GenerateGoogleAuthenticator(string label, string issuer)
        {
            var secret = new byte[80];
            using (var rng = new SecureRandom())
                rng.GetBytes(secret);

            return CreateGoogleAuthenticator(label, issuer, secret);
        }

        internal static TotpGeneratorSettings FinishParsingUri(
            string label,
            string issuer,
            byte[] secret,
            ByteEncoding encoding,
            HmacAlgorithm algo,
            int digits,
            byte[] additional,
            IDictionary<string, StringValues> args)
        {
            var periodr = args.TryGetValue(ParamPeriod, out var periods) ? periods.First() : "30";

            return new TotpGeneratorSettings(label, issuer, secret, encoding, algo, digits, additional, int.Parse(periodr, NumberStyles.Integer, CultureInfo.InvariantCulture));
        }
    }
}
