using Microsoft.AspNetCore.Mvc;

namespace MAOToolkit.Utilities.ActionResults;

/// <summary>
/// Returns an HTTP 304 response with an empty content length.
/// </summary>
public class NotModifiedResult() : StatusCodeResult(304)
{
    /// <summary>
    /// Executes the result and sets an empty content length.
    /// </summary>
    public override void ExecuteResult(ActionContext context)
    {
        // Explicitly set the Content-Length header so the client doesn't wait for
        // content but keeps the connection open for other requests
        context.HttpContext.Response.Headers.ContentLength = 0;

        base.ExecuteResult(context);
    }

    /// <summary>
    /// Executes the result asynchronously and sets an empty content length.
    /// </summary>
    public override Task ExecuteResultAsync(ActionContext context)
    {
        // Explicitly set the Content-Length header so the client doesn't wait for
        // content but keeps the connection open for other requests
        context.HttpContext.Response.Headers.ContentLength = 0;

        return base.ExecuteResultAsync(context);
    }
}
