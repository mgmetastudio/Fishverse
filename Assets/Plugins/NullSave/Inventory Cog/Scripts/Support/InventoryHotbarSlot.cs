using System.IO;

namespace NullSave.TOCK.Inventory
{
    public class InventoryHotbarSlot 
    {

        #region Variables

        private InventoryCog inventory;
        private int index;

        #endregion

        #region Constructors

        public InventoryHotbarSlot(InventoryCog inventory, int index)
        {
            this.index = index;
            this.inventory = inventory;
        }

        #endregion

        #region Properties

        public InventoryItem AssignedItem { get; private set; }

        #endregion

        #region Public Methods

        public void AssignItem(InventoryItem item)
        {
            if (item != null && !item.allowHotbar) return;

            AssignedItem = item;
            inventory.Hotbar.onHotbarChanged?.Invoke(index);
        }

        /// <summary>
        /// Load slot state
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="inventory"></param>
        public void StateLoad(Stream stream, InventoryCog inventory)
        {
            string instance = stream.ReadStringPacket();
            if(instance != string.Empty)
            {
                AssignedItem = inventory.GetItemByInstanceId(instance);
            }
        }

        /// <summary>
        /// Save slot state
        /// </summary>
        /// <param name="stream"></param>
        public void StateSave(Stream stream)
        {
            if (AssignedItem == null)
            {
                stream.WriteStringPacket(string.Empty);
            }
            else
            {
                stream.WriteStringPacket(AssignedItem.InstanceId);
            }
        }

        #endregion

    }
}