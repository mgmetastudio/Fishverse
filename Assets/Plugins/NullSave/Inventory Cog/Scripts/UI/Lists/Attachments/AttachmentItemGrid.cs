using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NullSave.TOCK.Inventory
{
    public class AttachmentItemGrid : AttachmentItemList
    {

        #region Variables

        // UI
        public ItemUI itemUIPrefab;
        public GridPageMode pageMode;
        public bool fillContainer;

        // Navigation
        public string inputHorizontal = "Horizontal";
        public string inputVertical = "Vertical";
        public KeyCode keyLeft = KeyCode.A;
        public KeyCode keyUp = KeyCode.W;
        public KeyCode keyRight = KeyCode.D;
        public KeyCode keyDown = KeyCode.S;

        private List<ItemUI> loadedItems;
        private GridLayoutGroup prefabContainer;
        private RectTransform pcRT;

        private int selIndex = -1;

        private float nextRepeat;
        private bool waitForZero;
        private bool unlockedThisFrame;
        private bool keepAlive;

        #endregion

        #region Properties

        public int ColsPerRow
        {
            get
            {
                float w = pcRT.sizeDelta.x;
                if (w <= 0) w = pcRT.rect.width;
                float availWidth = w - prefabContainer.padding.horizontal;
                return Mathf.FloorToInt((availWidth + prefabContainer.spacing.x) / (prefabContainer.cellSize.x + prefabContainer.spacing.x));
            }
        }

        public int ItemsPerPage
        {
            get
            {
                return ColsPerRow * RowsPerPage;
            }
        }

        public int RowsPerPage
        {
            get
            {
                float h = pcRT.sizeDelta.y;
                if (h <= 0) h = pcRT.rect.height;
                float availHeight = h - prefabContainer.padding.vertical;
                return Mathf.FloorToInt((availHeight + prefabContainer.spacing.y) / (prefabContainer.cellSize.y + prefabContainer.spacing.y));
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
            }
        }

        public override ItemUI SelectedItem
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

        private void OnDestroy()
        {
            keepAlive = false;
        }

        private void Update()
        {
            if (!LockInput)
            {
                UpdateNavigation();

                if (unlockedThisFrame)
                {
                    unlockedThisFrame = false;
                    return;
                }

                switch (selectionMode)
                {
                    case NavigationTypeEx.ByButton:
                        if (InventoryCog.GetButtonDown(buttonSubmit))
                        {
                            AttachToItem();
                        }
                        break;
                    case NavigationTypeEx.ByKey:
                        if (InventoryCog.GetKeyDown(keySubmit))
                        {
                            AttachToItem();
                        }
                        break;
                }
            }
        }

        #endregion

        #region Public Methods

        public override void LoadAttachments(AttachmentSlot attachSlot)
        {
            ClearItems();
            Slot = attachSlot;
            if (attachSlot == null) return;
            Item = attachSlot.ParentItem;

            LoadAttachments(Inventory.GetAvailableAttachments(attachSlot.ParentItem, attachSlot.AttachPoint.pointId));
            
            StartCoroutine("MonitorSlot", attachSlot);
            
            SelectedIndex = 0;
        }

        public override void LoadAttachments(InventoryItem item)
        {
            ClearItems();
            Slot = null;
            Item = item;
            
            LoadAttachments(Inventory.GetAvailableAttachments(item));

            item.onAttachmentAdded.AddListener((InventoryItem attachment) => LoadAttachments(item));
            SelectedIndex = 0;
        }

        #endregion

        #region Private Methods

        private void ClearItems()
        {
            if (loadedItems != null)
            {
                foreach (ItemUI ui in loadedItems)
                {
                    if (ui.gameObject)
                    {
                        Destroy(ui.gameObject);
                    }
                }
            }

            selIndex = -1;
            onSelectionChanged?.Invoke(-1);

            loadedItems = new List<ItemUI>();
        }

        private void LoadAttachments(List<InventoryItem> items)
        {
            foreach (InventoryItem attachment in items)
            {
                ItemUI itemUI = Instantiate(itemUIPrefab, prefabContainer.transform);
                itemUI.LoadItem(Inventory, null, attachment);
                itemUI.SetSelected(loadedItems.Count == 0 && !LockInput);
                loadedItems.Add(itemUI);
                if(selectionMode == NavigationTypeEx.ByClick)
                {
                    itemUI.onClick.AddListener((ItemUI ui) => {
                        SelectedItem = ui;
                        AttachToItem();
                    });
                }
            }

            if (fillContainer)
            {
                // Ensure content height
                RectTransform parentRT = pcRT.parent.gameObject.GetComponent<RectTransform>();
                float ph = parentRT.sizeDelta.y;
                if (ph == 0)
                {
                    ph = parentRT.rect.height;
                    if (ph == 0)
                    {
                        parentRT = parentRT.parent.gameObject.GetComponent<RectTransform>();
                        ph = parentRT.sizeDelta.y;
                        if (ph == 0)
                        {
                            ph = parentRT.rect.height;
                        }
                    }
                }
                if (pcRT.sizeDelta.y < ph)
                {
                    pcRT.sizeDelta = new Vector2(pcRT.sizeDelta.x, ph);
                }

                while (loadedItems.Count < ItemsPerPage)
                {
                    ItemUI ui = Instantiate(itemUIPrefab, prefabContainer.transform);
                    ui.LoadItem(Inventory, null);
                    ui.SetSelected(false);
                    loadedItems.Add(ui);
                }
            }

        }

        internal override void LockStateChanged()
        {
            if (hideSelectionWhenLocked && SelectedItem != null)
            {
                unlockedThisFrame = true;
                SelectedItem.SetSelected(!LockInput);
            }
        }

        private IEnumerator MonitorSlot(AttachmentSlot slot)
        {
            keepAlive = true;
            while (keepAlive)
            {
                yield return new WaitForEndOfFrame();
                if (slot != Slot)
                {
                    break;
                }
                else if (slot.AttachedItem != null && slot.AttachedItem != Slot.AttachedItem)
                {
                    keepAlive = false;
                    LoadAttachments(slot);
                    break;
                }
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