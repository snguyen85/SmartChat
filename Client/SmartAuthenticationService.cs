using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using SmartChat.Shared.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace SmartChat.Client
{
    public class SmartAuthenticationService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly ILocalStorageService _localStorage;

        public SmartAuthenticationService(IHttpClientFactory clientFactory,
                           AuthenticationStateProvider authenticationStateProvider,
                           ILocalStorageService localStorage)
        {
            _clientFactory = clientFactory;
            _authenticationStateProvider = authenticationStateProvider;
            _localStorage = localStorage;
        }

        public async Task<RegisterResult> Register(RegisterModel registerModel)
        {
            var client = _clientFactory.CreateClient("SmartChat.Server");

            var response = await client.PostAsJsonAsync<RegisterModel>("account", registerModel);
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<RegisterResult>(content);

            return result;
        }

        public async Task<LoginResult> Login(LoginModel loginModel)
        {
            var loginAsJson = JsonSerializer.Serialize(loginModel);

            var client = _clientFactory.CreateClient("SmartChat.Server");

            var response = await client.PostAsync("account/login", new StringContent(loginAsJson, Encoding.UTF8, "application/json"));
            var content = await response.Content.ReadAsStringAsync();
            var serializationOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var loginResult = JsonSerializer.Deserialize<LoginResult>(content, serializationOptions);

            if (response.IsSuccessStatusCode)
            {
                await _localStorage.SetItemAsync("authToken", loginResult.Token);
                ((SmartAuthenticationStateProvider)_authenticationStateProvider).AuthenticateUser(loginModel.Username);
            }

            return loginResult;
        }

        public async Task Logout()
        {
            await _localStorage.RemoveItemAsync("authToken");
            ((SmartAuthenticationStateProvider)_authenticationStateProvider).LogUserOut();
        }
    }
}
