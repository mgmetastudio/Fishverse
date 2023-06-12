using UnityEditor;
using UnityEngine;

namespace NullSave.TOCK.Inventory
{
    [CanEditMultipleObjects()]
    [CustomEditor(typeof(InventoryTheme))]
    public class InventoryThemeEditor : TOCKEditorV2
    {

        #region Unity Methods

        public override void OnInspectorGUI()
        {
            //displayFlags = (DisplayFlags)serializedObject.FindProperty("z_display_flags").intValue;
            MainContainerBegin("Inventory Theme", "Icons/tock-ui", false);

            DrawUI();

            MainContainerEnd();
        }

        #endregion

        #region Private Methods

        private void DrawUI()
        {
            SectionHeader("UI");
            SimpleProperty("displayName");
            SimpleProperty("description");
            SimpleProperty("spawnTag");

            GUILayout.Space(6);
            GUILayout.BeginVertical("box");
            GUILayout.Label("Inventory Cog will look for a canvas with the tag specified in 'Spawn Tag' to use when spawning prompts and menus. Please make sure your canvas is tagged appropriately.", Skin.GetStyle("BodyText"));
            GUILayout.EndVertical();

            SectionHeader("Object Interaction");
            SimpleProperty("enableTriggers");
            SimpleProperty("enableRaycast");
            if(SimpleBool("enableRaycast"))
            {
                SimpleProperty("raycastSource");
                SimpleProperty("raycastCulling");
                SimpleProperty("maxDistance");
                SimpleProperty("raycastOffset");
            }

            SectionHeader("UI Interaction");
            SimpleProperty("generalUI");
            SimpleProperty("enableUIClick", "Click to Select");

            SectionHeader("Prompts");
            SimpleProperty("renamePrompt");
            SimpleProperty("promptUI");
            SimpleProperty("itemText");
            SimpleProperty("merchantText");
            SimpleProperty("containerText");
            SimpleProperty("craftingText");

            SectionHeader("Windows");
            SimpleProperty("inventoryMenu");
            SimpleProperty("itemManagerMenu");
            SimpleProperty("itemContainerMenu");
            SimpleProperty("attachments");
            SimpleProperty("craftingMenu");
            SimpleProperty("craftDetailMenu");
            SimpleProperty("merchantMenu");
            SimpleProperty("tradeMenu");
            SimpleProperty("containerMenu");

            SectionHeader("Item Counts");
            SimpleProperty("countSelection");
            SimpleProperty("minCount");
            GUILayout.Space(6);
            GUILayout.BeginVertical("box");
            GUILayout.Label("Inventory Cog can automatically display a 'Count Selection' prompt when working buying/selling/dropping/taking items. The 'Min Count' property dtermines how many of the item must be available to display the prompt. If the available count is lower a single item will be bought/sold/etc.", Skin.GetStyle("BodyText"));
            GUILayout.EndVertical();

            SectionHeader("Count Prompt Texts");
            SimpleProperty("buyPrompt", "Buy");
            SimpleProperty("sellPrompt", "Sell");
            SimpleProperty("dropPrompt", "Drop");
            SimpleProperty("consumePrompt", "Consume");
            SimpleProperty("breakdownPrompt", "Breakdown");
            SimpleProperty("craftPrompt", "Craft");
            SimpleProperty("genericPrompt", "Generic");

            SectionHeader("Rarities");
            SerializedProperty list = serializedObject.FindProperty("rarityLevels");
            for (int i = 0; i < list.arraySize; i++)
            {
                GUILayout.BeginHorizontal();

                GUILayout.Label("Rarity " + i);
                GUILayout.FlexibleSpace();
                if (i < list.arraySize && i >= 0)
                {
                    EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i).FindPropertyRelative("name"), new GUIContent(string.Empty, null, string.Empty));
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label(string.Empty);
                GUILayout.FlexibleSpace();

                EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i).FindPropertyRelative("color"), new GUIContent(string.Empty, null, string.Empty));
                GUILayout.EndHorizontal();

                GUILayout.Space(4);
            }

        }

        #endregion

        #region Window Methods

        public void DrawInspector()
        {
            //displayFlags = (DisplayFlags)serializedObject.FindProperty("z_display_flags").intValue;
            DrawUI();
        }

        public void DrawStep0()
        {
            SectionHeader("Basic Information");
            SimpleProperty("displayName");
            SimpleProperty("description");
            SimpleProperty("spawnTag");

            GUILayout.Space(6);
            GUILayout.BeginVertical("box");
            GUILayout.Label("Inventory Cog will look for a canvas with the tag specified in 'Spawn Tag' to use when spawning prompts and menus. Please make sure your canvas is tagged appropriately.", Skin.GetStyle("BodyText"));
            GUILayout.EndVertical();

            SectionHeader("Object Interaction");
            SimpleProperty("enableTriggers");
            SimpleProperty("enableRaycast");
            if (SimpleBool("enableRaycast"))
            {
                SimpleProperty("maxDistance");
                SimpleProperty("raycastOffset");
            }

            SectionHeader("UI Interaction");
            SimpleProperty("enableUIClick", "Click to Select");

            SectionHeader("Item Counts");
            SimpleProperty("minCount");
            GUILayout.Space(6);
            GUILayout.BeginVertical("box");
            GUILayout.Label("Inventory Cog can automatically display a 'Count Selection' prompt when working buying/selling/dropping/taking items. The 'Min Count' property dtermines how many of the item must be available to display the prompt. If the available count is lower a single item will be bought/sold/etc.", Skin.GetStyle("BodyText"));
            GUILayout.EndVertical();

        }

        public void DrawStep1()
        {
            SectionHeader("Prompts");
            GUILayout.Label("A single prompt object is used to work in multiple settings. Use the prompt texts below to determine the action text you\r\n" +
                "wish to use in each of the cases.");
            GUILayout.Space(6);
            SimpleProperty("itemText");
            SimpleProperty("merchantText");
            SimpleProperty("containerText");
            SimpleProperty("craftingText");

            SectionHeader("Rarity");
            SerializedProperty list = serializedObject.FindProperty("rarityLevels");
            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical();
            for (int i = 0; i < 6; i++)
            {
                GUILayout.BeginHorizontal();

                GUILayout.Label("Rarity " + i);
                GUILayout.FlexibleSpace();
                if (i < list.arraySize && i >= 0)
                {
                    EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i).FindPropertyRelative("name"), new GUIContent(string.Empty, null, string.Empty));
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label(string.Empty);
                GUILayout.FlexibleSpace();

                EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i).FindPropertyRelative("color"), new GUIContent(string.Empty, null, string.Empty));
                GUILayout.EndHorizontal();

                GUILayout.Space(4);
            }
            GUILayout.EndVertical();

            GUILayout.Space(80);
        
            GUILayout.BeginVertical();
            for (int i = 6; i < list.arraySize; i++)
            {
                GUILayout.BeginHorizontal();

                GUILayout.Label("Rarity " + i);
                GUILayout.FlexibleSpace();
                if (i < list.arraySize && i >= 0)
                {
                    EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i).FindPropertyRelative("name"), new GUIContent(string.Empty, null, string.Empty));
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label(string.Empty);
                GUILayout.FlexibleSpace();

                EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i).FindPropertyRelative("color"), new GUIContent(string.Empty, null, string.Empty));
                GUILayout.EndHorizontal();

                GUILayout.Space(4);
            }
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();



        }

        #endregion

    }
}