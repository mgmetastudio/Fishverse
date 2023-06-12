using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace NullSave.TOCK.Inventory
{
    [HierarchyIcon("tock-menu", false)]
    public class ContainerMenuUI : MonoBehaviour, IMenuHost
    {

        #region Variables

        // Closing
        public NavigationType closeMode = NavigationType.ByButton;
        public string closeButton = "Cancel";
        public KeyCode closeKey = KeyCode.Escape;

        // Load type
        public ListLoadMode loadMode = ListLoadMode.OnEnable;

        // Player inventory
        public InventoryCog inventory;
        public InventoryItemList localInventory;

        // Container inventory
        public InventoryContainer container;
        public InventoryItemList containerInventory;
        public TextMeshProUGUI containerName;

        // Events
        public UnityEvent onOpen, onClose, onIsEmpty;

        #endregion

        #region Properties

        public InventoryContainer Container
        {
            get { return container; }
            set { container = value; }
        }

        public InventoryCog Inventory
        {
            get { return inventory; }
            set { inventory = value; }
        }

        public InventoryItemList LocalInventory
        {
            get { return localInventory; }
            set { localInventory = value; }
        }

        public InventoryItemList ContainerInventory
        {
            get { return containerInventory; }
            set { containerInventory = value; }
        }

        #endregion

        #region Unity Methods

        private void OnEnable()
        {
            LoadComponents();

            if (loadMode == ListLoadMode.OnEnable)
            {
                RefreshLocalInventory();
                RefreshContainerInventory();
            }

            onOpen?.Invoke();
        }

        private void Start()
        {
            Inventory.onThemeWindowOpened?.Invoke(gameObject);
        }

        private void Update()
        {
            switch (closeMode)
            {
                case NavigationType.ByButton:
                    if (InventoryCog.GetButtonDown(closeButton))
                    {
                        Close();
                    }
                    break;
                case NavigationType.ByKey:
                    if (InventoryCog.GetKeyDown(closeKey))
                    {
                        Close();
                    }
                    break;
            }
        }

        #endregion

        #region Public Methods

        public void Close()
        {
            Inventory.ActiveTheme.CloseMenu(gameObject);
            Inventory.onThemeWindowClosed?.Invoke(gameObject);
            onClose?.Invoke();
        }

        public void LoadComponents()
        {
            MenuHostHelper.LoadComponents(this, gameObject);
        }

        public void RefreshAll()
        {
            RefreshLocalInventory();
            RefreshContainerInventory();
        }

        public void RefreshLocalInventory()
        {
            if (localInventory != null)
            {
                if (Inventory != null)
                {
                    localInventory.Inventory = Inventory;
                    localInventory.LoadItems();
                }
            }
        }

        public void RefreshContainerInventory()
        {
            if (containerInventory != null)
            {
                if (Container != null)
                {
                    containerInventory.Container = Container;
                    containerInventory.LoadItems();
                    if (Container.StoredItems.Count == 0) onIsEmpty?.Invoke();

                    if(containerName != null)
                    {
                        containerName.text = Container.displayName;
                    }
                }
            }
        }

        public void StoreSelected()
        {
            ItemUI itemUI = localInventory.SelectedItem;
            if (itemUI == null) return;

            container.AddStoredItem(itemUI.Item, itemUI.Item.CurrentCount);
            Inventory.RemoveItem(itemUI.Item, itemUI.Item.CurrentCount);

            localInventory.ReloadLast();
            containerInventory.ReloadLast();
        }

        public void StoreAllWithMaxRarity(int maxRarity)
        {
            List<InventoryItem> toRemove = new List<InventoryItem>();
            foreach(InventoryItem item in Inventory.Items)
            {
                if(item.rarity <= maxRarity)
                {
                    if (container.AddStoredItem(item, item.CurrentCount))
                    {
                        toRemove.Add(item);
                    }
                }
            }

            foreach(InventoryItem item in toRemove)
            {
                Inventory.RemoveItem(item, item.CurrentCount);
            }
        }

        public void TakeAll()
        {
            if (Container.StoredItems.Count == 0) return;

            foreach (InventoryItem item in Container.StoredItems)
            {
                Inventory.AddToInventory(item, item.CurrentCount);
            }
            container.ClearItems();

            if (localInventory != null) localInventory.ReloadLast();
            if (containerInventory != null)
            {
                containerInventory.ReloadLast();
                containerInventory.SelectedIndex = -1;
            }
        }

        public void TakeSelected()
        {
            ItemUI itemUI = containerInventory.SelectedItem;
            if (itemUI == null || itemUI.Item == null) return;
            
            Inventory.AddToInventory(itemUI.Item, itemUI.Item.CurrentCount);
            Container.RemoveItem(itemUI.Item, itemUI.Item.CurrentCount);

            if (localInventory != null) localInventory.ReloadLast();
            if (containerInventory != null) containerInventory.ReloadLast();
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