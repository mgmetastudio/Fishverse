using LibEngine.Meta;
using LibEngine.Repository;

namespace LibEngine.Meta
{
    public interface IShopServiceMeta : IShopService<ItemMetaPublicScheme>
    {

    }

    public class MetaShopService : ShopService<ItemMetaPublicScheme>, IShopServiceMeta
    {
        public MetaShopService(IRepositoryFull<ItemMetaPublicScheme> repository) : base(repository)
        {
        }
    }
}
