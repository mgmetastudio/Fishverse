#if GDTK
using UnityEditor;

namespace NullSave.GDTK.Stats
{
    [CustomEditor(typeof(ClassListUI))]
    public class ClassListUIEditor : GDTKEditor
    {

        #region Unity Methods

        public override void OnInspectorGUI()
        {
            MainContainerBegin();

            SectionHeader("Stats");
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
            SimpleProperty("onNoClasses");
            SimpleProperty("onHasClasses");
            SimpleProperty("onClassesChanged");

            MainContainerEnd();
        }

        #endregion

    }
}
#endif