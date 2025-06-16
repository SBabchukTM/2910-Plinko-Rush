using System;

namespace Application.Services.UserData
{
    [Serializable]
    public class UserLoginData
    {
        public int LoginStreak = 0;
        public string LastLoginTimeString = String.Empty;   
    }
}