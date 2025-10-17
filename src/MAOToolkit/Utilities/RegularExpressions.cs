using System.Text.RegularExpressions;

namespace MAOToolkit.Utilities;

public static partial class RegularExpressions
{
    public static readonly Regex URL = _url();
    //public static readonly Regex Url = new(@"https?:\/\/(?:www\.)?[-a-zA-Z0-9@:%._\+~#=]{2,256}\.[a-z]{2,4}\b(?:[-a-zA-Z0-9@:%_\+.~#?&//=]*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    public static readonly Regex IPAddress = _ipAddress();
    public static readonly Regex Email = _email();
    public static readonly Regex UnicodeString = _unicodeString();
    public static readonly Regex DoubleWhitespaces = _doubleWhitespaces();
    public static readonly Regex NotDigits = _notDigits();
    public static readonly Regex HexColor = _hexColor();

    [GeneratedRegex("(?:(?:https?|ftp|file):\\/\\/|www\\.|ftp\\.)(?:\\([-A-Z0-9+&@#\\/%=~_|$?!:,.]*\\)|[-A-Zа-я0-9+&@#\\/%=~_|$?!:;,.])*(?:\\([-A-Z0-9+&@#\\/%=~_|$?!:,.]*\\)|[A-Z0-9+&@#\\/%=~_|$])", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex _url();
    [GeneratedRegex("^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$|^(?:[0-9a-fA-F]{1,4}:){7}[0-9a-fA-F]{1,4}$|^::(?:[0-9a-fA-F]{1,4}:){0,6}[0-9a-fA-F]{1,4}$|^[0-9a-fA-F]{1,4}::(?:[0-9a-fA-F]{1,4}:){0,5}[0-9a-fA-F]{1,4}$|^[0-9a-fA-F]{1,4}:[0-9a-fA-F]{1,4}::(?:[0-9a-fA-F]{1,4}:){0,4}[0-9a-fA-F]{1,4}$|^(?:[0-9a-fA-F]{1,4}:){0,2}[0-9a-fA-F]{1,4}::(?:[0-9a-fA-F]{1,4}:){0,3}[0-9a-fA-F]{1,4}$|^(?:[0-9a-fA-F]{1,4}:){0,3}[0-9a-fA-F]{1,4}::(?:[0-9a-fA-F]{1,4}:){0,2}[0-9a-fA-F]{1,4}$|^(?:[0-9a-fA-F]{1,4}:){0,4}[0-9a-fA-F]{1,4}::(?:[0-9a-fA-F]{1,4}:)?[0-9a-fA-F]{1,4}$|^(?:[0-9a-fA-F]{1,4}:){0,5}[0-9a-fA-F]{1,4}::[0-9a-fA-F]{1,4}$|^(?:[0-9a-fA-F]{1,4}:){0,6}[0-9a-fA-F]{1,4}::$", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex _ipAddress();
    [GeneratedRegex("(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|\"(?:[\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x21\\x23-\\x5b\\x5d-\\x7f]|\\\\[\\x01-\\x09\\x0b\\x0c\\x0e-\\x7f])*\")@((?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-z0-9-]*[a-z0-9]:(?:[\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x21-\\x5a\\x53-\\x7f]|\\\\[\\x01-\\x09\\x0b\\x0c\\x0e-\\x7f])+)\\])", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex _email();
    [GeneratedRegex("\\\\[Uu]([0-9A-Fa-f]{4})", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex _unicodeString();
    [GeneratedRegex("[^\\S ]\\s*|\\s{2,}", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex _doubleWhitespaces();
    [GeneratedRegex("[^\\d]", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex _notDigits();
    [GeneratedRegex("^#(?:[0-9a-fA-F]{3}){1,2}$", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex _hexColor();
}