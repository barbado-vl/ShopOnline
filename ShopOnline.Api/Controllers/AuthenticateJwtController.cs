using Microsoft.AspNetCore.Mvc;
using ShopOnline.Api.Repositories;
using ShopOnline.Api.Repositories.Contracts;
using ShopOnline.Models.Dtos;


namespace ShopOnline.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticateJwtController : ControllerBase
    {
        private readonly UserRepository userRepository;
        private readonly IShoppingCartRepository shoppingCartRepository;

        public AuthenticateJwtController(UserRepository userRepository,
                                         IShoppingCartRepository shoppingCartRepository)
        {
            this.userRepository = userRepository;
            this.shoppingCartRepository = shoppingCartRepository;
        }

        [HttpPost()]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] UserDto login)
        {
            try
            {
                var user = await userRepository.userManager.FindByNameAsync(login.UserName);

                if (user == null)
                {
                    return NoContent();
                }

                if (!await userRepository.userManager.CheckPasswordAsync(user, login.UserPassword))
                {
                    throw new Exception($"invalid password for {user.UserName}");
                }

                string accessToken = await userRepository.GenerateJwt(user);

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
                if (await userRepository.userManager.FindByNameAsync(login.UserName) != null)
                {
                    throw new Exception($"User with such name: {login.UserName} is already exist");
                }

                var user = userRepository.CreateCustomer(login.UserName);

                var result = await userRepository.userManager.CreateAsync(user, login.UserPassword);

                if (result.Succeeded)
                {
                    var userIdentity = await userRepository.userManager.FindByNameAsync(user.UserName);

                    await shoppingCartRepository.CreateCustomerCart(user.CustomerId);

                    string accessToken = await userRepository.GenerateJwt(user);

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
                var user = await userRepository.userManager.FindByIdAsync(login.UserId);

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
