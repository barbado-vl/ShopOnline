using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using ShopOnline.Models.Dtos;
using ShopOnline.Web.Services.Contracts;

namespace ShopOnline.Web.Pages
{
    public class CheckoutBase : ComponentBase
    {
        [Inject]
        public IJSRuntime Js { get; set; }

        protected IEnumerable<CartItemDto> ShoppingCartItems { get; set; }

        protected int TotalQty { get; set; }

        protected string PaymentDescription { get; set; }

        protected decimal PaymentAmount { get; set; }

        [Inject]
        public IShoppingCartService ShoppingCartService { get; set; }

        [CascadingParameter]
        private Task<AuthenticationState> AuthenticationStateTask { get; set; }


        protected override async Task OnInitializedAsync()
        {
            try
            {
                if (await UserAuthorised())
                {
                    ShoppingCartItems = await ShoppingCartService.GetItems();

                    if (ShoppingCartItems != null)
                    {
                        Guid orderGuid = Guid.NewGuid();

                        PaymentAmount = ShoppingCartItems.Sum(x => x.TotalPrice);
                        TotalQty = ShoppingCartItems.Sum(x => x.Qty);
                        PaymentDescription = $"O_{1}_{orderGuid}";
                    }
                }
            }
            catch (Exception)
            {
                // Log exception
                throw;
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            try
            {
                if(firstRender)
                {
                    await Js.InvokeVoidAsync("initPayPalButton");
                }
            }
            catch (Exception)
            {
                // Log exception
                throw;
            }
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
