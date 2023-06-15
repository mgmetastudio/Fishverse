using System;
using System.IO;
using UnityEngine;

namespace NullSave.TOCK.Inventory
{
    [Serializable]
    public class LoadoutSkill
    {

        #region Variables

        [Tooltip("Skill item to assign")] public InventoryItem skillItem;
        [Tooltip("Skill slot to assign to")] public string skillSlotId;
        [Tooltip("Force item into inventory")] public bool forceItem;

        #endregion

        #region Public Methods

        public void Load(InventoryCog inventory, Stream stream)
        {
            skillItem = inventory.GetItemByName(stream.ReadStringPacket());
            skillSlotId = stream.ReadStringPacket();
            forceItem = stream.ReadBool();
        }

        public void Save(Stream stream)
        {
            stream.WriteStringPacket(skillItem.name);
            stream.WriteStringPacket(skillSlotId);
            stream.WriteBool(forceItem);
        }

        #endregion

    }
}