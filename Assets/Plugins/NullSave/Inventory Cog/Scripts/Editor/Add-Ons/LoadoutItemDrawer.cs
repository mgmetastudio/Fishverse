using UnityEditor;
using UnityEngine;

namespace NullSave.TOCK.Inventory
{
    [CustomPropertyDrawer(typeof(LoadoutItem))]
    public class LoadoutItemDrawer : PropertyDrawer
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
            int lineCount = 3;

            object item = property.FindPropertyRelative("item").objectReferenceValue;
            if (item != null)
            {
                InventoryItem invItem = (InventoryItem)item;
                if (invItem.itemType != ItemType.Ammo)
                {
                    lineCount += 1;

                    if (property.FindPropertyRelative("autoEquip").boolValue)
                    {
                        lineCount += 1;
                    }
                }
            }

            return (EditorGUIUtility.singleLineHeight * lineCount) + (lineCount * 2) + 4;
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

            object item = property.FindPropertyRelative("item").objectReferenceValue;
            InventoryItem invItem = null;
            if (item != null)
            {
                invItem = (InventoryItem)item;
            }

            var rect = position;
            rect.height = EditorGUIUtility.singleLineHeight;

            if (invItem != null && invItem.itemType == ItemType.Skill)
            {
                Rect rect2 = rect;
                rect2.width -= 24;
                EditorGUI.PropertyField(rect2, property.FindPropertyRelative("item"));
                rect2.x += rect2.width;
                rect2.width = 24;
                EditorGUI.LabelField(rect2, new GUIContent(BadItem));
            }
            else
            {
                EditorGUI.PropertyField(rect, property.FindPropertyRelative("item"));
            }
            rect.y += rect.height + 2;

            EditorGUI.PropertyField(rect, property.FindPropertyRelative("count"));
            rect.y += rect.height + 2;

            EditorGUI.PropertyField(rect, property.FindPropertyRelative("forceItem"));
            rect.y += rect.height + 2;

            if (item != null)
            {
                EditorGUI.PropertyField(rect, property.FindPropertyRelative("autoEquip"));
                rect.y += rect.height + 2;

                if (property.FindPropertyRelative("autoEquip").boolValue)
                {
                    EditorGUI.PropertyField(rect, property.FindPropertyRelative("equipPointId"));
                    rect.y += rect.height + 2;
                }
            }

            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }

        #endregion

    }
}
