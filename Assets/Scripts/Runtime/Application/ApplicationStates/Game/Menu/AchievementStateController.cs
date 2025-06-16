using Application.Game;
using Application.Services.UserData;
using Application.UI;
using Core;
using Core.Factory;
using Core.StateMachine;
using Cysharp.Threading.Tasks;
using Runtime.Application.AchievementSystem;
using Runtime.Application.ShopSystem;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class AchievementStateController : StateController
{
    private readonly IUiService _uiService;
    private readonly ISettingProvider _settingProvider;
    private readonly GameObjectFactory _gameObjectFactory;
    private readonly UserDataService _userDataService;
    private AchievementScreen _screen;

    private List<AchievementView> _viewList = new List<AchievementView>();

    public AchievementStateController(
        ILogger logger,
        IUiService uiService,
            ISettingProvider settingProvider,
            GameObjectFactory gameObjectFactory,
            UserDataService userDataService) : base(logger)
    {
        _uiService = uiService;
        _settingProvider = settingProvider;
        _gameObjectFactory = gameObjectFactory;
        _userDataService = userDataService;
    }

    public override async UniTask Enter(CancellationToken cancellationToken = default)
    {
        _screen = CreateScreen();
        SetShopConfig();
        await Task.CompletedTask;
    }

    public override UniTask Exit()
    {
        _screen.OnBackPress -= GoBack;
        _uiService.HideScreen(ConstScreens.AchievementScreen);
        return base.Exit();
    }

    private AchievementScreen CreateScreen()
    {
        var screen = _uiService.GetScreen<AchievementScreen>(ConstScreens.AchievementScreen);
        screen.ShowAsync().Forget();
        screen.OnBackPress += GoBack;

        return screen;
    }

    private async void GoBack()
    {
        Unsubscribe();
        await _uiService.HideScreen(ConstScreens.AchievementScreen);
        await GoTo<MenuStateController>();
    }

    private void SetShopConfig()
    {
        var achievementsSetup = _settingProvider.Get<AchievementsSetup>();
        _viewList.Clear();
        _screen.CleanAchievement();
        CheckAchievement();

        var number = 1;
        foreach (var item in achievementsSetup.AchievementConfigs)
        {
            var achievementGameObject = _gameObjectFactory.Create(achievementsSetup.AchievementView.gameObject, _screen.Content.transform);
            var achievementView = achievementGameObject.GetComponent<AchievementView>();

            var achievement = _userDataService.GetUserData().AchievementData.GetAchievements().Find(x => x.achievementType == item.AchievementType);

            var status = achievement.IsUnlocked ? AchievementStatus.Claim : AchievementStatus.InProgress;
            if (achievement.IsReceived)
            {
                status = achievement.IsReceived ? AchievementStatus.Claimed : AchievementStatus.Claim;
            }

            achievementView.Initialize(item.AchievementType, item.Name, item.Description, status, item.Reward, number);
            _viewList.Add(achievementView);
            achievementView.OnClaimButtonPressed += GetReward;
            number++;
        }
    }

    private void Unsubscribe()
    {
        foreach (var item in _viewList)
        {
            item.OnClaimButtonPressed -= GetReward;
        }
    }

    private void CheckAchievement()
    {
        var progressData = _userDataService.GetUserData().UserProgressData;
        var achievementData = _userDataService.GetUserData().AchievementData;

        if (progressData.FinishedLevels > 0)
        {
            achievementData.CompleteAchievement(AchievementType.FirstSwing);
        }
        if (progressData.FinishedLevels >= 9)
        {
            achievementData.CompleteAchievement(AchievementType.GolfPro);
        }

        var inventoryData = _userDataService.GetUserData().UserInventory.PurchasedGameItemsIDs;

        var shopItems = _settingProvider.Get<ShopSetup>().ShopItems;


        bool hasAllShopItems = shopItems.All(id => inventoryData.Contains(id.ItemID));

        if (hasAllShopItems)
        {
            achievementData.CompleteAchievement(AchievementType.Collector);
        }
    }

    private void GetReward(AchievementType achievementType, int reward)
    {
        var userData = _userDataService.GetUserData();
        userData.UserInventory.Balance += reward;
        userData.AchievementData.ClaimedAchievementReward(achievementType);
        SetShopConfig();
    }
}