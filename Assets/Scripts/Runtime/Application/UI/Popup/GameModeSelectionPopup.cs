using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI
{
    public class GameModeSelectionPopup : BasePopup
    {
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _twoPlayersMode;
        [SerializeField] private Button _singlePlayerMode;

        public event Action<GameMode> OnModeSelected;

        public override UniTask Show(BasePopupData data, CancellationToken cancellationToken = default)
        {
            _singlePlayerMode.onClick.RemoveAllListeners();
            _singlePlayerMode.onClick.AddListener(() => OnGameModePressed(GameMode.SinglePlayer));

            _twoPlayersMode.onClick.RemoveAllListeners();
            _twoPlayersMode.onClick.AddListener(() => OnGameModePressed(GameMode.TwoPlayers));

            _closeButton.onClick.AddListener(DestroyPopup);
            return base.Show(data, cancellationToken);
        }

        private void OnGameModePressed(GameMode gameMode)
        {
            OnModeSelected?.Invoke(gameMode);
            DestroyPopup();
        }
    }
}