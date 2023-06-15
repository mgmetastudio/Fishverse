using System;

namespace NullSave.TOCK.Inventory
{
    [Serializable]
    public class ItemReferenceEx
    {

        #region Variables

        public InventoryItem item;
        public int count;
        public bool forceEquip;

        #endregion

    }
}