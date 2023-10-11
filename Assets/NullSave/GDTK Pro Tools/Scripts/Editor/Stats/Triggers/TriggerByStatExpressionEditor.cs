#if GDTK
using UnityEditor;

namespace NullSave.GDTK.Stats
{
    [CustomEditor(typeof(TriggerByStatExpression))]
    public class TriggerByStatExpressionEditor : GDTKStatsEditor
    {

        #region Unity Methods

        public override void OnInspectorGUI()
        {
            MainContainerBegin();

            SectionHeader("Stats");
            SimpleProperty("statSource");
            switch ((StatSourceReference)SimpleValue<int>("statSource"))
            {
                case StatSourceReference.DirectReference:
                    SimpleProperty("stats");
                    break;
                case StatSourceReference.FindInRegistry:
                    SimpleProperty("key");
                    break;
            }
            SectionHeader("Behavior");
            SimpleProperty("expression");

            SectionHeader("Events");
            SimpleProperty("onMatch");
            SimpleProperty("onMismatch");

            MainContainerEnd();
        }

        #endregion

    }
}
#endif