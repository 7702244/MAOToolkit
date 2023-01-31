using MAOToolkit.Extensions;

namespace MAOToolkit.Utilities.HttpClientHandlers
{
    public class LoggingHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            try
            {
                // base.SendAsync calls the inner handler.
                return await base.SendAsync(request, cancellationToken);
            }
            catch (Exception ex)
            {
                ex.Data["HttpClientRequest"] = await request.ToRawAsync();

                throw;
            }
        }
    }
}