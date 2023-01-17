using Microsoft.Extensions.Primitives;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ShopOnline.Api.Security
{
    public static class JwtDecoder
    {
        public static async Task<int> JwtDecode(StringValues bearertoken)
        {
            var token = bearertoken.ToString().Split(' ').Last();

            var jWTHandler = new JwtSecurityTokenHandler();
            var jsonToken = jWTHandler.ReadToken(token);
            var jwtSecurityTokenens = jsonToken as JwtSecurityToken;

            var userClaims = jwtSecurityTokenens.Claims.ToList();

            return int.Parse(userClaims.Find(c => c.Type == "CustomerId").Value);
        }
    }
}
