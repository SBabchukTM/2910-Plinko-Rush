
namespace Runtime.Application.Services.Shop
{
    public interface IShopItemsDisplayService : ISetShopSetup
    {
        void CreateShopItems();

        void UpdateItemsStatus();
    }
}