using System;

namespace NullSave.TOCK.Inventory
{
    [Serializable]
    public class InventoryItemCondition
    {

        #region Variables

        public InventoryItem item;
        public int minCount = 1;
        public int maxCount = -1;

        #endregion

        #region Public Methods

        public bool IsConditionMet(InventoryCog inventory)
        {
            int count = inventory.GetItemTotalCount(item);
            return count >= minCount && (count <= maxCount || maxCount == -1);
        }

        #endregion

    }
}