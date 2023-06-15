using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NullSave.TOCK.Inventory
{
    [HierarchyIcon("tock-ui", false)]
    public class RecipeUI : MonoBehaviour, IPointerClickHandler
    {

        #region Variables

        // Recipe UI
        public Image icon;
        public TextMeshProUGUI displayName, description, categoryName, duration, queuedCount;
        public string durationFormat = "{0}";
        public string instantText = "Instant";
        public Color craftableColor = Color.white;
        public GameObject hideIfInstant;
        // https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-timespan-format-strings
        [Tooltip("Uses standard c# time formatting")] public string timeFormat = "m\\:ss";
        public Color uncraftableColor = new Color(1, 1, 1, 0.4f);
        public GameObject lockedIndicator, selectedIndicator, craftableIndicator;
        public CraftingUIColor colorApplication = (CraftingUIColor)63;
        public string countFormat = "({count})";
        public Slider queueProgress;
        public GameObject hideIfNoQueue;
        public bool monitorQueue = false;
        public List<GameObject> hideWhenNull;

        public RarityColorIndicator rarityColorIndicator;
        public Slider raritySlider;
        public bool hideIfRarityZero;

        // component listing
        public RecipeComponentUI componentUIprefab;
        public Transform componentContainer;
        public Sprite currencyImage;

        public RecipeUIClick onClick, onCanCraft, onCannotCraft;

        private List<RecipeComponentUI> loadedComponents = new List<RecipeComponentUI>();

        #endregion

        #region Properties

        public InventoryCog Inventory { get; set; }

        public CraftingRecipe Recipe { get; set; }

        #endregion

        #region Unity Methods

        private void Update()
        {
            if (!monitorQueue || queueProgress == null || Recipe == null || Recipe.craftTime == CraftingTime.Instant) return;
            queueProgress.minValue = 0;
            queueProgress.maxValue = 1;
            queueProgress.value = Inventory.GetQueuedFirstProgress(Recipe);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            onClick?.Invoke(this);
        }

        #endregion

        #region Public Methods

        public void ClearComponents()
        {
            foreach (RecipeComponentUI component in loadedComponents)
            {
                Destroy(component.gameObject);
            }
            loadedComponents.Clear();
        }

        public void Click()
        {
            onClick?.Invoke(this);
        }

        public void LoadRecipe(CraftingRecipe recipe, bool craftable)
        {
            LoadRecipe(recipe, craftable, 1);
        }

        public void LoadRecipe(CraftingRecipe recipe, bool craftable, int craftCount)
        {
            Recipe = recipe;

            if (recipe == null)
            {
                if (icon != null) icon.gameObject.SetActive(false);
                if (displayName != null) displayName.gameObject.SetActive(false);
                if (description != null) description.gameObject.SetActive(false);
                if (categoryName != null) categoryName.gameObject.SetActive(false);
                if (craftableIndicator != null) craftableIndicator.gameObject.SetActive(false);
                if (lockedIndicator != null) lockedIndicator.gameObject.SetActive(false);
                if (hideIfInstant != null) hideIfInstant.gameObject.SetActive(false);
                if (duration != null) duration.gameObject.SetActive(false);
                if (raritySlider != null) raritySlider.gameObject.SetActive(false);
                if (hideIfNoQueue != null) hideIfNoQueue.gameObject.SetActive(false);
                if (rarityColorIndicator != null) rarityColorIndicator.gameObject.SetActive(false);
                if (selectedIndicator != null) selectedIndicator.SetActive(false);
                onCannotCraft?.Invoke(this);
                foreach (GameObject go in hideWhenNull)
                {
                    go.SetActive(false);
                }
                ClearComponents();
                Unsubscribe();
                return;
            }

            Color useColor = (recipe.unlocked && craftable) ? craftableColor : uncraftableColor;
            if (recipe.unlocked && craftable)
            {
                onCanCraft?.Invoke(this);
            }
            else
            {
                onCannotCraft?.Invoke(this);
            }

            // Recipe UI
            if (icon != null)
            {
                icon.sprite = recipe.icon;
                if ((colorApplication & CraftingUIColor.Icon) == CraftingUIColor.Icon)
                {
                    icon.color = craftable ? craftableColor : uncraftableColor;
                }
                icon.gameObject.SetActive(icon.sprite != null);
            }

            if (displayName != null)
            {
                displayName.gameObject.SetActive(true);
                displayName.text = recipe.displayName;
                if ((colorApplication & CraftingUIColor.RecipeName) == CraftingUIColor.RecipeName)
                {
                    displayName.color = useColor;
                }
            }

            if (description != null)
            {
                description.gameObject.SetActive(true);
                description.text = recipe.description;
                if ((colorApplication & CraftingUIColor.Description) == CraftingUIColor.Description)
                {
                    description.color = useColor;
                }
            }

            if (categoryName != null)
            {
                categoryName.gameObject.SetActive(true);
                categoryName.text = recipe.craftingCategory.displayName;
                if ((colorApplication & CraftingUIColor.Category) == CraftingUIColor.Category)
                {
                    categoryName.color = useColor;
                }
            }

            if (lockedIndicator != null)
            {
                lockedIndicator.SetActive(!recipe.Unlocked);
            }

            if (craftableIndicator != null)
            {
                craftableIndicator.SetActive(recipe.unlocked && craftable);
            }

            if (hideIfInstant != null && recipe.craftTime == CraftingTime.Instant)
            {
                hideIfInstant.SetActive(false);
            }

            if (duration != null)
            {
                switch (recipe.craftTime)
                {
                    case CraftingTime.Instant:
                        duration.text = durationFormat.Replace("{0}", instantText);
                        break;
                    case CraftingTime.RealTime:
                    case CraftingTime.GameTime:
                        System.TimeSpan ts = System.TimeSpan.FromSeconds(recipe.craftSeconds);
                        duration.text = durationFormat.Replace("{0}", ts.ToString(timeFormat));
                        break;
                }
            }

            if (componentUIprefab != null)
            {
                ClearComponents();

                if (recipe.componentType == ComponentType.Standard)
                {
                    foreach (ItemReference component in recipe.components)
                    {
                        RecipeComponentUI goComponent = Instantiate(componentUIprefab, componentContainer);
                        goComponent.LoadComponent(new ItemReference(component.item, component.count * craftCount), Inventory, 0, 0);
                        loadedComponents.Add(goComponent);
                    }
                }
                else
                {
                    foreach (AdvancedComponent component in recipe.advancedComponents)
                    {
                        RecipeComponentUI goComponent = Instantiate(componentUIprefab, componentContainer);
                        goComponent.LoadComponent(new ItemReference(component.item, component.count * craftCount), Inventory, component.minCondition, component.minRarity);
                        loadedComponents.Add(goComponent);
                    }
                }
            }

            if (rarityColorIndicator != null)
            {
                rarityColorIndicator.SetRarity(recipe.rarity);
            }

            if (raritySlider != null)
            {
                raritySlider.minValue = 0;
                raritySlider.maxValue = 10;
                raritySlider.value = recipe.rarity;
                if (hideIfRarityZero)
                {
                    raritySlider.gameObject.SetActive(recipe.rarity > 0);
                }
            }

            // Subscriptions
            Unsubscribe();
            if (monitorQueue)
            {
                Subscribe();
                QueueUpdated(null, 0);
            }
        }

        public void SetSelected(bool selected)
        {
            if (selectedIndicator != null) selectedIndicator.SetActive(selected && Recipe != null);
        }

        #endregion

        #region Private Methods

        private void QueueUpdated(CraftingRecipe recipe, int count)
        {
            int queued = Inventory.GetQueuedCount(Recipe);
            if (hideIfNoQueue)
            {
                hideIfNoQueue.SetActive(queued > 0);
            }
            if (queuedCount != null)
            {
                queuedCount.text = countFormat.Replace("{count}", queued.ToString());
            }

        }

        private void Subscribe()
        {
            if (Inventory != null)
            {
                Inventory.onCraftQueued.AddListener(QueueUpdated);
                Inventory.onQueuedCraftComplete.AddListener(QueueUpdated);
            }
        }

        private void Unsubscribe()
        {
            if (Inventory != null)
            {
                Inventory.onCraftQueued.RemoveListener(QueueUpdated);
                Inventory.onQueuedCraftComplete.RemoveListener(QueueUpdated);
            }
        }

        #endregion

    }
}