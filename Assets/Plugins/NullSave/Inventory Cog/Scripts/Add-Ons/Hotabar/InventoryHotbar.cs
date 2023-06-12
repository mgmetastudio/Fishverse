using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace NullSave.TOCK.Inventory
{
    [RequireComponent(typeof(InventoryCog))]
    public class InventoryHotbar : MonoBehaviour
    {

        #region Variables

        public int hotbarSize = 6;
        public bool hotbarFreeSlot = false;
        public bool hotbarDragDrop = true;

        public PageChanged onHotbarChanged;

        private InventoryCog inventory;

        #endregion

        #region Properties

        public bool Initialized { get; private set; }

        public List<InventoryHotbarSlot> HotbarSlots { get; private set; }

        #endregion

        #region Unity Methods

        private void Awake()
        {
            inventory = GetComponentInChildren<InventoryCog>();
        }

        private void OnEnable()
        {
            if(!Initialized)
            {
                HotbarSlots = new List<InventoryHotbarSlot>();
                for (int i = 0; i < hotbarSize; i++)
                {
                    HotbarSlots.Add(new InventoryHotbarSlot(inventory, i));
                }

                Initialized = true;
            }
        }

        #endregion

        #region Public Methods

        public void Load(Stream stream, InventoryCog inventory)
        {
            int count = stream.ReadInt();
            HotbarSlots = new List<InventoryHotbarSlot>();
            for (int i = 0; i < count; i++)
            {
                InventoryHotbarSlot slot = new InventoryHotbarSlot(inventory, i);
                slot.StateLoad(stream, inventory);
                HotbarSlots.Add(slot);
            }
        }

        #endregion

    }
}