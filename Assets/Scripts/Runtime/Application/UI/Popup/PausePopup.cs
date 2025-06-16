using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI
{
    public class PausePopup : BasePopup
    {
        [SerializeField] private Button _homeButton;
        [SerializeField] private Button _continueButton;

        public event Action OnHomeButtonPressed;
        public event Action OnContinueButtonPressed;

        public override UniTask Show(BasePopupData data, CancellationToken cancellationToken = default)
        {
            _homeButton.onClick.AddListener(() => OnHomeButtonPressed?.Invoke());
            _continueButton.onClick.AddListener(() => OnContinueButtonPressed?.Invoke());
            return base.Show(data, cancellationToken);
        }
    }
}