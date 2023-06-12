using UnityEditor;

namespace NullSave.TOCK.Inventory
{
    [CanEditMultipleObjects()]
    [CustomEditor(typeof(ItemUI))]
    public class ItemUIEditor : TOCKEditorV2
    {

        #region Unity Methods

        public override void OnInspectorGUI()
        {
            MainContainerBegin("Item UI", "Icons/item_icon", false);

            SectionHeader("Item Info");
            SimpleProperty("itemImage", "Icon");
            SimpleProperty("displayName", "Name");
            SimpleProperty("description");
            SimpleProperty("subtext");
            SimpleProperty("hideIfNoSubtext");
            SimpleProperty("weight");
            SimpleProperty("value");
            SimpleProperty("repairComponents");
            SimpleProperty("recipeUI");

            SectionHeader("Background");
            SimpleProperty("bkgItem", "Default");
            SimpleProperty("bkgNoItem", "No Item");

            SectionHeader("Drag and Drop");
            SimpleProperty("enableDragDrop", "Enable");

            SectionHeader("Count");
            SimpleProperty("countPrefix", "Prefix");
            SimpleProperty("count", "Text");
            SimpleProperty("countSuffix", "Suffix");
            SimpleProperty("hideIfCountSub2", "Hide if Count < 2");

            SectionHeader("Indicators");
            SimpleProperty("selectedIndicator", "Selected");
            SimpleProperty("equipableIndicator", "Can Equip");
            SimpleProperty("equippedIndicator", "Is Equiped");
            SimpleProperty("lockedIndicator");
            SimpleProperty("rarityColorIndicator");
            SimpleProperty("conditionSlider");
            SimpleProperty("hideIfConditionZero", "Hide if Condition 0");
            SimpleProperty("hideIfConditionOne", "Hide if Condition 1");
            SimpleProperty("raritySlider");
            SimpleProperty("hideIfRarityZero", "Hide if Rarity 0");

            SectionHeader("Attachment Slots");
            SimpleProperty("slotContainer");
            SimpleProperty("slotPrefab");

            SectionHeader("Tags");
            SimpleProperty("tagPrefab", "Prefab");
            SimpleProperty("tagContainer", "Container");

            SectionHeader("Events");
            SimpleProperty("onClick");
            SimpleProperty("onLoadedItem");

            MainContainerEnd();
        }

        #endregion

    }
}