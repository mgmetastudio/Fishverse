#if GDTK
using UnityEditor;

namespace NullSave.GDTK.Stats
{
    [CustomEditor(typeof(StatSlider))]
    public class StatSliderEditor : GDTKStatsEditor
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
            SimpleProperty("setValueWithSlider");
            SimpleProperty("image");

            MainContainerEnd();
        }

        #endregion

    }
}
#endif