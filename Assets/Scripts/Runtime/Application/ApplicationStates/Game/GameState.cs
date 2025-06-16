using System.Threading;
using Cysharp.Threading.Tasks;
using Application.Game;
using Core.StateMachine;
using Runtime.Application.ApplicationStates.Game.Controllers;
using ILogger = Core.ILogger;

namespace Application.GameStateMachine
{
    public class GameState : StateController
    {
        private readonly StateMachine _stateMachine;

        private readonly MenuStateController _menuStateController;
        private readonly ShopStateController _shopStateController;
        private readonly AchievementStateController _achievementStateController;
        private readonly AccountScreenStateController _accountScreenStateController;
        private readonly GameplayStateController _gameplayStateController;
        private readonly LevelSelectionMenuController _levelSelectionMenuController;
        private readonly LeaderboardStateController _leaderboardStateController;
        private readonly UserDataStateChangeController _userDataStateChangeController;
        private readonly InitShopState _initShopState;
        private readonly PausePopupStateController _pausePopupStateController;
        private readonly GameEndPopupStateController _gameEndPopupStateController;

        public GameState(ILogger logger,
            MenuStateController menuStateController,
            ShopStateController shopStateController,
            AchievementStateController achievementStateController,
            AccountScreenStateController accountScreenStateController,
            GameplayStateController gameplayStateController,
            LevelSelectionMenuController levelSelectionMenuController,
            LeaderboardStateController leaderboardStateController,
            StateMachine stateMachine,
            UserDataStateChangeController userDataStateChangeController,
            PausePopupStateController pausePopupStateController,
            GameEndPopupStateController gameEndPopupStateController,
            InitShopState initShopState) : base(logger)
        {
            _stateMachine = stateMachine;
            _menuStateController = menuStateController;
            _shopStateController = shopStateController;
            _achievementStateController = achievementStateController;
            _accountScreenStateController = accountScreenStateController;
            _gameplayStateController = gameplayStateController;
            _levelSelectionMenuController = levelSelectionMenuController;
            _leaderboardStateController = leaderboardStateController;
            _userDataStateChangeController = userDataStateChangeController;
            _pausePopupStateController = pausePopupStateController;
            _gameEndPopupStateController = gameEndPopupStateController;
            _initShopState = initShopState;
        }

        public override async UniTask Enter(CancellationToken cancellationToken)
        {
            await _userDataStateChangeController.Run(default);

            _stateMachine.Initialize(
                _menuStateController,
                _shopStateController,
                _accountScreenStateController,
                _initShopState,
                _achievementStateController,
                _gameplayStateController,
                _levelSelectionMenuController,
                _leaderboardStateController,
                _pausePopupStateController,
                _gameEndPopupStateController);
            _stateMachine.GoTo<MenuStateController>().Forget();
        }
    }
}