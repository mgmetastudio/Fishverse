using System;
using LibEngine.Auth;
using System.Net.Http;
using Newtonsoft.Json;
using LibEngine.Repository;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

namespace LibEngine.Client
{

    public class ApiRepositoryScheme<Titem, T> : ApiRepositoryScheme<Titem, T, T> where T : IRepositoryFull<Titem>
    {
        public ApiRepositoryScheme(IAuthManager authManager, HttpClient httpClient, IAuthUserState authUserState, IAuthValidateService authValidateService)
            : base(authManager, httpClient, authUserState, authValidateService)
        {
        }
    }

    public class ApiRepositoryScheme<Titem, T, IT> : ApiBaseSchemeController<T, IT>, IRepositoryFull<Titem> where T : IT where IT : IRepositoryFull<Titem>
    {
        private IRepositoryFull<Titem> repository;

        public ApiRepositoryScheme(IAuthManager authManager, HttpClient httpClient, IAuthUserState authUserState, IAuthValidateService authValidateService)
            : base(authManager, httpClient, authUserState, authValidateService)
        {
        }

        protected void TryBuildRepository()
        {
            if(repository == null)
            {
                repository = new RepositoryFullList<Titem>();
            }
        }


        protected override void ProcessLoadingScheme()
        {
            base.ProcessLoadingScheme();
            var scheme = GetScheme();

            TryBuildRepository();
            repository.AddItems(scheme.GetItems());
        }

        //protected async UniTask<T> GetSchemeAsync()
        //{
        //    //SetAuth();

        //    var response = await httpClient.GetAsync(GetUrlQuery()); //
        //    response.EnsureSuccessStatusCode();

        //    var responseContent = await response.Content.ReadAsStringAsync();

        //    var schemeData = JsonConvert.DeserializeObject<T>(responseContent);

        //    return schemeData;
        //}

        //protected override void ProcessLoadingScheme()
        //{
        //    var schemeData = GetSchemeAsync().GetAwaiter().GetResult();
        //    repository.AddItem(schemeData);
        //}

        public ICollection<Titem> GetItems()
        {
            return repository.GetItems();
        }

        public void AddItem(Titem item)
        {
            repository.AddItem(item);
        }

        public void RemoveItem(Titem item)
        {
            repository.RemoveItem(item);
        }

        public Titem GetItemByExpression(Func<Titem, bool> expressionPredicate)
        {
            return repository.GetItemByExpression(expressionPredicate);
        }

        public ICollection<Titem> GetItemsByExpression(Func<Titem, bool> expressionPredicate)
        {
            return repository.GetItemsByExpression(expressionPredicate);
        }

        protected override IT GetCreateNewStatsScheme()
        {
            return default; //?
        }

        public void AddItems(ICollection<Titem> _items)
        {
            repository.AddItems(_items);
        }
    }
}
