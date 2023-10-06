#if GDTK
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NullSave.GDTK.Stats
{
    [CustomEditor(typeof(StatEventTrigger))]
    public class StatEventTriggerEditor : GDTKStatsEditor
    {

        #region Fields

        private StatsDatabase db;

        #endregion

        #region Unity Methods

        private void OnEnable()
        {
            db = FindObjectOfType<StatsDatabase>();
        }

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
            if (db != null)
            {
                SimpleStringSearchProperty(serializedObject.FindProperty("eventId"), "Event Id", BuildEventIds);
            }
            else
            {
                SimpleProperty("eventId");
            }

            SectionHeader("Events");
            SimpleProperty("onEventRaised");


            MainContainerEnd();
        }

        #endregion

        #region Private Methods

        private void BuildEventIds(List<string> output)
        {
            foreach(GDTKEvent ev in db.events)
            {
                output.Add(ev.info.id);
            }
            output.Sort();
        }

        #endregion

    }
}
#endif