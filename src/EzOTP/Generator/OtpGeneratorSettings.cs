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
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.WebUtilities;

namespace EzOTP
{
    /// <summary>
    /// Represents settings for a code generator.
    /// </summary>
    public abstract class OtpGeneratorSettings
    {
        private const string ParamSecret = "secret";
        private const string ParamIssuer = "issuer";
        private const string ParamAlgorithm = "algorithm";
        private const string ParamDigits = "digits";
        private const string ParamEncoding = "encoding";
        private const string ParamAdditional = "additional";

        private const string UriSchema = "otpauth";
        private const string UriHostTime = "totp";
        private const string UriHostCounter = "hotp";

        /// <summary>
        /// Gets the type of this generator.
        /// </summary>
        public ChallengeType Type { get; }

        /// <summary>
        /// Gets the label for this generator.
        /// </summary>
        public string Label { get; }

        /// <summary>
        /// Gets the issuer for this generator.
        /// </summary>
        public string Issuer { get; }

        /// <summary>
        /// Gets the secret used for generating codes.
        /// </summary>
        public ImmutableArray<byte> Secret { get; }

        /// <summary>
        /// Gets the encoding used to encode the secret.
        /// </summary>
        public ByteEncoding SecretEncoding { get; }

        /// <summary>
        /// Gets the algorithm used to generate codes.
        /// </summary>
        public HmacAlgorithm Algorithm { get; }

        /// <summary>
        /// Gets the number of digits to output.
        /// </summary>
        public int Digits { get; }

        /// <summary>
        /// Gets the additional data to append to the challenge if applicable.
        /// </summary>
        public ImmutableArray<byte> Additional { get; }

        internal OtpGeneratorSettings(
            ChallengeType type, 
            string label, 
            string issuer, 
            byte[] secret, 
            ByteEncoding encoding,
            HmacAlgorithm algo, 
            int digits, 
            byte[] additional)
        {
            this.Type = type;
            this.Label = label;
            this.Issuer = issuer;
            this.Secret = ImmutableArray.Create(secret);
            this.SecretEncoding = encoding;
            this.Algorithm = algo;
            this.Digits = digits;
            this.Additional = additional != null
                ? ImmutableArray.Create(additional)
                : ImmutableArray<byte>.Empty;
        }

        /// <summary>
        /// Gets the current value of the counter.
        /// </summary>
        /// <returns>Current counter value</returns>
        public abstract long GetCounterValue();

        /// <summary>
        /// Generates input for the HMAC provider.
        /// </summary>
        /// <param name="target">Target buffer to place the challenge into.</param>
        /// <param name="bytesWritten">Number of bytes written.</param>
        /// <param name="offset">Amount to offset the counter by.</param>
        /// <returns>Whether the operation was successful.</returns>
        public bool TryGetChallengeInput(Span<byte> target, out int bytesWritten, int offset = 0)
            => this.TryGetChallengeInput(this.GetCounterValue() + offset, target, out bytesWritten);

        /// <summary>
        /// Generates input for the HMAC provider.
        /// </summary>
        /// <param name="value">Counter value to use as basis.</param>
        /// <param name="target">Target buffer to place the challenge into.</param>
        /// <param name="bytesWritten">Number of bytes written.</param>
        /// <returns>Whether the operation was successful.</returns>
        public bool TryGetChallengeInput(long value, Span<byte> target, out int bytesWritten)
        {
            bytesWritten = 0;
            if (target.Length < 8 + this.Additional.Length)
                return false;

            BinaryPrimitives.WriteInt64BigEndian(target, value);
            this.Additional.AsSpan().CopyTo(target.Slice(8));
            return true;
        }

        /// <summary>
        /// Generates input for the HMAC provider.
        /// </summary>
        /// <param name="offset">Amount to offset the counter by.</param>
        /// <returns>Generated input.</returns>
        public byte[] GetChallengeInput(int offset = 0)
            => this.GetChallengeInput(this.GetCounterValue() + offset);

        /// <summary>
        /// Generates input for the HMAC provider.
        /// </summary>
        /// <param name="value">Counter value to use as basis.</param>
        /// <returns>Generated input.</returns>
        public byte[] GetChallengeInput(long value)
        {
            var buff = new byte[8 + this.Additional.Length];
            var buffs = buff.AsSpan();

            BinaryPrimitives.WriteInt64BigEndian(buffs, value);
            this.Additional.AsSpan().CopyTo(buffs.Slice(8));
            return buff;
        }

        /// <summary>
        /// Provides any additional parameters for URL serialization.
        /// </summary>
        /// <returns>Enumerable of key-value pairs.</returns>
        protected abstract IEnumerable<KeyValuePair<string, string>> ProvideUrlParameters();

        /// <summary>
        /// Converts these settings to an URI.
        /// </summary>
        /// <returns>Constructed URI.</returns>
        public Uri ToUri()
        {
            var ub = new UriBuilder
            {
                Scheme = UriSchema,
                Host = this.Type switch
                {
                    ChallengeType.Time    => UriHostTime,
                    ChallengeType.Counter => UriHostCounter,
                    _                     => throw new ArgumentException("Invalid type specified.", nameof(this.Type))
                },
                Path = HttpUtility.UrlEncode(this.Label)
            };

            var enc = ByteEncodingSelector.FromId(this.SecretEncoding);
            var ssize = enc.EstimateEncodedSize(this.Secret.Length);
            Span<char> sspan = stackalloc char[ssize];
            if (!enc.TryEncode(this.Secret.AsSpan(), sspan, out ssize))
                throw new Exception("Failed to encode secret.");

            var dict = new Dictionary<string, string>(7)
            {
                [ParamSecret] = new string(sspan.Slice(0, ssize)),
                [ParamAlgorithm] = this.Algorithm switch
                {
                    HmacAlgorithm.Md5    => "MD5",
                    HmacAlgorithm.Sha1   => "SHA1",
                    HmacAlgorithm.Sha256 => "SHA256",
                    HmacAlgorithm.Sha384 => "SHA384",
                    HmacAlgorithm.Sha512 => "SHA512",
                    _                    => throw new ArgumentException("Invalid algorithm specified.", nameof(this.Algorithm))
                },
                [ParamDigits] = this.Digits.ToString(CultureInfo.InvariantCulture)
            };

            if (this.SecretEncoding != ByteEncoding.Base32)
                dict[ParamEncoding] = this.SecretEncoding switch
                {
                    ByteEncoding.Base16 => "base16",
                    ByteEncoding.Base64 => "base64",
                    _                   => throw new ArgumentException("Invalid encoding specified.", nameof(this.SecretEncoding))
                }; // No base32 since it's implicit

            if (!string.IsNullOrWhiteSpace(this.Issuer))
                dict[ParamIssuer] = this.Issuer;

            if (this.Additional.Length > 0)
            {
                ssize = enc.EstimateEncodedSize(this.Additional.Length);
                sspan = stackalloc char[ssize];
                if (!enc.TryEncode(this.Additional.AsSpan(), sspan, out ssize))
                    throw new Exception("Failed to encode additional data.");

                dict[ParamAdditional] = new string(sspan);
            }

            foreach (var (k, v) in this.ProvideUrlParameters())
                dict[k] = v;

            return new Uri(QueryHelpers.AddQueryString(ub.Uri.ToString(), dict));
        }

        /// <summary>
        /// Parses an OTP authentication URI to a settings object.
        /// </summary>
        /// <param name="uri">URI to parse.</param>
        /// <returns>Parsed settings.</returns>
        public static OtpGeneratorSettings ParseUri(Uri uri)
        {
            if (!string.Equals(uri.Scheme, UriSchema, StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Supplied URI is not an OTP authentication URI.", nameof(uri));

            var type = ChallengeTypeParser.ParseName(uri.Host);
            var label = HttpUtility.UrlDecode(uri.AbsolutePath.Substring(1));
            var args = QueryHelpers.ParseQuery(uri.Query);

            var secretr = args[ParamSecret].First();
            var issuer = args.TryGetValue(ParamIssuer, out var issuers) ? issuers.First() : null;
            var algor = args.TryGetValue(ParamAlgorithm, out var algos) ? algos.First() : "sha1";
            var digitsr = args.TryGetValue(ParamDigits, out var digitss) ? digitss.First() : "6";
            var encodingr = args.TryGetValue(ParamEncoding, out var encodings) ? encodings.First() : "base32";
            var additionalr = args.TryGetValue(ParamAdditional, out var additionals) ? additionals.First() : null;

            var encid = ByteEncodingSelector.ParseName(encodingr);
            var enc = ByteEncodingSelector.FromId(encid);
            Span<byte> buff = stackalloc byte[enc.EstimateDecodedSize(secretr.Length)];
            if (!enc.TryDecode(secretr, buff, out var secretlen))
                throw new ArgumentException("Failed to decode secret.", nameof(uri));

            var secret = buff.Slice(0, secretlen).ToArray();

            if (additionalr != null)
            {
                buff = stackalloc byte[enc.EstimateDecodedSize(additionalr.Length)];
                if (!enc.TryDecode(additionalr, buff, out var additionallen))
                    throw new ArgumentException("Failed to decode additional data.", nameof(uri));
            }

            var additional = additionalr != null ? buff.ToArray() : null;
            var digits = int.Parse(digitsr, NumberStyles.Integer, CultureInfo.InvariantCulture);
            var algo = HmacProviderSelector.ParseName(algor);

            return type switch
            {
                ChallengeType.Time    => TotpGeneratorSettings.FinishParsingUri(label, issuer, secret, encid, algo, digits, additional, args),
                ChallengeType.Counter => HotpGeneratorSettings.FinishParsingUri(label, issuer, secret, encid, algo, digits, additional, args),
                _                     => throw new ArgumentException("Invalid challenge type specified.", nameof(uri))
            };
        }
    }
}
