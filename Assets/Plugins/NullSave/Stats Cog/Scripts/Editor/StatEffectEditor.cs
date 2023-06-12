using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NullSave.TOCK.Stats
{
    [CustomEditor(typeof(StatEffect))]
    public class StatEffectEditor : TOCKEditorV2
    {

        #region Enumerations

        private enum DisplayFlags
        {
            None = 0,
            Behaviour = 1,
            Modifiers = 2,
            UI = 4,
            PreventEffects = 8,
            CancelEffects = 16
        }

        #endregion

        #region Variables

        private StatEffect myTarget;
        private DisplayFlags displayFlags;
        private List<int> expandedMods;

        #endregion

        #region Unity Methods

        public void OnEnable()
        {
            if (target is StatEffect)
            {
                myTarget = (StatEffect)target;
            }

            expandedMods = new List<int>();
        }

        public override void OnInspectorGUI()
        {
            MainContainerBegin("Stat Effect", "Icons/statscog");

            DrawInspector(null);

            MainContainerEnd();
        }

        #endregion

        #region Window Methods

        internal void DrawInspector(StatsCog statsCog)
        {
            displayFlags = (DisplayFlags)serializedObject.FindProperty("z_display_flags").intValue;

            if (SectionToggle((int)displayFlags, (int)DisplayFlags.Behaviour, "Behaviour", GetIcon("Behaviour", "Icons/tock-behaviour")))
            {
                SimpleProperty("isDetrimental", "Detrimental");
                SimpleProperty("isBenificial", "Benifecial");
                SimpleProperty("effectParticles", "Particles");
                SimpleProperty("canStack", "Stackable");
                SimpleProperty("hasLifeSpan");
                if (serializedObject.FindProperty("hasLifeSpan").boolValue)
                {
                    SimpleProperty("lifeInSeconds");
                    if (!serializedObject.FindProperty("canStack").boolValue)
                    {
                        SimpleProperty("resetLifeOnAdd");
                    }
                }
                SimpleProperty("isConditional");
                if (SimpleBool("isConditional"))
                {
                    SimpleProperty("condition");
                }
            }

            if (SectionToggle((int)displayFlags, (int)DisplayFlags.UI, "UI", GetIcon("UI", "Icons/tock-ui")))
            {
                SimpleProperty("category");
                SimpleProperty("sprite");
                SimpleProperty("displayName");
                SimpleProperty("description");
                SimpleProperty("startedText");
                SimpleProperty("endedText");
                SimpleProperty("removedText");
                SimpleProperty("displayInList");
            }

            bool buttonPressed = false;
            bool removeElement = false;
            int removeIndex = 0;
            if (SectionToggleWithButton((int)displayFlags, (int)DisplayFlags.Modifiers, " New ", out buttonPressed, "Modifiers", GetIcon("Tag", "Icons/tock-tag")))
            {

                if (buttonPressed)
                {
                    if (myTarget.modifiers == null) myTarget.modifiers = new List<StatModifier>();
                    myTarget.modifiers.Insert(0, new StatModifier());
                    expandedMods.Clear();
                    expandedMods.Add(0);
                }

                SerializedProperty list = serializedObject.FindProperty("modifiers");
                for (int i = 0; i < list.arraySize; i++)
                {
                    if (i < list.arraySize && i >= 0)
                    {
                        SerializedProperty modifier = list.GetArrayElementAtIndex(i);
                        if (DrawModifer(modifier, statsCog, i))
                        {
                            removeElement = true;
                            removeIndex = i;
                        }
                    }
                }
            }

            if(removeElement)
            {
                myTarget.modifiers.RemoveAt(removeIndex);
                expandedMods.Clear();
                Repaint();
            }

            if (SectionToggle((int)displayFlags, (int)DisplayFlags.PreventEffects, "preventEffects", typeof(StatEffect), "Prevent Effects", GetIcon("Prevent", "Icons/tock-prevent")))
            {
                SimpleList("preventEffects");
            }

            if (SectionToggle((int)displayFlags, (int)DisplayFlags.CancelEffects, "cancelEffects", typeof(StatEffect), "Cancel Effects", GetIcon("Clear", "Icons/tock-clear")))
            {
                SimpleList("cancelEffects");
            }
        }

        internal List<int> DrawInspector(StatsCog statsCog, List<int> prevExpanded)
        {
            return prevExpanded;
        }

        #endregion

        #region Private Methods

        private bool DrawModifer(SerializedProperty modifier, StatsCog statsCog, int index)
        {
            Rect clickRect;
            bool delAtEnd = false;
            GUILayout.BeginVertical(BoxSub, GUILayout.ExpandWidth(true));
            GUILayout.BeginHorizontal();

            GUILayout.Label("[" + index + "] " + modifier.FindPropertyRelative("displayText").stringValue);
            GUILayout.FlexibleSpace();
            FoldoutTrashOnly(out delAtEnd, true);

            GUILayout.EndHorizontal();
            GUILayout.Space(4);
            GUILayout.EndVertical();
            clickRect = GUILayoutUtility.GetLastRect();

            if (expandedMods.Contains(index))
            {
                GUILayout.Space(-6);
                GUILayout.BeginVertical(BoxSub, GUILayout.ExpandWidth(true));
                SimplePropertyRelative(modifier, "icon");
                SimplePropertyRelative(modifier, "displayText");
                SimplePropertyRelative(modifier, "textColor");
                SimplePropertyRelative(modifier, "hideInList");

                GUILayout.Space(6);

                if (statsCog == null)
                {
                    SimplePropertyRelative(modifier, "affectedStat");
                }
                else
                {
                    int selIndex = 0;
                    string[] statNames = new string[statsCog.stats.Count];
                    for (int i = 0; i < statNames.Length; i++)
                    {
                        statNames[i] = statsCog.stats[i].name;
                        if (statNames[i] == modifier.FindPropertyRelative("affectedStat").stringValue)
                        {
                            selIndex = i;
                        }
                    }
                    selIndex = EditorGUILayout.Popup(new GUIContent("Affected Stat", null, "Name of the stat targeted by this modifier"), selIndex, statNames);
                    if (statsCog.stats.Count > selIndex)
                    {
                        modifier.FindPropertyRelative("affectedStat").stringValue = statsCog.stats[selIndex].name;
                    }
                }

                SimplePropertyRelative(modifier, "effectType");
                SimplePropertyRelative(modifier, "valueTarget");
                SimplePropertyRelative(modifier, "valueType");

                if (modifier.FindPropertyRelative("valueType").intValue == (int)EffectValueTypes.ValueSet && modifier.FindPropertyRelative("effectType").intValue != (int)EffectTypes.Instant)
                {
                    GUIStyle s = new GUIStyle(EditorStyles.label);
                    s.normal.textColor = Color.red;
                    EditorGUILayout.LabelField("'Value Set' is only valid for 'Instant' effects.", s);
                }
                else
                {
                    GUILayout.BeginHorizontal();
                    SimplePropertyRelative(modifier, "value");
                    if (statsCog != null)
                    {
                        GUILayout.Label(statsCog.ValidateExpression(modifier.FindPropertyRelative("value").stringValue) ? GetIcon("OK", "ok-circle-green") : GetIcon("Fail", "warn-circle-red"), GUILayout.Height(16), GUILayout.Width(16), GUILayout.ExpandWidth(false));
                    }
                    GUILayout.EndHorizontal();
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