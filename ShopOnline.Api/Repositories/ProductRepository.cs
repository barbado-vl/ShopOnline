﻿using Microsoft.EntityFrameworkCore;
using ShopOnline.Api.Data;
using ShopOnline.Api.Entities;
using ShopOnline.Api.Repositories.Contracts;

namespace ShopOnline.Api.Repositories
{
    public class ProductRepository : IProductsRepository
    {
        private readonly ShopOnlineDbContext shopOnlineDbContext;

        public ProductRepository(ShopOnlineDbContext shopOnlineDbContext)
        {
            this.shopOnlineDbContext = shopOnlineDbContext;
        }


        public async Task<ProductCategory> GetCategory(int id)
        {
            var category = await shopOnlineDbContext.ProductCategories.SingleOrDefaultAsync(c => c.Id == id);
            return category;
        }

        public async Task<IEnumerable<ProductCategory>> GetCategories()
        {
            var categories = await shopOnlineDbContext.ProductCategories.ToListAsync();

            return categories;
        }


        public async Task<Product> GetItem(int id)
        {
            var product = await shopOnlineDbContext.Products.FindAsync(id);
            return product;
        }

        public async Task<IEnumerable<Product>> GetItems()
        {
            var products = await shopOnlineDbContext.Products.ToListAsync();

            return products;
        }

        
    }
}