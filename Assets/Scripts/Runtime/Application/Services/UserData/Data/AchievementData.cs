using System;
using System.Collections.Generic;

namespace Application.Services.UserData
{
    [Serializable]
    public class Achievement
    {
        public AchievementType achievementType;
        public bool IsUnlocked;
        public bool IsReceived;

        public Achievement(AchievementType achievementType)
        {
            IsUnlocked = false;
            IsReceived = false;
            this.achievementType = achievementType;
        }
    }

    [Serializable]
    public class AchievementData
    {
        public List<Achievement> achievements = new List<Achievement>();
        public Achievement FirstSwing = new Achievement(AchievementType.FirstSwing);
        public Achievement GolfPro = new Achievement(AchievementType.GolfPro);
        public Achievement PrecisionMaster = new Achievement(AchievementType.PrecisionMaster);
        public Achievement MultiplayerChampion = new Achievement(AchievementType.MultiplayerChampion);
        public Achievement UnderPar = new Achievement(AchievementType.UnderPar);
        public Achievement Collector = new Achievement(AchievementType.Collector);
        public Achievement GolfKing = new Achievement(AchievementType.GolfKing);

        public bool AreAllAchievementsUnlockedExceptGolfKing()
        {
            return FirstSwing.IsUnlocked &&
                   GolfPro.IsUnlocked &&
                   PrecisionMaster.IsUnlocked &&
                   MultiplayerChampion.IsUnlocked &&
                   UnderPar.IsUnlocked &&
                   Collector.IsUnlocked;
        }

        public void UpdateGolfKingStatus()
        {
            if (AreAllAchievementsUnlockedExceptGolfKing())
                GolfKing.IsUnlocked = true;
        }

        public List<Achievement> GetAchievements()
        {
            achievements.Clear();
            achievements = new List<Achievement>()
            {
                FirstSwing,
                GolfPro,
                PrecisionMaster,
                MultiplayerChampion,
                UnderPar,
                Collector,
                GolfKing
            };
            return achievements;
        }

        public void CompleteAchievement(AchievementType achievementType)
        {
            var achievement = GetAchievements().Find(a => a.achievementType == achievementType);

            if (achievement == null)
            {
                UnityEngine.Debug.LogWarning($"Achievement of type {achievementType} not found.");
                return;
            }

            if (!achievement.IsUnlocked)
            {
                achievement.IsUnlocked = true;
                UpdateGolfKingStatus();
            }
        }

        public void ClaimedAchievementReward(AchievementType achievementType)
        {
            var achievement = GetAchievements().Find(a => a.achievementType == achievementType);

            if (achievement == null)
            {
                UnityEngine.Debug.LogWarning($"Achievement of type {achievementType} not found.");
                return;
            }
            achievement.IsReceived = true;
        }
    }

    public enum AchievementType
    {
        FirstSwing,
        GolfPro,
        PrecisionMaster,
        MultiplayerChampion,
        UnderPar,
        Collector,
        GolfKing
    }
}