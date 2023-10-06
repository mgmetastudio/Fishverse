#if GDTK
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace NullSave.GDTK.Stats
{
    [CustomEditor(typeof(StatsDatabase))]
    public class StatsDatabaseEditor : GDTKStatsEditor
    {

        #region Fields

        private int index, entryIndex;
        private Vector2 scroll, scrollEntries, scrollEdit;
        private SerializedProperty editing;
        private bool skipFrame;

        private StatsDatabase myTarget;

        // Search
        private string searchValue;

#if GDTK_Inventory2
        private Inventory.InventoryDatabase inventoryDB;
#else
        private object inventoryDB;
#endif

        // Panes
        private List<IStatEditorPane> panes;

        #endregion

        #region Unity Methods

        private void OnEnable()
        {
            index = 0;
            entryIndex = -1;
            scroll = scrollEdit = scrollEntries = Vector2.zero;
            editing = null;
            if (target is StatsDatabase database)
            {
                myTarget = database;
#if GDTK_Inventory2
                inventoryDB = FindObjectOfType<Inventory.InventoryDatabase>();
#endif

                panes = new List<IStatEditorPane>();
                //panes.Add(new ActionSequencePane());
                panes.Add(new AttributePane(serializedObject, "attributes"));
                panes.Add(new BackgroundPane(serializedObject, "backgrounds", inventoryDB));
                panes.Add(new ClassPane(serializedObject, "classes", inventoryDB));
                panes.Add(new EventPane(serializedObject, "events"));
                //panes.Add(new LanguagePane(serializedObject, "languages"));
                panes.Add(new LevelRewardsPane(serializedObject, "levelRewards", inventoryDB));
                panes.Add(new PerkPane(serializedObject, "perks"));
                panes.Add(new RacePane(serializedObject, "races", inventoryDB));
                panes.Add(new StatusConditionPane(serializedObject, "statusConditions"));
                panes.Add(new StatusEffectPane(serializedObject, "effects"));
                panes.Add(new CustomRewardsPane(serializedObject, "customRewards", inventoryDB));

                panes = panes.OrderBy(x => x.Name).ToList();
            }
        }

        public override void OnInspectorGUI()
        {
            MainContainerBegin();

            EditorGUILayout.HelpBox("This object will be marked as DoNotDestroy and will follow between scenes. If you already have an instance in a new scene, this one will replace it.", MessageType.Info);

            SectionHeader("Data", GetIcon("icons/db"));
            SimpleProperty("dataSource");
            switch ((DataSource)SimpleValue<int>("dataSource"))
            {
                case DataSource.AssetBundle:
                    SimpleProperty("bundleName", "Bundle");
                    SimpleProperty("path", "Path");
                    SimpleProperty("assetName", "Asset Name");
                    SimpleProperty("loadOnAwake");
                    break;
                case DataSource.PersistentData:
                    SimpleProperty("path", "Relative Path");
                    SimpleProperty("loadOnAwake");
                    break;
                case DataSource.Resources:
                    SimpleProperty("path", "Relative Path");
                    SimpleProperty("loadOnAwake");
                    break;
            }

            GUILayout.Space(12);
            if (GUILayout.Button(new GUIContent("Open Editor", GetIcon("icons/stats-window-icon")), GUILayout.Height(24)))
            {
                StatsDatabaseWindow.ShowWindow((StatsDatabase)target);
            }

            MainContainerEnd();
        }

        #endregion

        #region Internal Methods

        internal void DrawEditorWindow()
        {
            serializedObject.Update();
            GUILayout.BeginHorizontal();
            DrawCategories();
            DrawEntries();
            DrawEditSection();
            GUILayout.EndHorizontal();
            serializedObject.ApplyModifiedProperties();
        }

        #endregion

        #region Private Methods

        private void DrawCategories()
        {
            GUILayout.BeginVertical(GUILayout.Width(160));

            GUILayout.BeginVertical(Styles.SlimBox);
            SectionHeader("Categories");
            GUILayout.Space(8);
            GUILayout.EndVertical();

            // Section list
            scroll = GUILayout.BeginScrollView(scroll, Styles.ListItem, GUILayout.ExpandHeight(true));

            for (int i = 0; i < panes.Count; i++)
            {
                if (GUILayout.Button(panes[i].Name, i == index ? Styles.ListItemHover : Styles.ListItem, GUILayout.ExpandWidth(true), GUILayout.Height(30)))
                {
                    index = i;
                    entryIndex = -1;
                    editing = null;
                    searchValue = string.Empty;
                    GUI.FocusControl(null);
                }
            }

            GUILayout.EndScrollView();

            GUILayout.EndVertical();
        }

        private void DrawEditSection()
        {
            GUILayout.BeginVertical();

            GUILayout.BeginVertical(Styles.SlimBox);
            GUILayout.Space(12);
            GUILayout.BeginHorizontal();
            GUILayout.Label(panes[index].Name, Styles.SectionHeaderStyle);
            GUILayout.BeginVertical();
            GUILayout.Space(4);
            GUILayout.Label(panes[index].Description);
            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(8);
            GUILayout.EndVertical();

            // Section list
            scrollEdit = GUILayout.BeginScrollView(scrollEdit, GUILayout.ExpandHeight(true));

            if (skipFrame)
            {
                skipFrame = false;

                GUILayout.EndScrollView();

                GUILayout.BeginVertical(Styles.SlimBox, GUILayout.Height(36));
                GUILayout.Space(6);
                GUILayout.BeginHorizontal();
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();

                GUILayout.EndHorizontal();

                return;
            }

            if (editing == null && (index != 0 || entryIndex == -1))
            {
                GUILayout.FlexibleSpace();
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label("No item selected");
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.FlexibleSpace();
            }
            else
            {
                panes[index].EditEntry(this, myTarget, ref entryIndex, editing);
            }

            GUILayout.EndScrollView();

            GUILayout.BeginVertical(Styles.SlimBox, GUILayout.Height(36));
            GUILayout.Space(6);
            GUILayout.BeginHorizontal();
            if (panes[index].IsValidEntry(entryIndex, editing))
            {
                if (GUILayout.Button(new GUIContent("Delete Entry", GetIcon("icons/trash-small")), GUILayout.Height(24)))
                {
                    GUI.FocusControl(null);
                    panes[index].DeleteEntry(myTarget, serializedObject, ref entryIndex, ref editing);
                }
                if (GUILayout.Button(new GUIContent("Duplicate Entry", GetIcon("icons/duplicate")), GUILayout.Height(24)))
                {
                    GUI.FocusControl(null);
                    panes[index].DuplicateEntry(myTarget, ref entryIndex, ref editing);
                }
            }
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(new GUIContent("Import", GetIcon("icons/import-small")), GUILayout.Height(24)))
            {
                GUI.FocusControl(null);
                JsonImport();
            }
            GUILayout.Space(8);
            if (GUILayout.Button(new GUIContent("Export", GetIcon("icons/export-small")), GUILayout.Height(24)))
            {
                GUI.FocusControl(null);
                JsonExport();
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
        }

        private void DrawEntries()
        {
            GUILayout.BeginVertical(GUILayout.Width(200));

            GUILayout.BeginVertical(Styles.SlimBox);
            SectionHeader("Entries");
            GUILayout.Space(8);
            GUILayout.EndVertical();

            // Searchbar
            GUILayout.BeginHorizontal(GUI.skin.FindStyle("Toolbar"), GUILayout.Height(20));
            GUI.SetNextControlName("txtSearch");
            string newSearch = GUILayout.TextField(searchValue, GetSearchbarStyle());
            if (GUILayout.Button(string.Empty, GetSearchbarCancelStyle()))
            {
                newSearch = string.Empty;
                GUI.FocusControl(null);
            }
            GUILayout.EndHorizontal();

            if (newSearch != searchValue)
            {
                searchValue = newSearch;
            }

            // Section list
            scrollEntries = GUILayout.BeginScrollView(scrollEntries, Styles.ListItem, GUILayout.ExpandHeight(true));

            panes[index].ListEntries(myTarget, ref entryIndex, searchValue, ref editing, editing == null);

            GUILayout.EndScrollView();

            GUILayout.BeginVertical(Styles.SlimBox, GUILayout.Height(36));
            GUILayout.Space(6);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(new GUIContent("Add New", GetIcon("icons/add-small")), GUILayout.Height(24), GUILayout.Width(86)))
            {
                GUI.FocusControl(null);
                panes[index].AddEntry(this, myTarget, ref entryIndex, ref editing, ref skipFrame);
            }

            if (GUILayout.Button(new GUIContent("Sort", GetIcon("icons/list")), GUILayout.Height(24), GUILayout.Width(86)))
            {
                GUI.FocusControl(null);
                panes[index].SortEntries(myTarget, ref entryIndex);
            }

            GUILayout.FlexibleSpace();

            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
        }

        private void JsonExport()
        {
            string exportPath = EditorUtility.SaveFilePanel("Export Stats Database", Application.dataPath, "StatsDB", "json");
            if (!string.IsNullOrEmpty(exportPath))
            {
                File.WriteAllText(exportPath, myTarget.JSONExport());
                AssetDatabase.Refresh();
            }
            AssetDatabase.Refresh();
        }

        private void JsonImport()
        {
            string importPath = EditorUtility.OpenFilePanel("Import Stats Database", Application.dataPath, "json");
            if (!string.IsNullOrEmpty(importPath))
            {
                bool doClear = EditorUtility.DisplayDialog("Stats Database", "Clear existing data before importing?", "Yes", "No");
                myTarget.JSONImport(File.ReadAllText(importPath), doClear);
                EditorUtility.SetDirty(myTarget);
            }

        }

        #endregion

    }
}
#endif