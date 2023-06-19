namespace LeanCode.Contracts;

/// <summary>
/// Marker interface, do not use directly.
/// </summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage("?", "CA1040", Justification = "Marker interface.")]
public interface INotification { }

/// <summary>
/// Marker interface, do not use directly.
/// </summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage("?", "CA1040", Justification = "Marker interface.")]
public interface IProduceNotification<out TNotification>
    where TNotification : INotification
{ }
