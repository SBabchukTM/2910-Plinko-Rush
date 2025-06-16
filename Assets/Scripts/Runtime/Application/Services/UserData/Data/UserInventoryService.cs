using System;
using System.Collections.Generic;
using Core;
using Runtime.Application.ShopSystem;

namespace Application.Services.UserData
{
    public class UserInventoryService : IUserInventoryService
    {
        private readonly UserDataService _userDataService;
        private readonly ISettingProvider _settingProvider;

        public event Action<int> BalanceChangedEvent;

        public UserInventoryService(UserDataService userDataService,
            ISettingProvider settingProvider)
        {
            _userDataService = userDataService;
            _settingProvider = settingProvider;
        }

        public void SetBalance(int balance)
        {
            _userDataService.GetUserData().UserInventory.Balance = balance;
            BalanceChangedEvent?.Invoke(balance);
        }

        public void AddBalance(int amount)
        { 
            int balance = _userDataService.GetUserData().UserInventory.Balance + amount;
            SetBalance(balance);
        }

        public int GetBalance() => 
                _userDataService.GetUserData().UserInventory.Balance;

        public void UpdateUsedGameItemID(int userGameItemID) =>
                _userDataService.GetUserData().UserInventory.UsedGameItemID = userGameItemID;

        public ShopItem GetUsedGameItem()
        {
            ShopSetup shopSetup = _settingProvider.Get<ShopSetup>();
            int itemId = GetUsedGameItemID();
            return shopSetup.ShopItems[itemId];
        }

        public int GetUsedGameItemID() =>
                _userDataService.GetUserData().UserInventory.UsedGameItemID;

        public void AddPurchasedGameItemID(int userGameItemID) =>
                _userDataService.GetUserData().UserInventory.PurchasedGameItemsIDs.Add(userGameItemID);

        public List<int> GetPurchasedGameItemsIDs() =>
                _userDataService.GetUserData().UserInventory.PurchasedGameItemsIDs;
    }
}