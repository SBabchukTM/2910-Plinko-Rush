using Cysharp.Threading.Tasks;
using Runtime.Application.AchievementSystem;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace Application.UI
{
    public class AchievementScreen : UiScreen
    {
        [SerializeField] private Button _backButton;
        [SerializeField] private GameObject _content;

        public GameObject Content => _content;

        public event Action OnBackPress;

        public override UniTask ShowAsync(CancellationToken cancellationToken = default)
        {
            SubscribeToEvents();
            return base.ShowAsync(cancellationToken);
        }

        public void SubscribeToEvents()
        {
            _backButton.onClick.AddListener(() => OnBackPress?.Invoke());
        }

        public void UnSubscribe()
        {
            _backButton.onClick.RemoveAllListeners();
        }

        public void CleanAchievement()
        {
            foreach (Transform child in _content.transform)
            {
                Destroy(child.gameObject);
            }
        }
    }
}