using UnityEditor;

namespace NullSave.TOCK.Inventory
{
    [CanEditMultipleObjects()]
    [CustomEditor(typeof(CraftDetailMenuUI))]
    public class CraftDetailMenuUIEditor : TOCKEditorV2
    {

        #region Unity Methods

        public override void OnInspectorGUI()
        {
            MainContainerBegin("Craft Detail Menu", "Icons/tock-menu");

            SectionHeader("UI");
            SimpleProperty("minCount");
            SimpleProperty("maxCount");
            SimpleProperty("curCount");
            SimpleProperty("countSlider");
            SimpleProperty("recipeUI");

            SectionHeader("Events");
            SimpleProperty("onUncraftable");

        MainContainerEnd();
        }

        #endregion

    }
}