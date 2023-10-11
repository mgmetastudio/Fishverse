#if GDTK
using UnityEditor;

namespace NullSave.GDTK.Stats
{
    [CustomEditor(typeof(RaiseStatEvent))]
    public class RaiseStatEventEditor : GDTKStatsEditor
    {

        #region Unity Methods

        public override void OnInspectorGUI()
        {
            MainContainerBegin();

            SectionHeader("Stats", GetIcon("icons/stats"));
            SimpleProperty("statSource");
            switch ((StatSourceReference)SimpleValue<int>("statSource"))
            {
                case StatSourceReference.DirectReference:
                    SimpleProperty("stats", "Reference");
                    break;
                case StatSourceReference.FindInRegistry:
                    SimpleProperty("key");
                    break;
            }

            EditorGUILayout.Space(12);
            SectionHeader("Behavior", GetIcon("icons/behavior"));
            SimpleProperty("eventId");
            SimpleProperty("raiseWhen");


            MainContainerEnd();
        }

        #endregion

    }
}
#endif