using Cysharp.Threading.Tasks;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using LibEngine.States;
using UnityEngine.Events;
using LibsEngine.States.Implements;
using LibsEngine.States;

namespace LibEngine.Auth
{
    public class AuthBasePanelController : MonoBehaviour
    {
        [Header("InputFields")]
        [SerializeField] protected TMP_InputField _loginInputField;
        [SerializeField] protected TMP_InputField _passwordInputField;
        [SerializeField] protected HidenInputStateMachineMonobeh _hidenInputStates;

        [Header("Buttons")]
        [SerializeField] private Button _signInButton;
        [SerializeField] private Button _signUpButton;
        [SerializeField] private Button _asGuestButton;

        [SerializeField] private Button _continueButton;
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _refreshButton;
        [SerializeField] private Button _logoutButton;

        [Header("Texts")]
        [SerializeField] protected TMP_Text _descriptionInfoText;
        [SerializeField] protected TMP_Text _userNameText; //-
        [SerializeField] protected TMP_Text _profileStatsText; //-

        [Header("Other")]
        [SerializeField] private bool isSelfInitInStart;
        [SerializeField] private UnityEvent<bool> _onAuthorizedState;
        [SerializeField] protected BaseStateMachine<bool> _isOnAuthorizedState;
        [SerializeField] protected BaseStateMachine<bool> _isTaskInProgress;

        [SerializeField] private bool _isProvideValidateProcedure;

        [Inject] protected IAuthManager _authManager;
        [Inject] protected IProfileStatsController _profileStateController;
        [Inject] protected IUserStatsController _userStateController;
        [Inject] protected AuthSuccessedContinueFlowStrategy _authFlowStrategy;
        [Inject] protected IAuthValidateService _authValidateService;

        private Action onAuthorizedFunc;
        private Action onNotAuthorizedFunc;

        private Action onValidatedFunc;
        private Action onNotValidatedFunc;

        private Action onContinueButton;
        private Action onBackButton;

        protected UniTask currentTask;
        protected bool _isCurrentExistTask { get => _isTaskInProgress.State; set => _isTaskInProgress.State = value; }

        public void SetIsPossibleGuest(bool state)
        {
            _asGuestButton.gameObject.SetActive(state);
        }

        public void HideInputToggle()
        {
            _hidenInputStates.SetInvertedState();
        }

        public string GetTextInputField()
        {
            return _loginInputField.text;
        }

        public void SetOnAuthirizedFunc(Action act)
        {
            onAuthorizedFunc = act;
        }

        public void SetOnNotAuthirizedFunc(Action act)
        {
            onNotAuthorizedFunc = act;
        }

        public void SetOnValidatedFunc(Action act)
        {
            onValidatedFunc = act;
        }

        public void SetOnNotValidatedFunc(Action act)
        {
            onNotValidatedFunc = act;
        }

        public void SetOnContinueFunc(Action act)
        {
            onContinueButton = act;
        }

        public void SetOnBackFunc(Action act)
        {
            onBackButton = act;
        }

        public void SetDesriptionText(string descriptionText)
        {
            _descriptionInfoText.text = descriptionText;
        }

        Action OnOverridedOnSignInButton;
        protected void OnSignInButtonClick()
        {
            if (OnOverridedOnSignInButton != null)
            {
                OnOverridedOnSignInButton?.Invoke();
                return;
            }

            OnSignInButton();
        }

        Action OnOverridedOnSignUpButton;
        protected void OnSignUpButtonClick()
        {
            if(OnOverridedOnSignUpButton!=null)
            {
                OnOverridedOnSignUpButton?.Invoke();
                return;
            }

            OnSignUpButton();
        }

        Action OnOverridedOnGuestButton;
        protected void OnAsGuestButtonClick()
        {
            if (OnOverridedOnGuestButton != null)
            {
                OnOverridedOnGuestButton?.Invoke();
                return;
            }

            OnAsGuestButton();
        }

        protected void OnSignInButton()
        {
            var login = _loginInputField.text;
            var password = _passwordInputField.text;

            OnButtonCallExecute(() => _authManager.LoginAsync(login, password, onSuccess: token => CallUpdateUI(), SetDesriptionText));
        }

        protected void OnSignUpButton()
        {
            var login = _loginInputField.text;
            var password = _passwordInputField.text;

            OnButtonCallExecute(() => _authManager.SignUpAsync(login, password, onSuccess: token => CallUpdateUI(), SetDesriptionText));
        }

        protected void OnAsGuestButton()
        {
            OnButtonCallExecute(() => _authManager.LoginAsync(onSuccess: token => CallUpdateUI(), SetDesriptionText));
        }

        protected void OnLogoutButton()
        {
            OnButtonCallExecute(() => _authManager.Logout(FuncLogOut, SetDesriptionText));
        }

        private void FuncLogOut()
        {
            //CallUpdateUI(); //?

            onContinueButton?.Invoke();
        }

        protected virtual void OnContinueButtonClick()
        {
            OnButtonCallExecute(() => onContinueButton?.Invoke());
        }

        protected virtual void OnBackButtonClick()
        {
            OnButtonCallExecute(() => onBackButton?.Invoke());
        }

        private void OnButtonCallExecute(Action act)
        {
            if (_isCurrentExistTask)
                return;

            act?.Invoke();
        }

        private void Awake()
        {
            _signInButton?.onClick.AddListener(OnSignInButtonClick);
            _signUpButton?.onClick.AddListener(OnSignUpButtonClick);
            _asGuestButton?.onClick.AddListener(OnAsGuestButtonClick);
            _continueButton?.onClick.AddListener(OnContinueButtonClick);
            _backButton?.onClick.AddListener(OnBackButtonClick);
            _logoutButton?.onClick.AddListener(OnLogoutButton);
        }

        protected virtual void OnEnable()
        {
            UpdateState();
        }

        protected virtual void UpdateState()
        {
            _descriptionInfoText.text = null;

            _hidenInputStates.SetState(true, true);

            if (!isSelfInitInStart)
                return;

            CallUpdateUI();
        }

        private void OnDestroy()
        {
            _signInButton?.onClick.RemoveListener(OnSignInButtonClick);
            _signUpButton?.onClick.RemoveListener(OnSignUpButtonClick);
            _asGuestButton?.onClick.RemoveListener(OnAsGuestButtonClick);
            _continueButton?.onClick.RemoveListener(OnContinueButtonClick);
            _backButton?.onClick.RemoveListener(OnBackButtonClick);
            _logoutButton?.onClick.RemoveListener(OnLogoutButton);
        }

        public void SetOverrideSignUpButton(Action act)
        {
            OnOverridedOnSignUpButton = act;
        }

        public void CallUpdateUI()
        {
            if (_isCurrentExistTask)
                return;

            if(_isProvideValidateProcedure)
                currentTask = ValidateUserUI();
             else
                currentTask = UpdateUI();
        }

        protected async UniTask UpdateUI()
        {
            _isCurrentExistTask = true;

            try
            {
                var isAuthorized = _authManager.IsAuthorized(out string userId);

                if (isAuthorized)
                {
                    var profileStatsScheme = _profileStateController.GetScheme();
                    if (profileStatsScheme == null)
                        profileStatsScheme = await _profileStateController.GetSchemeAsync();

                    var userStatsScheme = _userStateController.GetScheme();
                    if (userStatsScheme == null)
                        userStatsScheme = await _userStateController.GetSchemeAsync();

                    OnAuthorizedHook();
                }
                else
                {
                    OnNotAuthorizedHook();
                }
                _onAuthorizedState?.Invoke(isAuthorized); //_userNameText.gameObject.SetActive(isAuthorized);
                _isOnAuthorizedState.State = isAuthorized;
            }
            catch (Exception)
            {
                //OnNotAuthorizedHook();
            }

            _isCurrentExistTask = false;
        }

        private async UniTask ValidateUserUI()
        {
            _isCurrentExistTask = true;

            try
            {
                var isValidated = await _authValidateService.IsValidAsync(SetDesriptionText);

                if (isValidated)
                {
                    OnValidateHook();
                }
                else
                {
                    OnNotValidateHook();
                }
                _isCurrentExistTask = false;
                _onAuthorizedState?.Invoke(isValidated); //_userNameText.gameObject.SetActive(isAuthorized);
                _isOnAuthorizedState.State = isValidated;
                return;
            }
            catch (Exception)
            {
                //OnNotAuthorizedHook();
            }

            _isCurrentExistTask = false;
        }

        protected virtual void OnAuthorizedHook()
        {
            if (_authFlowStrategy.IsCompletedState)
                return;

            var isAuthorized = _authManager.IsAuthorized(out string userId);

            if(_userNameText)
                _userNameText.text = $"Signed in as {userId}";
            var profileStatsScheme = _profileStateController.GetScheme();
            var userStatsScheme = _userStateController.GetScheme();

            if (profileStatsScheme != null)
            {
                // Update UI with profile stats
                if(_profileStatsText)
                    _profileStatsText.text = "Balance: " + profileStatsScheme.Coins.ToString(); //COINS FOR TEST PARAM
            }

            if (userStatsScheme != null)
            {
                // Update UI with user stats
                if (_userNameText)
                    _userNameText.text = userStatsScheme.UserName;
            }

            onAuthorizedFunc?.Invoke();
            //TryIncrementCoins();
        }

        protected virtual void OnNotAuthorizedHook()
        {
            if (_authFlowStrategy.IsCompletedState)
                return;

            onNotAuthorizedFunc?.Invoke();
            //_userNameText.text = "Not signed in";
        }

        protected virtual void OnValidateHook()
        {
            if (_authFlowStrategy.IsCompletedState)
                return;

            onValidatedFunc?.Invoke();
            //TryIncrementCoins();
        }

        protected virtual void OnNotValidateHook()
        {
            if (_authFlowStrategy.IsCompletedState)
                return;

            onNotValidatedFunc?.Invoke();
            //_userNameText.text = "Not signed in";
        }
    }
}