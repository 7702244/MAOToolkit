using Microsoft.AspNetCore.Mvc;

namespace MAOToolkit.Utilities.ActionResults
{
    public class HtmlResult : ContentResult
    {
        public HtmlResult(string content)
        {
            Content = content;
            ContentType = "text/html; charset=utf-8";
        }
    }
}