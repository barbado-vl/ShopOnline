using Microsoft.AspNetCore.Components;
using ShopOnline.Web.Security;

namespace ShopOnline.Web.Pages
{
    public class SignUpBase : ComponentBase
    {
        [Inject]
        public AuthenticateService AuthenticateService { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }

        public string AccountName { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

        public string Message { get; set; }


        protected async Task SignUp_Click()
        {
            try
            {
                if (string.IsNullOrEmpty(AccountName) || string.IsNullOrEmpty(Password) || string.IsNullOrEmpty(ConfirmPassword))
                {
                    Message = "empty field account name or password or confirm password";
                }
                else if (Password != ConfirmPassword)
                {
                    Message = "password and confirm password do not match";
                }
                else
                {
                    if (await AuthenticateService.Register(AccountName, Password))
                    {
                        NavigationManager.NavigateTo("/");
                    }
                    else
                    {
                        Message = "invalid account name or password";
                        throw new Exception(Message);
                    }
                }
            }
            catch (Exception ex)
            {
                Message = ex.Message;
            }
        }
    }
}
