using System.Threading.Tasks;

namespace Runtime.Application.ShopSystem
{
    public class ShopItemDisplayModel
    { 
        public TaskCompletionSource<bool> ShakeTaskCompletionSource { get; private set; } = new();
        public ShopItem ShopItem { get; private set; }
        public ShopItemState ItemState { get; private set; }
        
        public void SetShopItemState(ShopItemState shopItemState) =>
                ItemState = shopItemState;

        public void SetShopItem(ShopItem shopItem) =>
                ShopItem = shopItem;

        public void SetShakeCompletionSource(TaskCompletionSource<bool> taskCompletionSource) =>
                ShakeTaskCompletionSource = taskCompletionSource;
    }
}