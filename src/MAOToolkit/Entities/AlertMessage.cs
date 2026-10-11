namespace MAOToolkit.Entities;

/// <summary>
/// Represents a typed alert message.
/// </summary>
public readonly record struct AlertMessage
{
    /// <summary>
    /// Specifies the alert message type.
    /// </summary>
    public enum Kind
    {
        Success,
        Info,
        Warning,
        Danger
    }

    /// <summary>
    /// Gets the alert message type.
    /// </summary>
    public Kind Type { get; init; }

    /// <summary>
    /// Gets the alert message text.
    /// </summary>
    public string Text { get; init; }
}
