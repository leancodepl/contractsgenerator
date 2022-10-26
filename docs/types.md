# Known types

The generator supports a number of types that are known and considered part of the contracts. You can divide them into
two groups: .NET based and custom, provided by the contracts package.

The way the types are serialized can be found in [Serialization](./serialization.md).

## .NET Types

The generator supports a number of .NET types that can be freely used in the contracts. The notion here is
`(contracts type name & .NET type name)` or `(contracts type name)/(.NET type name)`.

1. [`Object`](https://docs.microsoft.com/en-us/dotnet/api/system.object),
2. [`String`](https://docs.microsoft.com/en-us/dotnet/api/system.string),
3. [`Guid`](https://docs.microsoft.com/en-us/dotnet/api/system.guid),
4. [`Uri`](https://docs.microsoft.com/en-us/dotnet/api/system.uri),
5. [`Boolean`/`bool`](https://docs.microsoft.com/en-us/dotnet/api/system.boolean),
6. [`UInt8`/`byte`](https://docs.microsoft.com/en-us/dotnet/api/system.byte),
7. [`Int8`/`sbyte`](https://docs.microsoft.com/en-us/dotnet/api/system.sbyte),
8. [`UInt16`/`ushort`](https://docs.microsoft.com/en-us/dotnet/api/system.uint16),
9. [`Int16`/`short`](https://docs.microsoft.com/en-us/dotnet/api/system.int16),
10. [`UInt32`/`uint`](https://docs.microsoft.com/en-us/dotnet/api/system.uint32),
11. [`Int32`/`int`](https://docs.microsoft.com/en-us/dotnet/api/system.int32),
12. [`UInt64`/`ulong`](https://docs.microsoft.com/en-us/dotnet/api/system.uint64),
13. [`Int64`/`long`](https://docs.microsoft.com/en-us/dotnet/api/system.int64),
14. [`Float32`/`float`](https://docs.microsoft.com/en-us/dotnet/api/system.single),
15. [`Float64`/`double`](https://docs.microsoft.com/en-us/dotnet/api/system.double),
16. [`DateOnly`](https://docs.microsoft.com/en-us/dotnet/api/system.dateonly),
17. [`TimeOnly`](https://docs.microsoft.com/en-us/dotnet/api/system.timeonly),
18. [`DateTimeOffset`](https://docs.microsoft.com/en-us/dotnet/api/system.datetimeoffset),
19. [`TimeSpan`](https://docs.microsoft.com/en-us/dotnet/api/system.timespan),
20. [`Array`/`List`](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1),
21. [`Map`/`Dictionary`](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2),
22. [`Attribute`](https://docs.microsoft.com/en-us/dotnet/api/system.attribute) (will only occur as a base for other types).

## Custom types

1. [`Query`](../src/LeanCode.Contracts/IQuery.cs),
2. [`Command`](../src/LeanCode.Contracts/ICommand.cs),
3. [`CommandResult`](../src/LeanCode.Contracts/CommandResult.cs),
4. [`Operation`](../src/LeanCode.Contracts/IOperation.cs),
5. [`Binary`](../src/LeanCode.Contracts/Binary.cs),
6. [`AuthorizeWhenAttribute`](../src/LeanCode.Contracts/Security/AuthorizeWhenAttribute.cs)
7. [`AuthorizeWhenHasAnyOfAttribute`](../src/LeanCode.Contracts/Security/AuthorizeWhenHasAnyOfAttribute.cs)
