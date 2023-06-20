using System;
using Cysharp.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net;

namespace LibEngine.Auth
{
    public interface IAuthUserState : IAuthRefreshToken
    {
        public string CurrentUserId { get; set; }
        public string AccessToken { get; set; }

        public void Reset();
    }

    public interface IAuthRefreshToken
    {
        public string RefreshToken { get; set; }
    }

    public static class AuthUserStateExtension
    {
        public static void SetLoginResponseDTO(this IAuthUserState authState, LoginResponseDTO login)
        {
            authState.AccessToken = login.AccessToken;
            authState.CurrentUserId = login.User.Uuid;
            authState.RefreshToken = login.RefreshToken; //optional
        }
    }

    public class LoginRequestDTO
    {
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }
    }

    public class LoginResponseDTO
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }

        [JsonProperty("user")]
        public UserDTO User { get; set; }
    }

    public class UserDTO
    {
        [JsonProperty("pk")]
        public int Pk { get; set; }

        [JsonProperty("uuid")]
        public string Uuid { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }
    }

    public class SignUpRequestDTO
    {
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("password1")]
        public string Password1 { get; set; }

        [JsonProperty("password2")]
        public string Password2 { get; set; }
    }

    public class SignUpResponseDTO
    {
        [JsonProperty("key")]
        public string Token { get; set; }
    }

    public class TokenResponseDTO
    {
        [JsonProperty("refresh")]
        public string RefreshToken { get; set; }

        [JsonProperty("access")]
        public string AccessToken { get; set; }
    }

    public class AuthUserState : IAuthUserState
    {
        public string CurrentUserId { get; set; }
        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }

        public void Reset()
        {
            CurrentUserId = default;
            AccessToken = default;
            RefreshToken = default;
        }
    }

    public class ApiUrl //todo: config
    {
        public static string BaseUrl = "https://api.hunter.metashooter.gg";
    }

    public class AuthApiProvider : IAuthProvider
    {
        public event Action OnAuthLogOut;

        private ISaveable<AuthUserState> saveableAuthUserState;
        private IAuthUserState authUserState;
        private HttpClient httpClient;

        private string baseUrl;

        private const string authKeyRoute = "rest-auth";

        public AuthApiProvider(HttpClient httpClient, IAuthUserState authUserState, ISaveable<AuthUserState> saveableAuthUserState)
        {
            this.authUserState = authUserState;
            this.saveableAuthUserState = saveableAuthUserState;
            this.httpClient = httpClient;
            this.baseUrl = ApiUrl.BaseUrl; //baseUrl;
        }

        public UniTask LoginAsync(Action<string> onSuccess, Action<string> onFailure)
        {
            return default;
        }

        public void Initialize()
        {
        }

        public bool IsAuthorized(out string userId)
        {
            var currentUserId = authUserState.CurrentUserId;

            if (currentUserId != default)
            {
                userId = currentUserId;
                return true;
            }

            userId = null;
            return false;
        }

        public async UniTask<(bool, string)> IsAuthorized(bool isForciblyRelogin = false)
        {
            bool isAuthExpired = false;

            if (isForciblyRelogin)
            {
                await LoginAsync(authUserState.AccessToken, default, (x) => { isAuthExpired = true; });
                if(isAuthExpired)
                {
                    authUserState.Reset();
                    return (false, default);
                }
            }

            var currentUserId = authUserState.CurrentUserId;

            if (currentUserId != default)
            {
                return (true, currentUserId);
            }

            return (false, default);
        }

        public bool IsAuthorizedState()
        {
            return IsAuthorized(out _);
        }

        public bool IsPossibleLogin()
        {
            return true;
        }

        public bool IsPossibleRegister()
        {
            return true; //false
        }

        public async UniTask<bool> TryAuthenticateProcess(Action<string> onSuccess, Action<string> onFailure)
        {
            if (this.IsAuthorizedState())
                return true;

            var loginTask = GetTaskAuthProcedure(); //default login
            await loginTask;

            var isAuthorizedState = this.IsAuthorizedState();

            if (isAuthorizedState)
            {
                authUserState.Reset();
                onSuccess?.Invoke(authUserState.AccessToken);
            }
            else
                onFailure?.Invoke(default);

            return isAuthorizedState;
        }

        protected virtual UniTask GetTaskAuthProcedure()
        {
            return LoginAsync(authUserState.AccessToken, default, default); //TryRefreshTokenUse()
        }

        protected virtual UniTask TryRefreshTokenUse()
        {
            return default;
        }

        public async UniTask LoginAsync(LoginRequestDTO loginRequest, Action<string> onSuccess, Action<string> onFailure)
        {
            var loginUrl = $"{baseUrl}/{authKeyRoute}/login/";

            try
            {
                var requestBody = JsonConvert.SerializeObject(loginRequest);
                var requestContent = new StringContent(requestBody, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync(loginUrl, requestContent);
                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    var loginResponse = JsonConvert.DeserializeObject<LoginResponseDTO>(responseBody);

                    authUserState.SetLoginResponseDTO(loginResponse);
                    saveableAuthUserState.Save();
                    SetAuth(authUserState.AccessToken);

                    onSuccess?.Invoke(loginResponse.AccessToken);
                }
                else
                {
                    onFailure?.Invoke(GetErrorMessage(await response.Content.ReadAsStringAsync()));
                }
            }
            catch (Exception e)
            {
                onFailure?.Invoke(e.Message);
            }
        }

        private void SetAuth(string token)
        {
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        public string GetErrorMessage(string errorResponse)
        {
            try
            {
                var json = JObject.Parse(errorResponse);

                // check for non_field_errors
                if (json["non_field_errors"] != null)
                {
                    var errors = json["non_field_errors"].ToObject<List<string>>();
                    return string.Join("\n", errors);
                }

                // check for other errors
                var messages = new List<string>();
                foreach (var property in json.Properties())
                {
                    if (property.Value.Type == JTokenType.Array)
                    {
                        var errors = property.Value.ToObject<List<string>>();
                        var message = $"{property.Name}: {string.Join(", ", errors)}";
                        messages.Add(message);
                    }
                }
                return string.Join("\n", messages);
            }
            catch (Exception)
            {
                return errorResponse;
            }
        }

        public async UniTask LoginAsync(string token, Action<string> onSuccess, Action<string> onFailure)
        {
            var loginUrl = $"{baseUrl}/{authKeyRoute}/login/";
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            try
            {
                var response = await httpClient.PostAsync(loginUrl, null);
                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    var loginResponse = JsonConvert.DeserializeObject<LoginResponseDTO>(responseBody);
                    authUserState.SetLoginResponseDTO(loginResponse);
                    saveableAuthUserState.Save();

                    SetAuth(authUserState.AccessToken);
                    onSuccess?.Invoke(loginResponse.AccessToken);
                }
                else
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();

                    if (errorResponse.Contains("token_not_valid"))
                    {
                        await RefreshTokenProcedure(token, onSuccess, onFailure);
                    }
                    else
                    {
                        onFailure?.Invoke(errorResponse);
                    }
                }
            }
            catch (Exception e)
            {
                onFailure?.Invoke(e.Message);
            }
            finally
            {
                httpClient.DefaultRequestHeaders.Authorization = null;
            }
        }

        public async UniTask RefreshTokenProcedure(string refreshToken, Action<string> onSuccess, Action<string> onFailure)
        {
            var refreshTokenUrl = $"{baseUrl}/{authKeyRoute}/token/refresh/";
            var requestBody = new
            {
                refresh = refreshToken
            };
            var jsonBody = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            try
            {
                var response = await httpClient.PostAsync(refreshTokenUrl, content);
                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    var tokenResponse = JsonConvert.DeserializeObject<TokenResponseDTO>(responseBody);
                    authUserState.AccessToken = tokenResponse.AccessToken;
                    saveableAuthUserState.Save();
                    onSuccess?.Invoke(tokenResponse.AccessToken);
                }
                else
                {
                    onFailure?.Invoke(await response.Content.ReadAsStringAsync());
                }
            }
            catch (Exception e)
            {
                onFailure?.Invoke(e.Message);
            }
        }

        public async UniTask LoginAsync(string login, string password, Action<string> onSuccess, Action<string> onFailure)
        {
            var loginUrl = $"{baseUrl}/{authKeyRoute}/login/";
            var loginRequest = new LoginRequestDTO
            {
                Email = login,
                Password = password
            };

            await LoginAsync(loginRequest, onSuccess, onFailure);
        }

        public async UniTask SignUpAsync(string login, string password, Action<string> onSuccess, Action<string> onFailure)
        {
            var signUpUrl = $"{baseUrl}/{authKeyRoute}/registration/";
            var signUpRequest = new SignUpRequestDTO
            {
                Email = login,
                Password1 = password,
                Password2 = password
            };

            try
            {
                var requestBody = JsonConvert.SerializeObject(signUpRequest);
                var requestContent = new StringContent(requestBody, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync(signUpUrl, requestContent);
                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    var signUpResponse = JsonConvert.DeserializeObject<SignUpResponseDTO>(responseBody);
                    SetAuth(signUpResponse.Token);
                    onSuccess?.Invoke(signUpResponse.Token);
                }
                else
                {
                    onFailure?.Invoke(await response.Content.ReadAsStringAsync());
                }
            }
            catch (Exception e)
            {
                onFailure?.Invoke(e.Message);
            }
        }

        public bool IsPossibleGuest()
        {
            return false;
        }

        public void Logout(Action onSuccess = default, Action<string> onFailure = default)
        {
            LogoutAsync(onSuccess, onFailure).Forget();
        }

        public async UniTask LogoutAsync(Action onSuccess, Action<string> onFailure)
        {
            var logoutUrl = $"{baseUrl}/rest-auth/logout/";

            try
            {
                var response = await httpClient.PostAsync(logoutUrl, null);

                if (response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.NoContent)
                {
                    onSuccess?.Invoke();
                    authUserState.Reset(); //?
                    saveableAuthUserState.Save();
                    OnAuthLogOut?.Invoke();
                }
                else
                {
                    onFailure?.Invoke(GetErrorMessage(await response.Content.ReadAsStringAsync()));
                }
            }
            catch (Exception e)
            {
                onFailure?.Invoke(e.Message);
            }
        }

    }
}
