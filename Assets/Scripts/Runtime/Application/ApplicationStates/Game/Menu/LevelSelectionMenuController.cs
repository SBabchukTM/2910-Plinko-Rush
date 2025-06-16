using System.Threading;
using Core.StateMachine;
using Application.UI;
using Cysharp.Threading.Tasks;
using ILogger = Core.ILogger;
using Application.Services.UserData;
using UnityEngine;
using System.Threading.Tasks;
using Core.UI;

namespace Application.Game
{
    public class LevelSelectionMenuController : StateController
    {
        private readonly IUiService _uiService;
        private readonly UserDataService _userDataService;
        private readonly GameplayStateController _gameplayStateController;

        private LevelSelectionScreen _screen;

        private int _selectedLevel;

        public LevelSelectionMenuController(ILogger logger, IUiService uiService, UserDataService userDataService, GameplayStateController gameplayStateController) : base(logger)
        {
            _uiService = uiService;
            _userDataService = userDataService;
            _gameplayStateController = gameplayStateController;
        }

        public override UniTask Enter(CancellationToken cancellationToken)
        {
            CreateScreen();
            SubscribeToEvents();
            return UniTask.CompletedTask;
        }

        private void CreateScreen()
        {
            _screen = _uiService.GetScreen<LevelSelectionScreen>(ConstScreens.LevelSelectionScreen);
            UserProgressData data = _userDataService.GetUserData().UserProgressData;
            _screen.Initialize(data.UnlockedLevels,data.FinishedLevels);
            _screen.ShowAsync().Forget();
        }

        private void SubscribeToEvents()
        {
            _screen.OnBackPressed += async () => await GoTo<MenuStateController>();

            _screen.OnLevelButtonPressed += async level => await OnLevelSelectedAsync(level);
        }

        public override async UniTask Exit()
        {
            await _uiService.HideScreen(ConstScreens.LevelSelectionScreen);
        }

        private async Task OnLevelSelectedAsync(int level)
        {
            _selectedLevel = level;
            var popup = await _uiService.ShowPopup(ConstPopups.GameModeSelectionPopup);
            var gameModeSelectionPopup = popup as GameModeSelectionPopup;
            gameModeSelectionPopup.OnModeSelected += async gameMode => await OnGameModeSelectedAsync(gameMode);
        }
        private async Task OnGameModeSelectedAsync(GameMode gameMode)
        {
            _gameplayStateController.Initial(_selectedLevel, gameMode);
            await GoTo<GameplayStateController>();
        }
    }
}