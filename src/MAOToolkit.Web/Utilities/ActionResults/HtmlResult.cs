using Microsoft.AspNetCore.Mvc;

namespace MAOToolkit.Utilities.ActionResults;

/// <summary>
/// Returns HTML content with an HTML content type.
/// </summary>
public class HtmlResult : ContentResult
{
    /// <summary>
    /// Creates a result containing HTML content.
    /// </summary>
    public HtmlResult(string content)
    {
        Content = content;
        ContentType = "text/html; charset=utf-8";
    }
}
