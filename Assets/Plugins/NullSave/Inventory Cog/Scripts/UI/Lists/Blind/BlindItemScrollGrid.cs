using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NullSave.TOCK.Inventory
{
    [RequireComponent(typeof(GridLayoutGroup))]
    public class BlindItemScrollGrid : BlindItemList
    {

        #region Variables

        // UI
        public ItemUI itemUIPrefab;
        public bool fillContainer;

        // Navigation
        public string inputHorizontal = "Horizontal";
        public string inputVertical = "Vertical";
        public KeyCode keyLeft = KeyCode.A;
        public KeyCode keyUp = KeyCode.W;
        public KeyCode keyRight = KeyCode.D;
        public KeyCode keyDown = KeyCode.S;

        private GridLayoutGroup prefabContainer;
        private RectTransform pcRT;

        private List<ItemUI> loadedItems;

        private int selIndex;

        private float nextRepeat;
        private bool waitForZero;
        private bool reloadCalled;

        #endregion

        #region Properties

        public int ColsPerRow
        {
            get
            {
                if (pcRT == null) return 0;
                float w = pcRT.sizeDelta.x;
                if (w <= 0) w = pcRT.rect.width;
                float availWidth = w - prefabContainer.padding.horizontal;
                return Mathf.FloorToInt((availWidth + prefabContainer.spacing.x) / (prefabContainer.cellSize.x + prefabContainer.spacing.x));
            }
        }

        public override int ItemsPerPage
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
                if (pcRT == null) return 0;
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
                if (loadedItems == null)
                {
                    if (value < 0) selIndex = -1;
                    return;
                }
                if (loadedItems.Count <= value)
                {
                    if (value == 0) return;
                    value = loadedItems.Count - 1;
                }
                if (value < -1) value = -1;
                if (selIndex <= loadedItems.Count - 1 && selIndex >= 0) loadedItems[selIndex].SetSelected(false);
                selIndex = value;

                if (selIndex >= 0 && selIndex <= loadedItems.Count - 1)
                {
                    if (!LockInput || !hideSelectionWhenLocked)
                    {
                        loadedItems[selIndex].SetSelected(true);
                        if (loadedItems[selIndex].Item == null && value != 0)
                        {
                            SelectedIndex = 0;
                        }
                    }
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

        private void Update()
        {
            reloadCalled = false;

            if (!LockInput)
            {
                UpdateNavigation();

                switch (selectionMode)
                {
                    case NavigationTypeEx.ByButton:
                        if (InventoryCog.GetButtonDown(buttonSubmit))
                        {
                            onItemSubmit?.Invoke(null, SelectedItem, null);
                        }
                        break;
                    case NavigationTypeEx.ByKey:
                        if (InventoryCog.GetKeyDown(keySubmit))
                        {
                            onItemSubmit?.Invoke(null, SelectedItem, null);
                        }
                        break;
                }
            }
        }

        #endregion

        #region Public Methods


        public override void RefreshList()
        {
            bool wasNeg = selIndex < 0;

            int totalItems = Items.Count;

            ClearItems();

            if (ColsPerRow <= 0)
            {
                StartCoroutine("UpdateRows");
                return;
            }

            if (prefabContainer == null)
            {
                prefabContainer = GetComponent<GridLayoutGroup>();
                pcRT = prefabContainer.GetComponent<RectTransform>();
            }

            for (int i = 0; i < Items.Count; i++)
            {
                ItemUI ui = Instantiate(itemUIPrefab, prefabContainer.transform);
                ui.valueModifier = valueModifier;
                ui.LoadItem(ThemeHost, null, Items[i]);
                ui.BlindListParent = this;
                ui.SetSelected(false);
                ui.onZeroCount.AddListener(ZeroCount);
                EventSubscribe(ui);
                loadedItems.Add(ui);
            }

            int rows = Mathf.CeilToInt(totalItems / (float)ColsPerRow);
            pcRT.sizeDelta = new Vector2(pcRT.sizeDelta.x, rows * (prefabContainer.cellSize.y + prefabContainer.spacing.y));
            if (selIndex >= 0)
            {
                SelectedIndex = 0;
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
                    ui.LoadItem(ThemeHost, null);
                    ui.SetSelected(false);
                    EventSubscribe(ui);
                    loadedItems.Add(ui);
                }
            }


            SelectedIndex = wasNeg ? -1 : 0;
        }

        #endregion

        #region Private Methods

        public Vector2 CellFromIndex(int index)
        {
            int cpr = ColsPerRow;
            int row = Mathf.FloorToInt(index / cpr);
            int col = index - (int)(row * cpr);
            return new Vector2(col, row);
        }

        private void EventSubscribe(ItemUI item)
        {
            if (allowSelectByClick || ThemeAllowClick)
            {
                item.onClick.AddListener(SelectItem);
            }

            if (selectionMode == NavigationTypeEx.ByClick)
            {
                item.onClick.AddListener(ClickSubmit);
            }

            if (itemTooltip != null)
            {
                item.onPointerEnter.AddListener(ItemPointerEnter);
                item.onPointerExit.AddListener(ItemPointerExit);
            }
        }

        private void ClearItems()
        {
            if (loadedItems != null)
            {
                foreach (ItemUI ui in loadedItems)
                {
                    ui.onZeroCount.RemoveListener(ZeroCount);
                    Destroy(ui.gameObject);
                }
            }

            loadedItems = new List<ItemUI>();
        }

        internal override void LockStateChanged()
        {
            if (hideSelectionWhenLocked && SelectedItem != null)
            {
                SelectedItem.SetSelected(!LockInput);
            }
        }

        #endregion

        #region Navigation Methods

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
            int cpr = ColsPerRow;
            int newIndex = selIndex + cpr;
            Vector2 cell = CellFromIndex(newIndex);
            if (cell.x <= cpr)
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
            int cpr = ColsPerRow;
            int newIndex = selIndex + 1;
            Vector2 cell = CellFromIndex(newIndex);
            if (cell.y != SelectedCell.y)
            {
                cell.x -= cpr;
            }

            if (cell.x < cpr && newIndex < loadedItems.Count)
            {
                SelectedIndex = newIndex;
            }

            UpdateRepeat();
        }

        private void NavigateUp()
        {
            if (loadedItems == null) return;

            // Up
            int newIndex = selIndex - ColsPerRow;
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

        private IEnumerator UpdateRows()
        {
            yield return new WaitForEndOfFrame();
            RefreshList();
        }

        private void ZeroCount(ItemUI item)
        {
            if (reloadCalled) return;
            reloadCalled = true;
            RefreshList();
        }

        #endregion

    }
}