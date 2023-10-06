#if GDTK
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NullSave.GDTK.Stats
{
    [CustomEditor(typeof(BasicStats))]
    public class BasicStatsEditor : GDTKStatsEditor
    {

        #region Enumerations

        private enum DisplayFlags
        {
            Stats = 1,
            Attributes = 2,
            Conditions = 4,
            Events = 8,
            Registry = 16,
            DataMigration = 32,
            Plugins = 8192,
        }

        #endregion

        #region Fields

        private BasicStats myTarget;
        private StatsDatabase db;
        private bool[] expanded;
        private bool doImport;
        private bool doExport;
        private List<string> attributeList;
        private List<string> conditionList;

        private UniversalObjectEditorInfo _plugins;
        private Rect buttonRect;

        #endregion

        #region Unity Methods

        private void OnEnable()
        {
            _plugins = new UniversalObjectEditorInfo();

            db = FindObjectOfType<StatsDatabase>();
            if (db != null)
            {
                attributeList = new List<string>();
                foreach (GDTKAttribute attrib in db.attributes)
                {
                    attributeList.Add(attrib.info.id);
                }

                conditionList = new List<string>();
                foreach (GDTKStatusCondition condition in db.statusConditions)
                {
                    conditionList.Add(condition.info.id);
                }

            }

            if (target is BasicStats stats)
            {
                myTarget = stats;
            }
        }

        public override void OnInspectorGUI()
        {
            int displayFlags = SimpleValue<int>("z_display_flags");

            MainContainerBegin();

            EditorGUILayout.HelpBox("This object responds to the debug console. Type [GameObjectName].Stats -h inside the debug console for a list of commands.", MessageType.Info);

            if (Application.isPlaying)
            {
                RunTime(displayFlags);
            }
            else
            {
                DesignTime(displayFlags);
            }

            MainContainerEnd();

            if (doImport)
            {
                doImport = false;
                string importPath = EditorUtility.OpenFilePanel("Import Stats", Application.dataPath, "json");
                if (!string.IsNullOrEmpty(importPath))
                {
                    StatsImportWindow.ShowWindow(myTarget, importPath);
                }
            }

            if (doExport)
            {
                doExport = false;
                StatsExportWindow.ShowWindow(myTarget);
            }
        }

        public override bool RequiresConstantRepaint()
        {
            return Application.isPlaying;
        }

        #endregion

        #region Private Methods

        private void DesignTime(int displayFlags)
        {

            DesignTimeToolRegistry(displayFlags, (int)DisplayFlags.Registry);

            DesignTimeStats(displayFlags, (int)DisplayFlags.Stats);

            DesignTimeStatusConditions(displayFlags, (int)DisplayFlags.Conditions, db, conditionList);

            DesignTimeAttributes(displayFlags, (int)DisplayFlags.Attributes, db, attributeList);

            DesignTimePlugins(displayFlags, (int)DisplayFlags.Plugins, myTarget, _plugins, ref buttonRect);

            DesignTimeData(displayFlags, (int)DisplayFlags.DataMigration, ref doImport, ref doExport);

        }

        private void RunTime(int displayFlags)
        {
            RunTimeToolRegistry(displayFlags, (int)DisplayFlags.Registry);

            RunTimeStats(displayFlags, (int)DisplayFlags.Stats, myTarget, ref expanded);

            RunTimeStatusConditions(displayFlags, (int)DisplayFlags.Conditions, myTarget);

            RunTimeAttributes(displayFlags, (int)DisplayFlags.Attributes, myTarget);
        }

        #endregion

    }
}
#endif