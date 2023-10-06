#if GDTK
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace NullSave.GDTK.Stats
{
    [CustomEditor(typeof(NPCStats))]
    public class NPCStatsEditor : GDTKStatsEditor
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
            DamageModifiers = 64,
            Effects = 128,
            ClassAndRace = 256,
            Leveling = 512,
            UI = 1024,
            Respawn = 4096,
            Plugins = 8192,
        }

        #endregion

        #region Fields

        private NPCStats myTarget;
        private StatsDatabase db;
        private bool[] expanded;
        private bool doImport;
        private bool doExport;
        private List<string> attributeList;
        private List<string> conditionList;
        private List<string> raceList;
        private List<string> classList;
        private List<string> backgroundList;

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

                raceList = new List<string>();
                foreach (GDTKRace mod in db.races)
                {
                    raceList.Add(mod.info.id);
                }

                backgroundList = new List<string>();
                foreach (GDTKBackground mod in db.backgrounds)
                {
                    backgroundList.Add(mod.info.id);
                }

                classList = new List<string>();
                foreach (GDTKClass mod in db.classes)
                {
                    classList.Add(mod.info.id);
                }
            }

            if (target is NPCStats stats)
            {
                myTarget = stats;
            }
        }

        public override void OnInspectorGUI()
        {
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.V && Event.current.control)
            {
                PasteStatFromClipboard(serializedObject.FindProperty("m_stats"));
            }

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

            DesignTimeRespawn(displayFlags, (int)DisplayFlags.Respawn);

            DesignTimePlugins(displayFlags, (int)DisplayFlags.Plugins, myTarget, _plugins, ref buttonRect);

            DesignTimeData(displayFlags, (int)DisplayFlags.DataMigration, ref doImport, ref doExport);

        }

        private void RunTime(int displayFlags)
        {
            RunTimeToolRegistry(displayFlags, (int)DisplayFlags.Registry);

            RunTimeActiveEffects(displayFlags, (int)DisplayFlags.Effects, myTarget);

            RunTimeStats(displayFlags, (int)DisplayFlags.Stats, myTarget, ref expanded);

            RunTimeStatusConditions(displayFlags, (int)DisplayFlags.Conditions, myTarget);

            RunTimeAttributes(displayFlags, (int)DisplayFlags.Attributes, myTarget);
        }

        #endregion

    }
}
#endif