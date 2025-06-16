using System.Threading;
using Application.Services.UserData;
using Core.StateMachine;
using Application.UI;
using Cysharp.Threading.Tasks;
using Runtime.Application.ApplicationStates.Game.Controllers;
using Runtime.Application.UserAccountSystem;
using ILogger = Core.ILogger;

namespace Application.Game
{
    public class MenuStateController : StateController
    {
        private readonly IUiService _uiService;
        private readonly StartSettingsController _startSettingsController;
        private readonly IUserInventoryService _userInventoryService;
        private readonly UserAccountService _userAccountService;

        private MenuScreen _screen;

        public MenuStateController(ILogger logger, IUiService uiService,
            StartSettingsController startSettingsController, IUserInventoryService userInventoryService, UserAccountService userAccountService) : base(logger)
        {
            _uiService = uiService;
            _startSettingsController = startSettingsController;
            _userInventoryService = userInventoryService;
            _userAccountService = userAccountService;
        }

        public override UniTask Enter(CancellationToken cancellationToken)
        {
            CreateScreen();
            SubscribeToEvents();
            return UniTask.CompletedTask;
        }

        public override async UniTask Exit()
        {
            await _uiService.HideScreen(ConstScreens.MenuScreen);
        }

        private void CreateScreen()
        {
            _screen = _uiService.GetScreen<MenuScreen>(ConstScreens.MenuScreen);
            _screen.Initialize(_userInventoryService.GetBalance());
            _screen.ShowAsync().Forget();
        }

        private void SubscribeToEvents()
        {
            _screen.OnLeaderboardButtonPressed += async () => await GoTo<LeaderboardStateController>();
            _screen.OnDailiesButtonPressed += async () => await _uiService.ShowPopup(ConstPopups.DailyRewardPopup);
            _screen.OnAccountButtonPressed += async () => await GoTo<AccountScreenStateController>();
            _screen.OnAchievementsButtonPressed += async () => await GoTo<AchievementStateController>();
            _screen.OnShopButtonPressed += async () => await GoTo<InitShopState>();
            _screen.OnPlayButtonPressed += async () => await GoTo<LevelSelectionMenuController>();
            _screen.OnTermsOfUseButtonPressed += async () => await _uiService.ShowPopup(ConstPopups.TermsOfUsePopup);
            _screen.OnPrivacyButtonPressed += async () => await _uiService.ShowPopup(ConstPopups.PrivacyPolicyPopup);
            _screen.OnInfoButtonPressed += async () => await _uiService.ShowPopup(ConstPopups.InfoPopup);
            _screen.OnSettingsButtonPressed += ShowSettingsButtonPopup;
        }

        private void ShowSettingsButtonPopup()
        {
            _startSettingsController.Run(default).Forget();
        }
    }
}