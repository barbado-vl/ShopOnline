using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;

namespace ShopOnline.Web
{
    public class PageHistoryState
    {
        public string PreviousPage { get; set; }


        public PageHistoryState()
        {
            PreviousPage = "/";
        }


        public void SetBackPage(string page)
        {
            if (page != "/login" && page != "/signup")
            {
                PreviousPage = page;
            }
        }
    }
}
