using System.Net;

namespace MAOToolkit.Utilities.HttpClientHandlers;

public class RetryHandler : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.TooManyRequests
            && response.Headers.RetryAfter is not null)
        {
            var delta = TimeSpan.Zero;
            if (response.Headers.RetryAfter.Date.HasValue)
            {
                delta = response.Headers.RetryAfter.Date.Value - DateTimeOffset.Now;
            }

            if (delta <= TimeSpan.Zero && response.Headers.RetryAfter.Delta.HasValue)
            {
                delta = response.Headers.RetryAfter.Delta.Value;
            }

            if (delta > TimeSpan.Zero)
            {
                await Task.Delay(delta, cancellationToken);

                var newRequest = CloneRequest(request);
                response = await base.SendAsync(newRequest, cancellationToken);
            }
        }

        return response;
    }
    
    private static HttpRequestMessage CloneRequest(HttpRequestMessage request)
    {
        var clone = new HttpRequestMessage(request.Method, request.RequestUri)
        {
            Version = request.Version,
            VersionPolicy = request.VersionPolicy,
            Content = request.Content
        };
        
        foreach (var header in request.Headers)
            clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
        
        foreach (var option in request.Options)
            clone.Options.Set(new HttpRequestOptionsKey<object?>(option.Key), option.Value);

        return clone;
    }
}