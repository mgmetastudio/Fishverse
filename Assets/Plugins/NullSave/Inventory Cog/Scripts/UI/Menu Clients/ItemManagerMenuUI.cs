using UnityEngine;

namespace NullSave.TOCK.Inventory
{
    public class ItemManagerMenuUI : MonoBehaviour, IItemHost, ICloseable, IMenuHost
    {

        #region Variables

        public NavigationType closeMode;
        public string closeButton = "Cancel";
        public KeyCode closeKey = KeyCode.Escape;

        #endregion

        #region Properties

        public LootItem LootItem { get; set; }

        public InventoryCog Inventory { get; set; }

        public InventoryItem InventoryItem { get; set; }

        public System.Action onCloseCalled { get; set; }

        #endregion

        #region Unity Methods

        private void Start()
        {
            Inventory.onThemeWindowOpened?.Invoke(gameObject);
        }

        public void Update()
        {
            if ((closeMode == NavigationType.ByButton && InventoryCog.GetButtonDown(closeButton)) ||
                (closeMode == NavigationType.ByKey && InventoryCog.GetKeyDown(closeKey)))
            {
                Inventory.ActiveTheme.CloseMenu();
            }
        }

        #endregion

        #region Public Methods

        public void BreakdownItem()
        {
            InventoryItem item = InventoryItem;
            if (Inventory.GetItemTotalCount(item) >= Inventory.ActiveTheme.minCount)
            {
                System.Action<bool, int> callback = new System.Action<bool, int>((bool confirm, int count) =>
                {
                    if (confirm)
                    {
                        Inventory.BreakdownItem(item, count);
                        Close();
                    }
                });

                Inventory.ActiveTheme.OpenCountSelect(item, Inventory.ActiveTheme.breakdownPrompt, callback);
            }
            else
            {
                Inventory.BreakdownItem(InventoryItem);
                Close();
            }
        }

        public void Close()
        {
            Inventory.onThemeWindowClosed?.Invoke(gameObject);
            Inventory.ActiveTheme.CloseMenu(gameObject);
        }

        public void ConsumeItem()
        {
            if (InventoryItem == null) return;
            Inventory.ConsumeItem(InventoryItem, 1);
            Close();
        }

        public void DropItem()
        {
            if (InventoryItem == null) return;

            int fullCount = Inventory.GetItemTotalCount(InventoryItem);
            if (fullCount >= Inventory.ActiveTheme.minCount)
            {
                System.Action<bool, int> onClose = new System.Action<bool, int>((bool confirm, int count) =>
                {
                    if (confirm && count > 0)
                    {
                        Inventory.DropItem(InventoryItem, count);
                        if (count >= InventoryItem.CurrentCount) Close();
                    }
                });

                Inventory.ActiveTheme.OpenCountSelect(InventoryItem, Inventory.ActiveTheme.dropPrompt, onClose);
            }
            else
            {
                Inventory.DropItem(InventoryItem, 1);
                if (InventoryItem.CurrentCount <= 1) Close();
            }
        }

        public void EquipItem()
        {
            if (InventoryItem == null) return;
            Inventory.EquipItem(InventoryItem);
            UpdateChildren();
        }

        public void LoadComponents()
        {
            MenuHostHelper.LoadComponents(this, gameObject);
        }

        public void OpenAttachmentsUI()
        {
            if (InventoryItem == null) return;

            System.Action onClose =new System.Action(() =>
            {
                UpdateChildren();
            });

            Inventory.ActiveTheme.OpenAttachments(InventoryItem, onClose);
        }

        public void OpenItemContainer()
        {
            if (InventoryItem == null || InventoryItem.itemType != ItemType.Container) return;
            Inventory.ActiveTheme.OpenItemContainer(InventoryItem, null);
        }

        public void RepairItem()
        {
            if (InventoryItem == null) return;
            Inventory.RepairItem(InventoryItem);
            UpdateChildren();
        }

        public void UnequipItem()
        {
            if (InventoryItem == null) return;
            Inventory.UnequipItem(InventoryItem);
            UpdateChildren();
        }

        public void UpdateChildren()
        {
            ItemHostHelper.UpdateChildren(this, gameObject);
            LoadComponents();
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