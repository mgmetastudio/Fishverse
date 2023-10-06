#if GDTK
using UnityEditor;

namespace NullSave.GDTK.Stats
{
    [CustomEditor(typeof(StatChangeTrigger))]
    public class StatChangeTriggerEditor : GDTKStatsEditor
    {

        #region Unity Methods

        public override void OnInspectorGUI()
        {
            MainContainerBegin();

            SectionHeader("Stats");
            SimpleProperty("statSource");
            switch((StatSourceReference)SimpleValue<int>("statSource"))
            {
                case StatSourceReference.DirectReference:
                    SimpleProperty("stats");
                    break;
                case StatSourceReference.FindInRegistry:
                    SimpleProperty("key");
                    break;
            }
            SimpleProperty("statId");

            SectionHeader("Respond To");
            SimpleProperty("maxChanged");
            SimpleProperty("minChanged");
            SimpleProperty("valueChanged");
            SimpleProperty("specialChanged");

            SectionHeader("Apply On Change");
            DrawStatModifierList(serializedObject.FindProperty("modifiers"), "Stat Modifiers", "Modifiers to apply on change", false);

            SectionHeader("Events");
            SimpleProperty("onChanged");


            MainContainerEnd();
        }

        #endregion

    }
}
#endif