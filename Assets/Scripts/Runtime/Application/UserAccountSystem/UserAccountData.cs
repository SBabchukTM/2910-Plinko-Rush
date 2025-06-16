using System;

namespace Runtime.Application.UserAccountSystem
{
    [Serializable]
    public class UserAccountData
    {
        public string Username = "Player";
        public string AvatarBase64 = String.Empty;

        public UserAccountData Copy()
        {
            return (UserAccountData)MemberwiseClone();
        }
    }
}