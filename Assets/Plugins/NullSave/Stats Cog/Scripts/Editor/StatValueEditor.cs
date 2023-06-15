using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace NullSave.TOCK.Stats
{
    [CustomEditor(typeof(StatValue))]
    public class StatValueEditor : TOCKEditorV2
    {

        #region Variables

        private StatValue myTarget;
        private ReorderableList commandList;

        #endregion

        #region Unity Methods

        private void OnEnable()
        {
            if (target is StatValue)
            {
                myTarget = (StatValue)target;

                // Header
                commandList = new ReorderableList(serializedObject, serializedObject.FindProperty("incrementCommand"), true, true, true, true);
                commandList.elementHeight = EditorGUIUtility.singleLineHeight + 4;
                commandList.drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, "Commands"); };

                // Elements
                commandList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    var element = commandList.serializedProperty.GetArrayElementAtIndex(index);
                    rect.y += 2;

                    EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element, new GUIContent("Command", null, string.Empty));
                };
            }
        }

        public override void OnInspectorGUI()
        {
            MainContainerBegin("Stat Value", "Icons/statscog");

            DrawInspector(null);

            MainContainerEnd();
        }

        #endregion

        #region Window Methods

        internal void DrawInspector(StatsCog statsCog)
        {
            SectionHeader("UI");
            SimpleProperty("icon");
            SimpleProperty("iconColor");
            SimpleProperty("displayName");
            SimpleProperty("displayInList");
            SimpleProperty("textColor");

            SectionHeader("Behaviour");
            SimpleProperty("category");

            DrawValidatedItem("value", statsCog);
            DrawValidatedItem("minValue", statsCog);
            DrawValidatedItem("maxValue", statsCog);
            SimpleProperty("startWithMaxValue", "Start w/ Max Value");
            SimpleProperty("treatAsInt");

            SectionHeader("Regeneration");
            SimpleProperty("enableRegen", "Enable");
            if (serializedObject.FindProperty("enableRegen").boolValue)
            {
                DrawValidatedItem("regenDelay", statsCog);
                DrawValidatedItem("regenPerSecond", statsCog);
            }

            SectionHeader("Incrementing");
            SimpleProperty("enableIncrement", "Enable");
            if (serializedObject.FindProperty("enableIncrement").boolValue)
            {
                SimpleProperty("incrementWhen", "Condition");
                DrawValidatedItem("incrementAmount", statsCog);

                commandList.DoLayoutList();
            }
        }

        #endregion

        #region Private Methods

        private void DrawValidatedItem(string propertyName, StatsCog statsCog)
        {
            GUILayout.BeginHorizontal();
            SimpleProperty(propertyName);
            if (statsCog != null)
            {
                Color c = GUI.contentColor;
                GUI.contentColor = EditorColor;
                GUILayout.Label(statsCog.ValidateExpression(SimpleString("value")) ? GetIcon("Pass", "Icons/tock-check") : GetIcon("Fail", "Icons/tock-alert"), GUILayout.Height(16), GUILayout.Width(16), GUILayout.ExpandWidth(false));
                GUI.contentColor = c;
            }
            GUILayout.EndHorizontal();

        }

        #endregion

    }
}