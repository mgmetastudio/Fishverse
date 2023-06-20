using LibEngine.Items;
using LibEngine.Repository;

namespace LibEngine
{
    public interface IShopService<T> : IRepositoryFull<T> where T : IItemKeyId
    {

    }

    public class ShopService<T> : BaseSomeRepositoryDecoratorService<T>, IShopService<T> where T : IItemKeyId
    {
        public ShopService(IRepositoryFull<T> repository) : base(repository)
        {

        }
    }
}
