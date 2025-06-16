using Core.Factory;
using Runtime.Application.Services.Shop;

namespace Runtime.Application.ShopSystem
{
    public class ShopItemDisplayController 
    {
        private readonly ShopItemDisplayModel _shopItemDisplayModel;
        private readonly GameObjectFactory _gameObjectFactory;
        private readonly IShopItemsStorage _shopItemsStorage;
        private readonly ISelectPurchaseItemService _selectPurchaseItemService;
        
        private ShopService _shopService;

        public ShopItemDisplayController(GameObjectFactory gameObjectFactory, IShopItemsStorage shopItemsStorage,
                ISelectPurchaseItemService selectPurchaseItemService)
        {
            _gameObjectFactory = gameObjectFactory;
            _shopItemsStorage = shopItemsStorage;
            _selectPurchaseItemService = selectPurchaseItemService;
        }

        public void SetShop(ShopService shopService) =>
                _shopService = shopService;

        public void CreateItemDisplayView(ShopItem shopItem)
        {
            var shopItemDisplay = _gameObjectFactory.Create(shopItem.ShopItemDisplayView);
            _shopItemsStorage.AddItemDisplay(shopItemDisplay);
            shopItemDisplay.SetShopItem(shopItem);
            SetItemState(shopItemDisplay.GetShopItemModel());
        }

        public void UpdateItemStates()
        {
            foreach (var itemDisplayView in _shopItemsStorage.GetItemDisplay())
            {
                var shopItemDisplayModel = itemDisplayView.GetShopItemModel();
                itemDisplayView.UpdateShopItemUI(_shopItemsStorage.GetItemStateConfig(shopItemDisplayModel.ItemState));
            }
        }

        private void SetItemState(ShopItemDisplayModel shopItemDisplayModel)
        {
            if(_shopService.IsSelected(shopItemDisplayModel.ShopItem))
                _selectPurchaseItemService.SelectPurchasedItem(shopItemDisplayModel);

            else
                shopItemDisplayModel.SetShopItemState(_shopService.IsPurchased(shopItemDisplayModel.ShopItem) ? ShopItemState.Purchased : ShopItemState.NotPurchased);
        }
    }
}