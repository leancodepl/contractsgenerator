# Serialization

Contracts use JSON format on the transport layer.

All three kinds of types (`TypeRef`) have defined serialization formats. Regardless of kind, a `TypeRef` with
`nullable = true` is serialized to a JSON `null`.

## `TypeRef::Generic`

Never serialized. Final structures need to have resolved generics to a statically known type.

## `TypeRef::Internal`

These are always composite objects and thus are serialized to a JSON object. Key being the property name and value being
the serialized value.

## `TypeRef::Known`

### `KnownType::Object`

Serialized to an empty JSON object.

### `KnownType::String`

Serialized to a JSON string.

### `KnownType::Guid`

Serialized to a JSON string with the [UUID](https://datatracker.ietf.org/doc/html/rfc4122) format.

### `KnownType::Uri`

Serialized to a JSON string following [RFC 3986](https://datatracker.ietf.org/doc/html/rfc3986).

### `KnownType::Boolean`

Serialized to a JSON boolean.

### `KnownType::UInt8`, `KnownType::Int8`, `KnownType::Int16`, `KnownType::UInt16`, `KnownType::Int32`, `KnownType::UInt32`, `KnownType::Int64`, `KnownType::UInt64`

Serialized to a JSON number with respected signedness, integer bit size, and no decimal part.

### `KnownType::Float32`, `KnownType::Float64`

Serialized to a JSON number with respected floating point bit size.

### `KnownType::DateOnly`

Serialized to a JSON string with the date format `yyyy-MM-dd`.

### `KnownType::TimeOnly`

Serialized to a JSON string with the time format `hh:mm:ss.SSS` (1ms precision).

### `KnownType::DateTimeOffset`

Serialized to a JSON string with the format `yyyy'-'MM'-'dd'T'hh':'mm':'ss.SSSZ` (where `Z` is offset specified as
`(±hh:mm|'Z')`).

### `KnownType::TimeSpan`

Serialized to a JSON string with the format `dd.hh:mm:ss.SSS` with an optional leading `-` (1ms precision).

TODO: how many days?

### `KnownType::Array`

Serialized to a JSON array where values are consequent serialized elements.

### `KnownType::Map`

Serialized to a JSON object. Keys type is valid only if it has a trivial string representation.The following types have
a trivial string representation:

- `KnownType::String`
- `KnownType::Guid`
- `KnownType::Uri`
- `KnownType::Boolean`
- `KnownType::UInt8`, `KnownType::Int8`, `KnownType::Int16`, `KnownType::UInt16`, `KnownType::Int32`,
  `KnownType::UInt32`, `KnownType::Int64`, `KnownType::UInt64`, `KnownType::Float32`, `KnownType::Float64`

If some of the aforementioned types do not serialize to a JSON string directly, their JSON value is wrapped with quotes
`"` to form a JSON string.  Values are serialized recursively.

### `KnownType::Query`, `KnownType::Command`, `KnownType::Operation`

Not serializable. It is assumed that CQRS methods do not end up in payloads.

### `KnownType::CommandResult`

Serialized to a JSON object with the following fields:

- `WasSuccessful` with a JSON boolean as value
- `ValidationErrors` with a JSON array of validation errors as value. Validation error is a JSON object with the
  following fields:
  - `ErrorCode` with a JSON number (integer) as value
  - `ErrorMessage` with a JSON string as value
  - `PropertyName` with a JSON string as value

### `KnownType::Binary`

Serialized to a JSON string as base64 encoded bytes.

### `KnownType::Attribute`, `KnownType::AuthorizeWhenAttribute`, `KnownType::AuthorizeWhenHasAnyOfAttribute`, `KnownType::QueryCacheAttribute`

Not serializable. It is assumed that attributes do not end up in payloads.
