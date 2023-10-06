#if GDTK
using UnityEditor;

namespace NullSave.GDTK.Stats
{
    [CustomEditor(typeof(StatusEffectListUI))]
    public class StatusEffectListUIEditor : GDTKEditor
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

            MainContainerEnd();
        }

        #endregion

    }
}
#endif