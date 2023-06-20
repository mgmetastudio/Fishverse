using UnityEngine;
using LibEngine.States;
using Zenject;
using UnityEngine.Events;
using LibEngine.Extensions;
using System;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;

namespace LibEngine.Auth
{
    public enum AuthorizationTypes
    {
        InitLoading,
        SignIn,
        SignUp,
        EnterNameUserStats,
        ValidateUser,
        LoginOut,
        LoginOutAuthorized
    }

    public class AuthSuccessedContinueFlowStrategy
    {
        public event Action OnContinueFlow;
        public bool IsCompletedState { get; private set; }

        public void CallContinueFlow()
        {
            OnContinueFlow?.Invoke();
            IsCompletedState = true;
        }

        public void Reset()
        {
            IsCompletedState = false;
        }
    }

    public class AuthValidationResponseDto
    {
        [JsonProperty("access")]
        public bool Access { get; set; }

        [JsonProperty("access_message")]
        public string AccessMessage { get; set; }
    }

    public class AuthMainWindowController : AuthBasePanelController
    {
        [SerializeField] private GameObjectsStateMachine<int> _panels;
        [SerializeField] private AuthorizationTypes _defaultState = AuthorizationTypes.InitLoading;

        [SerializeField] private AuthBasePanelController _initStartLoading;
        [SerializeField] private AuthBasePanelController _signInAuth;
        [SerializeField] private AuthBasePanelController _signUpAuth;
        [SerializeField] private AuthBasePanelController _enterName;
        [SerializeField] private AuthBasePanelController _validUser;
        [SerializeField] private AuthBasePanelController _validatePanel;

        [SerializeField] private AuthBasePanelController _logOutAuth;
        [SerializeField] private AuthBasePanelController _logOutAuthorized;

        [SerializeField] private UnityEvent _onLogOutDeclineEvent;

        public void HookOnOpenWindowAfterLogout()
        {
            //_authFlowStrategy.IsCompletedState = false;
            CallSetState((int)AuthorizationTypes.LoginOutAuthorized);
        }

        protected override void UpdateState()
        {
            base.UpdateState();
            StartAsync();
        }

        private async UniTask StartAsync()
        {
            var authProvider = _authManager.GetAuthProvider();
            //CallSetState((int)_defaultState);
            Action onAuthFunc = () => CallSetState((int)AuthorizationTypes.SignIn);

            await _authManager.TryAuthenticateProcess(default, default); //new

            var isAuthorized = _authManager.IsAuthorized(out string userId);
            if (isAuthorized)
                onAuthFunc = OpenValidatePanel;

            _initStartLoading.SetOnAuthirizedFunc(onAuthFunc);
            _initStartLoading.SetOnNotAuthirizedFunc(() => CallSetState((int)AuthorizationTypes.SignIn));

            _signInAuth.SetOnAuthirizedFunc(OpenValidatePanel); //OnContinueSetName //CallOpenEnterUSerStats
            //_signInAuth.SetOverrideSignUpButton(() => CallSetState((int)AuthorizationTypes.SignUp));
            _signInAuth.SetOverrideSignUpButton(OpenUrlMain);
            _signInAuth.SetIsPossibleGuest(authProvider.IsPossibleGuest());

            _signUpAuth.SetOnAuthirizedFunc(CallOpenEnterUSerStats);
            _signUpAuth.SetOnBackFunc(() => CallSetState((int)AuthorizationTypes.SignIn));

            _enterName.SetOnContinueFunc(OnContinueTrySetName); //Continue to the flow..

            _validatePanel.SetOnValidatedFunc(CallContinueAfterAuthFlow);
            _validatePanel.SetOnContinueFunc(OpenUrlMain);
            _validatePanel.SetOnBackFunc(() => CallSetState((int)AuthorizationTypes.LoginOut));

            _logOutAuth.SetOnContinueFunc(LogOutProcedure);
            _logOutAuth.SetOnBackFunc(() => CallSetState((int)AuthorizationTypes.ValidateUser));

            _logOutAuthorized.SetOnContinueFunc(LogOutProcedure);
            _logOutAuthorized.SetOnBackFunc(OnDeclineLogOut);

            _initStartLoading.CallUpdateUI();
        }

        private void OnDeclineLogOut()
        {
            _onLogOutDeclineEvent?.Invoke();
            CallContinueAfterAuthFlow();
        }

        private void LogOutProcedure()
        {
            _authManager.Logout();
            CallSetState((int)AuthorizationTypes.SignIn);
        }

        public void OpenUrlMain()
        {
            OpenUrl("https://hunter.metashooter.gg/");
        }

        public void OpenUrl(string url)
        {
            Application.OpenURL(url);
        }

        private void SetPlayerName(string name)
        {
            _userStateController.SetName(name, OpenValidatePanel, _enterName.SetDesriptionText);
        }

        private void CallContinueAfterAuthFlow()
        {
            _authFlowStrategy.CallContinueFlow();
        }

        private void OpenValidatePanel()
        {
            CallSetState((int)AuthorizationTypes.ValidateUser);
            //_validatePanel.
        }

        private void CallOpenEnterUSerStats()
        {
            CallSetState((int)AuthorizationTypes.EnterNameUserStats);
        }

        private void OnContinueTrySetName()
        {
            SetPlayerName(_enterName.GetTextInputField());
        }

        private void OnContinueSetName()
        {
            var userName = _userStateController.GetScheme().UserName;
            SetPlayerName(userName);
        }

        public void CallSetState(int state)
        {
            _panels.State = state;
        }

        protected override void OnAuthorizedHook()
        {
            //base.OnAuthorizedHook();
            if (_authFlowStrategy.IsCompletedState)
                return;

            //TryIncrementCoins();
            base.OnAuthorizedHook(); //?

            var userStats = _userStateController.GetScheme();
            var existName = userStats != null && !userStats.UserName.IsNullOrEmpty();

            if (existName)
                OnContinueTrySetName();
            else
                CallOpenEnterUSerStats();
        }

        protected override void OnNotAuthorizedHook()
        {
            base.OnNotAuthorizedHook();
        }

        private void TryIncrementCoins() //test
        {
            var profile = _profileStateController.GetScheme() as ProfileStatsScheme;
            if (profile == null)
                return;

            profile.Coins++;
            _profileStateController.Save();
        }
    }
}