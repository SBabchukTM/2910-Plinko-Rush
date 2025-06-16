using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Application.UI
{
    public class LeaderboardScreen : UiScreen
    {
        [SerializeField] private Button _backButton;
        [SerializeField] private RectTransform _parent;

        public event Action OnBackPressed;
        
        public void Initialize(List<RecordDisplay> records)
        {
            _backButton.onClick.AddListener(() => OnBackPressed?.Invoke());

            foreach (var record in records)
            {
                record.transform.SetParent(_parent, false);
            }
        }
    }
}