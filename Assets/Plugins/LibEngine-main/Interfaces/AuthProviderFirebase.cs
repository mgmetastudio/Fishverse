using System;
using Firebase.Auth;
using Zenject;
using Firebase;
using Cysharp.Threading.Tasks;

namespace LibEngine.Auth
{
    public class AuthProviderFirebase : IAuthProvider
    {
        private FirebaseAuth _firebaseAuth;

        public event Action OnAuthLogOut;

        [Inject]
        public void Inject(FirebaseApp app)
        {
            _firebaseAuth = FirebaseAuth.GetAuth(app);
            Initialize();
        }

        public void Initialize()
        {
            bool isSignOut = false;
            if (isSignOut)
                _firebaseAuth.SignOut();

            //TryAuthenticateProcess().Wait();
        }

        public bool IsAuthorized(out string userId)
        {
            var currentUser = _firebaseAuth?.CurrentUser;
            if (currentUser != null)
            {
                userId = currentUser.UserId;
                return true;
            }

            userId = null;
            return false;
        }

        public async UniTask<bool> TryAuthenticateProcess(Action<string> onSuccess, Action<string> onFailure)
        {
            if (this.IsAuthorizedState())
                return true;

            var loginTask = GetTaskAuthProcedure(); //default login
            await loginTask;

            var state = this.IsAuthorizedState();
            if (state)
                onSuccess?.Invoke(default);
            else
                onFailure?.Invoke(default);

            return this.IsAuthorizedState();
        }

        protected virtual UniTask GetTaskAuthProcedure()
        {
            return LoginAsync(null, null); //default
        }

        public async UniTask LoginAsync(Action<string> onSuccess, Action<string> onFailure)
        {
            // Implement Firebase login flow here
            try
            {
                var user = await _firebaseAuth.SignInAnonymouslyAsync();
                onSuccess?.Invoke(null);
            }
            catch (Exception ex)
            {
                onFailure?.Invoke(ex.Message);
            }
        }

        public async UniTask LoginAsync(string token, Action<string> onSuccess, Action<string> onFailure)
        {
            // Implement Firebase login with token flow here
            try
            {
                var credential = Firebase.Auth.OAuthProvider.GetCredential("google.com", token, null);
                var user = await _firebaseAuth.SignInWithCredentialAsync(credential);
                onSuccess?.Invoke(null);
            }
            catch (Exception ex)
            {
                onFailure?.Invoke(ex.Message);
            }
        }

        public async UniTask LoginAsync(string login, string password, Action<string> onSuccess, Action<string> onFailure)
        {
            try
            {
                // Implement Firebase login with email/password flow here
                var user = await _firebaseAuth.SignInWithEmailAndPasswordAsync(login, password);
                onSuccess?.Invoke(null);
            }
            catch (Exception ex)
            {
                onFailure?.Invoke(ex.Message);
            }
        }

        public async UniTask SignUpAsync(string login, string password, Action<string> onSuccess, Action<string> onFailure)
        {
            try
            {
                // Implement Firebase signup flow here
                var user = await _firebaseAuth.CreateUserWithEmailAndPasswordAsync(login, password);
                onSuccess?.Invoke(null);
            }
            catch (Exception ex)
            {
                onFailure?.Invoke(ex.Message);
            }
        }

        public bool IsPossibleLogin()
        {
            return true;
        }

        public bool IsPossibleRegister()
        {
            return true;
        }

        public bool IsPossibleGuest()
        {
            return true;
        }

        public void Logout(Action onSuccess = default, Action<string> onFailure = default)
        {
            //TODO
            OnAuthLogOut?.Invoke();
        }

        public async UniTask LogoutAsync(Action onSuccess, Action<string> onFailure)
        {
            //TODO
            OnAuthLogOut?.Invoke();
        }
    }
}
