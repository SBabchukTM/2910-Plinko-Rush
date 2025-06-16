using Application.Services.UserData;
using Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Application.AchievementSystem
{
    [CreateAssetMenu(fileName = "AchievementsSetup", menuName = "Config/AchievementsSetup")]
    public class AchievementsSetup : BaseSettings
    {
        [SerializeField] private AchievementView _achievementView;
        [SerializeField] private List<AchievementConfig> _achievementConfigs;

        public List<AchievementConfig> AchievementConfigs => _achievementConfigs;
        public AchievementView AchievementView => _achievementView;
    }
}