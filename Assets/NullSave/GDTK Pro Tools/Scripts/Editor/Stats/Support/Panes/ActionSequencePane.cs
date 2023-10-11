#if GDTK
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace NullSave.GDTK.Stats
{
    public class ActionSequencePane : IStatEditorPane
    {

        #region Fields

        private UniversalObjectEditorInfo _plugins;
        private UniversalObjectEditorInfo _addOnPlugins;
        private Rect btnRect;

        #endregion

        #region Properties

        public string Name { get { return "Action Sequences"; } }

        public string Description { get { return "Represents a list of actions to perform"; } }

        #endregion

        #region Constructors

        public ActionSequencePane()
        {
            _plugins = new UniversalObjectEditorInfo();
        }

        #endregion

        #region Public Methods

        public void AddEntry(GDTKStatsEditor editor, StatsDatabase database, ref int entryIndex, ref SerializedProperty editing, ref bool skipFrame)
        {
            database.actionSequences.Add(new ActionSequenceList());
            EditorUtility.SetDirty(database);
            entryIndex = database.actionSequences.Count - 1;
        }

        public void DeleteEntry(StatsDatabase database, SerializedObject obj, ref int entryIndex, ref SerializedProperty editing)
        {
            database.m_actionSequences.RemoveAt(entryIndex);
            EditorUtility.SetDirty(database);
            if (entryIndex > database.actionSequences.Count - 1)
            {
                entryIndex = database.actionSequences.Count - 1;
            }
        }

        public void DuplicateEntry(StatsDatabase database, ref int entryIndex, ref SerializedProperty editing)
        {
            database.actionSequences.Insert(entryIndex, database.actionSequences[entryIndex].Clone());
            entryIndex += 1;
        }

        public void EditEntry(GDTKStatsEditor editor, StatsDatabase database, ref int entryIndex, SerializedProperty editing)
        {
            EditorGUI.BeginChangeCheck();

            string id = database.actionSequences[entryIndex].id;
            editor.SectionHeader("Basic Info", GDTKEditor.GetIcon("icons/list"));
            database.actionSequences[entryIndex].id = EditorGUILayout.TextField("Id", database.actionSequences[entryIndex].id);
            if (string.IsNullOrEmpty(id))
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(EditorGUIUtility.labelWidth + 4);
                GUILayout.Label("Id is a required field", GDTKEditor.Styles.ErrorTextStyle);
                GUILayout.EndHorizontal();
            }
            else if (!id.IsAllowedId())
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(EditorGUIUtility.labelWidth + 4);
                GUILayout.Label("Id can only contain numbers, letters, and underscores", GDTKEditor.Styles.ErrorTextStyle);
                GUILayout.EndHorizontal();
            }

            GUILayout.Space(8);
            editor.SectionHeader("Actions", GDTKEditor.GetIcon("icons/plugin"));
            editor.DrawActionSequenceList(database.actionSequences[entryIndex].actions, _plugins);

            if (GUILayout.Button("Add Plugin"))
            {
                PopupWindow.Show(btnRect, new ActionPluginPicker() { target = database.actionSequences[entryIndex].actions, targetList = editing, width = btnRect.width });
            }
            if (Event.current.type == EventType.Repaint) btnRect = GUILayoutUtility.GetLastRect();

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(database);
            }

            _plugins.UpdateDragPosition(database.actionSequences[entryIndex].actions, editor, GDTKEditor.Styles.Redline);
        }

        public void ListEntries(StatsDatabase database, ref int entryIndex, string searchValue, ref SerializedProperty editing, bool needsRefresh)
        {
            string entryName;
            for (int i = 0; i < database.actionSequences.Count; i++)
            {
                entryName = database.actionSequences[i].id;
                if (string.IsNullOrEmpty(searchValue) || entryName.IndexOf(searchValue, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    if (GUILayout.Button(string.IsNullOrEmpty(entryName) ? "{ No Id }" : entryName, i == entryIndex ? GDTKEditor.Styles.ListItemHover : GDTKEditor.Styles.ListItem, GUILayout.ExpandWidth(true), GUILayout.Height(30)))
                    {
                        entryIndex = i;
                        editing = null;
                        _addOnPlugins = new UniversalObjectEditorInfo();
                        GUI.FocusControl(null);
                    }
                }
            }
        }

        public bool IsValidEntry(int entryIndex, SerializedProperty editing)
        {
            return entryIndex > -1;
        }

        public void SortEntries(StatsDatabase database, ref int entryIndex)
        {
            if (database.m_actionSequences.Count < 2) return;

            if (entryIndex == -1) entryIndex = 0;

            ActionSequenceList restoreTo = database.m_actionSequences[entryIndex];

            database.m_actionSequences = database.m_actionSequences.OrderBy(x => x.id).ToList();
            EditorUtility.SetDirty(database);

            for (int i = 0; i < database.m_actionSequences.Count; i++)
            {
                if (database.m_actionSequences[i] == restoreTo)
                {
                    entryIndex = i;
                    return;
                }
            }

        }

        #endregion

    }
}
#endif