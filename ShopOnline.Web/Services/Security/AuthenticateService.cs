using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using ShopOnline.Models.Dtos;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace ShopOnline.Web.Services.Security
{
    public class AuthenticateService
    {
        private readonly ILocalStorageService localStorage;

        private readonly IdentityAuthenticationStateProvider authenticationStateProvider;

        private readonly HttpClient httpClient;


        public AuthenticateService(AuthenticationStateProvider authenticationStateProvider, 
                                   ILocalStorageService localStorage,
                                   HttpClient httpClient)
        {
            this.localStorage = localStorage;
            this.authenticationStateProvider = (IdentityAuthenticationStateProvider)authenticationStateProvider;
            this.httpClient = httpClient;
        }


        public async Task<bool> Login(string login, string password)
        {
            try
            {
                UserDto userDto = new()
                {
                    UserName = login,
                    UserPassword = password
                };

                var response = await httpClient.PostAsJsonAsync<UserDto>("api/AuthenticateJwt/Login", userDto);

                if (response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    {
                        return false;
                    }

                    var token = await response.Content.ReadAsStringAsync();

                    await localStorage.SetItemAsync("token", token);
                    
                    authenticationStateProvider.MarkUserAsAuthenticated(token);

                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);

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

        public async Task<bool> Register(string login, string password)
        {
            try
            {
                UserDto userDto = new()
                {
                    UserName = login,
                    UserPassword = password
                };

                var response = await httpClient.PostAsJsonAsync<UserDto>("api/AuthenticateJwt/Register", userDto);

                if (response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    {
                        return false;
                    }

                    var token = await response.Content.ReadAsStringAsync();

                    await localStorage.SetItemAsync("token", token);

                    authenticationStateProvider.MarkUserAsAuthenticated(token);

                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);

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

        public async Task Logout()
        {
            await localStorage.RemoveItemAsync("token");

            await authenticationStateProvider.MarkLogouted();

            httpClient.DefaultRequestHeaders.Authorization = null;
        }
    }
}
