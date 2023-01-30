namespace MAOToolkit.Entities
{
    public readonly record struct AlertMessage
    {
        public enum Kind
        {
            success,
            info,
            warning,
            danger
        }

        public Kind Type { get; init; }

        public string Text { get; init; }
    }
}