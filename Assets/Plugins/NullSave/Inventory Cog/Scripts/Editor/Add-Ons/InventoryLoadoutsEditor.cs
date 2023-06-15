using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace NullSave.TOCK.Inventory
{
    [CanEditMultipleObjects()]
    [CustomEditor(typeof(InventoryLoadouts))]
    public class InventoryLoadoutsEditor : TOCKEditorV2
    {

        #region Variables

        private InventoryLoadouts myTarget;
        private List<int> expandedLoadouts;

        #endregion

        #region Unity Methods

        private void OnEnable()
        {
            if (target is InventoryLoadouts)
            {
                myTarget = (InventoryLoadouts)target;
                expandedLoadouts = new List<int>();
            }
        }

        public override void OnInspectorGUI()
        {
            MainContainerBeginSlim();

            GUILayout.Space(6);
            SerializedProperty list = serializedObject.FindProperty("availableLoadouts");

            // Header
            Rect clickRect;
            Color restore = GUI.color;

            GUILayout.BeginVertical(BoxSub, GUILayout.ExpandWidth(true));
            GUILayout.BeginHorizontal();

            GUILayout.Label("Available Loadouts");

            GUILayout.FlexibleSpace();

            GUI.color = Color.black;
            GUILayout.Label(GetIcon("Add", "Icons/tock-add"), GUILayout.ExpandHeight(false), GUILayout.ExpandWidth(false), GUILayout.Height(16), GUILayout.Width(18));
            clickRect = GUILayoutUtility.GetLastRect();

            GUI.color = restore;

            GUILayout.EndHorizontal();
            GUILayout.Space(4);
            GUILayout.EndVertical();

            int delIndex = -1;
            for (int i = 0; i < list.arraySize; i++)
            {
                if (DrawLoadout(list.GetArrayElementAtIndex(i), i))
                {
                    delIndex = i;
                }
            }

            if (delIndex > -1)
            {
                list.DeleteArrayElementAtIndex(delIndex);
            }

            if (Event.current.type == EventType.MouseUp && Event.current.button == 0 && clickRect.Contains(Event.current.mousePosition))
            {
                list.arraySize++;
            }

            MainContainerEnd();
        }

        #endregion

        #region Private Methods

        private bool DrawLoadout(SerializedProperty loadout, int index)
        {
            Rect clickRect;
            bool delAtEnd = false;

            GUILayout.Space(-4);

            GUILayout.BeginVertical(BoxBlue, GUILayout.ExpandWidth(true));
            GUILayout.BeginHorizontal();


            GUILayout.Label("[" + index + "] " + loadout.FindPropertyRelative("displayName").stringValue, Skin.GetStyle("PanelText"));
            GUILayout.FlexibleSpace();

            FoldoutTrashOnly(out delAtEnd);

            GUILayout.EndHorizontal();
            GUILayout.Space(4);
            GUILayout.EndVertical();
            clickRect = GUILayoutUtility.GetLastRect();

            if (expandedLoadouts.Contains(index))
            {
                GUILayout.Space(-6);
                GUILayout.BeginVertical(BoxWhite, GUILayout.ExpandWidth(true));

                SectionHeader("Behaviour");
                SimplePropertyRelative(loadout, "displayName");
                SimplePropertyRelative(loadout, "clearInventory");

                if ((BooleanSource)loadout.FindPropertyRelative("unlockedSource").intValue == BooleanSource.Static)
                {
                    EditorGUILayout.BeginHorizontal();
                    SimplePropertyRelative(loadout, "unlockedSource");
                    loadout.FindPropertyRelative("unlocked").boolValue = EditorGUILayout.Toggle(loadout.FindPropertyRelative("unlocked").boolValue);
                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                    EditorGUILayout.BeginHorizontal();
                    SimplePropertyRelative(loadout, "unlockedSource");
                    loadout.FindPropertyRelative("statUnlockedName").stringValue = EditorGUILayout.TextField(loadout.FindPropertyRelative("statUnlockedName").stringValue);
                    EditorGUILayout.EndHorizontal();
                }

                CustomSectionHeader("Items");
                DragDropItems(index);
                SimpleList(loadout.FindPropertyRelative("items"), null);

                CustomSectionHeader("Skills");
                DragDropSkills(index);
                SimpleList(loadout.FindPropertyRelative("skills"), null);

                GUILayout.Space(4);
                GUILayout.EndVertical();
            }

            if (Event.current.type == EventType.MouseUp && Event.current.button == 0 && clickRect.Contains(Event.current.mousePosition))
            {
                if (expandedLoadouts.Contains(index))
                {
                    expandedLoadouts.Remove(index);
                }
                else
                {
                    expandedLoadouts.Add(index);
                }

                Repaint();
            }

            return delAtEnd;
        }

        private void DragDropItems(int index)
        {
            System.Type acceptedType = typeof(InventoryItem);
            var dragAreaGroup = GUILayoutUtility.GetLastRect();
            switch (Event.current.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!dragAreaGroup.Contains(Event.current.mousePosition))
                        break;
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    if (Event.current.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();

                        foreach (var dragged in DragAndDrop.objectReferences)
                        {
                            if (dragged.GetType() == acceptedType || dragged.GetType().BaseType == acceptedType
                                || dragged.GetType().GetNestedTypes().Contains(acceptedType)
                                || (dragged is GameObject && ((GameObject)dragged).GetComponentInChildren(acceptedType) != null))
                            {
                                LoadoutItem li = new LoadoutItem();
                                li.item = (InventoryItem)dragged;
                                li.count = 1;
                                myTarget.availableLoadouts[index].items.Add(li);
                            }
                        }
                    }
                    serializedObject.ApplyModifiedProperties();
                    Event.current.Use();
                    break;
            }

            GUILayout.Space(4);
        }

        private void DragDropSkills(int index)
        {
            System.Type acceptedType = typeof(InventoryItem);
            var dragAreaGroup = GUILayoutUtility.GetLastRect();
            switch (Event.current.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!dragAreaGroup.Contains(Event.current.mousePosition))
                        break;
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    if (Event.current.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();

                        foreach (var dragged in DragAndDrop.objectReferences)
                        {
                            if (dragged.GetType() == acceptedType || dragged.GetType().BaseType == acceptedType
                                || dragged.GetType().GetNestedTypes().Contains(acceptedType)
                                || (dragged is GameObject && ((GameObject)dragged).GetComponentInChildren(acceptedType) != null))
                            {
                                InventoryItem item = (InventoryItem)dragged;
                                if (item.itemType == ItemType.Skill)
                                {
                                    LoadoutSkill li = new LoadoutSkill();
                                    li.skillItem = (InventoryItem)dragged;
                                    myTarget.availableLoadouts[index].skills.Add(li);
                                }
                            }
                        }
                    }
                    serializedObject.ApplyModifiedProperties();
                    Event.current.Use();
                    break;
            }

            GUILayout.Space(4);
        }

        private void CustomSectionHeader(string title)
        {
            GUILayout.Space(8);
            GUILayout.BeginHorizontal();
            GUILayout.Space(5);
            GUILayout.BeginVertical();
            GUILayout.Space(5);
            GUILayout.Label(title, Skin.GetStyle("SectionHeader"));
            GUILayout.EndVertical();
            Color res = GUI.color;
            GUILayout.BeginVertical();
            GUI.color = EditorColor;
            GUILayout.Space(4);
            GUILayout.Label(GetIcon("Drop", "Icons/tock-drop"), GUILayout.ExpandHeight(false), GUILayout.ExpandWidth(false), GUILayout.Height(16), GUILayout.Width(18));
            GUI.color = res;
            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        #endregion

    }
}