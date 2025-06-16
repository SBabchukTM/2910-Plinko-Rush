using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI
{
    public class GameEndPopup : BasePopup
    {
        [SerializeField] private Button _homeButton;
        [SerializeField] private Button _replayButton;
        [SerializeField] private TextMeshProUGUI _balanceText;
        [SerializeField] private TextMeshProUGUI _firstScoreText;
        [SerializeField] private TextMeshProUGUI _secondScoreText;
        [SerializeField] private GameObject _firstPlayerTitleGameObject;
        [SerializeField] private GameObject _secondPlayerTitleGameObject;

        public event Action OnHomePressed;
        public event Action OnReplayPressed;

        public override UniTask Show(BasePopupData data, CancellationToken cancellationToken = default)
        {
            _homeButton.onClick.AddListener(() => { OnHomePressed?.Invoke(); DestroyPopup(); });
            _replayButton.onClick.AddListener(() => { OnReplayPressed?.Invoke(); DestroyPopup(); });

            return base.Show(data, cancellationToken);
        }

        public void SetData(GameMode gameMode, int balance, int firstPlayerScore, int secondPlayerScore)
        {
            _balanceText.text = balance.ToString();
            _firstScoreText.text = $"Score {firstPlayerScore}";
            _secondScoreText.text = $"Score {secondPlayerScore}";

            bool isTwoPlayers = gameMode == GameMode.TwoPlayers;

            _firstPlayerTitleGameObject.SetActive(isTwoPlayers);
            _secondPlayerTitleGameObject.SetActive(isTwoPlayers);
        }
    }
}