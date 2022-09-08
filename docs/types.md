# Known types

The generator supports a number of types that are known and considered part of the contracts. You can divide them into
two groups: .NET based and custom, provided by the contracts package.

The way the types are serialized can be found in [Serialization](./serialization.md).

## .NET Types

The generator supports a number of .NET types that can be freely used in the contracts. The notion here is
`(contracts type name & .NET type name)` or `(contracts type name)/(.NET type name)`.

1. [`Object`](https://docs.microsoft.com/en-us/dotnet/api/system.object),
1. [`String`](https://docs.microsoft.com/en-us/dotnet/api/system.string),
1. [`Guid`](https://docs.microsoft.com/en-us/dotnet/api/system.guid),
1. [`Uri`](https://docs.microsoft.com/en-us/dotnet/api/system.uri),
1. [`Boolean`/`bool`](https://docs.microsoft.com/en-us/dotnet/api/system.boolean),
1. [`UInt8`/`byte`](https://docs.microsoft.com/en-us/dotnet/api/system.byte),
1. [`Int8`/`sbyte`](https://docs.microsoft.com/en-us/dotnet/api/system.sbyte),
1. [`UInt16`/`ushort`](https://docs.microsoft.com/en-us/dotnet/api/system.uint16),
1. [`Int16`/`short`](https://docs.microsoft.com/en-us/dotnet/api/system.int16),
1. [`UInt32`/`uint`](https://docs.microsoft.com/en-us/dotnet/api/system.uint32),
1. [`Int32`/`int`](https://docs.microsoft.com/en-us/dotnet/api/system.int32),
1. [`UInt64`/`ulong`](https://docs.microsoft.com/en-us/dotnet/api/system.uint64),
1. [`Int64`/`long`](https://docs.microsoft.com/en-us/dotnet/api/system.int64),
1. [`Float32`/`float`](https://docs.microsoft.com/en-us/dotnet/api/system.single),
1. [`Float64`/`double`](https://docs.microsoft.com/en-us/dotnet/api/system.double),
1. [`DateOnly`](https://docs.microsoft.com/en-us/dotnet/api/system.dateonly),
1. [`TimeOnly`](https://docs.microsoft.com/en-us/dotnet/api/system.timeonly),
1. [`DateTimeOffset`](https://docs.microsoft.com/en-us/dotnet/api/system.datetimeoffset),
1. [`TimeSpan`](https://docs.microsoft.com/en-us/dotnet/api/system.timespan),
1. [`Array`/`List`](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1),
1. [`Map`/`Dictionary`](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2),
1. [`Attribute`](https://docs.microsoft.com/en-us/dotnet/api/system.attribute) (will only occur as a base for other types).

## Custom types

1. [`Query`](../src/LeanCode.Contracts/IQuery.cs),
1. [`Command`](../src/LeanCode.Contracts/ICommand.cs),
1. [`CommandResult`](../src/LeanCode.Contracts/CommandResult.cs),
1. [`Operation`](../src/LeanCode.Contracts/IOperation.cs),
1. [`Binary`](../src/LeanCode.Contracts/Binary.cs),
1. [`AuthorizeWhenAttribute`](../src/LeanCode.Contracts/Security/AuthorizeWhenAttribute.cs)
1. [`AuthorizeWhenHasAnyOfAttribute`](../src/LeanCode.Contracts/Security/AuthorizeWhenHasAnyOfAttribute.cs)
1. [`QueryCacheAttribute`](../src/LeanCode.Contracts/QueryCacheAttribute.cs)
