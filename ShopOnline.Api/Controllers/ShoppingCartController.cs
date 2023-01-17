using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopOnline.Api.Extensions;
using ShopOnline.Api.Repositories.Contracts;
using ShopOnline.Api.Security;
using ShopOnline.Models.Dtos;
using System.IdentityModel.Tokens.Jwt;
using System.IO;

namespace ShopOnline.Api.Controllers
{
    [Authorize(Policy = "CustomerOnly")]
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingCartController : ControllerBase
    {
        private readonly IShoppingCartRepository shoppingCartRepository;
        private readonly IProductsRepository productsRepository;

        public ShoppingCartController(IShoppingCartRepository shoppingCartRepository,
                                      IProductsRepository productsRepository)
        {
            this.shoppingCartRepository = shoppingCartRepository;
            this.productsRepository = productsRepository;
        }

        [HttpGet]
        [Route("GetItems")]
        public async Task<ActionResult<IEnumerable<CartItemDto>>> GetItems()
        {
            try
            {
                if(!Request.Headers.TryGetValue("Authorization", out var bearerToken))
                {
                    return StatusCode(StatusCodes.Status511NetworkAuthenticationRequired);
                }

                int customerId= await JwtDecoder.JwtDecode(bearerToken);

                var cartItems = await shoppingCartRepository.GetItems(customerId);
                if (cartItems == null)
                {
                    return NoContent();
                }

                var products = await productsRepository.GetItems();
                if (products == null)
                {
                    throw new Exception("No products exist in the system");
                }

                var cartItemsDto = cartItems.ConvertToDto(products);

                return Ok(cartItemsDto);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<CartItemDto>> GetItem(int id)
        {
            try
            {
                var cartItem = await shoppingCartRepository.GetItem(id);

                if (cartItem == null)
                {
                    return NoContent();
                }

                var product = await productsRepository.GetItem(cartItem.ProductId);

                if (product == null)
                {
                    return NotFound();
                }

                var cartItemDto = cartItem.ConvertToDto(product);

                return Ok(cartItemDto);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }


        }

        [HttpPost]
        public async Task<ActionResult<CartItemDto>> Postitem([FromBody] CartItemToAddDto cartItemToAddDto)
        {
            try
            {
                if (!Request.Headers.TryGetValue("Authorization", out var bearerToken))
                {
                    return StatusCode(StatusCodes.Status511NetworkAuthenticationRequired);
                }

                int customerId = await JwtDecoder.JwtDecode(bearerToken);

                cartItemToAddDto.CartId = customerId;

                var newCartItem = await shoppingCartRepository.AddItem(cartItemToAddDto);

                if (newCartItem == null)
                {
                    return NoContent();
                }

                var product = await productsRepository.GetItem(newCartItem.ProductId);

                if (product == null)
                {
                    throw new Exception($"Something wet wrong when attempting to retrieve product(productId:{cartItemToAddDto.ProductId})");
                }

                var newCartItemDto = newCartItem.ConvertToDto(product);

                return CreatedAtAction(nameof(GetItem), new { id = newCartItemDto.Id }, newCartItemDto);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<CartItemDto>> DeleteItem(int id)
        {
            try
            {
                var cartItem = await shoppingCartRepository.DeleteItem(id);

                if(cartItem == null)
                {
                    return NotFound();
                }

                var product =await productsRepository.GetItem(cartItem.ProductId);
                
                if (product == null)
                {
                    return NotFound();
                }

                var cartItemDto = cartItem.ConvertToDto(product);

                return Ok(cartItemDto);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPatch("{id:int}")]
        public async Task<ActionResult<CartItemDto>> UpdateQty(int id, CartItemQtyUpdateDto cartItemQtyUpdateDto)
        {
            try
            {
                var cartItem = await shoppingCartRepository.UpdateQty(id, cartItemQtyUpdateDto);
                if(cartItem == null)
                {
                    return NotFound();
                }

                var product = await productsRepository.GetItem(cartItem.ProductId);

                var cartItemDto = cartItem.ConvertToDto(product);

                return Ok(cartItemDto);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }


        }

    }
}
