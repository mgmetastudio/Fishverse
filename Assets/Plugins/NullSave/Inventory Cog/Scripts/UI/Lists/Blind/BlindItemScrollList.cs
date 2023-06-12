using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NullSave.TOCK.Inventory
{
    [RequireComponent(typeof(ScrollRect))]
    public class BlindItemScrollList : BlindItemList
    {

        #region Variables

        // UI
        public ItemUI itemUIPrefab;
        public Padding padding;

        // Navigation
        public bool invertInput = false;
        public string navButton = "Vertical";
        public KeyCode backKey = KeyCode.UpArrow;
        public KeyCode nextKey = KeyCode.DownArrow;

        private List<ItemUI> loadedItems;
        private ScrollRect scrollRect;
        private RectTransform viewPort, content;

        private int selIndex;

        private float nextRepeat;
        private bool waitForZero;
        private ItemUI lastSel;

        #endregion

        #region Properties

        public override ItemUI SelectedItem
        {
            get
            {
                if (loadedItems == null) return null;
                if (selIndex < 0 || selIndex >= loadedItems.Count) return null;
                return loadedItems[selIndex];
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

        public override int SelectedIndex
        {
            get { return selIndex; }
            set
            {
                if (selIndex == value || loadedItems == null) return;
                if (value < 0)
                {
                    if (allowAutoWrap)
                    {
                        value = loadedItems.Count - 1;
                    }
                    else
                    {
                        return;
                    }
                }
                if (value >= loadedItems.Count)
                {
                    if (allowAutoWrap)
                    {
                        value = 0;
                    }
                    else
                    {
                        return;
                    }
                }
                UnselectCurrent();
                selIndex = value;
                if (selIndex >= 0 && selIndex < loadedItems.Count)
                {
                    if (!LockInput || !hideSelectionWhenLocked) loadedItems[selIndex].SetSelected(true);
                }

                onSelectionChanged?.Invoke(selIndex);
            }
        }

        #endregion

        #region Unity Methods

        private void Awake()
        {
            scrollRect = GetComponent<ScrollRect>();
            viewPort = scrollRect.viewport;
            content = viewPort.transform.GetChild(0).gameObject.GetComponent<RectTransform>();
        }

        private void Update()
        {
            if (!LockInput)
            {
                UpdateNavigation();
            }

            FocusSelected();
        }

        #endregion

        #region Public Methods

        public override void RefreshList()
        {
            ClearItems();

            float totalHeight = 0;

            foreach (InventoryItem item in Items)
            {
                ItemUI ui = Instantiate(itemUIPrefab, content);
                ui.valueModifier = valueModifier;
                ui.LoadItem(ThemeHost, null, item);
                ui.BlindListParent = this;
                ui.SetSelected(loadedItems.Count == 0 && (!LockInput || !hideSelectionWhenLocked));
                if (allowSelectByClick)
                {
                    ui.onClick.AddListener(SelectItem);
                }

                if (itemTooltip != null)
                {
                    ui.onPointerEnter.AddListener(ItemPointerEnter);
                    ui.onPointerExit.AddListener(ItemPointerExit);
                }

                RectTransform rt = ui.gameObject.GetComponent<RectTransform>();
                if (rt.rect.height <= 0)
                {
                    totalHeight += rt.sizeDelta.y + padding.Vertical;
                }
                else
                {
                    totalHeight += rt.rect.height + padding.Vertical;
                }

                loadedItems.Add(ui);
            }

            if (content != null)
            {
                content.sizeDelta = new Vector2(content.sizeDelta.x, totalHeight);
            }

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
                    Destroy(ui.gameObject);
                }
            }

            selIndex = -1;
            onSelectionChanged?.Invoke(-1);

            loadedItems = new List<ItemUI>();
        }

        private void FocusSelected()
        {
            ItemUI selItem = SelectedItem;
            if (selItem == lastSel || selItem == null) return;

            RectTransform rt = selItem.gameObject.GetComponent<RectTransform>();
            if (rt == null) rt = selItem.gameObject.GetComponentInChildren<RectTransform>();
            if (rt == null)
            {
                Debug.Log("No RectTransform for " + selItem.name);
                return;
            }

            float btm = Mathf.Abs(rt.anchoredPosition.y - (rt.rect.height * rt.pivot.y)) + padding.top;
            float top = btm - rt.rect.height - padding.top;

            if (btm > viewPort.rect.height)
            {
                scrollRect.content.anchoredPosition = new Vector2(0, btm - viewPort.rect.height);
            }
            else if (top < scrollRect.content.anchoredPosition.y)
            {
                scrollRect.content.anchoredPosition = new Vector2(0, top);
            }

            lastSel = selItem;
        }

        internal override void LockStateChanged()
        {
            if (hideSelectionWhenLocked)
            {
                if (LockInput)
                {
                    UnselectCurrent();
                }
                else
                {
                    SelectCurrent();
                }
            }
        }

        private void SelectCurrent()
        {
            if (loadedItems == null || selIndex < 0 || selIndex >= loadedItems.Count) return;
            loadedItems[selIndex].SetSelected(true);
        }

        private void UnselectCurrent()
        {
            if (loadedItems == null || selIndex < 0 || selIndex >= loadedItems.Count) return;
            loadedItems[selIndex].SetSelected(false);
        }

        #endregion

        #region Navigation Methods

        private float GetInput()
        {
            return InventoryCog.GetAxis(navButton);
        }

        private void NavigateByButton()
        {
            if (autoRepeat)
            {
                if (nextRepeat > 0) nextRepeat -= Time.deltaTime;
                if (nextRepeat > 0) return;
            }

            float input = GetInput();
            if (waitForZero && input != 0) return;

            if (invertInput)
            {
                input = -input;
            }

            if (input <= -0.1f)
            {
                NavigatePrevious();
            }
            else if (input >= 0.1f)
            {
                NavigateNext();
            }
            else if (input == 0)
            {
                nextRepeat = 0;
            }
        }

        private void NavigateByKey()
        {
            if (InventoryCog.GetKeyDown(backKey))
            {
                NavigatePrevious();
            }
            if (InventoryCog.GetKeyDown(nextKey))
            {
                NavigateNext();
            }
        }

        private void NavigateNext()
        {
            SelectedIndex += 1;
            UpdateRepeat();
        }

        private void NavigatePrevious()
        {
            SelectedIndex -= 1;
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