using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ShopOnline.Api.Security
{
    public class TokenParameters
    {
        public string Issuer => "Buyer";
        public string Audience => "ShopOnline";
        const string KEY = "401b09eDEb3c013d4ca54922bb802bec8FVd5318192b0WWW75f201d8b3727";

        public DateTime Expiry => DateTime.Now.AddDays(7);

        public SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }
    }
}
