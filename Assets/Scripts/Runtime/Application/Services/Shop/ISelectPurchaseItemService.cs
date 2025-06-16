using Runtime.Application.ShopSystem;

namespace Runtime.Application.Services.Shop
{
    public interface ISelectPurchaseItemService : ISetShopSetup
    {
        void SelectPurchasedItem(ShopItemDisplayModel shopItemModel);
    }
}