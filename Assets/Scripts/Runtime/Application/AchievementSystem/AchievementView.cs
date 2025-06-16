using Application.Services.UserData;
using Application.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Runtime.Application.AchievementSystem
{

    public class AchievementView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField] private TextMeshProUGUI _description;
        [SerializeField] private TextMeshProUGUI _rewardText;
        [SerializeField] private TextMeshProUGUI _numberText;
        [SerializeField] private int _reward;
        [SerializeField] private SimpleButton _button;
        [SerializeField] private TextMeshProUGUI _buttonText;

        public event Action<AchievementType, int> OnClaimButtonPressed;

        public void Initialize(AchievementType achievementType, string title, string description, AchievementStatus status, int reward, int number)
        {
            _name.text = title;
            _description.text = description;
            _numberText.text = number.ToString();
            _reward = reward;
            _rewardText.text = _reward.ToString();
            _button.Button.onClick.RemoveAllListeners();
            _button.Button.onClick.AddListener(() => OnClaimButtonPressed?.Invoke(achievementType, reward));
            switch (status)
            {
                case AchievementStatus.InProgress:
                    _buttonText.text = "InProgress";
                    _button.Button.interactable = false;
                    break;

                case AchievementStatus.Claim:
                    _buttonText.text = $"Claim";
                    _button.Button.interactable = true;
                    break;

                case AchievementStatus.Claimed:
                    _buttonText.text = "Claimed";
                    _button.Button.interactable = false;
                    break;
            }
        }
    }

    public enum AchievementStatus
    {
        InProgress,
        Claim,
        Claimed
    }
}