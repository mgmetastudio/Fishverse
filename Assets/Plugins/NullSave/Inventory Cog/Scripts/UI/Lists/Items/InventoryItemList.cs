using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace NullSave.TOCK.Inventory
{
    public class InventoryItemList : MonoBehaviour, IDropHandler
    {

        #region Variables

        // Load type
        public ListLoadMode loadMode = ListLoadMode.OnEnable;
        public string shareTag;

        public ListSource listSource;
        public InventoryCog inventoryCog;
        public InventoryContainer container;
        public InventoryMerchant merchant;
        public bool hideSelectionWhenLocked = true;
        public bool showLockedSlots;
        public bool enableDragDrop;

        // Extra UI
        public AttachmentsUI attachmentsClient;
        public ItemDetailUI detailClient;
        public ItemContainerMenuUI itemContainerUI;
        public bool hideEmptyDetails = true;
        public SingleCheckoutUI checkoutUI;
        public ItemTooltipUI itemTooltip;

        // Filtering
        public bool requireBreakdown;
        public bool requireRepair;
        public bool requireCanDrop;
        public bool excludeContainers;

        public ListCategoryFilter categoryFilter = ListCategoryFilter.All;
        public List<Category> categories;

        public bool useTagFiltering;
        public List<CustomTagFilter> tags;

        public bool usePaging;
        public int startPage = 0;

        // Navigation
        public bool allowAutoWrap;
        public bool allowSelectByClick;
        public NavigationType navigationMode;
        public bool autoRepeat = true;
        public float repeatDelay = 0.5f;
        public bool lockInput;

        public NavigationTypeEx selectionMode;
        public string buttonSubmit = "Submit";
        public KeyCode keySubmit = KeyCode.Return;
        public float valueModifier = 1;

        // Events
        public UnityEvent onInputLocked, onInputUnlocked, onBindingUpdated;
        public SelectedIndexChanged onSelectionChanged;
        public UnityEvent onNeedPreviousCategory, onNeedNextCategory;
        public PageChanged onPageChanged;
        public ItemListSubmit onItemSubmit;

        // Editor
        public int z_display_flags = 4095;

        // Overhead limiter
        internal bool refreshedThisFrame;
        internal int restoreIndex = -2;

        #endregion

        #region Properties

        public InventoryContainer Container { get { return container; } set { if (container == value) return; container = value; onBindingUpdated?.Invoke(); } }

        public InventoryItem ContainerItem { get; set; }

        public InventoryCog Inventory { get { return inventoryCog; } set { if (inventoryCog == value) return; inventoryCog = value; onBindingUpdated?.Invoke(); } }

        public virtual int ItemsPerPage
        {
            get { return int.MaxValue; }
        }

        public bool LockInput
        {
            get { return lockInput; }
            set
            {
                if (lockInput == value) return;
                lockInput = value;
                if (lockInput)
                {
                    onInputLocked?.Invoke();
                }
                else
                {
                    onInputUnlocked?.Invoke();
                }
                LockStateChanged();
            }
        }

        public InventoryMerchant Merchant { get { return merchant; } set { if (merchant == value) return; merchant = value; onBindingUpdated?.Invoke(); } }

        public virtual int SelectedIndex { get { throw new System.NotImplementedException(); } set { throw new System.NotImplementedException(); } }

        public virtual ItemUI SelectedItem { get { throw new System.NotImplementedException(); } set { throw new System.NotImplementedException(); } }

        public virtual bool ThemeAllowClick
        {
            get
            {
                if (ThemeHost == null || ThemeHost.ActiveTheme == null) return false;
                return ThemeHost.ActiveTheme.enableUIClick;
            }
        }

        public virtual InventoryCog ThemeHost { get; set; }

        #endregion

        #region Unity Methods

        public virtual void OnDrop(PointerEventData eventData)
        {
            if (!enableDragDrop) return;

            ItemUI draggedItem = eventData.pointerDrag.gameObject.GetComponentInChildren<ItemUI>();
            if (draggedItem != null)
            {
                if(ContainerItem != null)
                {
                    if (!draggedItem.Item.storedIn == ContainerItem)
                    {
                        AddDraggedItem(draggedItem.Item);
                        if (draggedItem.Inventory != null)
                        {
                            draggedItem.Inventory.RemoveItem(draggedItem.Item, draggedItem.Item.CurrentCount);
                        }
                        else if (draggedItem.Container != null)
                        {
                            draggedItem.Container.RemoveItem(draggedItem.Item, draggedItem.Item.CurrentCount);
                        }
                    }
                }
                else if (draggedItem.Inventory != null)
                {
                    if(draggedItem.Item.storedIn != null)
                    {
                        AddDraggedItem(draggedItem.Item);
                        draggedItem.Item.storedIn.RemoveItem(draggedItem.Item, draggedItem.Item.CurrentCount);
                    }
                    else if (draggedItem.Inventory != Inventory)
                    {
                        AddDraggedItem(draggedItem.Item);
                        draggedItem.Inventory.RemoveItem(draggedItem.Item, draggedItem.Item.CurrentCount);
                    }
                }
                else if (draggedItem.Container != null)
                {
                    if (draggedItem.Container != Container)
                    {
                        AddDraggedItem(draggedItem.Item);
                        draggedItem.Container.RemoveItem(draggedItem.Item, draggedItem.Item.CurrentCount);
                    }
                }

                draggedItem.OnEndDrag(eventData);
            }
        }

        #endregion

        #region Public Methods

        public virtual void BreakdownSelected()
        {
            if (LockInput) return;
            int selIndex = SelectedIndex;

            InventoryItem item = SelectedItem.Item;
            if (Inventory.GetItemTotalCount(item) >= Inventory.ActiveTheme.minCount)
            {
                System.Action<bool, int> callback = new System.Action<bool, int>((bool confirm, int count) =>
                {
                    if (confirm)
                    {
                        Inventory.BreakdownItem(item, count);
                        ReloadLast();
                        SelectedIndex = selIndex;
                    }
                });

                Inventory.ActiveTheme.OpenCountSelect(item, Inventory.ActiveTheme.breakdownPrompt, callback);
            }
        }

        public virtual void ConsumeSelected()
        {
            if (LockInput) return;

            int selIndex = SelectedIndex;
            InventoryItem item = SelectedItem.Item;
            if (Inventory.GetItemTotalCount(item) >= Inventory.ActiveTheme.minCount)
            {
                System.Action<bool, int> callback = new System.Action<bool, int>((bool confirm, int count) =>
                {
                    if (confirm)
                    {
                        Inventory.ConsumeItem(item, count);
                        ReloadLast();
                        SelectedIndex = selIndex;
                    }
                });

                Inventory.ActiveTheme.OpenCountSelect(item, Inventory.ActiveTheme.consumePrompt, callback);
            }
            else
            {
                Inventory.ConsumeItem(item, 1);
                ReloadLast();
                SelectedIndex = selIndex;
            }
        }

        public virtual void DropSelected()
        {
            if (LockInput) return;

            int selIndex = SelectedIndex;
            InventoryItem item = SelectedItem.Item;
            if (Inventory.GetItemTotalCount(item) >= Inventory.ActiveTheme.minCount)
            {
                System.Action<bool, int> callback = new System.Action<bool, int>((bool confirm, int count) =>
                {
                    if (confirm)
                    {
                        Inventory.DropItem(item, count);
                        ReloadLast();
                        SelectedIndex = selIndex;
                    }
                });

                Inventory.ActiveTheme.OpenCountSelect(item, Inventory.ActiveTheme.dropPrompt, callback);
            }
            else
            {
                Inventory.DropItem(item, 1);
                ReloadLast();
                SelectedIndex = selIndex;
            }
        }

        public virtual void EquipSelected()
        {
            if (LockInput) return;
            SelectedItem.Equip();
            UpdateDetailUI(SelectedItem.Item);
        }

        public virtual void LoadFromCategoyList(CategoryList source) { throw new System.NotImplementedException(); }

        public virtual void LoadFromCategoyList(CategoryList source, bool startFromLastPage) { throw new System.NotImplementedException(); }

        public virtual void LoadItems() { throw new System.NotImplementedException(); }

        public virtual void LoadItems(List<InventoryItem> items) { throw new System.NotImplementedException(); }

        public virtual void OpenItemContainer()
        {
            if (LockInput || SelectedItem.Item.itemType != ItemType.Container) return;


            System.Action onClose = new System.Action(() => { LockInput = false; ReloadLast(); });
            LockInput = true;
            Inventory.ActiveTheme.OpenItemContainer(SelectedItem.Item, onClose);
        }

        public virtual void OpenItemManager()
        {
            if (LockInput || ThemeHost == null || ThemeHost.ActiveTheme == null || SelectedItem == null || SelectedItem.Item == null) return;

            System.Action onClose = new System.Action(() =>
            {
                LockInput = false;
            });

            LockInput = true;
            ThemeHost.ActiveTheme.OpenItemManager(SelectedItem.Item, onClose);
        }

        public virtual void ReloadLast() { refreshedThisFrame = false; }

        public virtual void RenameSelected()
        {
            RenameSelected(null);
        }

        public virtual void RenameSelected(GameObject disableDuringRename)
        {
            if (LockInput) return;

            LockInput = true;
            System.Action onClosed = new System.Action(() =>
            {
                LockInput = false;
                int sel = SelectedIndex;
                ReloadLast();
                SelectedIndex = sel;
                if (disableDuringRename != null) disableDuringRename.SetActive(true);
            });

            if (disableDuringRename != null) disableDuringRename.SetActive(false);
            Inventory.ActiveTheme.OpenRename(SelectedItem.Item, onClosed);
        }

        public virtual void RepairSelected()
        {
            if (LockInput) return;
            Inventory.RepairItem(SelectedItem.Item);
            ReloadLast();
        }

        public virtual void SelectItem(ItemUI item)
        {
            SelectedItem = item;
        }

        public virtual void SetSelectedSkill(string skillSlot)
        {
            if (LockInput) return;
            Inventory.SkillAssign(SelectedItem.Item, skillSlot);
        }

        public virtual void ShowAttachmentsUI()
        {
            if (LockInput || SelectedItem.Item == null || SelectedItem.Item.attachRequirement == AttachRequirement.NoneAllowed) return;

            int restoreSel = SelectedIndex;
            System.Action onClose = new System.Action(() => { LockInput = false; ReloadLast(); SelectedIndex = restoreSel; });

            LockInput = true;
            Inventory.ActiveTheme.OpenAttachments(SelectedItem.Item, onClose);
        }

        public virtual void Sort(InventorySortOrder sortOrder)
        {
            switch (listSource)
            {
                case ListSource.InventoryCog:
                    Inventory.Sort(sortOrder);
                    break;
                case ListSource.InventoryContainer:
                    Container.Sort(sortOrder);
                    break;
                case ListSource.InventoryMerchant:
                    Merchant.Sort(sortOrder);
                    break;
            }
            ReloadLast();
        }

        public virtual void SortByOrderId(int inventorySortOrderId)
        {
            Sort((InventorySortOrder)inventorySortOrderId);
        }

        public virtual void ToggleSelectedSkill(string skillSlot)
        {
            if (LockInput) return;
            Inventory.SkillToggle(SelectedItem.Item, skillSlot);
        }

        public virtual void UnequipSelected()
        {
            if (LockInput) return;
            SelectedItem.Unequip();
            UpdateDetailUI(SelectedItem.Item);
        }

        public virtual void UpdateCheckoutUI()
        {
            if (checkoutUI != null)
            {
                checkoutUI.valueModifier = valueModifier;
                checkoutUI.LoadUI(this);
            }
        }

        public virtual void UpdateDetailUI(InventoryItem item)
        {
            if (detailClient != null)
            {
                detailClient.valueModifier = valueModifier;
                detailClient.LoadItem(Inventory, item, item == null ? null : item.category);
                if (item == null && hideEmptyDetails)
                {
                    detailClient.gameObject.SetActive(false);
                }
                else
                {
                    detailClient.gameObject.SetActive(true);
                }
            }
        }

        #endregion

        #region Private Methods

        private void AddDraggedItem(InventoryItem item)
        {
            if(ContainerItem != null)
            {
                ContainerItem.AddStoredItem(item, item.CurrentCount);
            }
            else if (Inventory != null)
            {
                Inventory.AddToInventory(item, item.CurrentCount);
            }
            else if (Container != null)
            {
                Container.AddStoredItem(item, item.CurrentCount);
            }
        }

        public virtual void ClickSubmit(ItemUI item, List<Category> categoryList)
        {
            onItemSubmit?.Invoke(this, SelectedItem, categoryList);
        }

        internal List<InventoryItem> FilterItems(List<InventoryItem> items)
        {
            List<InventoryItem> removeList = new List<InventoryItem>();

            // Generate item list
            if (useTagFiltering)
            {
                foreach (InventoryItem checkItem in items)
                {
                    foreach (CustomTagFilter tagFilter in tags)
                    {
                        if (!tagFilter.PassMatch(checkItem))
                        {
                            removeList.Add(checkItem);
                            break;
                        }
                    }
                }
            }

            foreach (InventoryItem item in items)
            {
                if ((requireBreakdown && !item.CanBreakdown) ||
                    (requireRepair && (!item.CanRepair || item.condition == 1)) ||
                    (requireCanDrop && !item.canDrop) ||
                    (excludeContainers && item.itemType == ItemType.Container))
                {
                    if (!removeList.Contains(item))
                    {
                        removeList.Add(item);
                    }
                }
            }

            // Remove filtered
            foreach (InventoryItem item in removeList)
            {
                items.Remove(item);
            }

            return items;
        }

        internal void ItemPointerEnter(ItemUI item)
        {
            if (item.Item == null) return;
            itemTooltip.gameObject.SetActive(true);
            itemTooltip.ShowTooltip(item.Inventory ?? ThemeHost, item);
        }

        internal void ItemPointerExit(ItemUI item)
        {
            if (itemTooltip.Target == item)
            {
                itemTooltip.gameObject.SetActive(false);
            }
        }

        internal virtual void LockStateChanged() { }

        private void UnlockInput()
        {
            LockInput = false;
            Inventory.onMenuClose.RemoveListener(UnlockInput);
        }

        private void PromptReload()
        {
            int sel = SelectedIndex;
            ReloadLast();
            SelectedIndex = sel;
            Inventory.onMenuClose.RemoveListener(PromptReload);
        }

        internal void Subscribe()
        {
            if (Inventory != null)
            {
                Inventory.onItemAdded.AddListener(UpdateInventory);
                Inventory.onItemRemoved.AddListener(UpdateInventory);
            }

            if (Container != null)
            {
                container.onItemStored.AddListener(UpdateInventory);
                container.onItemRemoved.AddListener(UpdateInventory);
            }

            if(ContainerItem != null)
            {
                ContainerItem.onStoredItemAdded.AddListener(ReloadLast);
                ContainerItem.onStoredItemRemoved.AddListener(ReloadLast);
            }
        }

        internal void Unsubscribe()
        {
            if (Inventory != null)
            {
                Inventory.onItemAdded.RemoveListener(UpdateInventory);
                Inventory.onItemRemoved.RemoveListener(UpdateInventory);
            }

            if (Container != null)
            {
                container.onItemStored.RemoveListener(UpdateInventory);
                container.onItemRemoved.RemoveListener(UpdateInventory);
            }

            if (ContainerItem != null)
            {
                ContainerItem.onStoredItemAdded.RemoveListener(ReloadLast);
                ContainerItem.onStoredItemRemoved.RemoveListener(ReloadLast);
            }
        }

        internal void UpdateInventory(InventoryItem item, int count)
        {
            ReloadLast();
        }

        #endregion

    }
}