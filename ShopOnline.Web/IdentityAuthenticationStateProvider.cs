using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace ShopOnline.Web
{
    public class IdentityAuthenticationStateProvider : AuthenticationStateProvider
    {
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            return new AuthenticationState(new ClaimsPrincipal());
        }
    }
}
