using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NullSave.TOCK.Inventory
{
    [HierarchyIcon("item-grid", false)]
    [RequireComponent(typeof(GridLayoutGroup))]
    public class RecipeItemGrid : RecipeList
    {

        #region Variables

        // UI
        public RecipeUI itemUIPrefab;
        public RecipeUI recipeDetail;
        public RecipeComponentUI componentUIprefab;
        public Transform componentContainer;
        public bool fillContainer;

        // Multi-craft
        public bool allowMulticraft = false;
        public CountSelectUI countSelectUI;
        public int minToShowCount = 5;
        public Transform countContainer;

        // Navigation
        public string inputHorizontal = "Horizontal";
        public string inputVertical = "Vertical";
        public KeyCode keyLeft = KeyCode.A;
        public KeyCode keyUp = KeyCode.W;
        public KeyCode keyRight = KeyCode.D;
        public KeyCode keyDown = KeyCode.S;

        private List<RecipeUI> loadedItems;
        private List<RecipeComponentUI> loadedComponents;

        private int selIndex;
        private List<string> lastCategoryFilter;

        private CountSelectUI spawnedCount;

        private float nextRepeat;
        private bool waitForZero;

        private GridLayoutGroup prefabContainer;
        private RectTransform pcRT;

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
                if (h == 0) h = pcRT.rect.height;
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
                    SetComponents(loadedItems[selIndex].Recipe);
                    SetDetail(loadedItems[selIndex].Recipe);
                }
                else
                {
                    SetComponents(null);
                    SetDetail(null);
                }
            }
        }

        public override RecipeUI SelectedItem
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

        private void OnEnable()
        {
            if (Inventory != null & loadMode == ListLoadMode.OnEnable)
            {
                LoadRecipes(categories);
            }
        }

        private void Start()
        {
            if (loadMode == ListLoadMode.OnEnable)
            {
                LoadRecipes(categories);
            }
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
                            onItemSubmit?.Invoke(this, SelectedItem);
                        }
                        break;
                    case NavigationTypeEx.ByKey:
                        if (InventoryCog.GetKeyDown(keySubmit))
                        {
                            onItemSubmit?.Invoke(this, SelectedItem);
                        }
                        break;
                }
            }
        }

        #endregion

        #region Public Methods

        public void CraftSelected()
        {
            if (SelectedItem == null) return;
            if (!SelectedItem.Recipe.Unlocked) return;

            int selected = SelectedIndex;
            int maxCraft = inventoryCog.GetCraftableCount(SelectedItem.Recipe);

            if (maxCraft >= Inventory.ActiveTheme.minCount)
            {
                Action<bool, int> callback = (bool craft, int count) =>
                {
                    if (craft)
                    {
                        CraftSelectedCount(count);
                    }
                };

                Inventory.ActiveTheme.OpenCountSelect(SelectedItem.Recipe, "Craft", callback);
            }
            else
            {
                inventoryCog.Craft(SelectedItem.Recipe, 1);
                ReloadLast();
                SelectedIndex = selected;
            }
        }

        public void CraftSelectedCount(int count)
        {
            if (!SelectedItem.Recipe.Unlocked) return;

            int selected = SelectedIndex;
            inventoryCog.Craft(SelectedItem.Recipe, count);
            ReloadLast();
            SelectedIndex = selected;
        }

        public override void LoadFromCategoyList(CraftingCategoryList source)
        {
            List<string> cats = new List<string>();
            CraftingCategory category = source.SelectedItem;

            if (category.name == "__ic_allitems")
            {
                switch (source.categoryFilter)
                {
                    case ListCategoryFilter.All:
                        LoadRecipes();
                        break;
                    default:
                        foreach (CraftingCategory cat in source.FilteredCategories)
                        {
                            cats.Add(cat.name);
                        }
                        LoadRecipes(cats);
                        break;
                }
            }
            else
            {
                cats.Add(category.name);
                LoadRecipes(cats);
            }
        }

        public override void LoadRecipes()
        {
            LoadRecipes(categories);
        }

        public override void LoadRecipes(List<string> categories)
        {
            if (Inventory == null) return;

            ClearItems();

            lastCategoryFilter = categories;
            foreach (CraftingRecipe recipe in Inventory.Recipes)
            {
                if ((recipe.craftingCategory != null && categories.Contains(recipe.craftingCategory.name)) || categories.Count == 0)
                {
                    if (recipe.DisplayInList)
                    {
                        RecipeUI ui = Instantiate(itemUIPrefab, transform);
                        ui.Inventory = Inventory;
                        ui.LoadRecipe(recipe, Inventory.GetRecipeCraftable(recipe));
                        ui.SetSelected(loadedItems.Count == 0 && (!LockInput || !hideSelectionWhenLocked));
                        if (allowSelectByClick || (Inventory.ActiveTheme != null && Inventory.ActiveTheme.enableUIClick))
                        {
                            ui.onClick.AddListener(SelectItem);
                        }
                        if(selectionMode == NavigationTypeEx.ByClick)
                        {
                            ui.onClick.AddListener(SubmitItem);
                        }
                        loadedItems.Add(ui);
                    }
                }
            }

            if(fillContainer)
            {
                // Ensure content height
                RectTransform parentRT = pcRT.parent.gameObject.GetComponent<RectTransform>();
                if (pcRT.sizeDelta.y < parentRT.rect.height)
                {
                    pcRT.sizeDelta = new Vector2(pcRT.sizeDelta.x, parentRT.rect.height);
                }

                if (ColsPerRow == 0 || RowsPerPage == 0)
                {
                    StartCoroutine("WaitForFrame");
                }
                else
                {
                    while (loadedItems.Count < ItemsPerPage)
                    {
                        RecipeUI ui = Instantiate(itemUIPrefab, transform);
                        ui.Inventory = Inventory;
                        ui.LoadRecipe(null, false);
                        ui.SetSelected(loadedItems.Count == 0 && (!LockInput || !hideSelectionWhenLocked));
                        loadedItems.Add(ui);
                    }
                }
            }

            SelectedIndex = 0;
        }

        public override void LoadRecipes(List<CraftingCategory> categories)
        {
            List<string> catNames = new List<string>();
            foreach(CraftingCategory category in categories)
            {
                catNames.Add(category.name);
            }
            LoadRecipes(catNames);
        }

        public void LoadRecipesForCategory(string category)
        {
            List<string> cats = new List<string>();
            cats.Add(category);
            LoadRecipes(cats);
        }

        public override void ReloadLast()
        {
            if (lastCategoryFilter == null) lastCategoryFilter = categories;
            LoadRecipes(lastCategoryFilter);
        }

        #endregion

        #region Private Methods

        public Vector2 CellFromIndex(int index)
        {
            int row = Mathf.FloorToInt(index / ColsPerRow);
            int col = index - (int)(row * ColsPerRow);
            return new Vector2(col, row);
        }

        private void ClearItems()
        {
            selIndex = -1;
            if (loadedItems != null)
            {
                foreach (RecipeUI ui in loadedItems)
                {
                    Destroy(ui.gameObject);
                }
            }
            loadedItems = new List<RecipeUI>();

            ClearComponents();
        }

        private void ClearComponents()
        {
            if (loadedComponents != null)
            {
                foreach (RecipeComponentUI ui in loadedComponents)
                {
                    Destroy(ui.gameObject);
                }
            }
            loadedComponents = new List<RecipeComponentUI>();
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
            if (selIndex < 0 || selIndex >= loadedItems.Count) return;
            loadedItems[selIndex].SetSelected(true);
        }

        private void UnselectCurrent()
        {
            if (selIndex < 0 || selIndex >= loadedItems.Count) return;
            loadedItems[selIndex].SetSelected(false);
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
            // Up
            int newIndex = selIndex - (int)ColsPerRow;
            if (newIndex >= 0)
            {
                SelectedIndex = newIndex;
            }

            UpdateRepeat();
        }

        private void SetComponents(CraftingRecipe recipe)
        {
            if (componentUIprefab == null) return;

            foreach (RecipeComponentUI component in loadedComponents)
            {
                Destroy(component.gameObject);
            }
            loadedComponents.Clear();

            if (recipe.componentType == ComponentType.Standard)
            {
                foreach (ItemReference component in recipe.components)
                {
                    RecipeComponentUI newComponent = Instantiate(componentUIprefab, componentContainer);
                    newComponent.LoadComponent(component, inventoryCog, 0, 0);
                    loadedComponents.Add(newComponent);
                }
            }
            else
            {
                foreach (AdvancedComponent component in recipe.advancedComponents)
                {
                    RecipeComponentUI newComponent = Instantiate(componentUIprefab, componentContainer);
                    newComponent.LoadComponent(new ItemReference(component.item, component.count), inventoryCog, component.minCondition, component.minRarity);
                    loadedComponents.Add(newComponent);
                }
            }
        }

        private void SetDetail(CraftingRecipe recipe)
        {
            if (recipeDetail != null)
            {
                recipeDetail.Inventory = inventoryCog;
                recipeDetail.LoadRecipe(recipe, inventoryCog.GetRecipeCraftable(recipe));
            }
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

        private IEnumerator WaitForFrame()
        {
            yield return new WaitForEndOfFrame();
            ReloadLast();
        }

        #endregion

    }
}