using UnityEngine;
using Firebase.Database;
using System;

namespace LibEngine.Auth
{
    public class UserStatsController : BaseFirebaseStatsController<UserProfileScheme, IUserProfileScheme>, IUserStatsController
    {
        public UserStatsController(IAuthManager authManager, DatabaseReference databaseReference)
            : base(authManager, databaseReference) { }

        protected override string dataKey => "users";

        protected override Type GetImplementedType()
        {
            return typeof(UserProfileScheme);
        }

        public void SetName(string name, Action onSuccess, Action<string> onError)
        {
            if (!IsValidNickname(name))
            {
                onError?.Invoke("Invalid Name");
                return;
            }

            var scheme = (UserProfileScheme)GetScheme();
            scheme.UserName = name;
            Save();
            onSuccess?.Invoke();
        }

        public bool IsValidNickname(string nickname)
        {
            int minLength = 3;
            int maxLength = 20;

            if (nickname.Length < minLength || nickname.Length > maxLength)
            {
                return false;
            }

            foreach (char c in nickname)
            {
                if (!char.IsLetterOrDigit(c) && c != '_')
                {
                    return false;
                }
            }

            return true;
        }

        protected override IUserProfileScheme GetCreateNewStatsScheme()
        {
            System.Random r = new System.Random();
            var randomValue = r.Next(0, 9999); //Max range
            var randomValueStr = randomValue.ToString();
            var userName = "Player_" + randomValueStr;
            var user = new UserProfileScheme() { UserName = userName };
            return user;
        }
    }
}