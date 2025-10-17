namespace MAOToolkit.Entities;

public readonly record struct AlertMessage
{
    public enum Kind
    {
        Success,
        Info,
        Warning,
        Danger
    }

    public Kind Type { get; init; }

    public string Text { get; init; }
}