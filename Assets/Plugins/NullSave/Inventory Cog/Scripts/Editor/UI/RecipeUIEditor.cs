using UnityEditor;
using UnityEngine;

namespace NullSave.TOCK.Inventory
{
    [CustomEditor(typeof(RecipeUI))]
    public class RecipeUIEditor : TOCKEditorV2
    {

        #region Unity Methods

        public override void OnInspectorGUI()
        {
            MainContainerBegin("Recipe UI", "Icons/tock-ui");

            SectionHeader("Recipe UI Components");
            SimpleProperty("icon");
            SimpleProperty("displayName");
            SimpleProperty("description");
            SimpleProperty("categoryName");
            SimpleProperty("lockedIndicator");
            SimpleProperty("selectedIndicator");
            SimpleProperty("craftableIndicator");
            SimpleProperty("rarityColorIndicator");
            SimpleProperty("raritySlider");
            SimpleProperty("hideIfRarityZero", "Hide if Rarity 0");

            SectionHeader("Duration");
            SimpleProperty("duration");
            SimpleProperty("durationFormat");
            SimpleProperty("instantText");
            SimpleProperty("timeFormat");
            SimpleProperty("hideIfInstant");

            SectionHeader("Queue UI Components");
            SimpleProperty("monitorQueue");
            if (serializedObject.FindProperty("monitorQueue").boolValue)
            {
                SimpleProperty("queuedCount");
                SimpleProperty("countFormat");
                SimpleProperty("queueProgress");
                SimpleProperty("hideIfNoQueue");
            }

            SectionHeader("Colors");
            SimpleProperty("craftableColor");
            SimpleProperty("uncraftableColor");
            SimpleMultiSelect("colorApplication");

            SectionHeader("Component List");
            SimpleProperty("componentContainer", "Container");
            SimpleProperty("componentUIprefab", "Prefab");
            SimpleProperty("currencyImage");

            SectionHeader("Hide if Null Recipe", "hideWhenNull", typeof(GameObject));
            SimpleList("hideWhenNull");

            SectionHeader("Events");
            SimpleProperty("onClick");
            SimpleProperty("onCanCraft");
            SimpleProperty("onCannotCraft");

            MainContainerEnd();
        }

        #endregion

    }
}
