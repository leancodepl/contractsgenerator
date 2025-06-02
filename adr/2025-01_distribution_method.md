# Distribution Method

## Status

Approved and implemented (2025-05-30)

## Context

Our Contracts Generator targets .NET 6 only which is problematic. It must use .NET no older than the one the contracts
project is targeting (because we can't use MSBuild/SDKs targeting newer runtimes in-proc) and we should not target only
the latest TFM to avoid forcing consumers to use different SDK versions. We could solve it, by multi-targeting the CG
but then we must distribute the binary somehow.

### Distribute multiple release binaries manually

Simple solution that moves the responsibility of choosing appropriate release binary to `generate.sh` script or client
generator. However, the logic behind choosing it might be tricky, and there are already other solutions that let us take
advantage of .NET's own version resolution logic.

### Distribute the generator as .NET tool

If we package the generator as a .NET tool, we could push a single package to a NuGet feed that includes multiple
binaries, each targeting a different supported TFM. This way, we can rely on `dotnet tool run` picking appropriate
binary for any given context, but it would also require us to move generator version selection mechanism away from
client-controlled environment variables to .NET tool manifest files, which should be kept in a location discoverable
by .NET driver.

Additional advantage of this approach would be the possibility to drop `generate.sh` script completely and have client
generators invoke `dotnet tool restore` then `dotnet tool run` directly, greatly improving compatibility with Windows
systems that don't have a Bourne shell interpreter installed by default.

## Decision

We have decided to distribute the generator as .NET tool, modify client generators to call `dotnet tool` directly, and
remove `generate.sh` altogether. This moves the responsibility of choosing the .NET generator version to the backend,
trading ensured compatibility between server and client generators for ensured compatibility between .NET generator
and contracts themselves.
