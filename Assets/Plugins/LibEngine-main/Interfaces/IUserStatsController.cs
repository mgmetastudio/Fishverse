using System;

namespace LibEngine.Auth
{
    public interface IUserStatsController : ISchemeController<IUserProfileScheme>
    {
        void SetName(string name, Action onSuccess, Action<string> onError);
    }
}