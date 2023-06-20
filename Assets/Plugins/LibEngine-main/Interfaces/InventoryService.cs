using LibEngine.Meta;
using LibEngine.Items;
using LibEngine.Repository;

namespace LibEngine
{
    public interface IInventoryService : IRepositoryFull<OwnItem>
    {

    }

    public class InventoryService : BaseSomeRepositoryDecoratorService<OwnItem>, IInventoryService
    {
        public InventoryService(IRepositoryFull<OwnItem> repository) : base(repository)
        {
        }
    }
}
