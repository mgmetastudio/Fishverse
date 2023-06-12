using System.Collections.Generic;
using UnityEngine;

namespace NullSave.TOCK.Inventory
{
    public class CraftMenuUI : MonoBehaviour, ICloseable, IMenuHost
    {

        #region Enumerations

        public enum CraftingType
        {
            Normal = 0,
            Blind = 1
        }

        #endregion

        #region Variables

        public CraftingType craftingType;

        // Normal Crafting
        public RecipeItemGrid recipeGrid;

        // Blind Crafting
        public InventoryItemList playerInventory;
        public BlindItemList blindList;
        public bool destroyIfNoMatch = false;
        public bool alwaysUseSingle = false;

        #endregion

        #region Properties

        public InventoryCog Inventory { get; set; }

        public System.Action onCloseCalled { get; set; }

        #endregion

        #region Unityh Methods

        private void Start()
        {
            Inventory.onThemeWindowOpened?.Invoke(gameObject);
        }

        #endregion

        #region Public Methods

        public void AddSelectedToBlindCraft()
        {
            if(playerInventory == null)
            {
                Debug.LogWarning("No player inventory supplied!");
                return;
            }

            if (blindList == null)
            {
                Debug.LogWarning("No blind list supplied!");
                return;
            }

            int sel = playerInventory.SelectedIndex;
            InventoryItem item = playerInventory.SelectedItem.Item;
            if (!alwaysUseSingle && Inventory.GetItemTotalCount(item) >= Inventory.ActiveTheme.minCount)
            {
                System.Action<bool, int> callback = new System.Action<bool, int>((bool confirm, int count) =>
                {
                    if (confirm)
                    {
                        InventoryItem instance = Instantiate(item);
                        instance.name = item.name;
                        instance.CurrentCount = count;
                        Inventory.RemoveItem(item, count);
                        blindList.Items.Add(instance);
                        blindList.RefreshList();
                    }
                });

                Inventory.ActiveTheme.OpenCountSelect(item, Inventory.ActiveTheme.craftPrompt, callback);
            }
            else
            {
                InventoryItem instance = Instantiate(item);
                instance.name = item.name;
                instance.CurrentCount = 1;
                Inventory.RemoveItem(item, 1);
                blindList.Items.Add(instance);
                blindList.RefreshList();
            }
            playerInventory.SelectedIndex = sel;

        }

        public void BlindCraft()
        {
            if(blindList == null)
            {
                Debug.LogWarning("No blind list set, cannot blind craft!");
                return;
            }

            // Create List
            List<ItemReference> itemList = new List<ItemReference>();
            foreach (InventoryItem item in blindList.Items)
            {
                itemList.Add(new ItemReference(item, item.CurrentCount));
            }

            // Create master list (to take care of duplicates)
            int i;
            bool found;
            List<ItemReference> master = new List<ItemReference>();
            foreach (ItemReference item in itemList)
            {
                found = false;
                for (i = 0; i < master.Count; i++)
                {
                    if (master[i].item.name == item.item.name)
                    {
                        master[i].count += item.count;
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    master.Add(item);
                }
            }

            // Check for recipe
            CraftingRecipe recipe = Inventory.GetRecipe(master);
            if(recipe == null)
            {
                if(!destroyIfNoMatch)
                {
                    foreach (InventoryItem item in blindList.Items)
                    {
                        Inventory.AddToInventory(item, item.CurrentCount);
                    }
                }
            }
            else
            {
                Inventory.Craft(master);
            }

            blindList.Items.Clear();
            blindList.RefreshList();
        }

        public void Close()
        {
            Inventory.onThemeWindowClosed?.Invoke(gameObject);
            Inventory.ActiveTheme.CloseMenu(gameObject);
        }

        public void LoadComponents()
        {
            MenuHostHelper.LoadComponents(this, gameObject);
        }

        public void LoadCrafting(InventoryCog inventory)
        {
            if (recipeGrid != null)
            {
                recipeGrid.Inventory = inventory;
                recipeGrid.LoadRecipes();
            }
        }

        public void LoadCrafting(InventoryCog inventory, List<CraftingCategory> allowedCategories)
        {
            if (recipeGrid != null)
            {
                recipeGrid.Inventory = inventory;
                recipeGrid.LoadRecipes(allowedCategories);
            }
        }

        #endregion

        #region Menu Save/Load Methods

        public void Load(string filename)
        {
            Inventory.InventoryStateLoad(filename);
        }

        public void Load(System.IO.Stream stream)
        {
            Inventory.InventoryStateLoad(stream);
        }

        public void Save(string filename)
        {
            Inventory.InventoryStateSave(filename);
        }

        public void Save(System.IO.Stream stream)
        {
            Inventory.InventoryStateSave(stream);
        }

        #endregion

    }
}