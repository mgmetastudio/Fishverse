using System;
using Cysharp.Threading.Tasks;
using LibEngine.Auth;

namespace LibEngine
{
    public abstract class BaseSchemeControllerAuthDepended<T, IT> : ISchemeController<IT>  where T : IT
    {
        protected IAuthManager _authManager;

        protected T _statsScheme;

        protected bool _isLoading;

        protected const int _asyncWaitTimeOutLoad = 5;

        public BaseSchemeControllerAuthDepended(IAuthManager authManager)
        {
            _authManager = authManager;
            //Initialize();
        }

        protected virtual void Initialize()
        {
            LoadStatsScheme();
            _authManager.OnAuthUpdated += LoadStatsScheme;
            _authManager.OnAuthLogOut += OnLogOutAuth;
        }

        public IT GetScheme()
        {
            if (_isLoading)
                return default;

            if (_authManager.IsAuthorizedState())
            {
                //if (_statsScheme == null)
                //    LoadStatsScheme();
                return _statsScheme;
            }

            return default;
        }

        public async UniTask<IT> GetSchemeAsync()
        {
            if (_statsScheme == null)
            {
                var predicate = new Func<bool>(() => GetScheme() != null);
                await UniTask.WaitUntil(predicate).Timeout(TimeSpan.FromSeconds(_asyncWaitTimeOutLoad));
                _statsScheme = (T)GetScheme();
            }

            return _statsScheme;
        }

        protected virtual void OnLogOutAuth()
        {
            Reset();
        }

        protected void LoadStatsScheme()
        {
            if (!_authManager.IsAuthorizedState())
                return;

            if (_isLoading)
                return;

            _isLoading = true;
            _statsScheme = default;

            ProcessLoadingScheme(); //waiting?
        }

        protected virtual void ProcessLoadingScheme()
        {

        }

        protected virtual Type GetImplementedType()
        {
            return typeof(T);
        }

        public virtual void Save()
        {

        }

        protected abstract IT GetCreateNewStatsScheme();

        public virtual void Reset()
        {
            _statsScheme = default;
        }
    }
}