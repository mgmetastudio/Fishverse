using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NullSave.TOCK.Stats
{
    public static class StatEditorHelper
    {

        #region Public Methods

        public static bool DrawConditionalBool(SerializedObject serializedObject, string propertyName, string title, TOCKEditorV2 editor)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty(propertyName));

            SerializedProperty boolTest = serializedObject.FindProperty(propertyName);
            ConditionalValueSource vs = (ConditionalValueSource)editor.SimpleInt(boolTest, "valueSource");
            return (vs == ConditionalValueSource.Static && editor.SimpleBool(boolTest, "value")) ||
                vs == ConditionalValueSource.StatsCog;
        }

        public static bool DrawConditionalBool(SerializedProperty stat, string propertyName, string title, TOCKEditorV2 editor)
        {
            editor.SimplePropertyRelative(stat, propertyName, title);
            SerializedProperty boolTest = stat.FindPropertyRelative(propertyName);
            ConditionalValueSource vs = (ConditionalValueSource)editor.SimpleInt(boolTest, "valueSource");
            return (vs == ConditionalValueSource.Static && editor.SimpleBool(boolTest, "value")) ||
                vs == ConditionalValueSource.StatsCog;
        }

        public static string DrawDebugInfo(StatsCog myTarget, int index, string debugCommand, TOCKEditorV2 editor)
        {
            GUIStyle boldLabel = GUI.skin.GetStyle("BoldLabel");
            float debugWidth = (EditorGUIUtility.currentViewWidth - EditorGUIUtility.labelWidth - 45) / 2;

            GUILayout.Space(-5);
            GUILayout.BeginVertical("box");

            GUILayout.BeginHorizontal();
            GUILayout.Label("Value", boldLabel, GUILayout.ExpandWidth(true));
            GUILayout.Label("Current", boldLabel, GUILayout.Width(debugWidth));
            GUILayout.Label("Base", boldLabel, GUILayout.Width(debugWidth));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Minimum", GUILayout.ExpandWidth(true));
            GUILayout.Label(myTarget.Stats[index].CurrentMinimum.ToString(), GUILayout.Width(debugWidth));
            GUILayout.Label(myTarget.Stats[index].CurrentBaseMinimum.ToString(), GUILayout.Width(debugWidth));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Value", GUILayout.ExpandWidth(true));
            GUILayout.Label(myTarget.Stats[index].CurrentValue.ToString(), GUILayout.Width(debugWidth));
            GUILayout.Label(myTarget.Stats[index].CurrentBaseValue.ToString(), GUILayout.Width(debugWidth));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Maximum", GUILayout.ExpandWidth(true));
            GUILayout.Label(myTarget.Stats[index].CurrentMaximum.ToString(), GUILayout.Width(debugWidth));
            GUILayout.Label(myTarget.Stats[index].CurrentBaseMaximum.ToString(), GUILayout.Width(debugWidth));
            GUILayout.EndHorizontal();

            if (myTarget.stats[index].enableRegen)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Regen Delay", GUILayout.ExpandWidth(true));
                GUILayout.Label(myTarget.Stats[index].CurrentBaseRegenAmount.ToString(), GUILayout.Width(debugWidth));
                GUILayout.Label(myTarget.Stats[index].CurrentBaseRegenDelay.ToString(), GUILayout.Width(debugWidth));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Regen Amount", GUILayout.ExpandWidth(true));
                GUILayout.Label(myTarget.Stats[index].CurrentBaseRegenAmount.ToString(), GUILayout.Width(debugWidth));
                GUILayout.Label(myTarget.Stats[index].CurrentBaseRegenDelay.ToString(), GUILayout.Width(debugWidth));
                GUILayout.EndHorizontal();
            }

            GUILayout.Space(6);
            GUILayout.Label("Active Modifiers", boldLabel);
            if (myTarget.Stats[index].ActiveModifiers.Count == 0)
            {
                GUILayout.Label("{ None }");
            }
            else
            {
                foreach (StatModifier mod in myTarget.Stats[index].ActiveModifiers)
                {
                    string appVal = mod.AppliedValue.ToString();
                    if (!appVal.StartsWith("-"))
                    {
                        appVal = "+" + appVal;
                    }
                    GUILayout.Label(appVal);
                }
            }

            GUILayout.Space(6);
            GUILayout.Label("Command", boldLabel);
            GUILayout.BeginHorizontal();
            debugCommand = GUILayout.TextField(debugCommand);
            if (GUILayout.Button("Send", GUILayout.Width(48)))
            {
                myTarget.SendCommand(debugCommand);
                debugCommand = string.Empty;
                editor.Repaint();
            }
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            return debugCommand;
        }

        public static bool DrawModifer(SerializedProperty modifier, StatsCog statProcessor, int index, List<int> expandedMods, TOCKEditorV2 editor, bool forceSustained = false)
        {
            Rect clickRect;
            bool delAtEnd = false;
            GUILayout.BeginVertical(editor.BoxSub, GUILayout.ExpandWidth(true));
            GUILayout.BeginHorizontal();

            GUILayout.Label("[" + index + "] " + modifier.FindPropertyRelative("displayName").stringValue);
            GUILayout.FlexibleSpace();
            editor.FoldoutTrashOnly(out delAtEnd);

            GUILayout.EndHorizontal();
            GUILayout.Space(4);
            GUILayout.EndVertical();
            clickRect = GUILayoutUtility.GetLastRect();

            if (expandedMods.Contains(index))
            {
                GUILayout.Space(-6);
                GUILayout.BeginVertical(editor.BoxSub, GUILayout.ExpandWidth(true));
                editor.SimplePropertyRelative(modifier, "category");
                editor.SimplePropertyRelative(modifier, "displayName");
                editor.SimplePropertyRelative(modifier, "description");
                editor.SimplePropertyRelative(modifier, "icon");
                editor.SimplePropertyRelative(modifier, "showInList");

                GUILayout.Space(6);

                editor.SimplePropertyRelative(modifier, "targetname");
                if (forceSustained)
                {
                    EffectTypes mt = (EffectTypes)editor.SimpleInt(modifier, "modifierType");
                    if (mt != EffectTypes.Sustained)
                    {
                        mt = EffectTypes.Sustained;
                        editor.SimpleInt(modifier, "modifierType", (int)mt);
                    }

                    EditorGUI.BeginDisabledGroup(true);
                    editor.SimplePropertyRelative(modifier, "modifierType");
                    EditorGUI.EndDisabledGroup();
                }
                else
                {
                    editor.SimplePropertyRelative(modifier, "modifierType");
                }
                editor.SimplePropertyRelative(modifier, "valueTarget");
                editor.SimplePropertyRelative(modifier, "valueType");

                if (modifier.FindPropertyRelative("valueType").intValue == (int)EffectValueTypes.ValueSet && modifier.FindPropertyRelative("effectType").intValue != (int)EffectTypes.Instant)
                {
                    GUIStyle s = new GUIStyle(EditorStyles.label);
                    s.normal.textColor = Color.red;
                    EditorGUILayout.LabelField("'Value Set' is only valid for 'Instant' effects.", s);
                }
                else
                {
                    editor.SimplePropertyRelative(modifier, "value");
                }

                GUILayout.Space(4);
                GUILayout.EndVertical();
            }

            if (!delAtEnd && Event.current.type == EventType.MouseUp && Event.current.button == 0 && clickRect.Contains(Event.current.mousePosition))
            {
                if (expandedMods.Contains(index))
                {
                    expandedMods.Remove(index);
                }
                else
                {
                    expandedMods.Add(index);
                }

                editor.Repaint();
            }

            if (delAtEnd)
            {
                return true;
            }

            return false;
        }

        public static int DrawModifiers(SerializedProperty modifiers, List<int> expandedMods, StatsCog statProcessor, TOCKEditorV2 editor, bool forceSustained = false)
        {
            bool buttonPressed;
            editor.SectionHeaderWithButton("Modifiers", " Add ", out buttonPressed);
            int removeIndex = -1;

            for (int i = 0; i < modifiers.arraySize; i++)
            {
                if (i < modifiers.arraySize && i >= 0)
                {
                    SerializedProperty modifier = modifiers.GetArrayElementAtIndex(i);
                    if (DrawModifer(modifier, statProcessor, i, expandedMods, editor, forceSustained))
                    {
                        removeIndex = i;
                    }
                }
            }

            if (buttonPressed)
            {
                modifiers.arraySize++;
                SerializedProperty mod = modifiers.GetArrayElementAtIndex(modifiers.arraySize - 1);
                editor.SimpleBool(mod, "showInList", true);
                if (forceSustained)
                {
                    editor.SimpleInt(mod, "modifierType", (int)EffectTypes.Sustained);
                }
            }

            return removeIndex;
        }

        public static void DrawStat(SerializedProperty stat, Dictionary<SerializedProperty, StatValueEditor> statEditors, StatsCog statProcessor, TOCKEditorV2 editor)
        {
            if (!statEditors.ContainsKey(stat))
            {
                statEditors.Add(stat, (StatValueEditor)Editor.CreateEditor(stat.objectReferenceValue, typeof(StatValueEditor)));
            }

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField(stat, new GUIContent("StatValue"));
            EditorGUI.EndDisabledGroup();

            statEditors[stat].serializedObject.Update();
            statEditors[stat].DrawInspector(statProcessor);
            statEditors[stat].serializedObject.ApplyModifiedProperties();
        }

        public static void DrawStatsList(SerializedProperty list, List<StatValue> rawStats, List<int> debugStats, List<int> expandedStats, Dictionary<SerializedProperty, StatValueEditor> statEditors, StatsCog statProcessor, ref bool dragStat, ref int curIndex, ref int startIndex, ref Vector2 startPos, ref string debugCommand, TOCKEditorV2 editor)
        {
            if (rawStats == null) return;

            Color resContent = GUI.contentColor;
            Color resBackground = GUI.backgroundColor;
            Color resColor = GUI.color;
            Color resText = editor.ButtonLeft.normal.textColor;
            bool isDebug, isExpanded;
            bool doDelete = false;
            int refIndex = 0;

            for (int i = 0; i < rawStats.Count; i++)
            {
                isDebug = debugStats.Contains(i);
                isExpanded = expandedStats.Contains(i);

                GUILayout.BeginHorizontal();

                if (dragStat && curIndex == i)
                {
                    GUILayout.Space(-6);
                    GUI.contentColor = editor.EditorColor;
                    GUILayout.Label(editor.GetIcon("Insert", "Icons/tock-above"), GUILayout.Height(18), GUILayout.Width(24));
                    GUI.contentColor = resContent;
                }

                string errors;
                if (rawStats[i].Validate(statProcessor, out errors))
                {
                    GUI.backgroundColor = new Color(0, 0.75f, 0);
                    GUI.contentColor = Color.white;
                    GUILayout.Label(editor.GetIcon("Checkmark", "Icons/tock-check"), editor.ButtonLeft, GUILayout.Height(18), GUILayout.Width(24));
                    GUI.backgroundColor = resBackground;
                    GUI.contentColor = resContent;
                }
                else
                {
                    GUI.backgroundColor = new Color(0.75f, 0, 0);
                    GUI.contentColor = Color.white;
                    GUILayout.Label(editor.GetIcon("Error", "Icons/tock-alert"), editor.ButtonLeft, GUILayout.Height(18), GUILayout.Width(24));
                    GUI.backgroundColor = resBackground;
                    GUI.contentColor = resContent;
                }

                // Debug button
                if (Application.isPlaying)
                {
                    GUI.backgroundColor = new Color(0.235f, 0.541f, 0.722f);
                    GUI.contentColor = Color.white;
                    GUILayout.Label(editor.GetIcon("Debug", "Icons/tock-debug"), isDebug ? editor.ButtonMidPressed : editor.ButtonMid, GUILayout.Height(18), GUILayout.Width(24));
                    GUI.backgroundColor = resBackground;
                    GUI.contentColor = resContent;

                    if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDown)
                    {
                        if (isDebug)
                        {
                            debugStats.Remove(i);
                        }
                        else
                        {
                            debugStats.Add(i);
                        }

                        editor.Repaint();
                    }
                }

                if (!Application.isPlaying && !dragStat)
                {
                    if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDrag)
                    {
                        dragStat = true;
                        startIndex = i;
                        curIndex = i;
                        startPos = Event.current.mousePosition;
                        editor.Repaint();
                    }
                }

                if (dragStat && curIndex == i)
                {
                    editor.ButtonMid.normal.textColor = Color.blue;
                    GUILayout.Label(rawStats[startIndex].name + " >> " + rawStats[i].name, editor.ButtonMid, GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth - 112));
                    editor.ButtonMid.normal.textColor = resText;
                }
                else
                {
                    if (Application.isPlaying)
                    {
                        GUILayout.Label("(" + rawStats[i].CurrentValue + ") " + rawStats[i].name, isExpanded ? editor.ButtonRightPressed : editor.ButtonRight, GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth - 48));
                    }
                    else
                    {
                        GUILayout.Label(rawStats[i].name, isExpanded ? editor.ButtonMidPressed : editor.ButtonMid, GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth - 48));
                    }
                }

                if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDown)
                {
                    if (isExpanded)
                    {
                        expandedStats.Remove(i);
                    }
                    else
                    {
                        expandedStats.Add(i);
                    }
                    editor.Repaint();
                }

                if (!Application.isPlaying)
                {
                    // Delete Button
                    GUI.contentColor = editor.EditorColor;
                    GUILayout.Label(new GUIContent(editor.GetIcon("Delete", "Icons/tock-trash"), "Delete"), editor.ButtonRight, GUILayout.Height(18), GUILayout.Width(24));
                    GUI.contentColor = resContent;
                    if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDown)
                    {
                        doDelete = true;
                        refIndex = i;
                    }
                }

                GUILayout.EndHorizontal();


                // Debug Information
                if (isDebug)
                {
                    debugCommand = StatEditorHelper.DrawDebugInfo(statProcessor, i, debugCommand, editor);
                }

                // Stat Information
                if (isExpanded)
                {
                    GUILayout.Space(-5);
                    GUILayout.BeginVertical(editor.SubSectionBox);

                    if (errors != null)
                    {
                        GUILayout.Label(errors, editor.ErrorTextStyle);
                    }

                    StatEditorHelper.DrawStat(list.GetArrayElementAtIndex(i), statEditors, statProcessor, editor);
                    GUILayout.EndVertical();
                }

            }

            if (doDelete)
            {
                rawStats.RemoveAt(refIndex);
                return;
            }
        }

        public static int DrawTrait(SerializedProperty trait, StatsCog statProcessor, List<int> expandedMods, TOCKEditorV2 editor)
        {
            editor.SimplePropertyRelative(trait, "category");
            editor.SimplePropertyRelative(trait, "traitId");
            editor.SimplePropertyRelative(trait, "displayName");
            editor.SimplePropertyRelative(trait, "description");
            editor.SimplePropertyRelative(trait, "icon");
            editor.SimplePropertyRelative(trait, "showInList");
            editor.SimplePropertyRelative(trait, "unlocked");

            return DrawModifiers(trait.FindPropertyRelative("modifiers"), expandedMods, statProcessor, editor, true);
        }

        public static int DrawOptionsList(SerializedProperty list, string[] options, int selIndex, TOCKEditorV2 editor, bool insertAtZero = true)
        {
            Color resContent = GUI.contentColor;
            bool doDelete = false;
            int refIndex = -1;

            GUILayout.BeginHorizontal();
            GUILayout.Label("Available ");
            selIndex = EditorGUILayout.Popup(selIndex, options);
            if (GUILayout.Button(" Add "))
            {
                if (insertAtZero)
                {
                    list.InsertArrayElementAtIndex(0);
                    list.GetArrayElementAtIndex(0).stringValue = options[selIndex];
                }
                else
                {
                    list.arraySize++;
                    list.GetArrayElementAtIndex(list.arraySize - 1).stringValue = options[selIndex];
                }
            }
            GUILayout.EndHorizontal();

            for (int i = 0; i < list.arraySize; i++)
            {
                GUILayout.BeginHorizontal();

                GUILayout.Label(list.GetArrayElementAtIndex(i).stringValue, editor.ButtonLeft);

                // Delete Button
                GUI.contentColor = editor.EditorColor;
                GUILayout.Label(new GUIContent(editor.GetIcon("Delete", "Icons/tock-trash"), "Delete"), editor.ButtonRight, GUILayout.Height(18), GUILayout.Width(24));
                GUI.contentColor = resContent;
                if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDown)
                {
                    doDelete = true;
                    refIndex = i;
                }

                GUILayout.EndHorizontal();
            }

            if (doDelete)
            {
                list.DeleteArrayElementAtIndex(refIndex);
            }

            return selIndex;
        }

        #endregion

    }
}