namespace LeanCode.Contracts;

/// <summary>
/// Marker interface, do not use directly.
/// </summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "?",
    "CA1040",
    Justification = "Marker interface."
)]
public interface IOperation { }

[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "?",
    "CA1040",
    Justification = "Marker interface."
)]
public interface IOperation<out TResult> : IOperation { }
