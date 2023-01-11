using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using System.Security.Claims;

namespace ShopOnline.Web
{
    public class HardCoded
    {
        public const int UserId = 102591;
        public const int CartId = 1;

        [CascadingParameter]
        private static Task<AuthenticationState>? AuthenticationStateTask { get; set; }


        //public static async Task<Guid> GetUserId()
        //{
        //    var authState = await AuthenticationStateTask;
        //    var user = authState.User;

        //    if (user != null && user.Identity.IsAuthenticated)
        //    {
        //        return new Guid(user.FindFirst(ClaimTypes.NameIdentifier).Value);
        //    }

        //    return new Guid();
        //}

    }
}
