using UnityEngine;
using Firebase.Database;
using Newtonsoft.Json;

namespace LibEngine.Auth
{
    public abstract class BaseFirebaseStatsController<T, IT> : BaseSchemeControllerAuthDepended<T, IT> where T : IT
    {
        protected abstract string dataKey { get; }
        private DatabaseReference _databaseReference;

        public BaseFirebaseStatsController(IAuthManager authManager, DatabaseReference databaseReference)
            : base(authManager)
        {
            _databaseReference = databaseReference;
            Initialize();
        }

        protected override void ProcessLoadingScheme()
        {
            base.ProcessLoadingScheme();
            _authManager.IsAuthorized(out string userId);
            _databaseReference.Child(dataKey).Child(userId).GetValueAsync().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("Failed to load stats: " + task.Exception);
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    var json = snapshot.GetRawJsonValue();
                    if (json != null)
                        _statsScheme = (T)JsonConvert.DeserializeObject(json, GetImplementedType());

                    _isLoading = false;

                    if (_statsScheme == null)
                    {
                        _statsScheme = (T)GetCreateNewStatsScheme();
                        Save();
                    }

                }
            });
        }

        private void UpdateSaveStatsScheme()
        {
            if (!_authManager.IsAuthorized(out string userId))
                return;

            if (_isLoading || _statsScheme == null)
                return;

            var statsRef = _databaseReference.Child(dataKey).Child(userId);

            var rawJson = JsonConvert.SerializeObject(_statsScheme);
            statsRef.SetRawJsonValueAsync(rawJson);
        }

        public override void Save()
        {
            base.Save();
            UpdateSaveStatsScheme();
        }
    }
}