using Microsoft.AspNetCore.Mvc;

namespace MAOToolkit.Utilities.ActionResults
{
    public class NotModifiedResult : StatusCodeResult
    {
        public NotModifiedResult()
            : base(304)
        {
        }

        public override void ExecuteResult(ActionContext context)
        {
            // Explicitly set the Content-Length header so the client doesn't wait for
            // content but keeps the connection open for other requests
            context.HttpContext.Response.Headers.ContentLength = 0;

            base.ExecuteResult(context);
        }

        public override Task ExecuteResultAsync(ActionContext context)
        {
            // Explicitly set the Content-Length header so the client doesn't wait for
            // content but keeps the connection open for other requests
            context.HttpContext.Response.Headers.ContentLength = 0;

            return base.ExecuteResultAsync(context);
        }
    }
}