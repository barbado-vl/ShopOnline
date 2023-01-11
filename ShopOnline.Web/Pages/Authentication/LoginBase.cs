using Microsoft.AspNetCore.Components;
using ShopOnline.Web.Security;

namespace ShopOnline.Web.Pages
{
    public class LoginBase : ComponentBase
    {
        [Inject]
        public AuthenticateService AuthenticateService { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }


        public string AccountName {  get; set; }
        public string Password { get; set; }
        
        public string Message { get; set; }


        protected async Task Login_Click()
        {
            try
            {
                if(!string.IsNullOrEmpty(AccountName) && !string.IsNullOrEmpty(Password))
                {
                    if (await AuthenticateService.Login(AccountName, Password))
                    {
                        NavigationManager.NavigateTo("/");
                    }
                    else
                    {
                        Message = "invalid account name or password";
                        throw new Exception(Message);
                    }

                }
                else
                {
                    Message = "empty field account name or password";
                }
            }
            catch (Exception ex)
            {
                Message = ex.Message.Split('-').Last().ToString();
            }
        }
    }
}
