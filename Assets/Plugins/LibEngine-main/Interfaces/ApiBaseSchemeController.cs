using System;
using LibEngine.Auth;
using System.Net.Http.Headers;
using System.Net.Http;
using Newtonsoft.Json;

namespace LibEngine.Client
{
    public abstract class ApiBaseSchemeController<T> : ApiBaseSchemeController<T, T>
    {
        public ApiBaseSchemeController(IAuthManager authManager, HttpClient httpClient, IAuthUserState authUserState, IAuthValidateService authValidateService)
            : base(authManager, httpClient, authUserState, authValidateService)
        {
            
        }
    }

    public abstract class ApiBaseSchemeController<T, IT> : BaseSchemeControllerAuthDepended<T, IT> where T : IT // <IUserProfileScheme>
    {
        protected HttpClient httpClient;
        protected IAuthUserState authUserState;
        protected IAuthValidateService authValidateService;

        protected string baseUrl;
        protected virtual string routeUrl { get; set; } = "/user/profile/";

        public ApiBaseSchemeController(IAuthManager authManager, HttpClient httpClient, IAuthUserState authUserState, IAuthValidateService authValidateService)
            : base(authManager)
        {
            this.httpClient = httpClient;
            this.authUserState = authUserState;
            this.authValidateService = authValidateService;
            baseUrl = ApiUrl.BaseUrl;
            Initialize();
        }

        protected override Type GetImplementedType()
        {
            return typeof(T);
        }

        protected virtual void SetAuth() //?todo
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authUserState.AccessToken);
        }

        protected virtual string GetUrlQuery()
        {
            return $"{baseUrl}{routeUrl}";
        }

        protected override void ProcessLoadingScheme()
        {
            SetAuth();

            _isLoading = true;
            var response = httpClient.GetAsync(GetUrlQuery()).Result;

            if (!response.IsSuccessStatusCode)
            {
                authUserState.Reset();
                _isLoading = false;
                return;
                //Raise Auth Expired Event
                //throw new Exception($"Failed to load stats scheme. Status code: {response.StatusCode}");
            }

            var responseContent = response.Content.ReadAsStringAsync().Result;

            // Парсим содержимое в объект типа T
            var schemeData = JsonConvert.DeserializeObject<T>(responseContent);

            // Сохраняем схему в поле _statsScheme
            _statsScheme = schemeData;

            _isLoading = false;
        }

        //protected abstract IT GetCreateNewStatsScheme();
        //{
        //    //System.Random r = new System.Random();
        //    //var randomValue = r.Next(0, 9999); //Max range
        //    //var randomValueStr = randomValue.ToString();
        //    //var userName = "Player_" + randomValueStr;
        //    //var user = new T() { UserName = userName };
        //    //return user;
        //}
    }
}
