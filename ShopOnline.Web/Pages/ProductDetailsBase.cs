using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using ShopOnline.Models.Dtos;
using ShopOnline.Web.Services.Contracts;

namespace ShopOnline.Web.Pages
{
    public class ProductDetailsBase : ComponentBase
    {
        [Parameter]
        public int Id { get; set; }

        [Inject]
        public IProductService ProductService { get; set; }

        [Inject]
        public IShoppingCartService ShoppingCartService { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public IManageProductsLocalStorageService ManageProductsLocalStorageService { get; set; }

        [Inject]
        public IManageCartItemsLocalStorageService ManageCartItemsLocalStorageService { get; set; }

        public ProductDto Product { get; set; }

        public string ErrorMessage { get; set; }

        private List<CartItemDto> ShoppingCartItems { get; set; }

        [CascadingParameter]
        private Task<AuthenticationState> AuthenticationStateTask { get; set; }

        public bool IsUserAuthorized { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                Product = await GetProductById(Id);

                IsUserAuthorized = await UserAuthorised();

                if (IsUserAuthorized)
                {
                    ShoppingCartItems = await ManageCartItemsLocalStorageService.GetCollection();
                }           
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }


        protected async Task AddToCart_Click(CartItemToAddDto cartItemToAddDto)

        {
            try
            {
                var cartItemDto = await ShoppingCartService.AddItem(cartItemToAddDto);

                if (cartItemDto != null)
                {
                    ShoppingCartItems.Add(cartItemDto);
                    await ManageCartItemsLocalStorageService.SaveCollection(ShoppingCartItems);
                }

                NavigationManager.NavigateTo("/ShoppingCart");
            }
            catch (Exception)
            {
                //Log exception
            }
        }

        private async Task<ProductDto> GetProductById(int id)
        {
            var productDtos = await ManageProductsLocalStorageService.GetCollection();

            if (productDtos != null)
            {
                return productDtos.SingleOrDefault(p => p.Id == id);
            }
            return null;
        }

        private async Task<bool> UserAuthorised()
        {
            var authState = await AuthenticationStateTask;
            var user = authState.User;

            if (user != null && user.Identity.IsAuthenticated)
            {
                return true;
            }

            return false;
        }

    }
}
