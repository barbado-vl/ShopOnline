using Microsoft.AspNetCore.Identity;

namespace ShopOnline.Api.Entities
{
    public class User : IdentityUser
    {
        public string UserRole { get; set; }
    }
}
