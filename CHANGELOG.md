# Change Log

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [2.0.0] (unreleased)

### Added

- `LeanCode.Contracts` package with basic contracts definition,
- `Binary` type,
- `Operation` type,
- Basic docs,
- Add `dotnet contracts-generate` global tool,
- Allow running the generator in check-only mode,
- _Invalid type_ analyzer.

### Breaking changes

- The generator depends on `LeanCode.Contracts` instead of `LeanCode.CQRS` package,
- `Date`, `Time` and `DateTime` known-types are gone.

## [1.0.0]

### Added

- Embedding contracts in DLLs,

## 0.1.0

### Added

- Initial contracts definition,
- Support for commands, queries, DTOs, enums,
- Support for basic known types,
- Basic analyzers.

[1.0.0]: https://github.com/leancodepl/contractsgenerator/compare/v0.1.0-alpha11...v1.0.0
[2.0.0]: https://github.com/leancodepl/contractsgenerator/compare/v1.0.0...v2.0.0-alpha.1
