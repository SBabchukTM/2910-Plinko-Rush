using System.Linq;
using Application.Services.UserData;
using Runtime.Application.Services.Shop;

namespace Runtime.Application.ShopSystem
{
    public class ShopService : ISetShopSetup
    {
        private readonly IUserInventoryService _userInventoryService;

        private ShopSetup _shopSetup;

        public ShopService(IUserInventoryService userInventoryService) =>
                _userInventoryService = userInventoryService;

        public void SetShopSetup(ShopSetup shopSetup) =>
                _shopSetup = shopSetup;

        public void PurchaseShopItem(ShopItemDisplayView shopItemDisplayView)
        {
            _userInventoryService.AddPurchasedGameItemID(shopItemDisplayView.GetShopItemModel().ShopItem.ItemID);
            _userInventoryService.UpdateUsedGameItemID(shopItemDisplayView.GetShopItemModel().ShopItem.ItemID);
            _userInventoryService.AddBalance(-shopItemDisplayView.GetShopItemModel().ShopItem.ItemPrice);
        }

        public bool CanPurchaseItem(ShopItem shopItem) => _userInventoryService.GetBalance() >= shopItem.ItemPrice;

        public bool IsPurchased(ShopItem shopItem) => _userInventoryService.GetPurchasedGameItemsIDs().Contains(shopItem.ItemID);

        public bool IsSelected(ShopItem shopItem) =>
                _userInventoryService.GetUsedGameItemID() == shopItem.ItemID;
    }
}