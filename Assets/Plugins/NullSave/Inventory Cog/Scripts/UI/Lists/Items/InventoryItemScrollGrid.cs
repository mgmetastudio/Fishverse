using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace NullSave.TOCK.Inventory
{
    [HierarchyIcon("item-grid", false)]
    [RequireComponent(typeof(GridLayoutGroup))]
    public class InventoryItemScrollGrid : InventoryItemList
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

        private GridLayoutGroup prefabContainer;
        private RectTransform pcRT;
        private RectTransform viewPort;
        private ScrollRect scrollRect;

        private List<ItemUI> loadedItems;
        private List<Category> lastCategoryFilter;
        private List<InventoryItem> itemCache;

        private int selIndex;
        private ItemUI lastSel;

        private float nextRepeat;
        private bool waitForZero;
        private bool reloadCalled;

        private CategoryList lastSource;

        #endregion

        #region Properties

        public int ColsPerRow
        {
            get
            {
                if (pcRT == null) return 0;
                float w = pcRT.rect.width;
                if (w <= 0) w = pcRT.sizeDelta.x;
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
                float h = pcRT.rect.height;
                if (h <= 0) h = pcRT.sizeDelta.y;
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
                if (refreshedThisFrame)
                {
                    restoreIndex = value;
                    return;
                }
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
                        if(loadedItems[selIndex].Item == null && value != 0)
                        {
                            SelectedIndex = 0;
                        }
                    }
                    UpdateDetailUI(loadedItems[selIndex].Item);
                }
                else
                {
                    UpdateDetailUI(null);
                }

                onSelectionChanged?.Invoke(selIndex);
                UpdateCheckoutUI();
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
            viewPort = prefabContainer.gameObject.transform.parent.GetComponent<RectTransform>();
            scrollRect = viewPort.gameObject.transform.parent.GetComponent<ScrollRect>();
        }

        private void OnEnable()
        {
            if (loadMode == ListLoadMode.OnEnable && Inventory != null)
            {
                LoadItems();
            }
            else if(loadMode == ListLoadMode.FromShareTag)
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

            FocusSelected();
        }

        #endregion

        #region Public Methods

        public override void LoadFromCategoyList(CategoryList source)
        {
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
            if (!gameObject.activeInHierarchy) return;

            if (ColsPerRow <= 0)
            {
                StartCoroutine("UpdateRows");
                return;
            }

            Unsubscribe();
            Subscribe();

            itemCache = items;
            int totalItems = itemCache.Count;

            ClearItems();

            if (prefabContainer == null)
            {
                prefabContainer = GetComponent<GridLayoutGroup>();
                pcRT = prefabContainer.GetComponent<RectTransform>();
            }

            for (int i = 0; i < itemCache.Count; i++)
            {
                ItemUI ui = Instantiate(itemUIPrefab, prefabContainer.transform);
                ui.valueModifier = valueModifier;
                ui.LoadItem(Inventory, Container, itemCache[i]);
                ui.ItemListParent = this;
                ui.SetSelected(false);
                ui.onZeroCount.AddListener(ZeroCount);
                EventSubscribe(ui);
                loadedItems.Add(ui);
            }

            if (showLockedSlots)
            {
                int lockedSlots = GetLockedSlots(lastCategoryFilter);
                totalItems += lockedSlots;

                for (int i = 0; i < lockedSlots; i++)
                {
                    ItemUI ui = Instantiate(itemUIPrefab, prefabContainer.transform);
                    ui.LoadLockedSlot(Inventory);
                    ui.SetSelected(false);
                    EventSubscribe(ui);
                    loadedItems.Add(ui);
                }
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
                if(ph == 0)
                {
                    ph = parentRT.rect.height;
                    if(ph == 0)
                    {
                        parentRT = parentRT.parent.gameObject.GetComponent<RectTransform>();
                        ph = parentRT.sizeDelta.y;
                        if(ph == 0)
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
                    EventSubscribe(ui);
                    loadedItems.Add(ui);
                }
            }
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
                foreach(InventoryCog ic in FindObjectsOfType<InventoryCog>())
                {
                    if(ic.shareTag == shareTag)
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
            int cpr = ColsPerRow;
            int row = Mathf.FloorToInt(index / cpr);
            int col = index - (int)(row * cpr);
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

            loadedItems = new List<ItemUI>();
        }

        private void EventSubscribe(ItemUI item)
        {
            if (allowSelectByClick || ThemeAllowClick)
            {
                item.onClick.AddListener(SelectItem);
            }

            if (selectionMode == NavigationTypeEx.ByClick)
            {
                item.onClick.AddListener((ItemUI itemUI) => ClickSubmit(itemUI, lastCategoryFilter));
            }

            if (itemTooltip != null)
            {
                item.onPointerEnter.AddListener(ItemPointerEnter);
                item.onPointerExit.AddListener(ItemPointerExit);
            }
        }

        private IEnumerator FinalizeReloadLast()
        {
            yield return new WaitForEndOfFrame();

            refreshedThisFrame = false;
            bool wasNeg = selIndex < 0;
            if (lastSource == null)
            {
                LoadItems();
            }
            else
            {
                LoadFromCategoyList(lastSource);
            }
            SelectedIndex = restoreIndex == -2 ? wasNeg ? -1 : 0 : restoreIndex;
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

            float btm = Mathf.Abs(rt.anchoredPosition.y - (rt.rect.height * rt.pivot.y)) + prefabContainer.padding.top;
            float top = btm - rt.rect.height - prefabContainer.padding.top;

            if (btm > viewPort.rect.height + scrollRect.content.anchoredPosition.y)
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

        private void LoadFromContainer(List<Category> catFilter)
        {
            if (Container == null) return;
            lastCategoryFilter = catFilter;

            // Load filtered items
            List<InventoryItem> items = FilterItems(Container.GetStoredItems(catFilter));
            if(pageMode == GridPageMode.AllAvailable && Container.hasMaxStoreSlots)
            {
                while(items.Count < Container.maxStoreSlots)
                {
                    items.Add(null);
                }
            }
            LoadItems(items);
        }

        private void LoadFromContainerItem(List<Category> catFilter)
        {
            if (ContainerItem == null) return;
            lastCategoryFilter = catFilter;

            // Load filtered items
            List<InventoryItem> items = FilterItems(ContainerItem.GetStoredItems(catFilter));
            if (pageMode == GridPageMode.AllAvailable && ContainerItem.hasMaxStoreSlots)
            {
                while (items.Count < ContainerItem.maxStoreSlots)
                {
                    items.Add(null);
                }
            }
            LoadItems(items);
        }

        private void LoadFromInventory(List<Category> catFilter)
        {
            if (Inventory == null) return;
            lastCategoryFilter = catFilter;

            // Change to name list
            List<string> catNames = new List<string>();
            foreach(Category cat in catFilter)
            {
                catNames.Add(cat.name);
            }

            // Load items
            List<InventoryItem> catItems = new List<InventoryItem>();
            catItems.AddRange(Inventory.Items.Where(_ => catNames.Contains(_.category.name)));

            // Filter items
            List<InventoryItem> items = FilterItems(catItems);

            if (pageMode == GridPageMode.AllAvailable)
            {
                int emptySlots = 0;
                int lockedSlots = 0;

                foreach (Category category in catFilter)
                {
                    if (category.hasLockingSlots)
                    {
                        emptySlots += category.UnlockedSlots;
                        lockedSlots += category.MaximumSlots - category.UnlockedSlots;
                    }
                    else if (category.hasMaxSlots)
                    {
                        emptySlots += category.MaximumSlots;
                    }
                    else
                    {
                        emptySlots += category.UsedSlots;
                    }
                }

                while(items.Count < emptySlots)
                {
                    items.Add(null);
                }
            }

            LoadItems(items);
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
            while (ColsPerRow <= 0)
            {
                yield return new WaitForEndOfFrame();
            }

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