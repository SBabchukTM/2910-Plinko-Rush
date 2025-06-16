using System;
using System.Collections.Generic;
using Application.Services.UserData;
using Runtime.Application.ShopSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Application.UI
{
    public class ShopScreen : UiScreen
    {
        [SerializeField] private Button _goBackButton;
        [SerializeField] private TextMeshProUGUI _balanceText;
        [SerializeField] private RectTransform _shopItemsParent;
        
        private IUserInventoryService _userInventoryService;

        public event Action OnBackPressed;

        [Inject]
        public void Construct(IUserInventoryService userInventoryService) =>
                _userInventoryService = userInventoryService;
        
        private void Start()
        {
            SubscribeToEvents();
            UpdateBalance(_userInventoryService.GetBalance());
        }

        private void OnDestroy() =>
                UnSubscribe();

        public void SetShopItems(List<ShopItemDisplayView> items)
        {
            foreach (var item in items)
                item.transform.SetParent(_shopItemsParent, false);
        }

        private void UpdateBalance(int balance) => 
                _balanceText.text = balance.ToString();

        private void SubscribeToEvents()
        {
            _goBackButton.onClick.AddListener(() => OnBackPressed?.Invoke());
            _userInventoryService.BalanceChangedEvent += UpdateBalance;
        }

        private void UnSubscribe()
        {
            _goBackButton.onClick.RemoveAllListeners();
            _userInventoryService.BalanceChangedEvent -= UpdateBalance;
        }
    }
}