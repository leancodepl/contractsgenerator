# Guidelines

## General

### Properties should have public getters and setters

The properties are generated as-is, even if they don't have getter/setter or if getter/setter is not public. Using
anything else results in C#/contracts mismatch because client types _will_ have public getters and setters.

### Prefer `List` over array

### Prefer concrete types instead of interfaces

For example use `List` instead of `IList` or `IReadOnlyList`. Contracts need to be concrete and interfaces introduce
ambiguity. This causes problems not only in understanding, but also in (de-)serialization.

### Explicit interface implementation is discouraged

Explicit interface implementation allows to have a property name clash which will result in badly formed contracts.
This is currently not blocked, but might be in the future.

### Prefer Command + Query over Operation

Unless it is impossible to model as a two separate actions or the resulting API would be awkward.

### Prefer globally unique error codes in nested DTOs

2 DTOs can be used by 1 command in the future. In that case those DTOs will have to have unique error codes. Changing error codes values is a breaking change. To omit breaking change, it is better to have globally unique error codes in nested DTOs.

## DTOs & Enums

### Postfix DTOs & enums with `DTO`

Makes distinguishing the purpose easier.

### Always explicitly specify enum value

Makes maintaining compatibility easier and results in predictable contracts.

## Queries

### Prefer DTOs as a query results

DTO is named and has named properties, which makes understanding of the API easier. Primitive types lack that.
