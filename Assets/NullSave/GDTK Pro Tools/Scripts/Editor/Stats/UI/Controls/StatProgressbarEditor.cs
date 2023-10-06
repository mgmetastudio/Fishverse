#if GDTK
using UnityEditor;

namespace NullSave.GDTK.Stats
{
    [CustomEditor(typeof(StatProgressbar))]
    [CanEditMultipleObjects]
    public class StatProgressbarEditor : GDTKStatsEditor
    {

        #region Unity Methods

        public override void OnInspectorGUI()
        {
            MainContainerBegin();

            SectionHeader("Stat Source");
            SimpleProperty("source");
            switch ((StatSourceReference)SimpleValue<int>("source"))
            {
                case StatSourceReference.DirectReference:
                    SimpleProperty("m_stats");
                    break;
                case StatSourceReference.FindInRegistry:
                    SimpleProperty("key");
                    break;
            }

            SectionHeader("Behavior");
            SimpleProperty("valueId");
            SimpleProperty("minimumId");
            SimpleProperty("maximumId");
            SimpleProperty("image");
            SimpleProperty("colorImage");
            SimpleProperty("colorProgress");

            MainContainerEnd();
        }

        #endregion

    }
}
#endif