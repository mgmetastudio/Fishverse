using UnityEditor;

namespace NullSave.TOCK.Inventory
{
    [CanEditMultipleObjects()]
    [CustomEditor(typeof(CraftQueueUI))]
    public class CraftQueueUIEditor : TOCKEditorV2
    {

        #region Unity Methods

        public override void OnInspectorGUI()
        {
            MainContainerBegin("Craft Queue UI", "Icons/category");

            SectionHeader("UI");
            SimpleProperty("recipeUI");
            SimpleProperty("progress");
            SimpleProperty("startedTime");
            SimpleProperty("endTime");
            SimpleProperty("timeRemaining");
            SimpleProperty("count");

            SectionHeader("Behaviour");
            SimpleProperty("startEndFormat");
            SimpleProperty("timeFormat");

            MainContainerEnd();
        }

        #endregion

    }
}