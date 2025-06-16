using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Core.UI
{
    public class DailyRewardPopup : BasePopup
    {
        [SerializeField] private Button _closeButton;
        [SerializeField] private RectTransform _parent;
        
        private DailyRewardsFactory _dailyRewardsFactory;
        
        [Inject]
        private void Construct(DailyRewardsFactory factory)
        {
            _dailyRewardsFactory = factory;
        }
        
        public override UniTask Show(BasePopupData data, CancellationToken cancellationToken = default)
        {
            _closeButton.onClick.AddListener(DestroyPopup);
            foreach (var reward in _dailyRewardsFactory.CreateDailyRewardDisplayList())
            {
                reward.transform.SetParent(_parent, false);
            }
            return base.Show(data, cancellationToken);
        }
    }
}