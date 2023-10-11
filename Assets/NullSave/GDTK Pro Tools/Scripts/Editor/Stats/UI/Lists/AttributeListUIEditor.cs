#if GDTK
using UnityEditor;

namespace NullSave.GDTK.Stats
{
    [CustomEditor(typeof(AttributeListUI))]
    public class AttributeListUIEditor : GDTKEditor
    {

        #region Unity Methods

        public override void OnInspectorGUI()
        {
            MainContainerBegin();

            SectionHeader("Stats", GetIcon("icons/stats"));
            SimpleProperty("source");
            switch ((StatSourceReference)SimpleValue<int>("source"))
            {
                case StatSourceReference.DirectReference:
                    SimpleProperty("m_stats", "Reference");
                    break;
                case StatSourceReference.FindInRegistry:
                    SimpleProperty("key");
                    break;
            }

            SectionHeader("Behavior");
            SimpleProperty("uiPrefab", "UI Prefab");
            SimpleProperty("content");

            SectionHeader("Events");
            SimpleProperty("onListUpdated");

            MainContainerEnd();
        }

        #endregion

    }
}
#endif