﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NullSave.TOCK.Inventory
{
    public class AttachmentSlotGrid : AttachmentList
    {

        #region Variables

        // UI
        public AttachmentSlotUI slotUIPrefab;

        // Attachment Items
        public AttachmentItemList attachmentItemList;

        // Navigation
        public string inputHorizontal = "Horizontal";
        public string inputVertical = "Vertical";
        public KeyCode keyLeft = KeyCode.A;
        public KeyCode keyUp = KeyCode.W;
        public KeyCode keyRight = KeyCode.D;
        public KeyCode keyDown = KeyCode.S;

        private List<AttachmentSlotUI> loadedItems;
        private GridLayoutGroup prefabContainer;
        private RectTransform pcRT;

        private int selIndex = -1;

        private float nextRepeat;
        private bool waitForZero;

        #endregion

        #region Properties

        public int ColsPerRow
        {
            get
            {
                float w = pcRT.sizeDelta.x;
                if (w == 0) w = pcRT.rect.width;
                float availWidth = w - prefabContainer.padding.horizontal;
                return Mathf.FloorToInt((availWidth + prefabContainer.spacing.x) / (prefabContainer.cellSize.x + prefabContainer.spacing.x));
            }
        }

        public Vector2 SelectedCell
        {
            get
            {
                int row = Mathf.FloorToInt(selIndex / ColsPerRow);
                int col = selIndex - (int)(row * ColsPerRow);
                return new Vector2(col, row);
            }
            set
            {
                SelectedIndex = (int)(value.y * ColsPerRow + value.x);
            }
        }

        public override int SelectedIndex
        {
            get { return selIndex; }
            set
            {
                if (loadedItems == null) return;
                if (loadedItems.Count <= value) value = loadedItems.Count - 1;
                if (value < 0) value = 0;
                if (selIndex <= loadedItems.Count - 1 && selIndex >= 0) loadedItems[selIndex].SetSelected(false);
                selIndex = value;

                if (selIndex >= 0 && selIndex <= loadedItems.Count - 1)
                {
                    if (!LockInput || !hideSelectionWhenLocked) loadedItems[selIndex].SetSelected(true);
                }

                onSelectionChanged?.Invoke(selIndex);

                if (Inventory.GetAnyAttachmentsInInventory(SelectedItem.Slot.ParentItem))
                {
                    onAttachmentsAvail?.Invoke();
                }
                else
                {
                    onNoAttachmentsAvail?.Invoke();
                }

                if (SelectedItem.Slot.AttachedItem != null)
                {
                    onHasAttachments?.Invoke();
                }
                else
                {
                    onHasNoAttachments?.Invoke();
                }

                if (attachmentItemList != null)
                {
                    attachmentItemList.Inventory = Inventory;
                    attachmentItemList.ParentList = this;
                    attachmentItemList.LockInput = true;
                    attachmentItemList.LoadAttachments(SelectedItem.Slot);
                }
            }
        }

        public override AttachmentSlotUI SelectedItem
        {
            get
            {
                if (loadedItems == null) return null;

                if (selIndex >= 0 && selIndex <= loadedItems.Count - 1)
                {
                    return loadedItems[selIndex];
                }

                return null;
            }
            set
            {
                for (int i = 0; i < loadedItems.Count; i++)
                {
                    if (loadedItems[i] == value)
                    {
                        SelectedIndex = i;
                        return;
                    }
                }
            }
        }

        #endregion

        #region Unity Methods

        private void Awake()
        {
            prefabContainer = GetComponent<GridLayoutGroup>();
            pcRT = prefabContainer.GetComponent<RectTransform>();
        }

        private void Update()
        {
            if (!LockInput)
            {
                UpdateNavigation();

                switch (selectionMode)
                {
                    case NavigationTypeEx.ByButton:
                        if (InventoryCog.GetButtonDown(buttonSubmit))
                        {
                            OpenAttachments();
                        }
                        break;
                    case NavigationTypeEx.ByKey:
                        if (InventoryCog.GetKeyDown(keySubmit))
                        {
                            OpenAttachments();
                        }
                        break;
                }
            }
        }

        #endregion

        #region Public Methods

        public override void CloseAttachments()
        {
            if (attachmentItemList == null) return;

            LockInput = false;
            attachmentItemList.LockInput = true;
        }

        public override void LoadSlots(InventoryItem item)
        {
            Item = item;
            ClearItems();

            // Load points UI
            foreach (AttachmentSlot slot in item.Slots)
            {
                AttachmentSlotUI slotUI = Instantiate(slotUIPrefab, prefabContainer.transform);
                slotUI.LoadSlot(slot);
                slotUI.SetSelected(loadedItems.Count == 0);
                if(selectionMode == NavigationTypeEx.ByClick)
                {
                    slotUI.onClick.AddListener((AttachmentSlotUI ui) => SelectedItem = ui);
                }
                loadedItems.Add(slotUI);
            }

            item.onAttachmentAdded.AddListener(RefreshItem);
            item.onAttachmentRemoved.AddListener(RefreshItem);

            SelectedIndex = 0;
        }

        public override void OpenAttachments()
        {
            if (attachmentItemList == null) return;

            LockInput = true;
            attachmentItemList.LockInput = false;
        }

        public void RefreshItem(InventoryItem item)
        {
            LoadSlots(Item);
        }

        #endregion

        #region Private Methods

        private void ClearItems()
        {
            if (loadedItems != null)
            {
                foreach (AttachmentSlotUI ui in loadedItems)
                {
                    Destroy(ui.gameObject);
                }
            }

            selIndex = -1;
            onSelectionChanged?.Invoke(-1);

            Item.onAttachmentAdded.RemoveListener(RefreshItem);
            Item.onAttachmentRemoved.RemoveListener(RefreshItem);

            loadedItems = new List<AttachmentSlotUI>();
        }

        internal override void LockStateChanged()
        {
            if (hideSelectionWhenLocked && SelectedItem != null)
            {
                SelectedItem.SetSelected(!LockInput);
            }

            if (SelectedItem.Slot.ParentItem.HasAttachments)
            {
                onHasAttachments?.Invoke();
            }
            else
            {
                onHasNoAttachments?.Invoke();
            }
        }

        #endregion

        #region Navigation Methods

        public Vector2 CellFromIndex(int index)
        {
            int row = Mathf.FloorToInt(index / ColsPerRow);
            int col = index - (int)(row * ColsPerRow);
            return new Vector2(col, row);
        }

        private Vector2 GetInput()
        {
            return new Vector2(InventoryCog.GetAxis(inputHorizontal), InventoryCog.GetAxis(inputVertical));
        }

        private void NavigateByButton()
        {
            if (autoRepeat)
            {
                if (nextRepeat > 0) nextRepeat -= Time.deltaTime;
                if (nextRepeat > 0) return;
            }

            Vector2 input = GetInput();
            if (waitForZero)
            {
                if (input.x != 0 || input.y != 0) return;
                waitForZero = false;
            }

            if (input == Vector2.zero)
            {
                nextRepeat = 0;
            }
            if (Mathf.Abs(input.y) > Mathf.Abs(input.x))
            {
                // Vertical
                if (input.y > 0.1f)
                {
                    NavigateUp();
                }
                else if (input.y < -0.1f)
                {
                    NavigateDown();
                }
            }
            else
            {
                // Horizontal
                if (input.x < -0.1f)
                {
                    NavigateLeft();
                }
                else if (input.x > 0.1f)
                {
                    NavigateRight();
                }
            }
        }

        private void NavigateByKey()
        {
            if (InventoryCog.GetKeyDown(keyUp)) NavigateUp();
            if (InventoryCog.GetKeyDown(keyDown)) NavigateDown();
            if (InventoryCog.GetKeyDown(keyLeft)) NavigateLeft();
            if (InventoryCog.GetKeyDown(keyRight)) NavigateRight();
        }

        private void NavigateDown()
        {
            if (loadedItems == null) return;

            // Down
            int newIndex = selIndex + (int)ColsPerRow;
            Vector2 cell = CellFromIndex(newIndex);
            if (cell.x <= ColsPerRow)
            {
                SelectedIndex = newIndex;
            }
            UpdateRepeat();
        }

        private void NavigateLeft()
        {
            if (loadedItems == null) return;

            // Left
            int newIndex = selIndex - 1;
            Vector2 cell = CellFromIndex(newIndex);
            if (cell.y != SelectedCell.y)
            {
                newIndex = -1;
            }
            if (newIndex >= 0)
            {
                SelectedIndex = newIndex;
            }
            UpdateRepeat();
        }

        private void NavigateRight()
        {
            if (loadedItems == null) return;

            // Right
            int newIndex = selIndex + 1;
            Vector2 cell = CellFromIndex(newIndex);
            if (cell.y != SelectedCell.y)
            {
                cell.x += ColsPerRow;
            }

            if (cell.x < ColsPerRow && newIndex < loadedItems.Count)
            {
                SelectedIndex = newIndex;
            }

            UpdateRepeat();
        }

        private void NavigateUp()
        {
            if (loadedItems == null) return;

            // Up
            int newIndex = selIndex - (int)ColsPerRow;
            if (newIndex >= 0)
            {
                SelectedIndex = newIndex;
            }
            UpdateRepeat();
        }

        private void UpdateNavigation()
        {
            switch (navigationMode)
            {
                case NavigationType.ByButton:
                    NavigateByButton();
                    break;
                case NavigationType.ByKey:
                    NavigateByKey();
                    break;
            }
        }

        private void UpdateRepeat()
        {
            if (autoRepeat)
            {
                nextRepeat = repeatDelay;
            }
            else
            {
                waitForZero = true;
            }
        }

        #endregion

    }
}