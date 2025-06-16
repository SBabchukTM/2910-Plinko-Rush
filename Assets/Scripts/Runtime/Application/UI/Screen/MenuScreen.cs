using System;
using Application.Services.UserData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Application.UI
{
    public class MenuScreen : UiScreen
    {
        [SerializeField] private Button _leaderboardButton;
        [SerializeField] private Button _dailiesButton;
        [SerializeField] private Button _accountButton;
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _shopButton;
        [SerializeField] private Button _termsOfUseButton;
        [SerializeField] private Button _privacyButton;
        [SerializeField] private Button _InfoButton;
        [SerializeField] private Button _achievementsButton;

        [SerializeField] private TextMeshProUGUI _balanceText;

        public event Action OnLeaderboardButtonPressed;
        public event Action OnDailiesButtonPressed;
        public event Action OnAccountButtonPressed;
        public event Action OnPlayButtonPressed;
        public event Action OnSettingsButtonPressed;
        public event Action OnShopButtonPressed;
        public event Action OnTermsOfUseButtonPressed;
        public event Action OnPrivacyButtonPressed;
        public event Action OnInfoButtonPressed;
        public event Action OnAchievementsButtonPressed;

        [Inject]
        private void Construct(IUserInventoryService userInventoryService)
        {
            userInventoryService.BalanceChangedEvent += balance => _balanceText.text = balance.ToString();
        }

        public void Initialize(int balance)
        {
            _leaderboardButton.onClick.AddListener(() => OnLeaderboardButtonPressed?.Invoke());
            _dailiesButton.onClick.AddListener(() => OnDailiesButtonPressed?.Invoke());
            _accountButton.onClick.AddListener(() => OnAccountButtonPressed?.Invoke());
            _playButton.onClick.AddListener(() => OnPlayButtonPressed?.Invoke());
            _settingsButton.onClick.AddListener(() => OnSettingsButtonPressed?.Invoke());
            _shopButton.onClick.AddListener(() => OnShopButtonPressed?.Invoke());
            _termsOfUseButton.onClick.AddListener(() => OnTermsOfUseButtonPressed?.Invoke());
            _privacyButton.onClick.AddListener(() => OnPrivacyButtonPressed?.Invoke());
            _InfoButton.onClick.AddListener(() => OnInfoButtonPressed?.Invoke());
            _achievementsButton.onClick.AddListener(() => OnAchievementsButtonPressed?.Invoke());

            _balanceText.text = balance.ToString();
        }
    }
}