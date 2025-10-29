using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;

namespace BLCTWeb
{
    // Simple handler that attaches the antiforgery request token from the current HttpContext
    // to outgoing HttpClient requests so server-rendered components can call your API endpoints.
    public sealed class AntiforgeryMessageHandler : DelegatingHandler
    {
        private readonly IAntiforgery _antiforgery;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AntiforgeryMessageHandler(IAntiforgery antiforgery, IHttpContextAccessor httpContextAccessor)
        {
            _antiforgery = antiforgery;
            _httpContextAccessor = httpContextAccessor;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var context = _httpContextAccessor.HttpContext;
            if (context != null)
            {
                // Get (and if necessary store) the tokens for the current context
                var tokens = _antiforgery.GetAndStoreTokens(context);
                if (!string.IsNullOrEmpty(tokens.RequestToken))
                {
                    // Header name "RequestVerificationToken" matches default antiforgery header name used by MVC/Blazor templates.
                    // Adjust if your server expects a different header.
                    request.Headers.Remove("RequestVerificationToken");
                    request.Headers.Add("RequestVerificationToken", tokens.RequestToken);
                }
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}