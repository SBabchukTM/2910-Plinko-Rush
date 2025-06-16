using System;
using TMPro;
using UnityEngine;
using Application.Game;
using DG.Tweening;
using UnityEngine.UI;

namespace Application.UI
{
    public class GameplayScreen : UiScreen
    {
        [SerializeField] private SimpleButton _pauseButton;
        [SerializeField] private SimpleButton _heavyBallButton;
        [SerializeField] private SimpleButton _slimeBallButton;
        [SerializeField] private SimpleButton _airBallButton;
        [SerializeField] private SimpleButton _normalBallButton;

        [SerializeField] private TextMeshProUGUI _playerTurnText;
        [SerializeField] private TextMeshProUGUI _holeNumText;
        [SerializeField] private TextMeshProUGUI _parNumText;
        [SerializeField] private TextMeshProUGUI _shootsNumText;
        [SerializeField] private TextMeshProUGUI _balanceText;

        [SerializeField] private CanvasGroup _holeInOneGroup;
        [SerializeField] private CanvasGroup _birdieGroup;
        [SerializeField] private CanvasGroup _parGroup;
        [SerializeField] private CanvasGroup _bogeyGroup;

        [SerializeField] private BallConfigLibrary _ballConfigs;

        public event Action OnPauseButtonPress;
        public event Action<BallConfig> OnBallSelected;

        public void Init(Sprite sprite, int balance)
        {
            _balanceText.text = balance.ToString();

            _normalBallButton.gameObject.GetComponent<Image>().sprite = sprite;
            _pauseButton.Button.onClick.AddListener(() => OnPauseButtonPress?.Invoke());
            _heavyBallButton.Button.onClick.AddListener(() => BallSelectedPressed(BallType.Heavy));
            _slimeBallButton.Button.onClick.AddListener(() => BallSelectedPressed(BallType.Slime));
            _airBallButton.Button.onClick.AddListener(() => BallSelectedPressed(BallType.Air));
            _normalBallButton.Button.onClick.AddListener(() => BallSelectedPressed(BallType.Normal));

            SetButtonsActive(true);
        }

        public void SetPlayerTurnText(int playerIndex)
        {
            _playerTurnText.text = $"Player {playerIndex}";
        }

        public void SetHoleNumber(int currentHole)
        {
            _holeNumText.text = $"Hole {currentHole + 1}";
        }

        public void SetParNumber(int par)
        {
            _parNumText.text = $"Par {3}";
        }

        public void SetShotCount(int shots)
        {
            _shootsNumText.text = $"Shots {shots}";
        }

        public void SetButtonsActive(bool active)
        {
            _heavyBallButton.Button.interactable = active;
            _slimeBallButton.Button.interactable = active;
            _airBallButton.Button.interactable = active;
            _normalBallButton.Button.interactable = active;
        }

        public void BallSelectedPressed(BallType ballType)
        {
            OnBallSelected?.Invoke(_ballConfigs.GetConfig(ballType));
        }

        public async void ShowShotResultAsync(ShotResultType result)
        {
            CanvasGroup groupToShow = result switch
            {
                ShotResultType.HoleInOne => _holeInOneGroup,
                ShotResultType.Birdie => _birdieGroup,
                ShotResultType.Par => _parGroup,
                ShotResultType.Bogey => _bogeyGroup,
                _ => null
            };

            if (groupToShow == null) return;

            groupToShow.gameObject.SetActive(true);
            groupToShow.alpha = 0;

            Sequence sequence = DOTween.Sequence();
            sequence.Append(groupToShow.DOFade(1f, 0.5f))
                    .AppendInterval(1.5f)
                    .Append(groupToShow.DOFade(0f, 0.5f))
                    .OnComplete(() => groupToShow.gameObject.SetActive(false));

            await sequence.AsyncWaitForCompletion();
        }
    }
}