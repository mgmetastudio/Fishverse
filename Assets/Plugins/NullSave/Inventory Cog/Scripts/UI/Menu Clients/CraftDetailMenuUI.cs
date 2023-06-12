using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace NullSave.TOCK.Inventory
{
    public class CraftDetailMenuUI : MonoBehaviour, IMenuHost
    {

        #region Variables

        public TextMeshProUGUI minCount, maxCount, curCount;
        public RecipeUI recipeUI;
        public Slider countSlider;
        public UnityEvent onUncraftable;

        private Action<bool, int> closeCallback;
        private int maxCraft;

        #endregion

        #region Properties

        public InventoryCog Inventory { get; set; }

        public CraftingRecipe Recipe { get; private set; }

        #endregion

        #region Public Methods

        private void Start()
        {
            Inventory.onThemeWindowOpened?.Invoke(gameObject);
        }

        public void CancelCraft()
        {
            Inventory.onThemeWindowClosed?.Invoke(gameObject);
            closeCallback?.Invoke(false, 0);
        }

        public void ConfirmCraft()
        {
            Inventory.onThemeWindowClosed?.Invoke(gameObject);
            if (countSlider != null && countSlider.gameObject.activeSelf)
            {
                Inventory.Craft(Recipe, (int)countSlider.value);
                closeCallback?.Invoke(true, (int)countSlider.value);
            }
            else
            {
                Inventory.Craft(Recipe, 1);
                closeCallback?.Invoke(true, 1);
            }
        }

        public void LoadCraft(CraftingRecipe recipe, Action<bool, int> onCloseCallback)
        {
            Recipe = recipe;
            closeCallback = onCloseCallback;

            if(recipeUI != null)
            {
                recipeUI.Inventory = Inventory;
                recipeUI.LoadRecipe(recipe, Inventory.GetRecipeCraftable(recipe), 1);
            }

            maxCraft = Inventory.GetCraftableCount(recipe);

            if (minCount != null) minCount.text = "1";
            if (maxCount != null) maxCount.text = maxCraft <= 1 ? "1" : maxCraft.ToString();
            if (curCount != null) curCount.text = "1";

            if(countSlider != null)
            {
                countSlider.minValue = 1;
                countSlider.maxValue = maxCraft == 0 ? 1 : maxCraft;
                countSlider.value = 1;
                countSlider.onValueChanged.RemoveListener(UpdateCount);
                countSlider.onValueChanged.AddListener(UpdateCount);
                if(maxCraft == 0)
                {
                    countSlider.interactable = false;
                }
            }

            if(maxCraft < 1 || !recipe.Unlocked)
            {
                onUncraftable?.Invoke();
            }

            LoadComponents();
        }

        public void LoadComponents()
        {
            MenuHostHelper.LoadComponents(this, gameObject);
        }

        #endregion

        #region Private Methods

        private void UpdateCount(float count)
        {
            if (curCount != null)
            {
                curCount.text = ((int)count).ToString();
            }

            if (recipeUI != null)
            {
                recipeUI.Inventory = Inventory;
                recipeUI.LoadRecipe(Recipe, count <= maxCraft, (int)count);
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