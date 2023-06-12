using UnityEditor;

namespace NullSave.TOCK.Inventory
{
    [CanEditMultipleObjects()]
    [CustomEditor(typeof(CraftQueueScrollList))]
    public class CraftQueueScrollListEditor : TOCKEditorV2
    {

        #region Unity Methods

        public override void OnInspectorGUI()
        {
            MainContainerBegin("Craft Queue Scroll List", "Icons/tock-list");

            SectionHeader("Behaviour");
            SimpleProperty("loadMode");
            SimpleProperty("inventoryCog");
            SimpleProperty("itemPrefab");

            SectionHeader("Filtering");
            SimpleList("categories");

            MainContainerEnd();
        }

        #endregion

    }
}