# Contracts versioning and compatibility

## Status

Accepted and implemented (2025-06-09)

## Context

Switching to .NET tool distribution method removed the ability of client generators to choose any version of server
generator they desired. While this improved the compatibility of server generator with contracts themselves (because
it's now trivial to ensure they can be compiled properly as a CI step), it makes it harder to choose which version of
the client generator should be used, and to figure out whether it's safe to upgrade (or not).

### Versioning the contracts

The general idea to resolve this is to have the server generator emit some versioning info, in addition to standard
contracts output or when asked for specifically from the CLI. The output is a machine readable (Protobuf-encoded)
summary of currently emitted contracts version and a list of optional extensions, for which support is recommended but
might not be required, depending on the contracts provided as input. The client generator can parse this output and
report an error if major contracts version is not supported by this version of client generator, or warn if an optional
extension is unsupported.

The `Protocol` message is defined as follows:

```protobuf
message Protocol {
  string version = 1;
  repeated string extensions = 2;
}
```

For the current version number and a list of known extensions, see
[`Protocol.CurrentVersion.cs`](../src/LeanCode.ContractsGenerator/Protocol.CurrentVersion.cs).
This message is emitted as a part of `Export` message, or when the generator is called with `protocol` verb:

```bash
$ dotnet tool run dotnet-contracts-generate -- protocol | protoc --decode=leancode.contracts.Protocol -I …
version: "1"

$ dotnet tool run dotnet-contracts-generate -- protocol --allow-date-time | protoc --decode=leancode.contracts.Protocol …
version: "1"
extensions: "datetime"
```

In the above example, the server generator reports core contracts version of `1` requiring client generators to
support any contracts version from the range of `[1.0, 2.0)`. It may also emit contracts utilizing features from `datetime`
extension if `--allow-date-time` flag was passed as an additional CLI argument.

In order to ensure that server and client generator will be able to communicate with each other at least for the purpose
of exchanging compatibility information, new verb, format and encoding of core protocol version should be considered
calcified. Furthermore, the shape of returned object (properties other than `version`) can only be changed when
bumping major core protocol version.

## Questions

1. What should the verb/flag to emit compatibility info be called?
   * The verb has been named `protocol`.
2. Which format should the compatibility info be emitted in?
   * Protobuf, which is already supported by client generators. It can be decoded to something human-readable using
     `protoc` or a similar tool, but it's not impossible that we will add other optional formats in the future.
3. Do we even need a flag, or perhaps could we get away with always including this info as a part of emitted contracts?
   * In the initial version there is no flag, the emitted `Export` message always contains `Protocol` info.
   * We may add a flag to emit only this info for given set of input contracts in the future.
4. Do we need to split version numbers into major/minor/patch?
   * Core protocol version has major and minor version numbers; the minor number is considered to be optional
     (and assumed to be `0` when not present).
5. If we were to keep major and minor version numbers, what kind of changes would warrant bumping major version?
   * Anything that would break Protobuf message compatibility and prevent clients from parsing the `Export`.
6. How should we name and version contracts extensions?
   * Simple names, like `datetime`, should be used as extension names.
   * There is no explicit extension versioning; instead new extensions like `datetime2` will be created as needed.
7. Should the generator avoid mentioning extensions in `Export`s that were available but weren't actually used?
   * Nice to have but it's not a priority. To be reconsidered when we have extensions that aren't controlled by CLI flags.
