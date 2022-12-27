using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ShopOnline.Api.Entities;
using ShopOnline.Api.Repositories;
using ShopOnline.Api.Security;
using ShopOnline.Models.Dtos;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ShopOnline.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthenticateJwtController : ControllerBase
    {
        private readonly UserManager<User> UserManager;
        private readonly RoleManager<IdentityRole> RoleManager;

        private static TokenParameters TokenParameters => new();

        public AuthenticateJwtController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.UserManager = userManager;
            this.RoleManager = roleManager;
        }


        [Route("login")]
        [HttpPost()]
        public async Task<IActionResult> Login([FromBody] UserDto login)
        {
            try
            {
                var user = await UserManager.FindByNameAsync(login.UserName);

                if (user == null)
                {
                    return NoContent();
                }

                var isValidPassword = await UserManager.CheckPasswordAsync(user, login.Password);

                if (!isValidPassword)
                {
                    throw new Exception($"Invalid password for {user.UserName}");
                }

                string accessToken = await GenerateJwtToken(user);

                if (string.IsNullOrEmpty(accessToken))
                {
                    throw new Exception($"Token for {user.UserName} was not generate");
                }

                return Ok(new
                {
                    token = accessToken,
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [Route("Register")]
        [HttpPost()]
        public async Task<IActionResult> Register([FromBody] UserDto login)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(login.UserName))
                {
                    throw new Exception($"An empty string or a name with spaces is not allowed");
                }

                User user = new()
                {
                    UserName = login.UserName,
                };

                var result = await UserManager.CreateAsync(user, login.Password);

                if (result.Succeeded)
                {
                    var userIdentity = await UserManager.FindByNameAsync(user.UserName);

                    string accessToken = await GenerateJwtToken(user);

                    if (string.IsNullOrEmpty(accessToken))
                    {
                        throw new Exception($"Token for {user.UserName} was not generate");
                    }

                    return Ok(new
                    {
                        token = accessToken,
                    });
                }
                else
                {
                    throw new Exception($"Problem with registration, server can't complete the task");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


        private async Task<string> GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                new(ClaimsIdentity.DefaultNameClaimType, user.UserName),
                new(ClaimTypes.NameIdentifier, user.Id),
            };

            var userRoles = await UserManager.GetRolesAsync(user);

            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole));
                var role = await RoleManager.FindByNameAsync(userRole);

                if (role != null)
                {
                    var roleClaims = await RoleManager.GetClaimsAsync(role);

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
    }
}
