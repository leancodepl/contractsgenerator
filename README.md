# LeanCode Contracts Generator V2 PoC

The idea is to decouple parsing/compiling the contracts definitions (C#) from code generation. This means that:

1. .NET tool will only generate and intermediate definition (Protobuf),
2. Target-lang specific tool will generate client code.

This means that the only thing we must agree on (across languages) is the intermediate definition and feature set. :)

You can see a first proposal of the definition in `types.proto` files, along with example contracts definition generated in `example.pb`/`example.json`.

Features considered first-class:

1. Queries, Commands and DTOs (e.g. something that is not a query nor a command),
2. Enums,
3. Generics in types,
4. Type "extensions" (see below),
5. Attributes,
6. Comments on types/properties (maybe more?),
7. Properties & constants,
8. A set of "known" types (e.g. not defined in local contracts - see `KnownTypes` enum).

## Proposed conventions

There are some conventions related to the translation.

### 1 - There are no base types

The contracts do not distinguish between interfaces and classes - these are only types (statements). In contracts, if a type extends other type, it contains all of the properties that the parent type has. Class might extend multiple types. Thus, this works like interfaces in C# (and most other languages) - this preserves the hierarchy, but does not force C# model of types.

So, for example:

```csharp
interface IA { string A { get; set; } }
class ADTO : IA { public string A { get; set; } }
```

and

```csharp
class B { public string A { get; set; } }
class BDTO : B { }
```

results in the same contracts.

This might be problematic in the client-generated code, as this _might_ require multiple inheritance _if_ `B` will be ever constructed (eg. something returns `B` directly).

### 2 - Known types

There is an assumption that we base our contracts on a set of _known types_. These are types that have known semantics and are not directly defined in the contracts (e.g. `string` or `DateTimeOffset`). See `KnownTypes` enum for a list of known types (some of them can be ignored by the clients, like `Attribute`).

### 3 - Attributes

The C# attributes are returned as-is. We don't (yet) try to add meaning there, but clients are free to interpret them as they like.

There is a limitation regarding the attribute parameters - arrays are flattened & can't be nested.

### 4 - Error codes

We treat the error codes as a first class citizen. This is mostly done the way it was designed in TS generator, i.e. `static class ErrorCodes` in a command.

Additionally, we introduce error code sharing - error groups. See `ErrorCode` class on how that is modelled (and also `Export.KnownErrorGroups`).

### 5 - conversion restrictions

1. Not all types that can be constant in C# can be constant here (e.g. arrays).
2. Error codes are `int32`s,
3. `Number`s are `int64`s,
4. There are probably more restrictions that I've missed or introduced them unawarely.

## Usage

It is worth noting that only the .NET part will be shared between target language tools, everything else will be platform-specific. With this approach, we can optimize whatever we like to optimize - being it tool development experience, distribution simplicity or end-user experience.

We have a couple of options:

### Docker container

The easiest one to distribute - we can pack all the tools in a single docker container.

Pros:

* Easy distribution,
* Works everywhere,
* We can develop apps separately and mix-and-match versions.

Cons:

* macOS might be problematic,
* Windows without WSL2 can also be problematic,
* Versioning might be hard,
* Resulting images will be heavy.

### Separate tools

We can distribute every tool separately, leveraging language-specific tooling.

Pros:

* Easy distribution,
* We can develop apps separately and mix-and-match versions.

Cons: 

* Every developer has to have .NET Core installed or at least install the tool separately,
* It might be PITA to match versions correctly, as we must manage two separate tools,

### Embedded

We can go a step further and embed AoT-compiled (or just a single, framework-independent executable)
in the target-language tool.

Pros:

* The target tool controls version of the .NET counterpart,
* End-users don't need to have .NET Core installed,
* They don't need to know that the tool exists (!),
* The usage will boil down to `npx generate` or something like that.

Cons:

* Tool development might be harder, as we have a hard dependency of a specific generator version,
* Lang-dependent package will be heavy, as it needs to embed whole .NET Core runtime (if AoT does not work),
* The packaging might be awkward.
