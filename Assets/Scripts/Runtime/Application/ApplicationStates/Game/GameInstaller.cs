using Runtime.Application.ApplicationStates.Game.Controllers;
using Runtime.Application.Services.Shop;
using Runtime.Application.ShopSystem;
using UnityEngine;
using Zenject;

namespace Application.Game
{
    [CreateAssetMenu(fileName = "GameInstaller", menuName = "Installers/GameInstaller")]
    public class GameInstaller : ScriptableObjectInstaller<GameInstaller>
    {
        public override void InstallBindings()
        {
            BindController();
            BindServices();
            BindData();
            BindSystems();
        }

        private void BindData()
        {
            Container.Bind<IShopItemsStorage>().To<ShopItemsStorage>().AsSingle();
            Container.Bind<ShopItemDisplayModel>().AsTransient();
        }

        private void BindServices()
        {
            Container.Bind<IProcessPurchaseService>().To<ProcessPurchaseService>().AsSingle();
            Container.Bind<ISelectPurchaseItemService>().To<SelectPurchaseItemService>().AsSingle();
            Container.Bind<IPurchaseEffectsService>().To<PurchaseEffectsService>().AsSingle();
            Container.Bind<IShopItemsDisplayService>().To<ShopItemsDisplayService>().AsSingle();
        }

        private void BindController()
        {
            Container.Bind<MenuStateController>().AsSingle();
            Container.Bind<InitShopState>().AsSingle();
            Container.Bind<ShopStateController>().AsSingle();
            Container.Bind<AchievementStateController>().AsSingle();
            Container.Bind<AccountScreenStateController>().AsSingle();
            Container.Bind<StartSettingsController>().AsSingle();
            Container.Bind<ShopItemDisplayController>().AsSingle();
            Container.Bind<GameplayStateController>().AsSingle();
            Container.Bind<LevelSelectionMenuController>().AsSingle();
            Container.Bind<LeaderboardStateController>().AsSingle();
            Container.BindInterfacesAndSelfTo<RecordsFactory>().AsSingle();
            Container.BindInterfacesAndSelfTo<DailyRewardsFactory>().AsSingle();
            Container.Bind<PausePopupStateController>().AsSingle();
            Container.Bind<GameEndPopupStateController>().AsSingle();
            Container.Bind<GolfGameManager>().FromComponentInHierarchy().AsSingle();
            Container.Bind<GameplayObjectsActivator>().FromComponentInHierarchy().AsSingle();
        }

        private void BindSystems()
        {

        }
    }
}