using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using ShopOnline.Models.Dtos;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;

namespace ShopOnline.Web.Services.Security
{
    public class IdentityAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService localStorage;

        private readonly HttpClient httpClient;

        public IdentityAuthenticationStateProvider(ILocalStorageService localStorage, HttpClient httpClient)
        {
            this.localStorage = localStorage;
            this.httpClient = httpClient;
        }


        #region Take AuthenticationState

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await localStorage.GetItemAsync<string>("token");

            if (token != null)
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);

                var authState = JwtDecoder.GetStateFromJwt(token);

                var nameIdentifier = authState.User.FindFirst(ClaimTypes.NameIdentifier).Value;

                if (await IsTokenValid(nameIdentifier))
                {
                    return authState;
                }


            }

            return EmptyToken();
        }

        private async Task<bool> IsTokenValid(string nameIdentifier)
        {
            if (string.IsNullOrEmpty(nameIdentifier))
                return false;

            try
            {
                UserDto userDto = new()
                {
                    UserId = nameIdentifier,
                };

                var response = await httpClient.PostAsJsonAsync<UserDto>("api/AuthenticateJwt/Identify", userDto);

                if (response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    {
                        return false;
                    }

                    return true;
                }
                else
                {
                    var message = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Http status:{response.StatusCode} Message -{message}");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static AuthenticationState EmptyToken() => new(new ClaimsPrincipal(new ClaimsIdentity()));

        #endregion


        #region NotifyAuthenticationStateChanged

        public void MarkUserAsAuthenticated(string token)
        {
            var authState = Task.FromResult(JwtDecoder.GetStateFromJwt(token));
            NotifyAuthenticationStateChanged(authState);
        }

        public async Task MarkLogouted()
        {
            await localStorage.RemoveItemAsync("token");
            NotifyAuthenticationStateChanged(Task.FromResult(EmptyToken()));
        }

        #endregion
    }
}
