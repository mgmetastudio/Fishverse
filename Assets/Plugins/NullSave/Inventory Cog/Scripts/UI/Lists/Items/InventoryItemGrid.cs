using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace NullSave.TOCK.Inventory
{
    [HierarchyIcon("item-grid", false)]
    [RequireComponent(typeof(GridLayoutGroup))]
    public class InventoryItemGrid : InventoryItemList
    {

        #region Variables

        // UI
        public ItemUI itemUIPrefab;
        public GridPageMode pageMode;
        public bool alwaysFillPage;

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
        private List<Category> lastCategoryFilter;
        private List<InventoryItem> itemCache;

        private int selIndex;

        private float nextRepeat;
        private bool waitForZero;
        private int selectedPage;
        private bool startFromLast;
        private bool reloadCalled;

        private CategoryList lastSource;

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

        public virtual bool IsLockedToCategory { get; set; }

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

        public int SelectedPage
        {
            get
            {
                return selectedPage;
            }
            private set
            {
                selectedPage = value;
                onPageChanged?.Invoke(selectedPage);
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
                if(refreshedThisFrame)
                {
                    restoreIndex = value;
                    return;
                }
                if (loadedItems == null) return;
                if (loadedItems.Count <= value) value = loadedItems.Count - 1;
                if (value < -1) value = -1;
                if (selIndex <= loadedItems.Count - 1 && selIndex >= 0) loadedItems[selIndex].SetSelected(false);
                selIndex = value;

                if (selIndex >= 0 && selIndex <= loadedItems.Count - 1)
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
                        UpdateCheckoutUI();
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

        private void OnEnable()
        {
            if (loadMode == ListLoadMode.OnEnable && Inventory != null)
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
            reloadCalled = false;

            if (!LockInput)
            {
                UpdateNavigation();

                switch (selectionMode)
                {
                    case NavigationTypeEx.ByButton:
                        if (InventoryCog.GetButtonDown(buttonSubmit))
                        {
                            onItemSubmit?.Invoke(this, SelectedItem, lastCategoryFilter);
                        }
                        break;
                    case NavigationTypeEx.ByKey:
                        if (InventoryCog.GetKeyDown(keySubmit))
                        {
                            onItemSubmit?.Invoke(this, SelectedItem, lastCategoryFilter);
                        }
                        break;
                }
            }
        }

        #endregion

        #region Public Methods

        public override void LoadFromCategoyList(CategoryList source)
        {
            LoadFromCategoyList(source, false);
        }

        public override void LoadFromCategoyList(CategoryList source, bool startFromLastPage)
        {
            startFromLast = startFromLastPage;
            Category category = source.SelectedItem;
            if (category == null)
            {
                LoadItems();
                return;
            }
            if (category.name == "__ic_allitems")
            {
                LoadItems();
            }
            else
            {
                List<Category> catFilter = new List<Category>();
                switch (listSource)
                {
                    case ListSource.InventoryCog:
                        catFilter.Add(Inventory.GetCategory(category.name));
                        LoadFromInventory(catFilter);
                        break;
                    case ListSource.InventoryContainer:
                        catFilter.Add(category);
                        LoadFromContainer(catFilter);
                        break;
                    case ListSource.InventoryMerchant:
                        catFilter.Add(category);
                        LoadFromMerchant(catFilter);
                        break;
                    case ListSource.ContainerItem:
                        catFilter.Add(category);
                        LoadFromContainerItem(catFilter);
                        break;
                }
            }

            lastSource = source;
        }

        public override void LoadItems()
        {
            lastSource = null;

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
            itemCache = items;
            LoadPage(0);
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

        public Vector2 CellFromIndex(int index)
        {
            int row = Mathf.FloorToInt(index / ColsPerRow);
            int col = index - (int)(row * ColsPerRow);
            return new Vector2(col, row);
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

            UpdateDetailUI(null);
            selIndex = -1;
            onSelectionChanged?.Invoke(-1);

            loadedItems = new List<ItemUI>();
        }

        private IEnumerator FinalizeReloadLast()
        {
            yield return new WaitForEndOfFrame();

            refreshedThisFrame = false;
            selIndex = -1;
            int curPage = SelectedPage;
            if (lastSource == null)
            {
                LoadItems();
            }
            else
            {
                LoadFromCategoyList(lastSource, false);
            }
            SelectedPage = curPage;
            SelectedIndex = restoreIndex == -2 ? 0 : restoreIndex;
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
                                Category catRef = Inventory.GetCategory(category.name);
                                if (catRef.catUnlocked)
                                {
                                    catFilter.Add(catRef);
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
                                Category catRef = Inventory.GetCategory(category.name);
                                if (catRef.catUnlocked)
                                {
                                    catFilter.Add(catRef);
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
                                    Category catRef = Inventory.GetCategory(category.name);
                                    if (catRef.catUnlocked)
                                    {
                                        catFilter.Add(catRef);
                                    }
                                    break;
                            }
                        }
                    }
                    break;
            }

            return catFilter;
        }

        private int GetLockedSlots(List<Category> catFilter)
        {
            int lockedSlots = 0;
            if (listSource == ListSource.InventoryCog)
            {
                foreach (Category category in catFilter)
                {
                    if (category.hasLockingSlots)
                    {
                        lockedSlots += category.MaximumSlots - category.UnlockedSlots;
                    }
                }
            }
            return lockedSlots;
        }

        private int GetMaxPages(List<Category> catFilter, out int usedSlots)
        {
            usedSlots = 0;

            float slotsPerPage = (int)(ColsPerRow * RowsPerPage);
            if (listSource == ListSource.InventoryContainer)
            {
                if (pageMode == GridPageMode.AllAvailable)
                {
                    if (container.hasMaxStoreSlots)
                    {
                        usedSlots = container.maxStoreSlots;
                        return Mathf.CeilToInt(Container.maxStoreSlots / slotsPerPage);
                    }
                    else
                    {
                        usedSlots = Mathf.CeilToInt(Mathf.Max(container.StoredItems.Count, slotsPerPage));
                        return Mathf.CeilToInt(Mathf.Max(container.StoredItems.Count, slotsPerPage) / slotsPerPage);
                    }
                }
                else
                {
                    usedSlots = container.StoredItems.Count;
                    return Mathf.CeilToInt(Container.StoredItems.Count / slotsPerPage);
                }
            }
            else if (listSource == ListSource.ContainerItem)
            {
                if (pageMode == GridPageMode.AllAvailable)
                {
                    if (ContainerItem.hasMaxStoreSlots)
                    {
                        usedSlots = ContainerItem.maxStoreSlots;
                        return Mathf.CeilToInt(ContainerItem.maxStoreSlots / slotsPerPage);
                    }
                    else
                    {
                        usedSlots = Mathf.CeilToInt(Mathf.Max(ContainerItem.StoredItems.Count, slotsPerPage));
                        return Mathf.CeilToInt(Mathf.Max(ContainerItem.StoredItems.Count, slotsPerPage) / slotsPerPage);
                    }
                }
                else
                {
                    usedSlots = ContainerItem.StoredItems.Count;
                    return Mathf.CeilToInt(ContainerItem.StoredItems.Count / slotsPerPage);
                }
            }
            else
            {
                int lockedSlots = 0;
                foreach (Category category in catFilter)
                {
                    if (pageMode == GridPageMode.AllAvailable)
                    {
                        if (category.hasLockingSlots)
                        {
                            usedSlots += category.UnlockedSlots;
                            lockedSlots += category.MaximumSlots - category.UnlockedSlots;
                        }
                        else if (category.hasMaxSlots)
                        {
                            usedSlots += category.MaximumSlots;
                        }
                        else
                        {
                            usedSlots += category.UsedSlots;
                        }
                    }
                    else
                    {
                        usedSlots += category.UsedSlots;
                    }
                }

                if (showLockedSlots)
                {
                    return Mathf.CeilToInt((usedSlots + lockedSlots) / slotsPerPage);
                }
                else
                {
                    return Mathf.CeilToInt(usedSlots / slotsPerPage);
                }
            }
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

            // Load filtered items
            // Change to name list
            List<string> catNames = new List<string>();
            foreach (Category cat in catFilter)
            {
                catNames.Add(cat.name);
            }

            List<InventoryItem> catItems = new List<InventoryItem>();
            catItems.AddRange(Inventory.Items.Where(_ => catNames.Contains(_.category.name)));

            LoadItems(FilterItems(catItems));
        }

        private void LoadFromMerchant(List<Category> catFilter)
        {
            if (Inventory == null) return;
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

        private void LoadPage(int pageIndex)
        {
            if (!gameObject.activeInHierarchy) return;

            if (ColsPerRow <= 0)
            {
                StartCoroutine("UpdateRows");
                return;
            }

            Unsubscribe();
            Subscribe();

            int usedSlots;
            int maxPerPage = (int)(ColsPerRow * RowsPerPage);
            int maxPages = GetMaxPages(lastCategoryFilter, out usedSlots);
            int lockedSlots = 0;

            selIndex = -1;

            if (showLockedSlots)
            {
                lockedSlots = GetLockedSlots(lastCategoryFilter);
            }

            if (startFromLast)
            {
                pageIndex = maxPages - 1;
                startFromLast = false;
            }

            // Check for page constraints
            if (pageIndex < 0)
            {
                if (!IsLockedToCategory)
                {
                    onNeedPreviousCategory?.Invoke();
                    SelectedIndex = 0;
                }
                return;
            }
            if (pageIndex > maxPages)
            {
                if (!IsLockedToCategory)
                {
                    onNeedNextCategory?.Invoke();
                    SelectedIndex = 0;
                }
                return;
            }

            ClearItems();
            SelectedPage = pageIndex;

            int maxItems = Mathf.Min(maxPerPage, usedSlots);
            int usedItems = 0;
            int skipItems = SelectedPage * maxItems;

            for (int i = skipItems; i < itemCache.Count; i++)
            {
                ItemUI ui = Instantiate(itemUIPrefab, prefabContainer.transform);
                ui.valueModifier = valueModifier;
                ui.LoadItem(Inventory, Container, itemCache[i]);
                ui.ItemListParent = this;
                ui.SetSelected(loadedItems.Count == 0 && (!LockInput || !hideSelectionWhenLocked));
                ui.onZeroCount.AddListener(ZeroCount);
                EventSubscribe(ui);
                loadedItems.Add(ui);
                maxItems -= 1;
                usedItems += 1;
                if (maxItems == 0)
                {
                    break;
                }
            }

            if (skipItems > itemCache.Count)
            {
                maxItems = 0;
            }

            while (maxItems > 0)
            {
                ItemUI ui = Instantiate(itemUIPrefab, prefabContainer.transform);
                ui.ItemListParent = this;
                ui.valueModifier = valueModifier;
                ui.LoadItem(Inventory, Container, null);
                ui.SetSelected(false);
                EventSubscribe(ui);
                loadedItems.Add(ui);
                maxItems -= 1;
                usedItems += 1;
            }

            if (showLockedSlots)
            {
                while (usedItems < maxPerPage && lockedSlots > 0)
                {
                    ItemUI ui = Instantiate(itemUIPrefab, prefabContainer.transform);
                    ui.LoadLockedSlot(Inventory);
                    ui.SetSelected(false);
                    EventSubscribe(ui);
                    loadedItems.Add(ui);
                    lockedSlots -= 1;
                    usedItems += 1;
                }
            }

            if (pageMode == GridPageMode.AllAvailable && (alwaysFillPage || (lastCategoryFilter.Count == 1 && !lastCategoryFilter[0].hasMaxSlots)))
            {
                while (usedItems < maxPerPage)
                {
                    ItemUI ui = Instantiate(itemUIPrefab, prefabContainer.transform);
                    ui.ItemListParent = this;
                    ui.LoadItem(Inventory, Container, null);
                    ui.SetSelected(false);
                    EventSubscribe(ui);
                    loadedItems.Add(ui);
                    usedItems += 1;
                }
            }

            SelectedIndex = 0;
            SelectedPage = pageIndex;
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

        private void EventSubscribe(ItemUI item)
        {
            if (allowSelectByClick || ThemeAllowClick)
            {
                item.onClick.AddListener(SelectItem);
            }

            if(selectionMode == NavigationTypeEx.ByClick)
            {
                item.onClick.AddListener((ItemUI itemUI) => ClickSubmit(itemUI, lastCategoryFilter));
            }

            if (itemTooltip != null)
            {
                item.onPointerEnter.AddListener(ItemPointerEnter);
                item.onPointerExit.AddListener(ItemPointerExit);
            }
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
            else
            {
                if (SelectedPage > 0)
                {
                    Vector2 newSel = SelectedCell;
                    newSel.x = ColsPerRow - 1;

                    LoadPage(SelectedPage - 1);
                    SelectedCell = newSel;
                }
                else
                {
                    if (!IsLockedToCategory)
                    {
                        onNeedPreviousCategory?.Invoke();
                    }
                }
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
            else
            {
                if (SelectedPage < GetMaxPages(lastCategoryFilter, out int used) - 1)
                {
                    Vector2 newSel = SelectedCell;
                    newSel.x = 0;

                    LoadPage(SelectedPage + 1);
                    SelectedCell = newSel;
                }
                else
                {
                    if (!IsLockedToCategory)
                    {
                        onNeedNextCategory?.Invoke();
                    }
                }
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

        private IEnumerator UpdateRows()
        {
            yield return new WaitForEndOfFrame();
            ReloadLast();
        }

        private void ZeroCount(ItemUI item)
        {
            if (reloadCalled) return;
            reloadCalled = true;
            ReloadLast();
        }

        #endregion

    }
}