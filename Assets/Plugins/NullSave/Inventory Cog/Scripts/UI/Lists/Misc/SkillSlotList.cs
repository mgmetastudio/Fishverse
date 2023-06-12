using System.Collections.Generic;
using UnityEngine;

namespace NullSave.TOCK.Inventory
{
    public class SkillSlotList : MonoBehaviour
    {

        #region Variables

        public SkillSlotUI slotPrefab;

        private List<SkillSlotUI> loadedSlots;

        #endregion

        #region Public Methods

        public void ClearSlots()
        {
            if(loadedSlots == null)
            {
                loadedSlots = new List<SkillSlotUI>();
                return;
            }

            foreach(SkillSlotUI slot in loadedSlots)
            {
                Destroy(slot.gameObject);
            }

            loadedSlots.Clear();
        }

        public void LoadSlots(InventoryCog inventory)
        {
            ClearSlots();

            foreach(string slot in inventory.skillSlots)
            {
                SkillSlotUI slotUI = Instantiate(slotPrefab, transform);
                slotUI.slotName = slot;
                slotUI.inventorySource = inventory;
                loadedSlots.Add(slotUI);
            }
        }

        #endregion

    }
}