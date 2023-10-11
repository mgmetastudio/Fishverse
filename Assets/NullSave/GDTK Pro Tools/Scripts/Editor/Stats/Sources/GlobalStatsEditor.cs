#if GDTK
using System.IO;
using UnityEditor;
using UnityEngine;

namespace NullSave.GDTK.Stats
{
    [CustomEditor(typeof(GlobalStats))]
    public class GlobalStatsEditor : GDTKStatsEditor
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
            Plugins = 8192,
        }

        #endregion

        #region Fields

        private GlobalStats myTarget;
        private StatsDatabase db;
        private bool[] expanded;
        private bool doImport;
        private bool doExport;

        private UniversalObjectEditorInfo _plugins;
        private Rect buttonRect;

        #endregion

        #region Unity Methods

        private void OnEnable()
        {
            _plugins = new UniversalObjectEditorInfo();

            db = FindObjectOfType<StatsDatabase>();
            
            if (target is GlobalStats stats)
            {
                myTarget = stats;
            }
        }

        public override void OnInspectorGUI()
        {
            int displayFlags = SimpleValue<int>("z_display_flags");

            MainContainerBegin();

            EditorGUILayout.HelpBox("This object registers itself as 'GlobalStats' and responds to the debug console. Type GlobalStats -h inside the debug console for a list of commands.", MessageType.Info);

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
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.V && Event.current.control)
            {
                PasteStatFromClipboard(serializedObject.FindProperty("m_stats"));
            }

            DesignTimeStats(displayFlags, (int)DisplayFlags.Stats);

            DesignTimePlugins(displayFlags, (int)DisplayFlags.Plugins, myTarget, _plugins, ref buttonRect);

            DesignTimeData(displayFlags, (int)DisplayFlags.DataMigration, ref doImport, ref doExport);

        }

        private void RunTime(int displayFlags)
        {
            RunTimeStats(displayFlags, (int)DisplayFlags.Stats, myTarget, ref expanded);

        }

        #endregion

    }
}
#endif