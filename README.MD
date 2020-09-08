# EzOTP
A simple, lightweight, and easy to use implementation of One-Time Password protocol, both time-based and counter-based 
variants.

## Installation
To start using the package, just [install it from NuGet](https://www.nuget.org/packages/EzOTP/).

## Usage
The library is designed to be fairly straightforward and easy to use.

### Basic usage
The primary use case for the library is usage from QR codes, or other media containing [OTP authentication](https://github.com/google/google-authenticator/wiki/Key-Uri-Format) 
URIs (that is, URIs with `otpauth` scheme).

```cs
var otp = OtpGenerator.ParseUri("otpauth://totp/ACME%20Co:john@example.com?secret=DGW24UIKQZBELXEMY64PICAL5IGYMJM6&issuer=ACME%20Co&algorithm=SHA1&digits=6&period=30");
var code = otp.GenerateCode();
```

This will generate a code in `XXXXXX` format, as a string.

### Generate raw code
In the event a numeric code is more desired (e.g. for comparison purposes), one can use `GenerateRaw()` method instead. 
It returns an integer with appropriate number of digits.

```cs
var otp = OtpGenerator.ParseUri("otpauth://totp/ACME%20Co:john@example.com?secret=DGW24UIKQZBELXEMY64PICAL5IGYMJM6&issuer=ACME%20Co&algorithm=SHA1&digits=6&period=30");
var code = otp.GenerateRaw();
```

### Groupping digits
`groupSize` argument of `GenerateCode()` defines a maximum number of digits in a single group. For example, if the 
generator is configured for generating 6 digit-long codes, the output looks like so, depending on the value of 
`groupSize` argument:
- 1: `X X X X X X`
- 2: `XX XX XX`
- 3: `XXX XXX`
- 4: `XX XXXX`
- 5: `X XXXXX`
- 6+: `XXXXXX`

### Handling desynchronization
Desynchronization between 2 generators can occur. Because of this, EzOTP provides 2 ways of handling 
potentially-desynchronized generators.

#### Method 1: Offset
If the desynchronization offset of both counters is known, one can use `offset` argument of `GenerateCode()` method. 
This will add a fixed offset to generated counter values, without changing the counter value itself.

For example, using Google Authenticator-compatible examble from **Basic usage** section, let's assume the 
desynchronization between the two devices is between 60 and 90 seconds, such that current application is lagging.

Since Google Authenticator uses periods of 30 seconds, this means we have to add `60 / 30 = 2` periods to the counter, 
meaning we need to use an offset of 2:

```cs
var otp = OtpGenerator.ParseUri("otpauth://totp/ACME%20Co:john@example.com?secret=DGW24UIKQZBELXEMY64PICAL5IGYMJM6&issuer=ACME%20Co&algorithm=SHA1&digits=6&period=30");
var code = otp.GenerateCode(offset: 2);
```

Should the current application be the one ahead, an offset of -2 would be applied instead.

#### Method 2: Counter window
For authenticating, RFC actually recommends testing several codes in the past and future. However generating several 
codes using above methods is not the best choice, as it does trip the counter internally, meaning that if the generator 
type uses a counter (that is, is a HOTP generator), it will increase the internal counter value for every code 
generated.

To alleviate this issue, EzOTP provides methods for generating a "window" of codes, whilst incrementing the counter 
only once.

Assuming that user-provided code is in an `int` variable called `userCode`, one can validate their input like so:

```cs
var otp = OtpGenerator.ParseUri("otpauth://totp/ACME%20Co:john@example.com?secret=DGW24UIKQZBELXEMY64PICAL5IGYMJM6&issuer=ACME%20Co&algorithm=SHA1&digits=6&period=30");
if (!otp.GenerateRawWindow(window: 1).Contains(userCode))
    // fail the authentication process
```

The method optionally takes a `window` argument, which defines the window size. Given a window size of `n`, EzOTP will 
generate `n` codes before current counter value, 1 current counter value code, and `n` codes after current counter 
value.

Specifying a value of `0` or a negative number will use default window sizes. For TOTP it's 1, for HOTP it's 2.

### OTP generator configuration
An OTP generator can be constructed without parsing an URI, by constructing an instance of `OtpGeneratorConfiguration` 
directly. It's an abstract class, which serves as a common base for 2 configuration types:
- `TotpGeneratorConfiguration`: Contains TOTP-specific properties.
- `HotpGeneratorConfiguration`: Contains HOTP-specific properties.

The configuration instance can then be supplied to the constructor of `OtpGenerator`. This allows for storing OTP 
generator state and restoring it without having to use URIs. This is useful for HOTP particularly, where the counter 
value has to be re-recorded after every generation.

### Generating Google Authenticator URIs
One can easily generate configurations for Google Authenticator, by using `GenerateGoogleAuthenticator()` static method 
on either `TotpGeneratorConfiguration` or `HotpGeneratorConfiguration` classes.

These methods return configuration objects, that can then be plugged into `OtpGenerator` constructor, or just returned 
as an URI to a user.

## Support me
Lots of effort went into making this software.

If you feel like I'm doing a good job, or just want to throw money at me, you can do so through any of the following:
- [Patreon](https://patreon.com/emzi0767)
- [Ko-Fi](https://ko-fi.com/emzi0767)
- [PayPal](https://paypal.me/Emzi0767/5USD)

## Other questions
If you have other questions or would like to talk in general, feel free to visit my Discord server.

[![Emzi's Central Dispatch](https://discordapp.com/api/guilds/207879549394878464/embed.png?style=banner1)](https://discord.gg/rGKrJDR)
