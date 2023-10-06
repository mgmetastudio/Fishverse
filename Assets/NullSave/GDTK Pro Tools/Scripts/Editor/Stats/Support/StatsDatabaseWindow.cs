#if GDTK
using UnityEditor;
using UnityEngine;

namespace NullSave.GDTK.Stats
{
    public class StatsDatabaseWindow : GDTKEditorWindow
    {

        #region Constants

        private const string TITLE = "Stats Database";

        #endregion

        #region Fields

        private StatsDatabase database;
        private GameObject dbHost;
        private GameObject prefabRoot;
        private StatsDatabaseEditor dbEditor;

        #endregion

        #region Properties

        private bool skipNextPaint;

        #endregion

        #region Unity Methods

        private void OnEnable()
        {
            Selection.selectionChanged += SelectDatabase;
        }

        private void OnGUI()
        {

            if (database == null || dbEditor == null)
            {
                SelectDatabase();
            }

            // Make sure we have a database
            if (database == null || dbEditor == null)
            {
                ShowSelectDatabase();
                skipNextPaint = true;
                return;
            }

            if (skipNextPaint && Event.current.type == EventType.Repaint)
            {
                skipNextPaint = false;
                return;
            }

            // Begin Layout
            GUILayout.BeginVertical();

            // Check for prefab
            if (prefabRoot != null)
            {
                ShowPrefabWarning();
            }

            dbEditor.DrawEditorWindow();

            GUILayout.EndVertical();
        }

        #endregion

        #region Public Methods

        public static void ShowWindow(StatsDatabase database)
        {
            StatsDatabaseWindow w = GetWindow<StatsDatabaseWindow>(TITLE, typeof(SceneView));
            w.titleContent = new GUIContent(TITLE, GDTKEditor.GetIcon("Icons/stats-window-icon"));
            w.wantsMouseMove = true;
            w.database = database;
            w.dbEditor = null;
            w.Show();
        }

        #endregion

        #region Private Methods

        private void ConditionalRepaint()
        {
            if (Event.current == null)
            {
                Repaint();
            }
        }

        private void SelectDatabase()
        {
            // Get the selected game object
            GameObject activeObj = Selection.activeObject as GameObject;

            // Check for null selection
            if (activeObj == null)
            {
                ConditionalRepaint();
                return;
            }

            // Check for already selected
            if (database != null)
            {
                if (activeObj == dbHost || activeObj == prefabRoot)
                {
                    // Check in case this has become a prefab since last we looked
                    if (prefabRoot == null)
                    {
                        prefabRoot = PrefabUtility.GetOutermostPrefabInstanceRoot(activeObj);
                    }

                    if (dbEditor == null)
                    {
                        dbEditor = (StatsDatabaseEditor)Editor.CreateEditor(database, typeof(StatsDatabaseEditor));
                    }

                    ConditionalRepaint();
                    return;
                }
            }

            // Get selected object's stat db
            StatsDatabase selDB = activeObj.GetComponentInChildren<StatsDatabase>();
            if (selDB == null)
            {
                ConditionalRepaint();
                return;
            }

            database = selDB;
            dbHost = activeObj;
            prefabRoot = PrefabUtility.GetOutermostPrefabInstanceRoot(activeObj);
            dbEditor = (StatsDatabaseEditor)Editor.CreateEditor(database, typeof(StatsDatabaseEditor));

            ConditionalRepaint();
        }

        private void ShowSelectDatabase()
        {
            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            GUILayout.Label("Please select a Stats Database to edit", GDTKEditor.Styles.SectionHeaderStyle);

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
        }

        private void ShowPrefabWarning()
        {
            GUILayout.BeginHorizontal("box");
            if (GUILayout.Button(new GUIContent("Edit Prefab", GDTKEditor.GetIcon("icons/stats-window-icon")), GUILayout.Height(24)))
            {
                Selection.activeObject = AssetDatabase.LoadAssetAtPath(PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(prefabRoot), typeof(Object));
                AssetDatabase.OpenAsset(Selection.activeObject);

                database = ((GameObject)Selection.activeObject).GetComponent<StatsDatabase>();
                dbEditor = (StatsDatabaseEditor)Editor.CreateEditor(database, typeof(StatsDatabaseEditor));
            }
            GUILayout.BeginVertical();
            GUILayout.Space(4);
            GUILayout.Label("You are editing a prefab instance. Changes to this object will not apply to the prefab unless you select 'Apply' or click the 'Edit Prefab' button.", GDTKEditor.Styles.ErrorTextStyle);
            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        #endregion

    }
}
#endif