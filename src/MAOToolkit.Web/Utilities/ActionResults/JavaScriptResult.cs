using Microsoft.AspNetCore.Mvc;

namespace MAOToolkit.Utilities.ActionResults
{
    public class JavaScriptResult : ContentResult
    {
        public JavaScriptResult(string content)
        {
            Content = content;
            ContentType = "application/javascript; charset=utf-8";
        }
    }
}