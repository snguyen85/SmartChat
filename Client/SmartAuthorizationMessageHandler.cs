using Blazored.LocalStorage;
using System.Net.Http.Headers;

namespace SmartChat.Client
{
    public class SmartAuthorizationMessageHandler : DelegatingHandler
    {
        private readonly ILocalStorageService _localStorage;

        public SmartAuthorizationMessageHandler(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var hasToken = await _localStorage.ContainKeyAsync("authToken");

            if (hasToken)
            {
                var token = await _localStorage.GetItemAsync<string>("authToken");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                request.Headers.Authorization = null;
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
