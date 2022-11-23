using ShopOnline.Api.Entities;
using System.Collections;

namespace ShopOnline.Api.Repositories.Contracts
{
    public interface IProductsRepository
    {
        Task<IEnumerable<Product>> GetItems();
        Task<IEnumerable<ProductCategory>> GetCategories();
        Task<Product> GetItem(int id);
        Task<ProductCategory> GetCategory(int id);
    }
}
