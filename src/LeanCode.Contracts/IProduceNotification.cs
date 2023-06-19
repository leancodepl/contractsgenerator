namespace LeanCode.Contracts;

[System.Diagnostics.CodeAnalysis.SuppressMessage("?", "CA1040", Justification = "Marker interface.")]
public interface IProduceNotification<out TNotification>
    where TNotification : notnull { }
