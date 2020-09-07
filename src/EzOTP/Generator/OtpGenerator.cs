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

namespace EzOTP
{
    /// <summary>
    /// Generates one-time passwords for authentication.
    /// </summary>
    public sealed class OtpGenerator
    {
        /// <summary>
        /// Gets the configuration of the generator.
        /// </summary>
        public OtpGeneratorSettings Settings { get; }

        /// <summary>
        /// Creates a new OTP generator, with specified settings.
        /// </summary>
        /// <param name="settings">Generator settings to use.</param>
        public OtpGenerator(OtpGeneratorSettings settings)
        {
            this.Settings = settings;
        }

        /// <summary>
        /// Generates raw, numeric one-time password.
        /// </summary>
        /// <param name="offset">Counter offset to use when generating.</param>
        /// <returns>Generated password.</returns>
        public int GenerateRaw(long offset = 0L)
        {
            var bufflen = 8 + this.Settings.Additional.Length;
            Span<byte> challenge = stackalloc byte[bufflen];
            if (!this.Settings.TryGetChallengeInput(challenge, out bufflen, offset))
                throw new Exception("Failed to generate challenge.");

            using var hmac = HmacProviderSelector.FromId(this.Settings.Algorithm);
            Span<byte> buff = stackalloc byte[hmac.OutputSize];
            if (!hmac.TryCompute(this.Settings.Secret.AsSpan(), challenge, buff, out bufflen))
                throw new Exception("Failed to compute HMAC.");

            var transformer = CodeTransformerSelector.FromId(CodeTransformer.Rfc4226);
            if (!transformer.TryTransform(buff.Slice(0, bufflen), out var number))
                throw new Exception("Failed to transform HMAC.");

            return StringCodeFormatter.GetRawCode(number, this.Settings.Digits);
        }

        /// <summary>
        /// Generates one-time password.
        /// </summary>
        /// <param name="output">Buffer to place the generated password characters in.</param>
        /// <param name="groupSize">Optional. Maximum number of digits per digit group.</param>
        /// <param name="offset">Counter offset to use when generating.</param>
        /// <returns>Whether the operation was successful.</returns>
        public bool TryGenerate(Span<char> output, int groupSize = 0, long offset = 0L)
        {
            var bufflen = 8 + this.Settings.Additional.Length;
            Span<byte> challenge = stackalloc byte[bufflen];
            if (!this.Settings.TryGetChallengeInput(challenge, out bufflen, offset))
                return false;

            using var hmac = HmacProviderSelector.FromId(this.Settings.Algorithm);
            Span<byte> buff = stackalloc byte[hmac.OutputSize];
            if (!hmac.TryCompute(this.Settings.Secret.AsSpan(), challenge, buff, out bufflen))
                return false;

            var transformer = CodeTransformerSelector.FromId(CodeTransformer.Rfc4226);
            if (!transformer.TryTransform(buff.Slice(0, bufflen), out var number))
                return false;

            if (groupSize > 0)
                return StringCodeFormatter.TryFormatGroupped(number, this.Settings.Digits, output, groupSize);
            else
                return StringCodeFormatter.TryFormat(number, this.Settings.Digits, output);
        }

        /// <summary>
        /// Generates one-time password.
        /// </summary>
        /// <param name="groupSize">Optional. Maximum number of digits per digit group.</param>
        /// <param name="offset">Counter offset to use when generating.</param>
        /// <returns>Generated password.</returns>
        public string Generate(int groupSize = 0, long offset = 0L)
        {
            var bufflen = 8 + this.Settings.Additional.Length;
            Span<byte> challenge = stackalloc byte[bufflen];
            if (!this.Settings.TryGetChallengeInput(challenge, out bufflen, offset))
                throw new Exception("Failed to generate challenge.");

            using var hmac = HmacProviderSelector.FromId(this.Settings.Algorithm);
            Span<byte> buff = stackalloc byte[hmac.OutputSize];
            if (!hmac.TryCompute(this.Settings.Secret.AsSpan(), challenge, buff, out bufflen))
                throw new Exception("Failed to compute HMAC.");

            var transformer = CodeTransformerSelector.FromId(CodeTransformer.Rfc4226);
            if (!transformer.TryTransform(buff.Slice(0, bufflen), out var number))
                throw new Exception("Failed to transform HMAC.");

            if (groupSize > 0)
                return StringCodeFormatter.FormatGroupped(number, this.Settings.Digits, groupSize);
            else
                return StringCodeFormatter.Format(number, this.Settings.Digits);
        }

        /// <summary>
        /// Converts this generator into an URI.
        /// </summary>
        /// <returns>URI representation of this generator.</returns>
        public Uri ToUri()
            => this.Settings.ToUri();

        /// <summary>
        /// Parses an OTP authentication URI and creates a new generator.
        /// </summary>
        /// <param name="uri">URI to parse.</param>
        /// <returns>Generator with parsed settings.</returns>
        public static OtpGenerator ParseUri(Uri uri)
            => new OtpGenerator(OtpGeneratorSettings.ParseUri(uri));
    }
}
