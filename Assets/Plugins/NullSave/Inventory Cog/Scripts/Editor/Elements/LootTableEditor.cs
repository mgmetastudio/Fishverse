using UnityEditor;
using UnityEngine;

namespace NullSave.TOCK.Inventory
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(LootTable))]
    public class LootTableEditor : TOCKEditorV2
    {

        #region Variables

        private Vector2 scrollPos;

        #endregion

        #region Unity Methods

        public override void OnInspectorGUI()
        {
            MainContainerBeginSlim();

            // Header
            SectionHeader("Drop List");

            // List
            SerializedProperty list = serializedObject.FindProperty("dropList");
            GUILayout.BeginVertical();
            for (int i = 0; i < list.arraySize; i++)
            {
                if (i < list.arraySize && i >= 0)
                {
                    GUILayout.BeginVertical("Box");
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("X", GUILayout.Width(24)))
                    {
                        list.DeleteArrayElementAtIndex(i);
                        serializedObject.ApplyModifiedProperties();
                        return;
                    }
                    GUILayout.BeginVertical();
                    GUILayout.Space(5);
                    GUILayout.Label("Loot Drop " + (i + 1), Skin.GetStyle("SubHeader"));
                    GUILayout.Space(5);
                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();


                    SimplePropertyRelative(list.GetArrayElementAtIndex(i), "weight");
                    SimplePropertyRelative(list.GetArrayElementAtIndex(i), "usePlayerCondition", "Conditional");
                    if (SimpleBool(list.GetArrayElementAtIndex(i), "usePlayerCondition"))
                    {
                        SimplePropertyRelative(list.GetArrayElementAtIndex(i), "playerCondition");
                    }

                    SubHeader("Items");
                    GUILayout.Space(5);
                    SimpleList(list.GetArrayElementAtIndex(i).FindPropertyRelative("items"), null);
                    GUILayout.Space(5);
                    GUILayout.EndVertical();
                }

                GUILayout.Space(3);
            }
            GUILayout.Space(8);
            GUILayout.EndVertical();

            // Controls
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Add"))
            {
                list.arraySize++;
            }
            if (GUILayout.Button("Clear")) { list.arraySize = 0; }
            GUILayout.EndHorizontal();


            MainContainerEnd();
        }

        #endregion

    }
}