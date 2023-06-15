using UnityEditor;
using UnityEngine;

namespace NullSave.TOCK.Inventory
{
    [CustomPropertyDrawer(typeof(LoadoutSkill))]
    public class LoadoutSkillDrawer : PropertyDrawer
    {

        #region Variables

        private Texture2D badItem;

        #endregion

        #region Properties

        public Texture2D BadItem
        {
            get
            {
                if (badItem == null)
                {
                    badItem = (Texture2D)Resources.Load("Skins/warn-circle-red", typeof(Texture2D));
                }

                return badItem;
            }
        }

        #endregion

        #region Public Methods

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 3 + 10;
        }

        // Draw the property inside the given rect
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // Draw label
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            //// Don't make child fields be indented
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            object item = property.FindPropertyRelative("skillItem").objectReferenceValue;
            InventoryItem invItem = null;
            if (item != null)
            {
                invItem = (InventoryItem)item;
            }

            var rect = position;
            rect.height = EditorGUIUtility.singleLineHeight;

            if (invItem != null && invItem.itemType != ItemType.Skill)
            {
                Rect rect2 = rect;
                rect2.width -= 24;
                EditorGUI.PropertyField(rect2, property.FindPropertyRelative("skillItem"));
                rect2.x += rect2.width;
                rect2.width = 24;
                EditorGUI.LabelField(rect2, new GUIContent(BadItem));
            }
            else
            {
                EditorGUI.PropertyField(rect, property.FindPropertyRelative("skillItem"));
            }
            rect.y += rect.height + 2;

            EditorGUI.PropertyField(rect, property.FindPropertyRelative("skillSlotId"));
            rect.y += rect.height + 2;

            EditorGUI.PropertyField(rect, property.FindPropertyRelative("forceItem"));
            rect.y += rect.height + 2;

            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }

        #endregion

    }
}
