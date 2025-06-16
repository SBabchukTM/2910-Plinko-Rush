using System;
using Runtime.Application.UserAccountSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Application.UI
{
    public class AccountScreen : UiScreen
    {
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _saveButton;
        [SerializeField] private Button _changeAvatarButton;
        [SerializeField] private Image _avatarImage;
        [SerializeField] private TMP_InputField _nameInputField;

        public event Action OnBackPressed;
        public event Action OnSavePressed;
        public event Action OnChangeAvatarPressed;
        public event Action<string> OnNameChanged;

        public void Initialize()
        {
            _backButton.onClick.AddListener(() => OnBackPressed?.Invoke());
            _saveButton.onClick.AddListener(() => OnSavePressed?.Invoke());
            _changeAvatarButton.onClick.AddListener(() => OnChangeAvatarPressed?.Invoke());
            _nameInputField.onEndEdit.AddListener((value) => OnNameChanged?.Invoke(value));
        }

        public void SetData(UserAccountData data)
        {
            _nameInputField.text = data.Username;
        }

        public void SetAvatar(Sprite sprite)
        {
            if (sprite != null)
                _avatarImage.sprite = sprite;
        }
    }
}