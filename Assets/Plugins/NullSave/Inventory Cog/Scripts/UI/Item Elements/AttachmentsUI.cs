using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NullSave.TOCK.Inventory
{
    public class AttachmentsUI : MonoBehaviour, IMenuHost, IItemHost
    {

        #region Variables

        public TextMeshProUGUI itemTitle;
        public Image itemIcon;

        public AttachmentList slotList;
        public AttachmentItemGrid attachmentGrid;

        private System.Action callback;

        #endregion

        #region Properties

        public InventoryCog Inventory { get; set; }

        public InventoryItem InventoryItem { get; set; }

        public LootItem LootItem { get { return null; } set {; } }

        #endregion

        #region Public Methods

        public void Close()
        {
            gameObject.SetActive(false);
            callback?.Invoke();
        }

        public void LoadComponents()
        {
            MenuHostHelper.LoadComponents(this, gameObject);
        }

        public void LoadItem(InventoryCog inventory, InventoryItem item, System.Action onClose)
        {
            Inventory = inventory;
            InventoryItem = item;
            LoadComponents();
            UpdateChildren();

            callback = onClose;

            UpdateUI(null);

            if (slotList != null)
            {
                slotList.Inventory = inventory;
                slotList.LoadSlots(InventoryItem);
            }

            if(attachmentGrid != null)
            {
                attachmentGrid.Inventory = inventory;
                attachmentGrid.LoadAttachments(InventoryItem);
            }

            item.onAttachmentAdded.AddListener(UpdateUI);
            item.onAttachmentRemoved.AddListener(UpdateUI);
        }

        public void UpdateChildren()
        {
            ItemHostHelper.UpdateChildren(this, gameObject);
        }

        #endregion

        #region Private Methods

        private void UpdateUI(InventoryItem item)
        {
            // Update UI
            if (itemTitle != null) itemTitle.text = InventoryItem.DisplayName;
            if (itemIcon != null) itemIcon.sprite = InventoryItem.icon;
        }

        #endregion

        #region Menu Save/Load Methods

        public void Load(string filename)
        {
            Inventory.InventoryStateLoad(filename);
        }

        public void Load(System.IO.Stream stream)
        {
            Inventory.InventoryStateLoad(stream);
        }

        public void Save(string filename)
        {
            Inventory.InventoryStateSave(filename);
        }

        public void Save(System.IO.Stream stream)
        {
            Inventory.InventoryStateSave(stream);
        }

        #endregion

    }
}