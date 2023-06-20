using System;
using LibEngine.Auth;
using System.Net.Http;

namespace LibEngine.Client
{
    public class MetaApiUserStatsController : ApiBaseSchemeController<UserProfileMeta, IUserProfileScheme>, IUserStatsController
    {
        public MetaApiUserStatsController(IAuthManager authManager, HttpClient httpClient, IAuthUserState authUserState, IAuthValidateService authValidateService)
            : base(authManager, httpClient, authUserState, authValidateService)
        {
            
        }

        protected override void OnLogOutAuth()
        {
            base.OnLogOutAuth();
            authUserState.Reset();
            authValidateService.Reset();
            Save();
        }

        public void SetName(string name, Action onSuccess, Action<string> onError)
        {
            if (!IsValidNickname(name))
            {
                onError?.Invoke("Invalid Name");
                return;
            }

            var scheme = (UserProfileMeta)GetScheme();
            scheme.UserName = name;
            Save();
            onSuccess?.Invoke();
        }

        public bool IsValidNickname(string nickname)
        {
            int minLength = 3;
            int maxLength = 20;

            if (nickname.Length < minLength || nickname.Length > maxLength)
                return false;

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
            var user = new UserProfileMeta() { UserName = userName };
            return user;
        }

        //public IUserProfileScheme GetStatsScheme()
        //{
        //    throw new NotImplementedException();
        //}

        //public UniTask<IUserProfileScheme> GetStatsSchemeAsync()
        //{
        //    throw new NotImplementedException();
        //}


        public override void Reset()
        {
            base.Reset();
        }
    }
}
