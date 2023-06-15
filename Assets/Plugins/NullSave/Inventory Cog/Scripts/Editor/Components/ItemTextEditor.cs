using UnityEditor;

namespace NullSave.TOCK.Inventory
{
    [CanEditMultipleObjects()]
    [CustomEditor(typeof(ItemText))]
    public class ItemTextEditor : TOCKEditorV2
    {

        #region Unity Methods

        public override void OnInspectorGUI()
        {
            MainContainerBeginSlim();

            SectionHeader("Behaviour");
            SimpleProperty("targetProperty");
            if ((ItemTextTarget)SimpleInt("targetProperty") == ItemTextTarget.RarityName)
            {
                SimpleProperty("useColor");
            }
            SimpleProperty("format");

            MainContainerEnd();
        }

        #endregion

    }
}