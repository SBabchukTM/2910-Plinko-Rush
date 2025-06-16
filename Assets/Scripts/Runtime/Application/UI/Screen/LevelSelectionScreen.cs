using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Application.UI
{
    public class LevelSelectionScreen : UiScreen
    {
        [SerializeField] private Button _backButton;

        [SerializeField] private Sprite _unlockedSprite;
        [SerializeField] private Sprite _lockedSprite;
        [SerializeField] private Transform _levelSelectionButtonsParent;

        private LevelSelectionButton[] _levelSelectionButtons;

        public event Action OnBackPressed;
        public event Action OnMenuPressed;
        public event Action<int> OnLevelButtonPressed;

        public void Initialize(int unlockedLevels, int finishedLevels)
        {
            _levelSelectionButtons = _levelSelectionButtonsParent.GetComponentsInChildren<LevelSelectionButton>();
            InitializeButtons(unlockedLevels - 1, finishedLevels);

            _backButton.onClick.AddListener(() => OnBackPressed?.Invoke());
        }

        private void OnDestroy()
        {
            _backButton.onClick.RemoveAllListeners();

            for (int i = 0; i < _levelSelectionButtons.Length; i++)
            {
                _levelSelectionButtons[i].OnLevelSelected -= UpdateSelectedLevel;
            }
        }

        private void InitializeButtons(int unlockedLevelID, int finishedLevels)
        {
            for (int i = 0; i < _levelSelectionButtons.Length; i++)
            {
                bool locked = i > unlockedLevelID;
                bool finished = i < finishedLevels;
                _levelSelectionButtons[i].Initialize(locked, finished, locked ? _lockedSprite : _unlockedSprite);
                _levelSelectionButtons[i].OnLevelSelected += UpdateSelectedLevel;
            }
        }

        private void UpdateSelectedLevel(int level)
        {
            OnLevelButtonPressed?.Invoke(level);
        }
    }
}