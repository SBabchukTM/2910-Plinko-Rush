using System;

namespace Application.Services.UserData
{
    [Serializable]
    public class UserProgressData
    {
        public int HighestScore;
        public int UnlockedLevels = 1;
        public int FinishedLevels = 0;
    }
}