using Application.Services.Audio;
using Application.Services.UserData;
using Core.Services.Audio;
using Runtime.Application.ShopSystem;
using UnityEngine;

namespace Runtime.Application.Services.Shop
{
    public class SelectPurchaseItemService : ISelectPurchaseItemService
    {
        private readonly IAudioService _audioService;
        private readonly IUserInventoryService _userInventoryService;
        
        private ShopSetup _shopItemStateConfig;
        private ShopItemDisplayModel _shopItem;
        private ShopItemDisplayModel _previousItem;

        public SelectPurchaseItemService(IAudioService audioService, 
            IUserInventoryService userInventoryService)
        {
            _audioService = audioService;
            _userInventoryService = userInventoryService;
        }

        public void SetShopSetup(ShopSetup shopSetup) =>
                _shopItemStateConfig = shopSetup;

        public void SelectPurchasedItem(ShopItemDisplayModel shopItemModel)
        {
            SetItem(shopItemModel);
            UpdateStates();
        }
        
        private void SetItem(ShopItemDisplayModel shopItemModel)
        {
            if(_shopItem != null)
                _previousItem = _shopItem;

            _shopItem = shopItemModel;
            _userInventoryService.UpdateUsedGameItemID(_shopItem.ShopItem.ItemID);
        }

        private void UpdateStates()
        {
            _shopItem?.SetShopItemState(ShopItemState.Selected);
            _previousItem?.SetShopItemState(ShopItemState.Purchased);
        }
    }
}