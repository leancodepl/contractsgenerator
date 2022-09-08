# Dates in contracts

The contracts provide 4 date/datetime-like known types:

1. `DateOnly` - represents a _date_ **without** time, e.g. 2022-09-08,
2. `TimeOnly` - represents a time of day, e.g. 16:02:20. It is limited to 23:59:59.999,
3. `DateTimeOffset` - represents date with time and offset (e.g. event in a time and space), e.g. 2022-09-08 16:02:20 +02:00,
4. `TimeSpan` - represents a duration of time, e.g. 2 days, 10 hours, 50 min, 30 sec and 222 ms.

Previously, there was also a `DateTime` type but it got removed in v2.0 and is prohibited in contracts.

## Why `DateTime` got removed

`DateTime` represented date and time in UTC. This case is also handled by `DateTimeOffset` with `Offset = 00:00`, so
having `DateTime` introduced redundancy. There is also a problem with the `DateTime` implementation in supported
runtimes. Both .NET ([`DateTime` struct](https://docs.microsoft.com/en-us/dotnet/api/system.datetime)) and Dart
([`DateTime` class](https://api.dart.dev/stable/2.18.0/dart-core/DateTime-class.html)) provide types that allow handling
of the `DateTime` contracts type **but** this is not a one-to-one translation.

Both types allow _kind_ in dates - they can represent both UTC and user-local timezone. This introduces ambiguity in the
communication: what date will be sent by both parties as `DateTime`? Although contracts' `DateTime` was meant to be UTC,
you could abuse it to represent a vaguely formalized notion of "datetime in user timezone". Unfortunately, this
assumption, when not enough care is taken, results in very hard to spot bugs. It is better to be explicit in this regard.

If you need to represent a date-and-time in a vague "user timezone" (e.g. event date 10 years in future during which
timezones can change), go with separate `DateOnly` date and `TimeOnly` time. This describes the intention better and is
not prone to not-fully-precise serialization/deserialization problems.
