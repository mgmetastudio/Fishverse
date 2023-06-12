using System.Linq;
using UnityEditor;
using UnityEngine;

namespace NullSave.TOCK.Stats
{
    [CustomPropertyDrawer(typeof(StatConditionalBool))]
    public class StatConditionalBoolDrawer : PropertyDrawer
    {

        #region Variables

        private Texture2D menuArrow;
        private Color contentColor = GUI.contentColor;

        #endregion

        #region Properties

        private Texture2D MenuArrow
        {
            get
            {
                if (menuArrow == null)
                {
                    menuArrow = EditorHelper.GetIcon("MenuArrow", "Icons/menu-arrow");
                }

                return menuArrow;
            }
        }

        #endregion

        #region Public Methods

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight + 4;
        }

        // Draw the property inside the given rect
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var tt = fieldInfo.GetCustomAttributes(typeof(TooltipAttribute), true).FirstOrDefault() as TooltipAttribute;
            if (tt != null) label.tooltip = tt.tooltip;

            ConditionalValueSource valueSource = (ConditionalValueSource)EditorHelper.SimpleInt(property, "valueSource");
            EditorGUI.BeginProperty(position, label, property);

            // Draw label
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            //// Don't make child fields be indented
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            Rect rect = position;
            rect.height = EditorGUIUtility.singleLineHeight;

            switch (valueSource)
            {
                case ConditionalValueSource.Static:
                    EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width - 16, rect.height), property.FindPropertyRelative("value"), new GUIContent(string.Empty, null, string.Empty));
                    break;
                case ConditionalValueSource.StatsCog:
                    EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width - 16, rect.height), property.FindPropertyRelative("valueFromStat"), new GUIContent(string.Empty, null, string.Empty));
                    break;
            }

            GUI.contentColor = EditorHelper.EditorColor;
            EditorGUI.LabelField(new Rect(rect.x + rect.width - 12, rect.y, 12, rect.height), new GUIContent(MenuArrow, "Options"));
            GUI.contentColor = contentColor;

            if (rect.Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDown)
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("Static"), valueSource == ConditionalValueSource.Static, () => { EditorHelper.SimpleInt(property, "valueSource", (int)ConditionalValueSource.Static); property.serializedObject.ApplyModifiedProperties(); });
                menu.AddItem(new GUIContent("From Stat"), valueSource == ConditionalValueSource.StatsCog, () => { EditorHelper.SimpleInt(property, "valueSource", (int)ConditionalValueSource.StatsCog); property.serializedObject.ApplyModifiedProperties(); });
                menu.ShowAsContext();
            }

            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }

        #endregion

    }
}
