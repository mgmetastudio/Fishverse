using System;
using System.IO;
using UnityEngine;

namespace NullSave.TOCK.Inventory
{
    [Serializable]
    public class LoadoutItem
    {

        #region Variables

        [Tooltip("Item to place")] public InventoryItem item;
        [Tooltip("Count to place")] public int count = 1;
        [Tooltip("Equip item")] public bool autoEquip = true;
        [Tooltip("Equip Point to use (Optional)")] public string equipPointId;
        [Tooltip("Force item into inventory")] public bool forceItem;

        #endregion

        #region Public Methods

        public void Load(InventoryCog inventory, Stream stream)
        {
            item = inventory.GetItemByName(stream.ReadStringPacket());
            count = stream.ReadInt();
            autoEquip = stream.ReadBool();
            equipPointId = stream.ReadStringPacket();
            forceItem = stream.ReadBool();
        }

        public void Save(Stream stream)
        {
            stream.WriteStringPacket(item.name);
            stream.WriteInt(count);
            stream.WriteBool(autoEquip);
            stream.WriteStringPacket(equipPointId);
            stream.WriteBool(forceItem);
        }

        #endregion

    }
}