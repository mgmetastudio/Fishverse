using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NullSave.TOCK.Inventory
{
    [RequireComponent(typeof(ScrollRect))]
    [HierarchyIcon("tock-list", false)]
    public class InventoryItemScrollList : InventoryItemList
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
        private List<Category> lastCategoryFilter;

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
                        UpdateCheckoutUI();
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
                if (refreshedThisFrame)
                {
                    restoreIndex = value;
                    return;
                }
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
                    UpdateDetailUI(loadedItems[selIndex].Item);
                }
                else
                {
                    UpdateDetailUI(null);
                }
                UpdateCheckoutUI();
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

        private void OnEnable()
        {
            if (loadMode == ListLoadMode.OnEnable)
            {
                LoadItems();
            }
            else if (loadMode == ListLoadMode.FromShareTag)
            {
                StartCoroutine(Bind());
            }
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

        public override void LoadFromCategoyList(CategoryList source)
        {
            LoadFromCategoyList(source, false);
        }

        public override void LoadFromCategoyList(CategoryList source, bool startFromLastPage)
        {
            Category category = source.SelectedItem;
            if (category.name == "__ic_allitems")
            {
                LoadItems();
            }
            else
            {
                List<Category> catFilter = new List<Category>();
                catFilter.Add(category);
                switch (listSource)
                {
                    case ListSource.InventoryCog:
                        LoadFromInventory(catFilter);
                        break;
                    case ListSource.InventoryContainer:
                        LoadFromContainer(catFilter);
                        break;
                    case ListSource.InventoryMerchant:
                        LoadFromMerchant(catFilter);
                        break;
                    case ListSource.ContainerItem:
                        LoadFromContainerItem(catFilter);
                        break;
                }
            }
        }

        public override void LoadItems()
        {
            switch (listSource)
            {
                case ListSource.InventoryCog:
                    LoadFromInventory(GetCategoryFilter());
                    break;
                case ListSource.InventoryContainer:
                    LoadFromContainer(GetCategoryFilter());
                    break;
                case ListSource.InventoryMerchant:
                    LoadFromMerchant(GetCategoryFilter());
                    break;
                case ListSource.ContainerItem:
                    LoadFromContainerItem(GetCategoryFilter());
                    break;
            }
        }

        public override void LoadItems(List<InventoryItem> items)
        {
            Unsubscribe();
            Subscribe();

            ClearItems();

            float totalHeight = 0;

            foreach (InventoryItem item in items)
            {
                ItemUI ui = Instantiate(itemUIPrefab, content);
                ui.valueModifier = valueModifier;
                ui.LoadItem(Inventory, Container, item);
                ui.ItemListParent = this;
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

        public override void ReloadLast()
        {
            if (refreshedThisFrame || !gameObject.activeInHierarchy) return;
            restoreIndex = -2;
            refreshedThisFrame = true;
            StartCoroutine("FinalizeReloadLast");

        }

        #endregion

        #region Private Methods

        private IEnumerator Bind()
        {
            while (Inventory == null)
            {
                foreach (InventoryCog ic in FindObjectsOfType<InventoryCog>())
                {
                    if (ic.shareTag == shareTag)
                    {
                        Inventory = ic;
                        LoadItems();
                        break;
                    }
                }

                yield return new WaitForSeconds(0.1f);
            }
        }

        private void ClearItems()
        {
            if (loadedItems != null)
            {
                foreach (ItemUI ui in loadedItems)
                {
                    Destroy(ui.gameObject);
                }
            }

            UpdateDetailUI(null);
            selIndex = -1;
            onSelectionChanged?.Invoke(-1);

            loadedItems = new List<ItemUI>();
        }

        private IEnumerator FinalizeReloadLast()
        {
            yield return new WaitForEndOfFrame();

            refreshedThisFrame = false;
            if (lastCategoryFilter == null)
            {
                LoadItems();
            }
            else
            {
                switch (listSource)
                {
                    case ListSource.InventoryCog:
                        LoadFromInventory(lastCategoryFilter);
                        break;
                    case ListSource.InventoryContainer:
                        LoadFromContainer(lastCategoryFilter);
                        break;
                    case ListSource.InventoryMerchant:
                        LoadFromMerchant(GetCategoryFilter());
                        break;
                    case ListSource.ContainerItem:
                        LoadFromContainerItem(lastCategoryFilter);
                        break;
                }
            }
            SelectedIndex = restoreIndex == -2 ? 0 : restoreIndex;

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

        private List<Category> GetCategoryFilter()
        {
            // Generate category list
            List<Category> catFilter = new List<Category>();
            switch (categoryFilter)
            {
                case ListCategoryFilter.All:
                    foreach (Category category in InventoryDB.Categories)
                    {
                        switch (listSource)
                        {
                            case ListSource.InventoryContainer:
                            case ListSource.InventoryMerchant:
                            case ListSource.ContainerItem:
                                catFilter.Add(category);
                                break;
                            case ListSource.InventoryCog:
                                if (Inventory != null)
                                {
                                    Category catRef = Inventory.GetCategory(category.name);
                                    if (catRef.catUnlocked)
                                    {
                                        catFilter.Add(catRef);
                                    }
                                }
                                break;
                        }
                    }
                    break;
                case ListCategoryFilter.InList:
                    foreach (Category category in categories)
                    {
                        switch (listSource)
                        {
                            case ListSource.InventoryContainer:
                            case ListSource.InventoryMerchant:
                            case ListSource.ContainerItem:
                                catFilter.Add(category);
                                break;
                            case ListSource.InventoryCog:
                                if (Inventory != null)
                                {
                                    Category catRef = Inventory.GetCategory(category.name);
                                    if (catRef.catUnlocked)
                                    {
                                        catFilter.Add(catRef);
                                    }
                                }
                                break;
                        }
                    }
                    break;
                case ListCategoryFilter.NotInList:
                    foreach (Category category in InventoryDB.Categories)
                    {
                        if (!categories.Contains(category))
                        {
                            switch (listSource)
                            {
                                case ListSource.InventoryContainer:
                                case ListSource.InventoryMerchant:
                                case ListSource.ContainerItem:
                                    catFilter.Add(category);
                                    break;
                                case ListSource.InventoryCog:
                                    if (Inventory != null)
                                    {
                                        Category catRef = Inventory.GetCategory(category.name);
                                        if (catRef.catUnlocked)
                                        {
                                            catFilter.Add(catRef);
                                        }
                                    }
                                    break;
                            }
                        }
                    }
                    break;
            }

            return catFilter;
        }

        private void LoadFromContainer(List<Category> catFilter)
        {
            if (Container == null) return;
            lastCategoryFilter = catFilter;

            // Load filtered items
            LoadItems(FilterItems(Container.GetStoredItems(catFilter)));
        }

        private void LoadFromContainerItem(List<Category> catFilter)
        {
            if (ContainerItem == null) return;
            lastCategoryFilter = catFilter;

            // Load filtered items
            LoadItems(FilterItems(ContainerItem.GetStoredItems(catFilter)));
        }

        private void LoadFromInventory(List<Category> catFilter)
        {
            if (Inventory == null) return;
            lastCategoryFilter = catFilter;

            // Change to name list
            List<string> catNames = new List<string>();
            foreach (Category cat in catFilter)
            {
                catNames.Add(cat.name);
            }

            // Load filtered items
            LoadItems(FilterItems(Inventory.GetItems(catNames)));
        }

        private void LoadFromMerchant(List<Category> catFilter)
        {
            if (Merchant == null) return;
            lastCategoryFilter = catFilter;

            // Load filtered items
            List<InventoryItem> catItems = new List<InventoryItem>();
            foreach (ItemReference item in Merchant.BasicStock)
            {
                if (catFilter.Contains(item.item.category))
                {
                    catItems.Add(item.item);
                }
            }

            LoadItems(FilterItems(catItems));
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