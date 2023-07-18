using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace NullSave.TOCK.Inventory
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(InventoryCog))]
    public class InventoryCogEditor : TOCKEditorV2
    {

        #region Enumerations

        private enum DisplayFlags
        {
            None = 0,
            UI = 1,
            Recipes = 2,
            FailedCraft = 4,
            StartingItems = 8,
            Events = 16,
            EquipPoints = 32,
            StorePoints = 64,
            Skills = 128,
            ItemTag = 256,
            DB = 1024,
            ItemCats = 2048,
            Items = 4096,
            CraftCat = 8192,
            Debug = 16384,
        }

        #endregion

        #region Variables

        private InventoryCog myTarget;
        private DisplayFlags displayFlags;

        private string[] views = new string[] { "Debug", "Default" };
        private string[] spawnDrop = new string[] { "Automatic", "Manual" };
        private Vector2 itemScroll;

        private int boneIndex, boneIndex2;
        private InventoryDB db;
        private InventoryDBEditor dbEditor, dbPrefabEditor;
        private GameObject prefabRoot;
        private bool forceSave;
        private string[] themeNames;
        private List<InventoryItemUITag> tags, expandedTags;
        private Dictionary<InventoryItemUITag, InventoryItemUITagEditor> tagEditors;

        #endregion

        #region Unity Methods

        private void OnEnable()
        {
            if (target is InventoryCog)
            {
                myTarget = (InventoryCog)target;
            }

            // Find Inventory DB
            db = GameObject.FindObjectOfType<InventoryDB>();
            if (db != null)
            {
                dbEditor = (InventoryDBEditor)Editor.CreateEditor(db, typeof(InventoryDBEditor));

                prefabRoot = PrefabUtility.GetOutermostPrefabInstanceRoot(db);
                if (prefabRoot != null)
                {
                    dbPrefabEditor = (InventoryDBEditor)Editor.CreateEditor(prefabRoot.GetComponent<InventoryDB>(), typeof(InventoryDBEditor));
                }

                tags = new List<InventoryItemUITag>();
                expandedTags = new List<InventoryItemUITag>();
                tagEditors = new Dictionary<InventoryItemUITag, InventoryItemUITagEditor>();
                UpdateThemeList();
                UpdateTagsList();
            }
        }

        public override void OnInspectorGUI()
        {
            displayFlags = (DisplayFlags)serializedObject.FindProperty("z_display_flags").intValue;
            MainContainerBeginSlim();

            DrawDB();

            if (Application.isPlaying)
            {
                DrawDebug();
            }

            if (prefabRoot != null && myTarget.directDBPrefab)
            {
                dbPrefabEditor.serializedObject.Update();
            }
            else
            {
                if(dbEditor != null) dbEditor.serializedObject.Update();
            }

            using (EditorGUI.ChangeCheckScope scope = new EditorGUI.ChangeCheckScope())
            {
                DrawUI();
                if (!forceSave) DrawItemCategories();
                if (!forceSave) DrawItemTags();
                if (!forceSave) DrawItems();
                if (!forceSave) DrawStarting();
                if (!forceSave) DrawEquipPoints();
                if (!forceSave) DrawStorePoints();
                if (!forceSave) DrawSkills();
                if (!forceSave) DrawCraftingCategories();
                if (!forceSave) DrawRecipes();
                if (!forceSave) DrawCrafting();

                if (!forceSave && SectionToggle((int)displayFlags, (int)DisplayFlags.Events, "Events", GetIcon("Events", "Icons/tock-event")))
                {
                    SimpleProperty("onItemDropped");
                    SimpleProperty("onItemAdded");
                    SimpleProperty("onSpawnDropRequested");
                    SimpleProperty("onItemEquipped");
                    SimpleProperty("onItemStored");
                    SimpleProperty("onItemUnequipped");
                    SimpleProperty("onCraftingFailed");
                    SimpleProperty("onMenuOpen");
                    SimpleProperty("onMenuClose");
                }

                if (prefabRoot != null && myTarget.directDBPrefab)
                {
                    dbPrefabEditor.serializedObject.ApplyModifiedProperties();
                    if (scope.changed || forceSave)
                    {
                        PrefabUtility.ApplyPrefabInstance(prefabRoot, InteractionMode.AutomatedAction);
                    }
                }
                else
                {
                    if (dbEditor != null) dbEditor.serializedObject.ApplyModifiedProperties();
                }

                forceSave = false;
            }

            MainContainerEnd();
        }

        #endregion

        #region Private Methods

        private void DrawCrafting()
        {
            if (SectionToggle((int)displayFlags, (int)DisplayFlags.FailedCraft, "Failed Crafting", GetIcon("Recipe", "Icons/tock-alert")))
            {
                SimpleList("failedResult");
            }
        }

        private void DrawCraftingCategories()
        {
            if (dbEditor == null && dbPrefabEditor == null) return;

            if (SectionToggle((int)displayFlags, (int)DisplayFlags.CraftCat, "Crafting Categories", GetIcon("Category", "Icons/category")))
            {
                if (prefabRoot != null && myTarget.directDBPrefab)
                {
                    forceSave |= dbPrefabEditor.DrawCraftingCategoryList();
                }
                else
                {
                    forceSave |= dbEditor.DrawCraftingCategoryList();
                }
            }
        }

        private void DrawDB()
        {
#if PHOTON_UNITY_NETWORKING
            if (SectionToggle((int)displayFlags, (int)DisplayFlags.DB, "Inventory DB & PUN", GetIcon("DB", "Icons/tock-db")))
#else
            if (SectionToggle((int)displayFlags, (int)DisplayFlags.DB, "Inventory DB", GetIcon("DB", "Icons/tock-db")))
#endif
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.ObjectField("Active DB", db, typeof(InventoryDB), true);
                EditorGUI.EndDisabledGroup();
                if (prefabRoot != null)
                {
                    SimpleProperty("directDBPrefab", "Apply to Prefab");
                }
#if PHOTON_UNITY_NETWORKING
                SimpleProperty("punInstance", "Use PUN Instancing");
#endif

                if(db == null)
                {
                    if(GUILayout.Button("Add New DB to Scene"))
                    {
                        GameObject go = new GameObject("Inventory DB");
                        go.AddComponent(typeof(InventoryDB));
                    }
                }
            }
        }

        private void DrawDebug()
        {
            if (SectionToggle((int)displayFlags, (int)DisplayFlags.Debug, "Debug", GetIcon("Debug", "Icons/view")))
            {
                GUILayout.Label("Active Theme: " + (myTarget.headless ? "Headless" : myTarget.ActiveTheme == null ? "None" : myTarget.ActiveTheme.name));

                foreach (InventoryItem item in myTarget.Items)
                {
                    GUILayout.Label("(" + item.CurrentCount + ") " + item.DisplayName + (item.EquipState == EquipState.NotEquipped ? string.Empty : item.EquipState == EquipState.Equipped ? " [Equipped]" : " [Stored]"));
                }
            }
        }

        private void DrawEquipPoints()
        {
            if (SectionToggle((int)displayFlags, (int)DisplayFlags.EquipPoints, "Equip Points", GetIcon("Equip", "Icons/tock-equip")))
            {
                EquipPoint[] points = myTarget.GetComponentsInChildren<EquipPoint>();
                foreach (EquipPoint point in points)
                {
                    EditorGUILayout.ObjectField(point, typeof(EquipPoint), true);
                }
                if (points.Length == 0)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(24);
                    GUILayout.Label("{None}", Skin.GetStyle("SubHeader"));
                    GUILayout.EndHorizontal();
                }

                Animator anim = myTarget.GetComponentInChildren<Animator>();
                if (anim != null && anim.isHuman)
                {
                    SubHeader("Dynamic Creation");
                    boneIndex = EditorGUILayout.Popup("Add to Bone", boneIndex, bones, GUILayout.ExpandWidth(true));
                    if (GUILayout.Button("Add"))
                    {
                        Transform target = anim.GetBoneTransform((HumanBodyBones)boneIndex);
                        if (target == null)
                        {
                            EditorUtility.DisplayDialog("Inventgory Cog", "The requested bone '" + bones[boneIndex] + "' could not be found on the selected rig.", "OK");
                        }
                        else
                        {
                            GameObject newDD = new GameObject();
                            newDD.name = "EquipPoint_" + bones[boneIndex];
                            newDD.AddComponent<EquipPoint>().pointId = bones[boneIndex];
                            newDD.transform.SetParent(target);
                            newDD.transform.localPosition = Vector3.zero;
                            Selection.activeGameObject = newDD;
                        }
                    }
                }
            }

        }

        private void DrawItemCategories()
        {
            if (dbEditor == null && dbPrefabEditor == null) return;

            if (SectionToggle((int)displayFlags, (int)DisplayFlags.ItemCats, "Item Categories", GetIcon("Tags", "Icons/tock-tag-list")))
            {
                if (prefabRoot != null && myTarget.directDBPrefab)
                {
                    forceSave |= dbPrefabEditor.DrawItemCategoryList();
                }
                else
                {
                    forceSave |= dbEditor.DrawItemCategoryList();
                }
            }
        }

        private void DrawItems()
        {
            if (dbEditor == null && dbPrefabEditor == null) return;

            if (SectionToggle((int)displayFlags, (int)DisplayFlags.Items, "Item List", GetIcon("Item", "Icons/tock-sword")))
            {
                if (prefabRoot != null && myTarget.directDBPrefab)
                {
                    forceSave |= dbPrefabEditor.DrawItemsList(prefabRoot);
                }
                else
                {
                    forceSave |= dbEditor.DrawItemsList(null);
                }
            }
        }

        private void DrawItemTag(InventoryItemUITag tag)
        {
            Rect clickRect;

            GUILayout.Space(-4);

            GUILayout.BeginVertical(BoxBlue, GUILayout.ExpandWidth(true));
            GUILayout.BeginHorizontal();

            GUILayout.Label(tag.tagText, Skin.GetStyle("PanelText"));
            GUILayout.FlexibleSpace();

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField("", tag, typeof(InventoryItemUITag), false, GUILayout.MaxWidth(64));
            EditorGUI.EndDisabledGroup();

            GUILayout.EndHorizontal();
            GUILayout.Space(4);
            GUILayout.EndVertical();
            clickRect = GUILayoutUtility.GetLastRect();

            if (expandedTags.Contains(tag))
            {
                GUILayout.Space(-6);
                GUILayout.BeginVertical(BoxWhite, GUILayout.ExpandWidth(true));

                if (!tagEditors.ContainsKey(tag))
                {
                    tagEditors.Add(tag, (InventoryItemUITagEditor)Editor.CreateEditor(tag, typeof(InventoryItemUITagEditor)));
                }
                tagEditors[tag].serializedObject.Update();
                tagEditors[tag].DrawInspector();
                tagEditors[tag].serializedObject.ApplyModifiedProperties();

                GUILayout.Space(4);
                GUILayout.EndVertical();
            }

            if (Event.current.type == EventType.MouseUp && Event.current.button == 0 && clickRect.Contains(Event.current.mousePosition))
            {
                if (expandedTags.Contains(tag))
                {
                    expandedTags.Remove(tag);
                }
                else
                {
                    expandedTags.Add(tag);
                }

                Repaint();
            }
        }

        private void DrawItemTags()
        {
            if (SectionToggle((int)displayFlags, (int)DisplayFlags.ItemTag, "Item Tags", GetIcon("Tag", "Icons/tock-tag")))
            {
                // Header
                bool createNew = false;
                Rect clickRect;
                Color restore = GUI.color;
                SerializedProperty list = serializedObject.FindProperty("categories");

                GUILayout.BeginVertical(BoxSub, GUILayout.ExpandWidth(true));
                GUILayout.BeginHorizontal();

                GUILayout.Label("All Item Tags in Project");

                GUILayout.FlexibleSpace();

                GUI.color = Color.black;
                GUILayout.Label(GetIcon("Add", "Icons/tock-add"), GUILayout.ExpandHeight(false), GUILayout.ExpandWidth(false), GUILayout.Height(16), GUILayout.Width(18));
                clickRect = GUILayoutUtility.GetLastRect();
                if (Event.current.type == EventType.MouseUp && Event.current.button == 0 && clickRect.Contains(Event.current.mousePosition))
                {
                    createNew = true;
                }
                GUI.color = restore;

                GUILayout.EndHorizontal();
                GUILayout.Space(4);
                GUILayout.EndVertical();
                clickRect = GUILayoutUtility.GetLastRect();

                // foreach (InventoryItemUITag tag in tags)
                // {
                //     DrawItemTag(tag);
                // }

                if (createNew)
                {
                    ScriptableObject newItem = CreateNew("Item Tag", typeof(InventoryItemUITag));
                    if (newItem != null)
                    {
                        UpdateTagsList();
                    }
                }

            }
        }

        private void DrawRecipes()
        {
            if (dbEditor == null && dbPrefabEditor == null) return;

            if (SectionToggle((int)displayFlags, (int)DisplayFlags.Recipes, "Recipes", GetIcon("Recipes", "Icons/category_page")))
            {
                if (prefabRoot != null && myTarget.directDBPrefab)
                {
                    forceSave |= dbPrefabEditor.DrawRecipesList();
                }
                else
                {
                    forceSave |= dbEditor.DrawRecipesList();
                }
            }
        }

        private void DrawStorePoints()
        {
            if (SectionToggle((int)displayFlags, (int)DisplayFlags.StorePoints, "Store Points", GetIcon("Store", "Icons/store_point")))
            {
                StorePoint[] points = myTarget.GetComponentsInChildren<StorePoint>();
                foreach (StorePoint point in points)
                {
                    EditorGUILayout.ObjectField(point, typeof(StorePoint), true);
                }
                if (points.Length == 0)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(24);
                    GUILayout.Label("{None}", Skin.GetStyle("SubHeader"));
                    GUILayout.EndHorizontal();
                }

                Animator anim = myTarget.GetComponentInChildren<Animator>();
                if (anim != null && anim.isHuman)
                {
                    SubHeader("Dynamic Creation");
                    boneIndex = EditorGUILayout.Popup("Add to Bone", boneIndex, bones, GUILayout.ExpandWidth(true));
                    if (GUILayout.Button("Add"))
                    {
                        Transform target = anim.GetBoneTransform((HumanBodyBones)boneIndex);
                        if (target == null)
                        {
                            EditorUtility.DisplayDialog("Inventgory Cog", "The requested bone '" + bones[boneIndex] + "' could not be found on the selected rig.", "OK");
                        }
                        else
                        {
                            GameObject newDD = new GameObject();
                            newDD.name = "StorePoint_" + bones[boneIndex];
                            newDD.AddComponent<StorePoint>().pointId = bones[boneIndex] + " Store";
                            newDD.transform.SetParent(target);
                            newDD.transform.localPosition = Vector3.zero;
                            Selection.activeGameObject = newDD;
                        }
                    }
                }
            }

        }

        private void DrawSkills()
        {
            if (SectionToggle((int)displayFlags, (int)DisplayFlags.Skills, "Skill Slots", GetIcon("Skill", "Icons/skill")))
            {
                SimpleList("skillSlots");
            }
        }

        private void DrawStarting()
        {
            SimpleProperty("AllExisingPublicItems");

            bool hasFlag = (displayFlags & DisplayFlags.StartingItems) == DisplayFlags.StartingItems;

            bool resValue = hasFlag;
            SerializedProperty list = serializedObject.FindProperty("startingItems");

            // Top spacing
            GUILayout.Space(8);

            // Container start
            GUILayout.BeginHorizontal();

            // Expand collapse icon
            GUILayout.BeginVertical();
            Color res = GUI.color;
            GUILayout.Space(7);
            Texture2D texture = resValue ? ExpandedIcon : CollapsedIcon;
            GUI.color = EditorColor;
            GUILayout.Label(texture, GUILayout.Width(12));
            GUILayout.EndVertical();

            // Icon
            GUILayout.BeginVertical();
            GUILayout.Space(4);

            GUILayout.Label(GetIcon("Items", "Icons/loot_item"), GUILayout.Width(18), GUILayout.Height(18));
            GUILayout.EndVertical();
            GUI.color = res;

            // Title
            GUILayout.BeginVertical();
            GUILayout.Space(4);

            GUILayout.Label("Starting Items", Skin.GetStyle("SectionHeader"));
            GUILayout.EndVertical();

            // Drag and drop
            GUILayout.BeginVertical();
            GUILayout.Space(4);
            GUI.color = EditorColor;
            GUILayout.Label(GetIcon("Drop", "Icons/tock-drop"), GUILayout.ExpandHeight(false), GUILayout.ExpandWidth(false), GUILayout.Height(16), GUILayout.Width(18));
            GUI.color = res;
            GUILayout.EndVertical();

            GUILayout.FlexibleSpace();

            // Container End
            GUILayout.EndHorizontal();

            if (ProcessDragDrop(list, typeof(InventoryItem))) resValue = true;

            // Toggle
            if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseUp)
            {
                resValue = !resValue;
                Repaint();
            }

            GUILayout.Space(4);

            bool result = resValue;


            if (result != hasFlag)
            {
                displayFlags = result ? displayFlags | DisplayFlags.StartingItems : displayFlags & ~DisplayFlags.StartingItems;
                serializedObject.FindProperty("z_display_flags").intValue = (int)displayFlags;
            }

            if (hasFlag)
            {
                list = serializedObject.FindProperty("startingItems");
                int startSize = list.arraySize;
                itemScroll = SimpleList("startingItems", itemScroll, 120, 2);
                if (list.arraySize > startSize)
                {
                    SerializedProperty newItem = list.GetArrayElementAtIndex(list.arraySize - 1);
                    newItem.FindPropertyRelative("item").objectReferenceValue = null;
                    newItem.FindPropertyRelative("count").intValue = 1;
                }
            }
        }

        private void DrawUI()
        {
            if (SectionToggle((int)displayFlags, (int)DisplayFlags.UI, "UI & Themes", GetIcon("UI", "Icons/tock-ui")))
            {
                SimpleProperty("shareTag");
                SimpleProperty("headless", "UI-less Mode");
                if (!SimpleBool("headless"))
                {
                    if (db != null)
                    {
                        int selIndex = 0;
                        for (int i = 0; i < themeNames.Length; i++)
                        {
                            if (themeNames[i] == myTarget.defaultTheme)
                            {
                                selIndex = i;
                                break;
                            }
                        }

                        selIndex = EditorGUILayout.Popup(new GUIContent("Default Theme", "Theme to use by default when Inventory Cog loads"), selIndex, themeNames);
                        myTarget.defaultTheme = themeNames[selIndex];

                        if (prefabRoot != null && myTarget.directDBPrefab)
                        {
                            forceSave |= dbPrefabEditor.DrawThemeList();
                        }
                        else
                        {
                            forceSave |= dbEditor.DrawThemeList();
                        }
                    }
                    else
                    {
                        SimpleProperty("defaultTheme");
                    }
                }
            }
        }

        private bool ProcessDragDrop(SerializedProperty list, System.Type acceptedType)
        {
            bool resValue = false;

            var dragAreaGroup = GUILayoutUtility.GetLastRect();
            switch (Event.current.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!dragAreaGroup.Contains(Event.current.mousePosition))
                        break;
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    if (Event.current.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();

                        foreach (var dragged in DragAndDrop.objectReferences)
                        {
                            if (dragged.GetType() == acceptedType || dragged.GetType().BaseType == acceptedType
                                || dragged.GetType().GetNestedTypes().Contains(acceptedType)
                                || (dragged is GameObject && ((GameObject)dragged).GetComponentInChildren(acceptedType) != null))
                            {
                                list.arraySize++;
                                //list.GetArrayElementAtIndex(list.arraySize - 1).objectReferenceValue = dragged;

                                //ItemReference iRef = new ItemReference();
                                //iRef.item = (InventoryItem)dragged;
                                //iRef.count = 1;
                                //list.GetArrayElementAtIndex(list.arraySize - 1).objectReferenceValue = iRef;

                                list.GetArrayElementAtIndex(list.arraySize - 1).FindPropertyRelative("item").objectReferenceValue = dragged;
                                list.GetArrayElementAtIndex(list.arraySize - 1).FindPropertyRelative("count").intValue = 1;


                                resValue = true;
                            }
                        }
                    }
                    serializedObject.ApplyModifiedProperties();
                    Event.current.Use();
                    break;
            }

            return resValue;
        }

        private void UpdateTagsList()
        {
            List<InventoryItemUITag> foundTags = FindAssetsByType<InventoryItemUITag>();
            tags = foundTags.OrderBy(_ => _.name).ToList();
        }

        private void UpdateThemeList()
        {
            if (db.themes == null || db.themes.Count == 0)
            {
                themeNames = new string[] { "{None}" };
            }
            else
            {
                themeNames = new string[db.themes.Count];
                for (int i = 0; i < db.themes.Count; i++)
                {
                    if (db.themes[i] != null)
                    {
                        themeNames[i] = db.themes[i].displayName;
                    }
                }
            }
        }

        #endregion

    }
}