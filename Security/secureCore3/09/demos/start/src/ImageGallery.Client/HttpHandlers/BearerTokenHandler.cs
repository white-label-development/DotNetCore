using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ImageGallery.Client.HttpHandlers
{
    public class BearerTokenHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BearerTokenHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ??
                throw new ArgumentNullException(nameof(httpContextAccessor));
        }
        
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, 
            CancellationToken cancellationToken)
        {
            var accessToken = await _httpContextAccessor
                       .HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);

            var refreshToken = await _httpContextAccessor
                       .HttpContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);

            if (!string.IsNullOrWhiteSpace(accessToken))
            {
                request.SetBearerToken(accessToken);
            }

            return await base.SendAsync(request, cancellationToken);
        }         
    }
}
