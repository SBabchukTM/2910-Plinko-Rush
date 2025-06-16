using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Application.Services.UserData;
using Application.UI;
using Core;
using Core.StateMachine;
using Core.UI;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Application.Game
{
    public class GameplayStateController : StateController
    {
        private readonly IUiService _uiService;
        [SerializeField] private readonly GolfGameManager _golfGameManager;
        [SerializeField] private readonly GameplayObjectsActivator _gameplayObjectsActivator;
        [SerializeField] private readonly UserDataService _userDataService;
        [SerializeField] private readonly IUserInventoryService _userInventoryService;

        private List<HoleResult> _holeResults = new List<HoleResult>();
        private GameplayScreen _screen;
        private int _currentLevel = 0;

        private GameMode _gameMode = GameMode.TwoPlayers;
        public GameMode CurrentGameMode => _gameMode;

        public GameplayStateController(
            Core.ILogger logger,
            IUiService uiService,
            GolfGameManager golfGameManager,
            GameplayObjectsActivator gameplayObjectsActivator,
            UserDataService userDataService,
            IUserInventoryService userInventoryService) : base(logger)
        {
            _uiService = uiService;
            _golfGameManager = golfGameManager;
            _gameplayObjectsActivator = gameplayObjectsActivator;
            _userDataService = userDataService;
            _userInventoryService = userInventoryService;
        }

        public override async UniTask Enter(CancellationToken cancellationToken)
        {
            _gameplayObjectsActivator.Activate();
            await _golfGameManager.Initial(_currentLevel, _gameMode, cancellationToken);

            _golfGameManager.OnFinish += FinishLevel;
            _golfGameManager.OnBallInHole += ShowHoleResult;
            CreateScreen();
            SubscribeToScreenEvents();
            UpdateUI();

            var currentBall = _golfGameManager.GetCurrentBall();
            var sprite = _userInventoryService.GetUsedGameItem().ItemSprite;
            currentBall.SetSprite(sprite);

            _screen.BallSelectedPressed(BallType.Normal);
            _holeResults.Clear();
            await UniTask.CompletedTask;
        }

        public override async UniTask Exit()
        {
            await _gameplayObjectsActivator?.Deactivate();
            UnsubscribeFromScreenEvents();
            _golfGameManager.OnFinish -= FinishLevel;
            _golfGameManager.OnBallInHole -= ShowHoleResult;
            _golfGameManager.OnBallHit -= () => _screen.SetButtonsActive(false);
            _golfGameManager.OnNextTurn -= () => _screen.SetButtonsActive(true);
            await _uiService.HideScreen(ConstScreens.GameplayScreen);
        }

        public void Initial(int level, GameMode gameMode)
        {
            _currentLevel = level;
            _gameMode = gameMode;
            _golfGameManager.OnBallHit += () => _screen.SetButtonsActive(false);
            _golfGameManager.OnNextTurn += () => _screen.SetButtonsActive(true);
        }

        private void CreateScreen()
        {
            _screen = _uiService.GetScreen<GameplayScreen>(ConstScreens.GameplayScreen);
            _screen.ShowAsync().Forget();
            var sprite = _userInventoryService.GetUsedGameItem().ItemSprite;
            var balance = _userInventoryService.GetBalance();

            _screen.Init(sprite, balance);
        }

        private void SubscribeToScreenEvents()
        {
            _screen.OnPauseButtonPress += PauseButtonPressed;
            _screen.OnBallSelected += SelectBall;

            _golfGameManager.OnNextTurn += UpdateUI;
        }

        private void UnsubscribeFromScreenEvents()
        {
            _screen.OnPauseButtonPress -= PauseButtonPressed;
            _screen.OnBallSelected -= SelectBall;

            _golfGameManager.OnNextTurn -= UpdateUI;
        }

        private async void FinishLevel()
        {
            var finished = _userDataService.GetUserData().UserProgressData.FinishedLevels;
            _userDataService.GetUserData().UserProgressData.FinishedLevels = _currentLevel + 1 > finished ? _currentLevel + 1 : finished;
            if (_userDataService.GetUserData().UserProgressData.UnlockedLevels <= _currentLevel + 1)
            {
                _userDataService.GetUserData().UserProgressData.UnlockedLevels = _currentLevel + 2;
            }
            var popup = await _uiService.ShowPopup(ConstPopups.GameEndPopup) as GameEndPopup;
            var balance = _userDataService.GetUserData().UserInventory.Balance;

            var scores = _golfGameManager.GetScore();
            var firstPlayerScore = scores.player1;
            var secondPlayerScore = scores.player2;

            if (_gameMode == GameMode.SinglePlayer)
            {
                _userDataService.GetUserData().UserInventory.Balance += firstPlayerScore;
            }

            if (_gameMode == GameMode.SinglePlayer)
                CheckSingleAchievements();
            else
                CheckTwoPlayerAchievements();

            popup.SetData(_gameMode, balance, firstPlayerScore, secondPlayerScore);
            popup.OnHomePressed += async () => await GoTo<MenuStateController>();
            popup.OnReplayPressed += async () => await GoTo<GameplayStateController>();
        }

        private async void PauseButtonPressed()
        {
            Time.timeScale = 0;

            var popup = _uiService.GetPopup<PausePopup>(ConstPopups.PausePopup);

            popup.OnContinueButtonPressed += () =>
            {
                Time.timeScale = 1;
                popup.DestroyPopup();
            };

            popup.OnHomeButtonPressed += async () =>
            {
                Time.timeScale = 1;
                popup.DestroyPopup();
                await GoTo<MenuStateController>();
            };

            await popup.Show(null);
        }

        private void SelectBall(BallConfig ballConfig)
        {
            var currentBall = _golfGameManager.GetCurrentBall();

            if (currentBall != null)
            {
                currentBall.ApplyConfig(ballConfig);

                if (ballConfig.BallType == BallType.Normal)
                {
                    var sprite = _userInventoryService.GetUsedGameItem().ItemSprite;
                    currentBall.SetSprite(sprite);
                }

            }
        }

        private void UpdateUI()
        {
            _screen.SetPlayerTurnText(_golfGameManager.GetCurrentPlayerIndex() + 1);
            _screen.SetHoleNumber(_golfGameManager.GetCurrentHoleIndex());
            _screen.SetParNumber(3);
            _screen.SetShotCount(_golfGameManager.GetCurrentPlayerShotCount());
        }

        private void ShowHoleResult(HoleResult holeResult)
        {
            _screen.ShowShotResultAsync(holeResult.Result);

            if (_gameMode == GameMode.SinglePlayer)
            {
                _holeResults.Add(holeResult);
            }
        }

        private void CheckSingleAchievements()
        {
            var achievementData = _userDataService.GetUserData().AchievementData;

            bool hasHoleInOne = _holeResults.Any(hr => hr.Shots == 1);
            if (hasHoleInOne)
            {
                achievementData.CompleteAchievement(AchievementType.PrecisionMaster);
            }

            bool allUnderPar = _holeResults.All(hr => hr.Shots < hr.Par);
            if (allUnderPar)
            {
                achievementData.CompleteAchievement(AchievementType.UnderPar);
            }
        }

        private void CheckTwoPlayerAchievements()
        {
            var achievementData = _userDataService.GetUserData().AchievementData;

            var scores = _golfGameManager.GetScore();
            var firstPlayerScore = scores.player1;
            var secondPlayerScore = scores.player2;

            if (firstPlayerScore > secondPlayerScore)
            {
                achievementData.CompleteAchievement(AchievementType.MultiplayerChampion);
            }
        }

    }

    public enum BallType
    {
        Normal,
        Heavy,
        Slime,
        Air
    }

}
public enum GameMode
{
    SinglePlayer,
    TwoPlayers
}