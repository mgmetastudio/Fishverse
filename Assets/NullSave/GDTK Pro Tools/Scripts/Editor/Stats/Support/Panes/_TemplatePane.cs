#if GDTK
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NullSave.GDTK.Stats
{
    public class TemplatePane : IStatEditorPane
    {

        #region Fields

        internal SerializedObject serializedObject;
        internal SerializedProperty list;
        internal string field;
        internal List<string> entryList;

#if GDTK_Inventory2
        internal Inventory.InventoryDatabase inventoryDB;
#else
        internal object inventoryDB;
#endif

        #endregion

        #region Properties

        public virtual string Name { get { return ""; } }

        public virtual string Description { get { return ""; } }

        #endregion

        #region Constructors

        public TemplatePane(SerializedObject obj, string fieldName, object inventoryDB)
        {
            serializedObject = obj;
            field = fieldName;
            list = obj.FindProperty(field);
#if GDTK_Inventory2
            this.inventoryDB = (Inventory.InventoryDatabase)inventoryDB;
#else
            this.inventoryDB = inventoryDB;
#endif
        }

        #endregion

        #region Public Methods

        public virtual void AddEntry(GDTKStatsEditor editor, StatsDatabase database, ref int entryIndex, ref SerializedProperty editing, ref bool skipFrame)
        {
            list.arraySize++;
            editor.ClearLastChildBasicInfo(list);
            entryIndex = list.arraySize - 1;
            editing = list.GetArrayElementAtIndex(entryIndex);
            EditorUtility.SetDirty(database);
            skipFrame = true;
            entryList = null;
        }

        public virtual void DeleteEntry(StatsDatabase database, SerializedObject obj, ref int entryIndex, ref SerializedProperty editing)
        {
            if (list != null)
            {
                GDTKEditor.ConfirmDelete(list, entryIndex);

                if (list.arraySize > 0)
                {
                    if (entryIndex > list.arraySize - 1)
                    {
                        entryIndex = list.arraySize - 1;
                    }
                    editing = list.GetArrayElementAtIndex(entryIndex);
                }
                else
                {
                    entryIndex = -1;
                    editing = null;
                }

                RebuildEntries();
            }
        }

        public virtual void DuplicateEntry(StatsDatabase database, ref int entryIndex, ref SerializedProperty editing)
        {
            entryIndex += 1;
            if (entryIndex < list.arraySize)
            {
                editing = list.GetArrayElementAtIndex(entryIndex);
            }
            else
            {
                editing = null;
            }
            entryList = null;
        }

        public virtual void EditEntry(GDTKStatsEditor editor, StatsDatabase database, ref int entryIndex, SerializedProperty editing) { }

        public virtual void InfoSection(GDTKStatsEditor editor, ref int entryIndex, SerializedProperty editing)
        {
            editor.SectionHeader("Info", GDTKEditor.GetIcon("icons/list"));
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(editing.FindPropertyRelative("info"));
            if (EditorGUI.EndChangeCheck())
            {
                entryList[entryIndex] = editing.FindPropertyRelative("info").FindPropertyRelative("id").stringValue;
            }
        }

        public virtual bool IsValidEntry(int entryIndex, SerializedProperty editing)
        {
            return editing != null;
        }

        public virtual void ListEntries(StatsDatabase database, ref int entryIndex, string searchValue, ref SerializedProperty editing, bool needsRefresh)
        {
            if (entryList == null) RebuildEntries();
            for (int i = 0; i < entryList.Count; i++)
            {
                if (string.IsNullOrEmpty(searchValue) || entryList[i].IndexOf(searchValue, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    if (needsRefresh && i == entryIndex)
                    {
                        editing = list.GetArrayElementAtIndex(i);
                    }

                    if (GUILayout.Button(string.IsNullOrEmpty(entryList[i]) ? "{ No Id }" : entryList[i], i == entryIndex ? GDTKEditor.Styles.ListItemHover : GDTKEditor.Styles.ListItem, GUILayout.ExpandWidth(true), GUILayout.Height(30)))
                    {
                        entryIndex = i;
                        editing = list.GetArrayElementAtIndex(i);
                        GUI.FocusControl(null);
                    }
                }
            }
        }

        public virtual void RebuildEntries()
        {
            if (list == null || !list.isArray) return;
            entryList = new List<string>();

            for (int i = 0; i < list.arraySize; i++)
            {
                entryList.Add(list.GetArrayElementAtIndex(i).FindPropertyRelative("info").FindPropertyRelative("id").stringValue);
            }
        }

        public virtual void SortEntries(StatsDatabase database, ref int entryIndex)
        {
            if (list == null || list.arraySize < 2) return;

            for (int i = 0; i < list.arraySize; i++)
            {
                for (int j = 0; j < list.arraySize - 1; j++)
                {
                    if (CompareEntries(j, j + 1))
                    {
                        list.MoveArrayElement(j, j + 1);
                    }
                }
            }

            RebuildEntries();
        }

        #endregion

        #region Private Methods

        internal void BuildAttributeIds(List<string> output)
        {
            SerializedProperty list = serializedObject.FindProperty("attributes");
            for (int i = 0; i < list.arraySize; i++)
            {
                output.Add(list.GetArrayElementAtIndex(i).FindPropertyRelative("info").FindPropertyRelative("id").stringValue);
            }
            output.Sort();
        }

        internal void BuildClassIds(List<string> output)
        {
            SerializedProperty list = serializedObject.FindProperty("classes");
            for (int i = 0; i < list.arraySize; i++)
            {
                output.Add(list.GetArrayElementAtIndex(i).FindPropertyRelative("info").FindPropertyRelative("id").stringValue);
            }
            output.Sort();
        }

        internal void BuildConditionIds(List<string> output)
        {
            SerializedProperty list = serializedObject.FindProperty("statusConditions");
            for (int i = 0; i < list.arraySize; i++)
            {
                output.Add(list.GetArrayElementAtIndex(i).FindPropertyRelative("info").FindPropertyRelative("id").stringValue);
            }
            output.Sort();
        }

        internal void BuildEffectIds(List<string> output)
        {
            SerializedProperty list = serializedObject.FindProperty("effects");
            for (int i = 0; i < list.arraySize; i++)
            {
                output.Add(list.GetArrayElementAtIndex(i).FindPropertyRelative("info").FindPropertyRelative("id").stringValue);
            }
            output.Sort();
        }

        internal void BuildRaceIds(List<string> output)
        {
            SerializedProperty list = serializedObject.FindProperty("races");
            for (int i = 0; i < list.arraySize; i++)
            {
                output.Add(list.GetArrayElementAtIndex(i).FindPropertyRelative("info").FindPropertyRelative("id").stringValue);
            }
            output.Sort();
        }

        private bool CompareEntries(int id1, int id2)
        {
            string s1 = list.GetArrayElementAtIndex(id1).FindPropertyRelative("info").FindPropertyRelative("id").stringValue;
            if (s1 == null) return false;
            string s2 = list.GetArrayElementAtIndex(id2).FindPropertyRelative("info").FindPropertyRelative("id").stringValue;
            if (s2 == null) return true;
            return s1.CompareTo(s2) > 0;
        }

        internal bool EditBasicInfo(GDTKStatsEditor editor, SerializedProperty editing, SerializedProperty info, int entryIndex)
        {
            bool wantsRepaint = false;

            bool expanded = info.FindPropertyRelative("z_expanded").boolValue;
            bool result;

            result = editor.SectionToggle(expanded, "Info", GDTKEditor.GetIcon("icons/list"));
            if (expanded != result)
            {
                info.FindPropertyRelative("z_expanded").boolValue = result;
                wantsRepaint = true;
            }

            if (expanded)
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(editing.FindPropertyRelative("info"));
                if (EditorGUI.EndChangeCheck())
                {
                    entryList[entryIndex] = editing.FindPropertyRelative("info").FindPropertyRelative("id").stringValue;
                }
            }

            return wantsRepaint;
        }

        #endregion

    }
}
#endif