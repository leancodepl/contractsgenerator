# Contracts versioning and compatibility

## Status

Proposed (2025-06-02)

## Context

Switching to .NET tool distribution method removed the ability of client generators to choose any version of server
generator they desired. While this improved the compatibility of server generator with contracts themselves (because
it's now trivial to ensure they can be compiled properly as a CI step), it makes it harder to choose which version of
the client generator should be used, and to figure out whether it's safe to upgrade (or not).

### Versioning the contracts

The general idea to resolve this is to have the server generator emit some versioning info, in addition to standard
contracts output, when asked for specifically from the CLI, or both. The output would be a machine readable summary
of currently emitted contracts version (likely SemVer-compatible) and a list of optional extensions, for which support
is recommended but not required. The client generator could parse this output and report an error if major contracts
version is not supported by this version of client generator, or warn if an optional extension is unsupported.

```bash
$ dotnet tool run dotnet-contracts-generate -- compat-info
{
    "Version": "2.1",
    "Extensions": {
        "2025-05_datetime": "1.1",
        "2025-06_decimal": "1.0"
    }
}

$ dotnet tool run dotnet-contracts-generate -- project -p /path/to/some/Project.Contracts.csproj \
    --allow-date-time --check-only --print-compat-info
{
    "Version": "2.1",
    "Extensions": {
        "2025-05_datetime": "1.0"
    }
}
```

In the above example, the server generator reports core contracts version of `2.1` requiring client generators to
support any contracts version from the range of `[2.1, 3.0)` (or `[2.0, 3.0)` with a warning for anything below `2.1`).
Additionally, it may emit contracts utilizing features from `2025-05_datetime` and `2025-06_decimal` extensions,
at versions from the range of `[1.0, 1.1]` and `1.0` respectively, but for a particular set of contracts generated from
`Project.Contracts.csproj`, only `2025-05_datetime` at version `1.0` is required to be supported by client generators.

In order to ensure that server and client generator will be able to communicate with each other at least for the purpose
of exchanging compatibility information, new verb, flag, format and encoding of core contracts version will likely have
to be calcified. Furthermore, the shape of returned object (properties other than `Version`) can only be changed when
bumping major core version.

## Open questions

1. What should the verb/flag to emit compatibility info be called? In the example above, it's `compat-info` and
   `--print-compat-info` respectively but exact naming is TBD.
2. Which format should the compatibility info be emitted in? It might be JSON or Protobuf, with the former having the
   advantage of being more human-readable while the latter is already natively consumed by existing client generator,
   or something else entirely.
3. Do we even need `--print-compat-info` flag, or perhaps could we get away with always including this info as a part
   of emitted contracts protobuf message?
4. Do we need to split version numbers into major/minor/patch?
5. If we were to keep major and minor version numbers, what kind of changes would warrant bumping major version?
6. How should we name and version contracts extensions?
