using UnityEditor;
using UnityEngine;

namespace NullSave.TOCK.Inventory
{
    [CanEditMultipleObjects()]
    [CustomEditor(typeof(ItemDetailUI))]
    public class ItemDetailUIEditor : TOCKEditorV2
    {

        #region Unity Methods

        public override void OnInspectorGUI()
        {
            MainContainerBegin("Item Detail UI", "Icons/detailsUI");

            SectionHeader("Item");
            SimpleProperty("itemSprite", "Sprite");
            SimpleProperty("itemName", "Name");
            SimpleProperty("itemDescription", "Description");
            SimpleProperty("itemSubtext", "Subtext");
            SimpleProperty("itemPreview");
            SimpleProperty("recipeUI");
            SimpleProperty("ammoTypeText", "Ammo Type");
            SimpleProperty("valueText", "Value");
            SimpleProperty("weightText", "Weight");

            SectionHeader("Category");
            SimpleProperty("categorySprite", "Sprite");
            SimpleProperty("categoryName", "Name");

            SectionHeader("Condition");
            SimpleProperty("conditionText", "Text");
            SimpleProperty("conditionSlider", "Slider");

            SectionHeader("Rarity");
            SimpleProperty("rarityIndicator", "Color Indicator");
            SimpleProperty("raritySlider", "Slider");
            SimpleProperty("raritySprite", "Sprite");
            SimpleProperty("raritySpriteContainer", "Sprite Container");
            SimpleProperty("raritySpriteSize", "Sprite Size");

            SectionHeader("Tags");
            SimpleProperty("tagPrefab");
            SimpleProperty("tagContainer");
            SimpleProperty("includeAttachments");
            SubHeader("Tags Category Filter");
            SimpleList("tagLimitCategories", null);

            SectionHeader("Stats Cog");
            SimpleProperty("statModifierPrefab", "Modifier Prefab");
            SimpleProperty("statModContainer", "Modifier Container");
            SimpleProperty("statEffectList", "Effect List UI");

            MainContainerEnd();
        }

        #endregion

    }
}