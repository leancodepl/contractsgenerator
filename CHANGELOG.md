# Change Log

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## LeanCode.ContractsGenerator

### [2.0.0]

#### Added

- Support for `Binary` type,
- Support for `Operation` type,
- Support for `struct` members,
- Support for attributes on Enum members (`EnumValue`),
- `dotnet contracts-generate` global tool,
- Allow running the generator in check-only mode,
- _Invalid type_ analyzer,
- Basic docs.

#### Breaking changes

- The generator depends on `LeanCode.Contracts` instead of `LeanCode.CQRS` package,
- `Date`, `Time` and `DateTime` known-types are gone,
- The tool targets both .NET 6 and .NET 7 now.

### [1.0.0]

#### Added

- Embedding contracts in DLLs,

### 0.1.0

#### Added

- Support for commands, queries, DTOs, enums,
- Support for basic known types,
- Basic analyzers.

## LeanCode.Contracts

### 2.0.0 (unreleased)

- Added LeanPipe contracts.

### 1.1.1

#### Changed

- Added .NET 7 to target frameworks.

### 1.1.0

#### Added

- `Binary` data type.

### 1.0.1

#### Fixed

- Added missing nuspec properties and enable Source Link.

### 1.0.0

#### Added

- Initial contracts definition.

[1.0.0]: https://github.com/leancodepl/contractsgenerator/compare/v0.1.0-alpha11...v1.0.0
[2.0.0]: https://github.com/leancodepl/contractsgenerator/compare/v1.0.0...v2.0.0-alpha.1
