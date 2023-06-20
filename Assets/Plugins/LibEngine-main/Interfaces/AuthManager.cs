using System;
using Zenject;
using Cysharp.Threading.Tasks;

namespace LibEngine.Auth
{
    public interface IAuthInfo
    {
        bool IsAuthorized(out string userId);
    }

    public static class AuthInfoExtensions
    {
        public static bool IsAuthorizedState(this IAuthInfo authInfo)
        {
            return authInfo.IsAuthorized(out _);
        }
    }

    public interface IAuthManager : IAuthLogic
    {
        event Action OnAuthUpdated;

        IAuthProvider GetAuthProvider();
    }

    public interface IAuthLogic : IAuthInfo, IAuthLogin, IAuthSignUp, IAuthLogout
    {
        UniTask<bool> TryAuthenticateProcess(Action<string> onSuccess, Action<string> onFailure);
    }

    public interface IAuthSignUp
    {
        /// <summary>
        /// Registers a new user with the specified login/email and password.
        /// </summary>
        /// <param name="login">Login/email of user</param>
        /// <param name="password">Password of the new user.</param>
        /// <param name="onSuccess">Action called on successful sign uo. Returns an authentication token</param>
        /// <param name="onFailure">Action called on in case of an error. Return error string</param>
        UniTask SignUpAsync(string login, string password, Action<string> onSuccess, Action<string> onFailure);
    }

    public interface IAuthLogin
    {
        UniTask LoginAsync(Action<string> onSuccess, Action<string> onFailure);
        UniTask LoginAsync(string token, Action<string> onSuccess, Action<string> onFailure);
        UniTask LoginAsync(string login, string password, Action<string> onSuccess, Action<string> onFailure);
    }

    public interface IAuthLogout
    {
        event Action OnAuthLogOut;
        void Logout(Action onSuccess = default, Action<string> onFailure = default);

        UniTask LogoutAsync(Action onSuccess, Action<string> onFailure);
    }

    public interface IAuthAvailability
    {
        bool IsPossibleLogin();
        bool IsPossibleRegister();
        bool IsPossibleGuest();
    }

    public interface IAuthProvider : IAuthLogic, IAuthAvailability, IInitializable
    {
        
    }

    public class AuthManager : IAuthManager
    {
        public event Action OnAuthUpdated;
        public event Action OnAuthLogOut;

        public IAuthProvider GetAuthProvider() => _authProvider;

        private readonly IAuthProvider _authProvider;
        private readonly AuthSuccessedContinueFlowStrategy _authFlowStrategy;

        public AuthManager(IAuthProvider authProvider, AuthSuccessedContinueFlowStrategy authFlowStrategy)
        {
            _authProvider = authProvider;
            _authFlowStrategy = authFlowStrategy;
        }

        private void RaiseSuccessAuth(string token, Action<string> onSuccess)
        {
            OnAuthUpdated?.Invoke();
            onSuccess?.Invoke(token);
        }

        private void RaiseLogoutAuth(Action onSuccess)
        {
            _authFlowStrategy.Reset();
            OnAuthLogOut?.Invoke();
            onSuccess?.Invoke();
        }

        public bool IsAuthorized(out string userId) => _authProvider.IsAuthorized(out userId);

        public async UniTask LoginAsync(Action<string> onSuccess, Action<string> onFailure)
        {
            await _authProvider.LoginAsync((token) => { RaiseSuccessAuth(token, onSuccess); }, onFailure);;
        }

        public async UniTask LoginAsync(string token, Action<string> onSuccess, Action<string> onFailure)
        {
            await _authProvider.LoginAsync(token, (token) => { RaiseSuccessAuth(token, onSuccess); }, onFailure);
        }

        public async UniTask LoginAsync(string login, string password, Action<string> onSuccess, Action<string> onFailure)
        {
            await _authProvider.LoginAsync(login, password, (token) => { RaiseSuccessAuth(token, onSuccess); }, onFailure);
        }

        public async UniTask SignUpAsync(string login, string password, Action<string> onSuccess, Action<string> onFailure)
        {
            await _authProvider.SignUpAsync(login, password, (token) => { RaiseSuccessAuth(token, onSuccess); }, onFailure);
        }

        public async UniTask<bool> TryAuthenticateProcess(Action<string> onSuccess, Action<string> onFailure)
        {
            return await _authProvider.TryAuthenticateProcess((token) => { RaiseSuccessAuth(token, onSuccess); }, onFailure);
        }

        public void Logout(Action onSuccess = default, Action<string> onFailure = default)
        {
            LogoutAsync(onSuccess, onFailure);
            //GetAuthProvider().Logout(onSuccess, onFailure);
            //OnAuthLogout?.Invoke();
        }

        public async UniTask LogoutAsync(Action onSuccess, Action<string> onFailure)
        {
            await _authProvider.LogoutAsync(() => { RaiseLogoutAuth(onSuccess); }, onFailure);
        }
    }
}
