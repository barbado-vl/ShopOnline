

namespace ShopOnline.Models.Dtos
{
    public class CartItemToAddDto
    {
        public int ProductId { get; set; }
        public int CartId { get; set; }
        public int Qty { get; set; }
    }
}
