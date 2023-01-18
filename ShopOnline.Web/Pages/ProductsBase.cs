using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using ShopOnline.Models.Dtos;
using ShopOnline.Web.Services.Contracts;

namespace ShopOnline.Web.Pages
{
    public class ProductsBase : ComponentBase
    {
        [Inject]
        public IProductService ProductService { get; set; }

        [Inject]
        public IShoppingCartService ShoppingCartService { get; set; }

        [Inject]
        public IManageProductsLocalStorageService ManageProductsLocalStorageService { get; set; }

        public IEnumerable<ProductDto> Products { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }
        [Inject]
        public PageHistoryState PageHistoryState { get; set; }

        public string ErrorMessage { get; set; }

        [CascadingParameter]
        private Task<AuthenticationState> AuthenticationStateTask { get; set; }


        protected override async Task OnInitializedAsync()
        {
            try
            {
                await ClearLocalStorage();

                Products = await ManageProductsLocalStorageService.GetCollection();

                if (await UserAuthorised())
                {
                    var shoppingCartItems = await ShoppingCartService.GetItems();

                    var totalQty = shoppingCartItems.Sum(i => i.Qty);

                    ShoppingCartService.RaiseEventOnShoppingCartChanged(totalQty);
                }
                else
                {
                    PageHistoryState.SetBackPage(NavigationManager.Uri);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;

            }
        }


        protected IOrderedEnumerable<IGrouping<int, ProductDto>> GetGroupedProductsByCategory()
        {
            return from product in Products
                   group product by product.CategoryId into prodByCatGroup
                   orderby prodByCatGroup.Key
                   select prodByCatGroup;
        }

        protected string GetCategoryName(IGrouping<int, ProductDto> groupedProductDtos)
        {
            return groupedProductDtos.FirstOrDefault(pg => pg.CategoryId == groupedProductDtos.Key).CategoryName;
        }

        private async Task ClearLocalStorage()
        {
            await ManageProductsLocalStorageService.RemoveCollection();
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
