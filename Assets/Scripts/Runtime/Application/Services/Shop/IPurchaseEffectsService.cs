using System.Threading;
using Runtime.Application.ShopSystem;

namespace Runtime.Application.Services.Shop
{
    public interface IPurchaseEffectsService : ISetShopSetup
    {
        void PlayFailedPurchaseAttemptEffect(ShopItemDisplayView shopItemDisplayView, CancellationToken cancellationToken);
    }
}