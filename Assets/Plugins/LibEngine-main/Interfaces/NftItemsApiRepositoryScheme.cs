using LibEngine.Auth;
using System.Net.Http;
using LibEngine.Meta;

namespace LibEngine.Client
{
    public class NftItemsApiRepositoryScheme : ApiRepositoryScheme<ItemMetaPublicScheme, ItemMetaPublicSchemeCollection>
    {
        public NftItemsApiRepositoryScheme(IAuthManager authManager, HttpClient httpClient, IAuthUserState authUserState, IAuthValidateService authValidateService) 
            : base(authManager, httpClient, authUserState, authValidateService)
        {
        }

        protected override string routeUrl { get => "/nft/items"; set => base.routeUrl = value; }
    }
}
