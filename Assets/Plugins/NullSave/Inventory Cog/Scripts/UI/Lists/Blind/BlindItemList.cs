using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace NullSave.TOCK.Inventory
{
    public class BlindItemList : MonoBehaviour, IDropHandler
    {

        #region Variables

        public bool hideSelectionWhenLocked = true;
        public bool enableDragDrop;

        // Extra UI
        public ItemTooltipUI itemTooltip;

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
        public UnityEvent onInputLocked, onInputUnlocked;
        public SelectedIndexChanged onSelectionChanged;
        public ItemListSubmit onItemSubmit;

        // Editor
        public int z_display_flags = 4095;

        #endregion

        #region Properties

        public virtual List<InventoryItem> Items { get; set; } = new List<InventoryItem>();

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
                Items.Add(draggedItem.Item);
                draggedItem.Container.RemoveItem(draggedItem.Item, draggedItem.Item.CurrentCount);

                draggedItem.OnEndDrag(eventData);
            }
        }

        #endregion

        #region Public Methods

        public virtual void RefreshList() { }

        public virtual void SelectItem(ItemUI item)
        {
            SelectedItem = item;
        }

        public virtual void Sort(InventorySortOrder sortOrder)
        {
            switch (sortOrder)
            {
                case InventorySortOrder.ConditionAsc:
                    Items = Items.OrderBy(_ => _.condition).ThenBy(_ => _.DisplayName).ToList();
                    break;
                case InventorySortOrder.ConditionDesc:
                    Items = Items.OrderByDescending(_ => _.condition).ThenBy(_ => _.DisplayName).ToList();
                    break;
                case InventorySortOrder.DisplayNameAsc:
                    Items = Items.OrderBy(_ => _.DisplayName).ToList();
                    break;
                case InventorySortOrder.DisplayNameDesc:
                    Items = Items.OrderByDescending(_ => _.DisplayName).ToList();
                    break;
                case InventorySortOrder.ItemCountAsc:
                    Items = Items.OrderBy(_ => _.CurrentCount).ThenBy(_ => _.DisplayName).ToList();
                    break;
                case InventorySortOrder.ItemCountDesc:
                    Items = Items.OrderByDescending(_ => _.CurrentCount).ThenBy(_ => _.DisplayName).ToList();
                    break;
                case InventorySortOrder.ItemTypeAsc:
                    Items = Items.OrderBy(_ => _.itemType).ThenBy(_ => _.DisplayName).ToList();
                    break;
                case InventorySortOrder.ItemTypeDesc:
                    Items = Items.OrderByDescending(_ => _.itemType).ThenBy(_ => _.DisplayName).ToList();
                    break;
                case InventorySortOrder.RarityAsc:
                    Items = Items.OrderBy(_ => _.rarity).ThenBy(_ => _.DisplayName).ToList();
                    break;
                case InventorySortOrder.RarityDesc:
                    Items = Items.OrderByDescending(_ => _.rarity).ThenBy(_ => _.DisplayName).ToList();
                    break;
                case InventorySortOrder.ValueAsc:
                    Items = Items.OrderBy(_ => _.value).ThenBy(_ => _.DisplayName).ToList();
                    break;
                case InventorySortOrder.ValueDesc:
                    Items = Items.OrderByDescending(_ => _.value).ThenBy(_ => _.DisplayName).ToList();
                    break;
                case InventorySortOrder.WeightAsc:
                    Items = Items.OrderBy(_ => _.weight).ThenBy(_ => _.DisplayName).ToList();
                    break;
                case InventorySortOrder.WeightDesc:
                    Items = Items.OrderByDescending(_ => _.weight).ThenBy(_ => _.DisplayName).ToList();
                    break;
            }

            RefreshList();
        }

        public virtual void SortByOrderId(int inventorySortOrderId)
        {
            Sort((InventorySortOrder)inventorySortOrderId);
        }

        #endregion

        #region Private Methods

        public virtual void ClickSubmit(ItemUI item)
        {
            onItemSubmit?.Invoke(null, SelectedItem, null);
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
        }

        #endregion

    }
}