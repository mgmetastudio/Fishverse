#if GDTK
using UnityEditor;

namespace NullSave.GDTK.Stats
{
    [CustomEditor(typeof(RaceListUI))]
    public class RaceListUIEditor : GDTKEditor
    {

        #region Unity Methods

        public override void OnInspectorGUI()
        {
            MainContainerBegin();

            SectionHeader("Behavior");
            SimpleProperty("uiPrefab", "UI Prefab");
            SimpleProperty("content");
            SimpleProperty("destroyOnClose");

            SectionHeader("Stats");
            SimpleProperty("autoSubmit");
            SimpleProperty("assignSelected");
            if (SimpleValue<bool>("assignSelected"))
            {
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
            }

            SectionHeader("Events");
            SimpleProperty("onOpen");
            SimpleProperty("onClose");
            SimpleProperty("onSelectionChanged");
            SimpleProperty("onSubmit");

            MainContainerEnd();
        }

        #endregion

    }
}
#endif