using System;
using System.Collections.Generic;
using Application.Services;
using Application.Services.UserData;
using Core;
using Core.Factory;
using UnityEngine;
using Zenject;

public class DailyRewardsFactory : IInitializable
{
    private readonly ISettingProvider _settingProvider;
    private readonly IAssetProvider _assetProvider;
    private readonly UserDataService _userDataService;
    private readonly GameObjectFactory _gameObjectFactory;
    private readonly IUserInventoryService _userInventoryService;

    private GameObject _dailyRewardsPrefab;

    public DailyRewardsFactory(ISettingProvider settingProvider, IAssetProvider assetProvider,
        UserDataService userDataService, GameObjectFactory gameObjectFactory, IUserInventoryService userInventoryService)
    {
        _settingProvider = settingProvider;
        _assetProvider = assetProvider;
        _userDataService = userDataService;
        _gameObjectFactory = gameObjectFactory;
        _userInventoryService = userInventoryService;
    }

    public async void Initialize()
    {
        _dailyRewardsPrefab = await _assetProvider.Load<GameObject>(ConstPrefabs.DailyLoginPrefab);
    }

    public List<DailyRewardDisplay> CreateDailyRewardDisplayList()
    {
        List<DailyRewardDisplay> result = new();

        var config = _settingProvider.Get<DailyLoginRewardConfig>();

        var loginStreak = _userDataService.GetUserData().UserLoginData.LoginStreak;

        CreateCollectedRewardsDisplay(loginStreak, config, result);

        CreateCollectableRewardDisplay(loginStreak, config, result);

        CreateLockedRewardsDisplay(loginStreak, config, result);

        return result;
    }

    private void CreateCollectedRewardsDisplay(int loginStreak, DailyLoginRewardConfig config, List<DailyRewardDisplay> result)
    {
        for (int i = 0; i < loginStreak; i++)
        {
            var loginDisplay = _gameObjectFactory.Create<DailyRewardDisplay>(_dailyRewardsPrefab);
            loginDisplay.Initialize(true, false, config.CoinRewards[i]);

            result.Add(loginDisplay);
        }
    }

    private void CreateCollectableRewardDisplay(int loginStreak, DailyLoginRewardConfig config, List<DailyRewardDisplay> result)
    {
        bool showReward = false;

        var firstLogin = _userDataService.GetUserData().UserLoginData.LastLoginTimeString == String.Empty;
        if (firstLogin)
            showReward = true;
        else
        {
            var lastLoginTime = Convert.ToDateTime(_userDataService.GetUserData().UserLoginData.LastLoginTimeString);
            showReward = DateTime.Now.Date > lastLoginTime.Date;
        }

        if (showReward && loginStreak < config.CoinRewards.Count)
        {
            var loginDisplay = _gameObjectFactory.Create<DailyRewardDisplay>(_dailyRewardsPrefab);
            loginDisplay.Initialize(false, true, config.CoinRewards[loginStreak]);

            loginDisplay.OnCollected += UpdateLoginStreak;
            result.Add(loginDisplay);
        }
    }

    private void CreateLockedRewardsDisplay(int loginStreak, DailyLoginRewardConfig config, List<DailyRewardDisplay> result)
    {
        for (int i = loginStreak + 1; i < config.CoinRewards.Count; i++)
        {
            var loginDisplay = _gameObjectFactory.Create<DailyRewardDisplay>(_dailyRewardsPrefab);
            loginDisplay.Initialize(false, false, config.CoinRewards[i]);

            result.Add(loginDisplay);
        }
    }

    private void UpdateLoginStreak(DailyRewardDisplay display, int amount)
    {
        display.ClaimReward();

        _userInventoryService.AddBalance(amount);

        var loginData = _userDataService.GetUserData().UserLoginData;
        loginData.LoginStreak++;
        loginData.LastLoginTimeString = DateTime.Now.ToString();
    }
}