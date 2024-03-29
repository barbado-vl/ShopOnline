﻿using ShopOnline.Models.Dtos;

namespace ShopOnline.Web.Services.Contracts
{
    public interface IShoppingCartService
    {
        Task<List<CartItemDto>> GetItems();

        Task<CartItemDto> AddItem(CartItemToAddDto cartItemToAddDto);

        Task<CartItemDto> DeleteItem(int id);

        Task<CartItemDto> UpdateItem(CartItemQtyUpdateDto cartItemQtyUpdateDto);


        event Action<int> OnShoppingCartChanged;

        void RaiseEventOnShoppingCartChanged(int totalQty);
    }
}
