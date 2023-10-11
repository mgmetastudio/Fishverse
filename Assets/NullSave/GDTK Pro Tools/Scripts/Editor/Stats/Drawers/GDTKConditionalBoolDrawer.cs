#if GDTK
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace NullSave.GDTK.Stats
{
    [CustomPropertyDrawer(typeof(GDTKConditionalBool))]
    public class GDTKConditionalBoolDrawer : GDTKPropertyDrawer
    {

        #region Fields

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
                    menuArrow = GDTKEditor.GetIcon("MenuArrow", "Icons/menu-arrow");
                }

                return menuArrow;
            }
        }

        #endregion

        #region Public Methods

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        // Draw the property inside the given rect
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (fieldInfo.GetCustomAttributes(typeof(TooltipAttribute), true).FirstOrDefault() is TooltipAttribute tt) label.tooltip = tt.tooltip;

            // Draw label
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            Rect rect = position;
            rect.height = EditorGUIUtility.singleLineHeight;

            EditorGUI.BeginProperty(position, label, property);

            bool useStat = SimpleValue<bool>(property, "useExpression");
            if (useStat)
            {
                EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width - 16, rect.height), property.FindPropertyRelative("expression"), new GUIContent(string.Empty, null, string.Empty));
            }
            else
            {
                EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width - 16, rect.height), property.FindPropertyRelative("value"), new GUIContent(string.Empty, null, string.Empty));
            }

            GUI.contentColor = GDTKEditor.Styles.EditorColor;
            EditorGUI.LabelField(new Rect(rect.x + rect.width - 12, rect.y, 12, rect.height), new GUIContent(MenuArrow, "Options"));
            GUI.contentColor = contentColor;

            if (rect.Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDown)
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("Static"), !useStat, () => { SimpleValue(property, "useExpression", false); property.serializedObject.ApplyModifiedProperties(); });
                menu.AddItem(new GUIContent("Expression"), useStat, () => { SimpleValue(property, "useExpression", true); property.serializedObject.ApplyModifiedProperties(); });
                menu.ShowAsContext();
            }

            EditorGUI.EndProperty();
        }

        #endregion

    }
}
#endif