using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShopOnline.Api.Entities;
using ShopOnline.Api.Security;
using ShopOnline.Models.Dtos;


namespace ShopOnline.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticateJwtController : ControllerBase
    {
        private readonly UserManager<User> UserManager;
        private readonly RoleManager<IdentityRole> RoleManager;


        public AuthenticateJwtController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.UserManager = userManager;
            this.RoleManager = roleManager;
        }


        [HttpPost()]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] UserDto login)
        {
            try
            {
                var user = await UserManager.FindByNameAsync(login.UserName);

                if (user == null)
                {
                    return NoContent();
                }

                var isValidPassword = await UserManager.CheckPasswordAsync(user, login.UserPassword);

                if (!isValidPassword)
                {
                    throw new Exception($"invalid password for {user.UserName}");
                }

                string accessToken = await user.GenerateJwt(UserManager, RoleManager);

                if (string.IsNullOrEmpty(accessToken))
                {
                    throw new Exception($"Token for {user.UserName} was not generate");
                }

                return Ok(accessToken);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost()]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] UserDto login)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(login.UserName))
                {
                    throw new Exception($"An empty string or a name with spaces is not allowed");
                }

                var checkUserName = await UserManager.FindByNameAsync(login.UserName);

                if (checkUserName != null)
                {
                    throw new Exception($"User with such name: {login.UserName} is already exist");
                }

                User user = new()
                {
                    UserName = login.UserName,
                    UserRole = "Customer"
                };

                var result = await UserManager.CreateAsync(user, login.UserPassword);

                if (result.Succeeded)
                {
                    var userIdentity = await UserManager.FindByNameAsync(user.UserName);

                    string accessToken = await user.GenerateJwt(UserManager, RoleManager);

                    if (string.IsNullOrEmpty(accessToken))
                    {
                        throw new Exception($"Token for {user.UserName} was not generate");
                    }

                    return Ok(accessToken);
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

        [HttpPost()]
        [Route("Identify")]
        public async Task<IActionResult> Identify([FromBody] UserDto login)
        {
            try
            {
                var user = await UserManager.FindByIdAsync(login.UserId);

                if (user == null)
                {
                    return NoContent();
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
