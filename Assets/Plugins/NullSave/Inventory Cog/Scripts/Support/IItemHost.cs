namespace NullSave.TOCK.Inventory
{
    public interface IItemHost
    {

        #region Properties

        InventoryCog Inventory { get; set; }

        InventoryItem InventoryItem { get; set; }

        LootItem LootItem { get; set; }

        #endregion

        #region Methods

        void UpdateChildren();

        #endregion

    }
}