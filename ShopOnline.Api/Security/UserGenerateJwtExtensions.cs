using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using ShopOnline.Api.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ShopOnline.Api.Security
{
    public static class UserGenerateJwtExtensions
    {
        private static TokenParameters tokenParameters => new();

        public static async Task<string> GenerateJwt(this User user, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                new(ClaimsIdentity.DefaultNameClaimType, user.UserName),
                new(ClaimTypes.NameIdentifier, user.Id),
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

            var key = tokenParameters.GetSymmetricSecurityKey();
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                tokenParameters.Issuer,
                tokenParameters.Audience,
                claims,
                expires: tokenParameters.Expiry,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
