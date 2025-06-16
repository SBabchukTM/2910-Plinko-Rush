using System;
using System.Collections.Generic;
using Runtime.Application.UserAccountSystem;

namespace Application.Services.UserData
{
    [Serializable]
    public class UserData
    {
        public List<GameSessionData> GameSessionData = new List<GameSessionData>();
        public SettingsData SettingsData = new SettingsData();
        public GameData GameData = new GameData();
        public UserInventory UserInventory = new UserInventory();
        public UserAccountData UserAccountData = new UserAccountData();
        public UserLoginData UserLoginData = new UserLoginData();
        public UserProgressData UserProgressData = new UserProgressData();
        public AchievementData AchievementData = new AchievementData();
    }
}