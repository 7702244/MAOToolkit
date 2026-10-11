using Microsoft.AspNetCore.Mvc;

namespace MAOToolkit.Utilities.ActionResults;

/// <summary>
/// Returns JavaScript content with a JavaScript content type.
/// </summary>
public class JavaScriptResult : ContentResult
{
    /// <summary>
    /// Creates a result containing JavaScript content.
    /// </summary>
    public JavaScriptResult(string content)
    {
        Content = content;
        ContentType = "application/javascript; charset=utf-8";
    }
}
