using System.Threading;
using Core.UI;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Application.UI
{
    public class InfoPopup : BasePopup
    {
        [SerializeField] private Button _closeButton;

        public override UniTask Show(BasePopupData data, CancellationToken cancellationToken = default)
        {
            _closeButton.onClick.AddListener(DestroyPopup);
            return base.Show(data, cancellationToken);
        }
    }
}