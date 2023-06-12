using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace NullSave.TOCK.Stats
{
    [CustomEditor(typeof(StatEffectList))]
    public class StatEffectListEditor : TOCKEditorV2
    {

        #region Variables

        private Dictionary<StatEffect, StatEffectEditor> effectEditors;
        private List<string> expandedEffects;

        #endregion

        #region Unity Methods

        private void OnEnable()
        {
            if (target == null || serializedObject == null) return;
            effectEditors = new Dictionary<StatEffect, StatEffectEditor>();
            expandedEffects = new List<string>();
        }

        public override void OnInspectorGUI()
        {
            MainContainerBeginSlim();

            DrawEffects(null);

            MainContainerEnd();
        }

        #endregion

        #region Window Methods

        internal void AddEffect(StatEffect effect)
        {
            ((StatEffectList)target).availableEffects.Add(effect);
        }

        #endregion

        #region Shared Methods

        internal bool DrawEffects(StatsCog statsCog)
        {
            bool createNew = false;
            Rect clickRect;
            Color restore = GUI.color;
            SerializedProperty list = serializedObject.FindProperty("availableEffects");

            GUILayout.BeginVertical(BoxSub, GUILayout.ExpandWidth(true));
            GUILayout.BeginHorizontal();

            GUILayout.Label("Available Effects");

            GUI.color = Color.black;
            GUILayout.Label(GetIcon("Drop", "Icons/tock-drop"), GUILayout.ExpandHeight(false), GUILayout.ExpandWidth(false), GUILayout.Height(16), GUILayout.Width(18));
            GUI.color = restore;

            GUILayout.FlexibleSpace();

            GUI.color = Color.black;
            GUILayout.Label(GetIcon("Add", "Icons/tock-add"), GUILayout.ExpandHeight(false), GUILayout.ExpandWidth(false), GUILayout.Height(16), GUILayout.Width(18));
            clickRect = GUILayoutUtility.GetLastRect();
            if (Event.current.type == EventType.MouseUp && Event.current.button == 0 && clickRect.Contains(Event.current.mousePosition))
            {
                createNew = true;
            }

            GUILayout.Label(GetIcon("X", "Icons/tock-x"), GUILayout.ExpandHeight(false), GUILayout.ExpandWidth(false), GUILayout.Height(16), GUILayout.Width(18));
            clickRect = GUILayoutUtility.GetLastRect();
            if (Event.current.type == EventType.MouseUp && Event.current.button == 0 && clickRect.Contains(Event.current.mousePosition))
            {
                list.ClearArray();
                expandedEffects.Clear();
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
                return true;
            }
            GUI.color = restore;

            GUILayout.EndHorizontal();
            GUILayout.Space(4);
            GUILayout.EndVertical();

            HandleDragDrop(list, typeof(StatEffect));

            if (list.arraySize > 0)
            {
                bool removeItem = false;
                StatEffect targetEffect = null;
                int index = 0;
                for (int i = 0; i < list.arraySize; i++)
                {
                    StatEffect effect = (StatEffect)list.GetArrayElementAtIndex(i).objectReferenceValue;
                    if (DrawEffect(effect, statsCog))
                    {
                        removeItem = true;
                        targetEffect = effect;
                        index = i;
                    }
                }

                if (removeItem)
                {
                    list.GetArrayElementAtIndex(index).objectReferenceValue = null;
                    list.DeleteArrayElementAtIndex(index);
                    serializedObject.ApplyModifiedProperties();
                    expandedEffects.Clear();
                    Repaint();
                    return true;
                }
            }

            if(createNew)
            {
                ScriptableObject newItem = CreateNew("Stat Effect", typeof(StatEffect));
                if (newItem != null)
                {
                    list.arraySize++;
                    list.GetArrayElementAtIndex(list.arraySize - 1).objectReferenceValue = newItem;

                    serializedObject.ApplyModifiedProperties();
                    Repaint();
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region Private Methods

        private bool DrawEffect(StatEffect effect, StatsCog statsCog)
        {
            if (effect == null) return true;

            bool delAtEnd = false;
            Rect clickRect;

            GUILayout.Space(-4);

            GUILayout.BeginVertical(BoxGreen, GUILayout.ExpandWidth(true));
            GUILayout.BeginHorizontal();

            GUILayout.Label(GetIcon("OKWhite", "Skins/ok-circle"), GUILayout.ExpandHeight(false), GUILayout.ExpandWidth(false), GUILayout.Height(16), GUILayout.Width(18));
            GUILayout.Label(effect.name, Skin.GetStyle("PanelText"));
            GUILayout.FlexibleSpace();
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField(effect, typeof(StatEffect), false, GUILayout.MaxWidth(64));
            EditorGUI.EndDisabledGroup();
            FoldoutTrashOnly(out delAtEnd);

            GUILayout.EndHorizontal();
            GUILayout.Space(4);
            GUILayout.EndVertical();
            clickRect = GUILayoutUtility.GetLastRect();

            if (expandedEffects.Contains(effect.name))
            {
                GUILayout.Space(-6);
                GUILayout.BeginVertical(BoxWhite, GUILayout.ExpandWidth(true));

                if (!effectEditors.ContainsKey(effect))
                {
                    effectEditors.Add(effect, (StatEffectEditor)Editor.CreateEditor(effect, typeof(StatEffectEditor)));
                }

                effectEditors[effect].serializedObject.Update();
                effectEditors[effect].DrawInspector(statsCog);
                effectEditors[effect].serializedObject.ApplyModifiedProperties();

                GUILayout.Space(6);
                GUILayout.EndVertical();
            }

            if (!delAtEnd && Event.current.type == EventType.MouseUp && Event.current.button == 0 && clickRect.Contains(Event.current.mousePosition))
            {
                if (expandedEffects.Contains(effect.name))
                {
                    expandedEffects.Remove(effect.name);
                }
                else
                {
                    expandedEffects.Add(effect.name);
                }

                Repaint();
            }

            if (delAtEnd)
            {
                return true;
            }

            return false;
        }

        #endregion

    }
}