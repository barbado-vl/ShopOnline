using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using ShopOnline.Api.Data;
using ShopOnline.Api.Entities;
using ShopOnline.Api.Security;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ShopOnline.Api.Repositories
{
    public class UserRepository
    {
        internal readonly ShopOnlineDbContext shopOnlineDbContext;

        internal readonly UserManager<User> userManager;
        internal readonly RoleManager<IdentityRole> roleManager;

        private static TokenParameters TokenParameters => new();


        public UserRepository(ShopOnlineDbContext shopOnlineDbContext,
                              UserManager<User> userManager,
                              RoleManager<IdentityRole> roleManager)
        {
            this.shopOnlineDbContext = shopOnlineDbContext;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }


        internal async Task<string> GenerateJwt(User user)
        {
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                new(ClaimsIdentity.DefaultNameClaimType, user.UserName),
                new(ClaimTypes.NameIdentifier, user.Id),
                new("CustomerId", user.CustomerId.ToString()),
            };

            if (user.UserRole != null)
                claims.Add(new Claim(ClaimTypes.Role, user.UserRole));

            var userRoles = await userManager.GetRolesAsync(user);

            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole));
                var role = await roleManager.FindByNameAsync(userRole);

                if (role != null)
                {
                    var roleClaims = await roleManager.GetClaimsAsync(role);

                    claims.AddRange(roleClaims);
                }
            }

            var key = TokenParameters.GetSymmetricSecurityKey();
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                TokenParameters.Issuer,
                TokenParameters.Audience,
                claims,
                expires: TokenParameters.Expiry,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        internal User CreateCustomer(string userName)
        {
            return new()
            {
                UserName = userName,
                UserRole = "Customer",
                CustomerId = shopOnlineDbContext.Users.Count() + 1,
            };
        }

    }
}
