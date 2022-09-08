# Tutorial

This tutorial will show you how to write your first command, query and operation.

## Basics

Contracts are written in C#. You can use any C# feature, provided that the feature does not introduce any "logic". Any
non-structural code in contracts will be either rejected or ignored. You can find more about guidelines what features
can be used [here](./guidelines.md).

Contracts are based on 5 basic types:

1. DTOs (Data Transfer Objects),
2. Enums (also a form of Data Transfer Objects),
3. Queries,
4. Commands, and
5. Operations.

The first two (DTOs & Enums) represent plain data. The last three (Queries, Commands & Operations) represent API endpoints
that you can call. DTOs & enums don't have any particular semantic apart from being data, but queries, commands and
operations carry a semantic meaning that will be described later.

### A note on inheritance

Although C# allows inheriting from base classes and implementing interfaces, contracts have only one notion: extension.
This differs from C# in that you can _extend_ multiple classes/interfaces. Extension also means that the resulting type
is sum of all fields of base types but the resulting type has no (direct) correlation to the base type. This follows
the duck-typing pattern of TypeScript and other dynamically typed languages.

### Attributes

Every statement in contracts can have attributes. Attributes are used to pass some additional metadata that might be
used by either client or server implementation to augment the behavior of server/client. They don't have meaning at
contracts level.

A common use case for attributes is:

1. Deprecation with [`Obsolete`](https://docs.microsoft.com/en-us/dotnet/api/system.obsoleteattribute) attribute,
1. Authorization with a [`AuthorizeWhen`](../src/LeanCode.Contracts/Security/AuthorizeWhenAttribute.cs) or any of the
   derived classes.

## DTOs

DTOs are plain C# classes that might extend other classes or implement interfaces. Although C# has interfaces, classes,
structs and records, contracts have only one type: DTO.

DTO is used to pass data to or return data from the backend. It carries no other meaning. You can use almost all
[known types](./types.md) (with exceptions) or types that are defined by your project or referenced project (provided
that the referenced project carries the contracts payload) as properties.

A DTO can also define any number of constants, it can extend any DTO class and implement any number of interfaces. It
can also have any number of attributes assigned (they don't carry meaning on contracts level but might be used by
client or server implementations).

Here is an example DTO:

```csharp

// This will result in a DTO `IInterfaceDTO` with a single string property - `A`.
public interface IInterfaceDTO
{
    public string A { get; set; }
}

// This will result in a DTO with one property - `B` - that extends `IInterfaceDTO` type.
// The `abstract` modifier will be ignored by the generator.
public abstract class BaseDTO : IInterfaceDTO
{
    // Although it is implemented here, it will not be considered internal
    // `BaseDTO` property - it will come from extension from `IInterfaceDTO`
    public string A { get; set; }

    public int B { get; set; }
}

// This will result in a DTO that extends `BaseDTO` with one property - `C` -  and a constant `MyConstant`.
public class MyDTO : BaseDTO
{
    public const int MyConstant = 100;

    public Guid C { get; set; }
}
```

## Enums

Enumerations are C#'s `enum`s. They are translated mostly as-is, but members are always `int64`.

You can define enum like this:

```csharp
public enum MyEnumDTO
{
    Value1 = 1,
    Value2 = 20,
    Value3 = 30,
}
```

## Queries

Queries are **the** objects that allow you to get some data. Queries are DTOs marked with a special attribute: `IQuery`.
To define a query you define a normal DTO and you implement `IQuery<TReturnType>` marker attribute that specified return
type of the query. `TReturnType` will be used by server and client implementations to strongly-type the result of a
query. `TReturnType` type should be a proper DTO (but can also be an enum).

Queries should be named to fill in the gap in `get ...` sentence. For example `get my profile` - the query should be
named `MyProfile`.

```csharp
public class ProfileDTO
{
    public string FullName { get; set; }
}

public class MyProfile : IQuery<ProfileDTO>
{ }
```

or `get order by id`

```csharp
public class OrderDTO
{
    public Guid Id { get; set; }
    public long Amount { get; set; }
}

public class OrderById : IQuery<OrderDTO>
{
    public Guid Id { get; set; }
}
```

### Commands

Commands are **the** objects that allow to change the state of the server. Commands are (might be) validated and then
executed by the server. They don't return any data (apart from validation result) and are executed only if the
validation succeeds. Otherwise, the client gets an error.

To define a command, you define proper DTO and then mark it with `ICommand` attribute. Since commands do not return any
result, the interface is not generic - you don't specify return type, contrary to queries.

Commands should be named as imperative, i.e. `rate order` or `upload photo`.

Command validation is supported directly by the contracts. The way validation is reported is by the usage of error codes.
The codes are specified as a part of the command class, in a special inner class named `ErrorCodes`. An error code is
basically a numeric constant that server can use the report the error and client can use to distinguish it from different
errors.

```csharp
public class RateOrder : ICommand
{
    public Guid Id { get; set; }
    public int Rating { get; set; }

    // The static here is ignored by the contracts. Only the constants in `ErrorCodes` class are used.
    public static class ErrorCodes
    {
        public const OrderDoesNotExist = 1;
        public const OrderAlreadyRated = 2;
        public const RatingIsOutOfRange = 3;
    }
}
```

### Operations

Operations are an exception to the CQRS rule. Sometimes there are cases (esp. when integrating with external services)
where you can't split _reading_ the data from _changing_ it. You can think of operations as of commands that return
results. Operations are not validated, contrary to commands (you need to model validation yourself).

A common example of an operation is payment - most of the payment processors have "pay" action that both does the payment
and returns some metadata about the payment.

```csharp
public class PaymentResultDTO
{
    public long? Amount { get; set; }
    public string? TransactionId { get; set; }

    public bool WasSuccessful { get; set; }
}

public class PayForOrder : IOperation<PaymentResultDTO>
{
    public Guid OrderId { get; set; }
    public string PaymentToken { get; set; }
}
```
