using System.Threading;
using Runtime.Application.ShopSystem;

namespace Runtime.Application.Services.Shop
{
    public interface IProcessPurchaseService : ISetShopSetup
    {
        void ProcessPurchaseAttempt(ShopItemDisplayView shopItemDisplayView, CancellationToken cancellationToken);
    }
}