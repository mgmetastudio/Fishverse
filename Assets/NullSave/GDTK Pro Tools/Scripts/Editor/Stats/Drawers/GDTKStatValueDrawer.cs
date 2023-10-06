#if GDTK
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace NullSave.GDTK.Stats
{
    [CustomPropertyDrawer(typeof(GDTKStatValue))]
    public class GDTKStatValueDrawer : PropertyDrawer
    {

        #region Fields

        private Texture2D menuArrow;
        private Color contentColor = GUI.contentColor;
        private ReorderableList conditionList;

        #endregion

        #region Properties

        private Texture2D MenuArrow
        {
            get
            {
                if (menuArrow == null)
                {
                    menuArrow = GDTKEditor.GetIcon("MenuArrow", "Icons/menu-arrow");
                }

                return menuArrow;
            }
        }

        #endregion

        #region Unity Methods

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            CreateConditionList(property);
            return EditorGUIUtility.singleLineHeight + 2;
        }

        // Draw the property inside the given rect
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (fieldInfo.GetCustomAttributes(typeof(TooltipAttribute), true).FirstOrDefault() is TooltipAttribute tt) label.tooltip = tt.tooltip;

            ValueType valueType = (ValueType)GDTKEditor.SimpleValue<int>(property, "m_valueType");
            EditorGUI.BeginProperty(position, label, property);

            // Draw label
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            //// Don't make child fields be indented
            var indent = EditorGUI.indentLevel;

            Rect rect = position;
            rect.height = EditorGUIUtility.singleLineHeight;

            switch (valueType)
            {
                case ValueType.RandomRange:
                    float w = (rect.width - 18) / 2;
                    Rect r2 = new Rect(rect.x, rect.y, w, rect.height);
                    EditorGUI.PropertyField(r2, property.FindPropertyRelative("m_randomMin"), new GUIContent(string.Empty, null, string.Empty));
                    r2.x += w + 2;
                    EditorGUI.PropertyField(r2, property.FindPropertyRelative("m_randomMax"), new GUIContent(string.Empty, null, string.Empty));
                    break;
                case ValueType.Standard:
                    EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width - 16, rect.height), property.FindPropertyRelative("m_value"), new GUIContent(string.Empty, null, string.Empty));
                    break;
            }

            GUI.contentColor = GDTKEditor.Styles.EditorColor;
            EditorGUI.LabelField(new Rect(rect.x + rect.width - 12, rect.y, 12, rect.height), new GUIContent(MenuArrow, "Options"));
            GUI.contentColor = contentColor;

            if (rect.Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDown)
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("Standard"), valueType == ValueType.Standard, () => { GDTKEditor.SimpleValue(property, "m_valueType", (int)ValueType.Standard); property.serializedObject.ApplyModifiedProperties(); });
                menu.AddItem(new GUIContent("Random In Range"), valueType == ValueType.RandomRange, () => { GDTKEditor.SimpleValue(property, "m_valueType", (int)ValueType.RandomRange); property.serializedObject.ApplyModifiedProperties(); });
                menu.AddItem(new GUIContent("Conditional"), valueType == ValueType.Conditional, () => { GDTKEditor.SimpleValue(property, "m_valueType", (int)ValueType.Conditional); property.serializedObject.ApplyModifiedProperties(); });
                menu.ShowAsContext();
            }
            rect.y += rect.height + 2;

            if (valueType == ValueType.Conditional)
            {

                EditorGUI.indentLevel++;
                if (conditionList == null) CreateConditionList(property);
                conditionList.DoLayoutList();
                EditorGUI.indentLevel--;

            }

            EditorGUI.EndProperty();
        }

        #endregion

        #region Private Methods

        private void CreateConditionList(SerializedProperty property)
        {
            conditionList = new ReorderableList(property.serializedObject, property.FindPropertyRelative("m_conditions"), true, true, true, true);
            conditionList.elementHeight = (EditorGUIUtility.singleLineHeight + 2) * 2;
            conditionList.drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, "Conditional Values"); };
            conditionList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var element = conditionList.serializedProperty.GetArrayElementAtIndex(index);
                EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("condition"), new GUIContent("Condition", "Condition that must be met for this value"));
                rect.y += EditorGUIUtility.singleLineHeight + 2;
                EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("value"), new GUIContent("Value", "Value to return if condition is met"));
                rect.y += EditorGUIUtility.singleLineHeight + 2;
            };
        }

        #endregion

    }
}
#endif