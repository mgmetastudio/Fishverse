using NullSave.TOCK.Stats;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NullSave.TOCK.Inventory
{
    [HierarchyIcon("detailsUI", false)]
    public class ItemDetailUI : MonoBehaviour, IMenuHost, IItemHost
    {

        #region Variables

        public Image itemSprite;
        public TextMeshProUGUI itemName;
        public TextMeshProUGUI itemDescription;
        public TextMeshProUGUI itemSubtext;
        public ItemPreviewUI itemPreview;

        public TextMeshProUGUI categoryName;
        public Image categorySprite;

        public TextMeshProUGUI conditionText;
        public Slider conditionSlider;

        public RarityColorIndicator rarityIndicator;
        public Slider raritySlider;
        public Transform raritySpriteContainer;
        public Sprite raritySprite;
        public Vector2 raritySpriteSize = new Vector2(16, 16);

        public TextMeshProUGUI valueText;
        public TextMeshProUGUI weightText;

        public ItemTagUI tagPrefab;
        public Transform tagContainer;
        public List<string> tagLimitCategories;

        public TextMeshProUGUI ammoTypeText;

        public RecipeUI recipeUI;

        private List<GameObject> raritySprites;
        private ItemPreviewUI spawnedPreview;

        public StatModifierUI statModifierPrefab;
        public Transform statModContainer;

        public StatEffectListUI statEffectList;

        private bool activedSpawn;
        private GameObject activePlayerPreview;

        public float valueModifier = 1;
        public bool includeAttachments = true;

        #endregion

        #region Properties

        public InventoryCog Inventory { get; set; }

        public InventoryItem InventoryItem { get; set; }

        public LootItem LootItem { get { return null; } set {; } }

        #endregion

        #region Unity Methods

        private void OnDisable()
        {
            ClearPreview();
        }

        private void OnDestroy()
        {
            ClearPreview();
        }

        #endregion

        #region Public Methods

        public void BlankItem()
        {
            ClearItemTags();
            ClearStatModifiers();
            ClearStars();

            if (recipeUI != null) recipeUI.gameObject.SetActive(false);
            if (itemName != null) itemName.text = string.Empty;
            if (itemDescription != null) itemDescription.text = string.Empty;
            if (itemSubtext != null) itemSubtext.text = string.Empty;
            if (itemSprite != null)
            {
                itemSprite.sprite = null;
                itemSprite.enabled = false;
            }

            if (categoryName != null) categoryName.text = string.Empty;
            if (categorySprite != null)
            {
                categorySprite.sprite = null;
                categorySprite.enabled = false;
            }

            if (conditionText != null) conditionText.text = string.Empty;
            if (conditionSlider != null) conditionSlider.gameObject.SetActive(false);

            if (raritySlider != null) raritySlider.gameObject.SetActive(false);

            if (valueText != null) valueText.text = string.Empty;
            if (weightText != null) weightText.text = string.Empty;
            if (ammoTypeText != null) ammoTypeText.text = string.Empty;
            if (rarityIndicator != null) rarityIndicator.LoadItem(null);
        }

        public void LoadComponents()
        {
            MenuHostHelper.LoadComponents(this, gameObject);
        }

        public void LoadItem(InventoryCog inventory, InventoryItem item, Category category)
        {
            Inventory = inventory;
            InventoryItem = item;
            SetupPreview();
            ClearStatModifiers();
            ClearItemTags();
            ClearStars();
            LoadComponents();
            UpdateChildren();

            if (item == null)
            {
                BlankItem();
                return;
            }

            if (itemName != null) itemName.text = item.DisplayName;
            if (itemDescription != null) itemDescription.text = item.description;
            if (itemSubtext != null) itemSubtext.text = item.subtext;
            if (itemSprite != null)
            {
                itemSprite.sprite = item.icon;
                itemSprite.enabled = item.icon != null;
            }

            if (categoryName != null) categoryName.text = category.displayName;
            if (categorySprite != null)
            {
                categorySprite.sprite = category.icon;
                categorySprite.enabled = categorySprite.sprite != null;
            }

            if (conditionText != null)
            {
                conditionText.text = Mathf.Round(item.condition * 100) + "%";
            }
            if (conditionSlider != null)
            {
                conditionSlider.gameObject.SetActive(true);
                conditionSlider.value = item.condition;
            }

            if (raritySlider != null)
            {
                raritySlider.gameObject.SetActive(true);
                raritySlider.value = item.rarity;
            }

            if (rarityIndicator != null)
            {
                rarityIndicator.LoadItem(item);
            }

            if (raritySpriteContainer != null && raritySprite != null)
            {
                raritySprites = new List<GameObject>();
                for (int i = 0; i < item.rarity; i++)
                {
                    GameObject go = new GameObject("rarity_sprite");
                    RectTransform rt = go.AddComponent<RectTransform>();
                    rt.sizeDelta = raritySpriteSize;
                    Image img = go.AddComponent<Image>();
                    img.sprite = raritySprite;
                    img.preserveAspect = true;
                    go.transform.SetParent(raritySpriteContainer);
                    raritySprites.Add(go);
                }
            }

            if (valueText != null) valueText.text = (item.value * valueModifier).ToString();
            if (weightText != null) weightText.text = item.weight.ToString();

            if (ammoTypeText != null)
            {
                if (item.usesAmmo)
                {
                    ammoTypeText.text = item.ammoType;
                }
                else
                {
                    ammoTypeText.text = string.Empty;
                }
            }

            if (recipeUI != null)
            {
                if (recipeUI.Inventory == null)
                {
                    recipeUI.Inventory = inventory;
                }
                recipeUI.LoadRecipe(item.displayRecipe, inventory.GetRecipeCraftable(item.displayRecipe));
                recipeUI.gameObject.SetActive(item.displayRecipe != null);
            }

#if INVENTORY_COG
            if (statEffectList != null)
            {
                statEffectList.LoadEffects(item);
            }
#endif

            LoadStatMods();
            LoadItemTags();
        }

        public void UpdateChildren()
        {
            ItemHostHelper.UpdateChildren(this, gameObject);
        }

        #endregion

        #region Private Methods

        private void ClearItemTags()
        {
            if (tagContainer == null) return;

            ItemTagUI[] tags = tagContainer.gameObject.GetComponentsInChildren<ItemTagUI>();
            foreach (ItemTagUI tag in tags)
            {
                Destroy(tag.gameObject);
            }
        }

        private void ClearPreview()
        {
            if (spawnedPreview != null)
            {
                if (!activedSpawn)
                {
                    Destroy(spawnedPreview.gameObject);
                }
                else
                {
                    spawnedPreview.ClearPreview();
                    spawnedPreview.gameObject.SetActive(false);
                }
                spawnedPreview = null;
            }
        }

        private void ClearStars()
        {
            if (raritySprites != null)
            {
                foreach (GameObject go in raritySprites)
                {
                    Destroy(go);
                }
                raritySprites = null;
            }
        }

        private void ClearStatModifiers()
        {
            if (statModContainer == null) return;

            StatModifierUI[] mods = statModContainer.gameObject.GetComponentsInChildren<StatModifierUI>();
            foreach (StatModifierUI mod in mods)
            {
                Destroy(mod.gameObject);
            }
        }

        private void LoadStatMods()
        {
            if (statModContainer == null || Inventory == null || Inventory.gameObject.GetComponent<Stats.StatsCog>() == null) return;

            foreach (StatEffect effect in InventoryItem.statEffects)
            {
                foreach (StatModifier mod in effect.modifiers)
                {
                    if (!mod.hideInList)
                    {
                        Instantiate(statModifierPrefab, statModContainer).LoadModifier(Inventory, InventoryItem, mod);
                    }
                }
            }
        }

        private void LoadItemTags()
        {
            if (tagContainer == null) return;

            foreach (InventoryItemUITag tag in InventoryItem.uiTags)
            {
                if (tagLimitCategories.Count == 0 || tagLimitCategories.Contains(tag.Category))
                {
                    Instantiate(tagPrefab, tagContainer).LoadTag(Inventory, tag);
                }
            }

            if (includeAttachments)
            {
                foreach (AttachmentSlot slot in InventoryItem.Slots)
                {
                    if (slot.AttachedItem != null)
                    {
                        foreach (InventoryItemUITag tag in slot.AttachedItem.uiTags)
                        {
                            if (!tag.hideWhenAttached)
                            {
                                if (tagLimitCategories.Count == 0 || tagLimitCategories.Contains(tag.Category))
                                {
                                    Instantiate(tagPrefab, tagContainer).LoadTag(Inventory, tag);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void SetupPreview()
        {
            if (spawnedPreview != null)
            {
                spawnedPreview.LoadPreview(InventoryItem);
            }
            else if (itemPreview != null)
            {
                if (itemPreview.gameObject.scene == null)
                {
                    spawnedPreview = Instantiate(itemPreview);
                }
                else
                {
                    itemPreview.gameObject.SetActive(true);
                    spawnedPreview = itemPreview;
                    activedSpawn = true;
                }
                spawnedPreview.LoadPreview(InventoryItem);
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