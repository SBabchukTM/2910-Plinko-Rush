using System.Collections.Generic;

namespace Runtime.Application.ShopSystem
{
    public interface IShopItemsStorage
    {
        void AddItemDisplay(ShopItemDisplayView shopItemDisplay);

        List<ShopItemDisplayView> GetItemDisplay();

        void SetShopStateConfigs(List<ShopItemStateConfig> shopConfigShopItemStateConfigs);

        ShopItemStateConfig GetItemStateConfig(ShopItemState shopItemState);

        void Cleanup();
    }
}