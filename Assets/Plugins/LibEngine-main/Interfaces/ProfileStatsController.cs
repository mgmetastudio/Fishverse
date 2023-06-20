using UnityEngine;
using Firebase.Database;
using System;

namespace LibEngine.Auth
{
    public interface IProfileStatsController : ISchemeController<IProfileStatsScheme>
    {

    }

    public class ProfileStatsController : BaseFirebaseStatsController<ProfileStatsScheme, IProfileStatsScheme>, IProfileStatsController
    {
        public ProfileStatsController(IAuthManager authManager, DatabaseReference databaseReference)
            : base(authManager, databaseReference)
        {
        }

        protected override Type GetImplementedType()
        {
            return typeof(ProfileStatsScheme);
        }

        protected override string dataKey => "profiles";

        protected override IProfileStatsScheme GetCreateNewStatsScheme()
        {
            return new ProfileStatsScheme() { Coins = 100 };
        }
    }
}
