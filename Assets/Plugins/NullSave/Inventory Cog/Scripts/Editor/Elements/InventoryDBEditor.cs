using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace NullSave.TOCK.Inventory
{
    [CanEditMultipleObjects()]
    [CustomEditor(typeof(InventoryDB))]
    public class InventoryDBEditor : TOCKEditorV2
    {

        #region Variables

        private InventoryDB myTarget;
        internal ReorderableList categories, craftingCategories, items, recipes, themes;

        #endregion

        #region Unity Methods

        private void OnEnable()
        {
            if (target is InventoryDB)
            {
                myTarget = (InventoryDB)target;

                categories = new ReorderableList(serializedObject, serializedObject.FindProperty("categories"), true, true, true, true);
                categories.elementHeight = EditorGUIUtility.singleLineHeight + 2;
                categories.drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, "Item Categories"); };
                categories.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    var element = categories.serializedProperty.GetArrayElementAtIndex(index);
                    if (element.objectReferenceValue == null)
                    {
                        EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element, new GUIContent("{null}", null, string.Empty));
                    }
                    else
                    {
                        EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element, new GUIContent(((Category)element.objectReferenceValue).displayName, null, string.Empty));
                    }
                };

                items = new ReorderableList(serializedObject, serializedObject.FindProperty("availableItems"), true, true, true, true);
                items.elementHeight = EditorGUIUtility.singleLineHeight + 2;
                items.drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, "Items"); };
                items.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    var element = items.serializedProperty.GetArrayElementAtIndex(index);
                    if (element.objectReferenceValue == null)
                    {
                        EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element, new GUIContent("{null}", null, string.Empty));
                    }
                    else
                    {
                        EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element, new GUIContent(((InventoryItem)element.objectReferenceValue).DisplayName, null, string.Empty));
                    }
                };

                craftingCategories = new ReorderableList(serializedObject, serializedObject.FindProperty("craftingCategories"), true, true, true, true);
                craftingCategories.elementHeight = EditorGUIUtility.singleLineHeight + 2;
                craftingCategories.drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, "Crafting Categories"); };
                craftingCategories.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    var element = craftingCategories.serializedProperty.GetArrayElementAtIndex(index);
                    if (element.objectReferenceValue == null)
                    {
                        EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element, new GUIContent("{null}", null, string.Empty));
                    }
                    else
                    {
                        EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element, new GUIContent(((CraftingCategory)element.objectReferenceValue).displayName, null, string.Empty));
                    }
                };

                recipes = new ReorderableList(serializedObject, serializedObject.FindProperty("recipes"), true, true, true, true);
                recipes.elementHeight = EditorGUIUtility.singleLineHeight + 2;
                recipes.drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, "Recipes"); };
                recipes.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    var element = recipes.serializedProperty.GetArrayElementAtIndex(index);
                    if (element.objectReferenceValue == null)
                    {
                        EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element, new GUIContent("{null}", null, string.Empty));
                    }
                    else
                    {
                        EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element, new GUIContent(((CraftingRecipe)element.objectReferenceValue).displayName, null, string.Empty));
                    }
                };

                themes = new ReorderableList(serializedObject, serializedObject.FindProperty("themes"), true, true, true, true);
                themes.elementHeight = EditorGUIUtility.singleLineHeight + 2;
                themes.drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, "themes"); };
                themes.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    var element = themes.serializedProperty.GetArrayElementAtIndex(index);
                    if (element.objectReferenceValue == null)
                    {
                        EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element, new GUIContent("{null}", null, string.Empty));
                    }
                    else
                    {
                        EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element, new GUIContent(((InventoryTheme)element.objectReferenceValue).displayName, null, string.Empty));
                    }
                };

                // V3
                expandedThemes = new List<int>();
                themeEditors = new Dictionary<InventoryTheme, InventoryThemeEditor>();
                expandedCategories = new List<int>();
                categoryEditors = new Dictionary<Category, CategoryEditor>();
                expandedItems = new List<int>();
                itemEditors = new Dictionary<InventoryItem, InventoryItemEditor>();
                expandedCraftCats = new List<int>();
                craftCatEditors = new Dictionary<CraftingCategory, CraftingCategoryEditor>();
                expandedRecipes = new List<int>();
                recipeEditors = new Dictionary<CraftingRecipe, CraftingRecipeEditor>();

            }
        }

        public override void OnInspectorGUI()
        {
            MainContainerBegin("Inventory DB", "Icons/category");

            DrawThemes();
            DrawCategories();
            DrawItems();
            DrawCraftingCategories();
            DrawRecipes();

            MainContainerEnd();
        }

        #endregion

        #region Private Methods

        private void DrawCategories()
        {
            SectionHeader("Item Categories", "categories", typeof(Category));
            categories.DoLayoutList();
        }

        private void DrawItems()
        {
            SectionHeader("Items", "availableItems", typeof(InventoryItem));
            items.DoLayoutList();
        }

        private void DrawCraftingCategories()
        {
            SectionHeader("Crafting Categories", "craftingCategories", typeof(CraftingCategory));
            craftingCategories.DoLayoutList();
        }

        private void DrawRecipes()
        {
            SectionHeader("Recipes", "recipes", typeof(CraftingRecipe));
            recipes.DoLayoutList();
        }

        private void DrawThemes()
        {
            SectionHeader("Themes", "themes", typeof(InventoryTheme));
            themes.DoLayoutList();
        }

        private void ReloadFromProject()
        {
            if (GUILayout.Button("Refresh from Project"))
            {
                // Clear items
                myTarget.availableItems.Clear();
                myTarget.categories.Clear();
                myTarget.recipes.Clear();
                myTarget.craftingCategories.Clear();

                string[] result = AssetDatabase.FindAssets("t:InventoryItem");
                foreach (string item in result)
                {
                    myTarget.availableItems.Add(AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(item), typeof(InventoryItem)) as InventoryItem);
                }

                result = AssetDatabase.FindAssets("t:Category");
                foreach (string item in result)
                {
                    myTarget.categories.Add(AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(item), typeof(Category)) as Category);
                }

                result = AssetDatabase.FindAssets("t:CraftingCategory");
                foreach (string item in result)
                {
                    myTarget.craftingCategories.Add(AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(item), typeof(CraftingCategory)) as CraftingCategory);
                }

                result = AssetDatabase.FindAssets("t:CraftingRecipe");
                foreach (string item in result)
                {
                    myTarget.recipes.Add(AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(item), typeof(CraftingRecipe)) as CraftingRecipe);
                }
            }
        }

        #endregion

        #region Window Methods

        internal void AddCraftingCategory(CraftingCategory category)
        {
            SerializedProperty list = serializedObject.FindProperty("craftingCategories");
            list.arraySize++;
            list.GetArrayElementAtIndex(list.arraySize - 1).objectReferenceValue = category;
        }

        internal void AddItemCategory(Category category)
        {
            SerializedProperty list = serializedObject.FindProperty("categories");
            list.arraySize++;
            list.GetArrayElementAtIndex(list.arraySize - 1).objectReferenceValue = category;
        }

        internal void AddItem(InventoryItem item)
        {
            SerializedProperty list = serializedObject.FindProperty("availableItems");
            list.arraySize++;
            list.GetArrayElementAtIndex(list.arraySize - 1).objectReferenceValue = item;
        }

        internal void AddRecipe(CraftingRecipe recipe)
        {
            SerializedProperty list = serializedObject.FindProperty("recipes");
            list.arraySize++;
            list.GetArrayElementAtIndex(list.arraySize - 1).objectReferenceValue = recipe;
        }

        internal void AddTheme(InventoryTheme theme)
        {
            SerializedProperty list = serializedObject.FindProperty("themes");
            list.arraySize++;
            list.GetArrayElementAtIndex(list.arraySize - 1).objectReferenceValue = theme;
        }

        public int DrawItemCategories()
        {
            int result = -1;
            GUILayout.BeginVertical(GUILayout.Width(300));
            DrawCategories();

            if (categories.index > -1)
            {
                if (GUILayout.Button("Edit Selected"))
                {
                    result = categories.index;
                }
            }

            GUILayout.EndVertical();
            GUILayout.BeginHorizontal();
            GUILayout.Space(32);
            GUILayout.EndHorizontal();
            return result;
        }

        public int DrawItemList()
        {
            int result = -1;
            GUILayout.BeginVertical(GUILayout.Width(300));
            DrawItems();

            if (items.index > -1)
            {
                if (GUILayout.Button("Edit Selected"))
                {
                    result = items.index;
                }
            }

            GUILayout.EndVertical();
            GUILayout.BeginHorizontal();
            GUILayout.Space(32);
            GUILayout.EndHorizontal();
            return result;
        }

        public int DrawCraftCategories()
        {
            int result = -1;
            GUILayout.BeginVertical(GUILayout.Width(300));
            DrawCraftingCategories();

            if (craftingCategories.index > -1)
            {
                if (GUILayout.Button("Edit Selected"))
                {
                    result = craftingCategories.index;
                }
            }

            GUILayout.EndVertical();
            GUILayout.BeginHorizontal();
            GUILayout.Space(32);
            GUILayout.EndHorizontal();
            return result;
        }

        public int DrawCraftingRecipes()
        {
            int result = -1;
            GUILayout.BeginVertical(GUILayout.Width(300));
            DrawRecipes();

            if (recipes.index > -1)
            {
                if (GUILayout.Button("Edit Selected"))
                {
                    result = recipes.index;
                }
            }

            GUILayout.EndVertical();
            GUILayout.BeginHorizontal();
            GUILayout.Space(32);
            GUILayout.EndHorizontal();
            return result;
        }

        public int DrawThemesList()
        {
            int result = -1;
            GUILayout.BeginVertical(GUILayout.Width(300));
            DrawThemes();

            if (themes.index > -1)
            {
                if (GUILayout.Button("Edit Selected"))
                {
                    result = themes.index;
                }
            }

            GUILayout.EndVertical();
            GUILayout.BeginHorizontal();
            GUILayout.Space(32);
            GUILayout.EndHorizontal();
            return result;
        }

        #endregion

        #region UI3.0 Methods

        private float lastGoodWidth = 250;
        private List<int> expandedThemes, expandedCategories, expandedItems, expandedCraftCats, expandedRecipes;
        private Dictionary<InventoryTheme, InventoryThemeEditor> themeEditors;
        private Dictionary<Category, CategoryEditor> categoryEditors;
        private Dictionary<InventoryItem, InventoryItemEditor> itemEditors;
        private Dictionary<CraftingCategory, CraftingCategoryEditor> craftCatEditors;
        private Dictionary<CraftingRecipe, CraftingRecipeEditor> recipeEditors;

        private bool DrawCraftingCategory(SerializedProperty theme, int index, int length, float maxWidth)
        {
            if (theme == null) return true;

            CraftingCategory reference = (CraftingCategory)theme.objectReferenceValue;
            if (reference == null)
            {
                myTarget.craftingCategories.Remove(reference);
                return true;
            }

            Rect clickRect;

            GUILayout.Space(-4);

            GUILayout.BeginVertical(BoxBlue, GUILayout.ExpandWidth(true));
            GUILayout.BeginHorizontal();

            GUILayout.Label(reference.displayName, Skin.GetStyle("PanelText"), GUILayout.MaxWidth(maxWidth));
            GUILayout.FlexibleSpace();

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField(theme, new GUIContent(), GUILayout.MaxWidth(64));
            EditorGUI.EndDisabledGroup();

            bool delAtEnd, moveUp, moveDown;
            FoldoutNavigation(index > 0, out moveUp, index < length - 1, out moveDown, out delAtEnd);

            GUILayout.EndHorizontal();
            GUILayout.Space(4);
            GUILayout.EndVertical();
            clickRect = GUILayoutUtility.GetLastRect();

            if (expandedCraftCats.Contains(index))
            {
                GUILayout.Space(-6);
                GUILayout.BeginVertical(BoxWhite, GUILayout.ExpandWidth(true));

                if (!craftCatEditors.ContainsKey(reference))
                {
                    craftCatEditors.Add(reference, (CraftingCategoryEditor)Editor.CreateEditor(reference, typeof(CraftingCategoryEditor)));
                }
                craftCatEditors[reference].serializedObject.Update();
                craftCatEditors[reference].DrawInspector();
                craftCatEditors[reference].serializedObject.ApplyModifiedProperties();

                GUILayout.Space(4);
                GUILayout.EndVertical();
            }

            if (!delAtEnd && !moveDown && !moveUp && Event.current.type == EventType.MouseUp && Event.current.button == 0 && clickRect.Contains(Event.current.mousePosition))
            {
                if (expandedCraftCats.Contains(index))
                {
                    expandedCraftCats.Remove(index);
                }
                else
                {
                    expandedCraftCats.Add(index);
                }

                Repaint();
            }

            if (moveDown)
            {
                myTarget.craftingCategories.Remove(reference);
                myTarget.craftingCategories.Insert(index + 1, reference);

                for (int i = 0; i < expandedCraftCats.Count; i++)
                {
                    if (expandedCraftCats[i] == index + 1)
                    {
                        expandedCraftCats[i] = index;
                    }
                    else if (expandedCraftCats[i] == index)
                    {
                        expandedCraftCats[i] += 1;
                    }
                }

                return true;
            }

            if (moveUp)
            {
                myTarget.craftingCategories.Remove(reference);
                myTarget.craftingCategories.Insert(index - 1, reference);

                for (int i = 0; i < expandedCraftCats.Count; i++)
                {
                    if (expandedCraftCats[i] == index - 1)
                    {
                        expandedCraftCats[i] = index;
                    }
                    else if (expandedCraftCats[i] == index)
                    {
                        expandedCraftCats[i] -= 1;
                    }
                }

                return true;
            }

            if (delAtEnd)
            {
                myTarget.themes.RemoveAt(index);
                return true;
            }

            return false;
        }

        public bool DrawCraftingCategoryList()
        {
            bool majorChange = false;
            bool clearAtEnd = false;

            // Header
            bool createNew = false;
            Rect clickRect;
            Color restore = GUI.color;
            SerializedProperty list = serializedObject.FindProperty("craftingCategories");

            GUILayout.BeginVertical(BoxSub, GUILayout.ExpandWidth(true));
            GUILayout.BeginHorizontal();

            GUILayout.Label("Available Crafting Categories");

            GUI.color = Color.black;
            GUILayout.Label(GetIcon("Drop", "Icons/tock-drop"), GUILayout.ExpandHeight(false), GUILayout.ExpandWidth(false), GUILayout.Height(16), GUILayout.Width(18));
            GUI.color = restore;

            GUILayout.FlexibleSpace();

            GUI.color = Color.black;
            GUILayout.Label(GetIcon("Add", "Icons/tock-add"), GUILayout.ExpandHeight(false), GUILayout.ExpandWidth(false), GUILayout.Height(16), GUILayout.Width(18));
            clickRect = GUILayoutUtility.GetLastRect();
            if (Event.current.type == EventType.MouseUp && Event.current.button == 0 && clickRect.Contains(Event.current.mousePosition))
            {
                createNew = true;
            }

            GUILayout.Label(GetIcon("X", "Icons/tock-x"), GUILayout.ExpandHeight(false), GUILayout.ExpandWidth(false), GUILayout.Height(16), GUILayout.Width(18));
            clickRect = GUILayoutUtility.GetLastRect();
            if (Event.current.type == EventType.MouseUp && Event.current.button == 0 && clickRect.Contains(Event.current.mousePosition))
            {
                clearAtEnd = true;
            }
            GUI.color = restore;

            GUILayout.EndHorizontal();
            GUILayout.Space(4);
            GUILayout.EndVertical();

            if (HandleDragDrop(list, typeof(CraftingCategory)))
            {
                majorChange = true;
            }

            clickRect = GUILayoutUtility.GetLastRect();

            if (clickRect.width > 1)
            {
                lastGoodWidth = clickRect.width;
            }

            for (int i = 0; i < list.arraySize; i++)
            {
                if (DrawCraftingCategory(list.GetArrayElementAtIndex(i), i, list.arraySize, lastGoodWidth - 150))
                {
                    majorChange = true;
                }
            }

            if (createNew)
            {
                ScriptableObject newItem = CreateNew("Crafting Category", typeof(CraftingCategory));
                if(newItem != null)
                {
                    myTarget.craftingCategories.Add((CraftingCategory)newItem);
                    majorChange = true;
                }
            }

            if(clearAtEnd)
            {
                list.ClearArray();
                majorChange = true;
            }

            return majorChange;
        }

        private bool DrawItem(SerializedProperty item, int index, int length, float maxWidth)
        {
            if (item == null) return true;

            InventoryItem reference = (InventoryItem)item.objectReferenceValue;
            if (reference == null)
            {
                myTarget.availableItems.Remove(reference);
                return true;
            }

            Rect clickRect;

            GUILayout.Space(-4);

            GUILayout.BeginVertical(BoxBlue, GUILayout.ExpandWidth(true));
            GUILayout.BeginHorizontal();

            GUILayout.Label(reference.displayName, Skin.GetStyle("PanelText"), GUILayout.MaxWidth(maxWidth));
            GUILayout.FlexibleSpace();

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField(item, new GUIContent(), GUILayout.MaxWidth(64));
            EditorGUI.EndDisabledGroup();

            bool delAtEnd, moveUp, moveDown;
            FoldoutNavigation(index > 0, out moveUp, index < length - 1, out moveDown, out delAtEnd);

            GUILayout.EndHorizontal();
            GUILayout.Space(4);
            GUILayout.EndVertical();
            clickRect = GUILayoutUtility.GetLastRect();

            if (expandedItems.Contains(index))
            {
                GUILayout.Space(-6);
                GUILayout.BeginVertical(BoxWhite, GUILayout.ExpandWidth(true));

                if (!itemEditors.ContainsKey(reference))
                {
                    itemEditors.Add(reference, (InventoryItemEditor)Editor.CreateEditor(reference, typeof(InventoryItemEditor)));
                }
                itemEditors[reference].serializedObject.Update();
                itemEditors[reference].DrawInspector(myTarget, false, false, false, false);
                itemEditors[reference].serializedObject.ApplyModifiedProperties();

                GUILayout.Space(4);
                GUILayout.EndVertical();
            }

            if (!delAtEnd && !moveDown && !moveUp && Event.current.type == EventType.MouseUp && Event.current.button == 0 && clickRect.Contains(Event.current.mousePosition))
            {
                if (expandedItems.Contains(index))
                {
                    expandedItems.Remove(index);
                }
                else
                {
                    expandedItems.Add(index);
                }

                Repaint();
            }

            if (moveDown)
            {
                myTarget.availableItems.Remove(reference);
                myTarget.availableItems.Insert(index + 1, reference);

                for (int i = 0; i < expandedItems.Count; i++)
                {
                    if (expandedItems[i] == index + 1)
                    {
                        expandedItems[i] = index;
                    }
                    else if (expandedItems[i] == index)
                    {
                        expandedItems[i] += 1;
                    }
                }

                return true;
            }

            if (moveUp)
            {
                myTarget.availableItems.Remove(reference);
                myTarget.availableItems.Insert(index - 1, reference);

                for (int i = 0; i < expandedItems.Count; i++)
                {
                    if (expandedItems[i] == index - 1)
                    {
                        expandedItems[i] = index;
                    }
                    else if (expandedItems[i] == index)
                    {
                        expandedItems[i] -= 1;
                    }
                }

                return true;
            }

            if (delAtEnd)
            {
                myTarget.availableItems.RemoveAt(index);
                return true;
            }

            return false;
        }

        private bool DrawItemCategory(SerializedProperty category, int index, int length, float maxWidth)
        {
            if (category == null) return true;
            Category reference = (Category)category.objectReferenceValue;
            if (reference == null)
            {
                myTarget.categories.Remove(reference);
                return true;
            }

            bool delAtEnd = false;
            Rect clickRect;

            GUILayout.Space(-4);

            GUILayout.BeginVertical(BoxBlue, GUILayout.ExpandWidth(true));
            GUILayout.BeginHorizontal();

            GUILayout.Label(reference.displayName, Skin.GetStyle("PanelText"), GUILayout.MaxWidth(maxWidth));
            GUILayout.FlexibleSpace();

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField(category, new GUIContent(), GUILayout.MaxWidth(64));
            EditorGUI.EndDisabledGroup();

            FoldoutTrashOnly(out delAtEnd);

            GUILayout.EndHorizontal();
            GUILayout.Space(4);
            GUILayout.EndVertical();
            clickRect = GUILayoutUtility.GetLastRect();

            if (expandedCategories.Contains(index))
            {
                GUILayout.Space(-6);
                GUILayout.BeginVertical(BoxWhite, GUILayout.ExpandWidth(true));

                if (!categoryEditors.ContainsKey(reference))
                {
                    categoryEditors.Add(reference, (CategoryEditor)Editor.CreateEditor(reference, typeof(CategoryEditor)));
                }
                categoryEditors[reference].serializedObject.Update();
                categoryEditors[reference].DrawInspector();
                categoryEditors[reference].serializedObject.ApplyModifiedProperties();

                GUILayout.Space(4);
                GUILayout.EndVertical();
            }

            if (!delAtEnd && Event.current.type == EventType.MouseUp && Event.current.button == 0 && clickRect.Contains(Event.current.mousePosition))
            {
                if (expandedCategories.Contains(index))
                {
                    expandedCategories.Remove(index);
                }
                else
                {
                    expandedCategories.Add(index);
                }

                Repaint();
            }

            if (delAtEnd)
            {
                myTarget.categories.RemoveAt(index);
                return true;
            }

            return false;
        }

        public bool DrawItemCategoryList()
        {
            bool majorChange = false;
            bool clearAtEnd = false;

            // Header
            bool createNew = false;
            Rect clickRect;
            Color restore = GUI.color;
            SerializedProperty list = serializedObject.FindProperty("categories");

            GUILayout.BeginVertical(BoxSub, GUILayout.ExpandWidth(true));
            GUILayout.BeginHorizontal();

            GUILayout.Label("Available Categories");

            GUI.color = Color.black;
            GUILayout.Label(GetIcon("Drop", "Icons/tock-drop"), GUILayout.ExpandHeight(false), GUILayout.ExpandWidth(false), GUILayout.Height(16), GUILayout.Width(18));
            GUI.color = restore;

            GUILayout.FlexibleSpace();

            GUI.color = Color.black;
            GUILayout.Label(GetIcon("Add", "Icons/tock-add"), GUILayout.ExpandHeight(false), GUILayout.ExpandWidth(false), GUILayout.Height(16), GUILayout.Width(18));
            clickRect = GUILayoutUtility.GetLastRect();
            if (Event.current.type == EventType.MouseUp && Event.current.button == 0 && clickRect.Contains(Event.current.mousePosition))
            {
                createNew = true;
            }

            GUILayout.Label(GetIcon("X", "Icons/tock-x"), GUILayout.ExpandHeight(false), GUILayout.ExpandWidth(false), GUILayout.Height(16), GUILayout.Width(18));
            clickRect = GUILayoutUtility.GetLastRect();
            if (Event.current.type == EventType.MouseUp && Event.current.button == 0 && clickRect.Contains(Event.current.mousePosition))
            {
                clearAtEnd = true;
            }
            GUI.color = restore;

            GUILayout.EndHorizontal();
            GUILayout.Space(4);
            GUILayout.EndVertical();
            clickRect = GUILayoutUtility.GetLastRect();

            if (HandleDragDrop(list, typeof(Category)))
            {
                majorChange = true;
            }
            if (clickRect.width > 1)
            {
                lastGoodWidth = clickRect.width;
            }


            if (createNew)
            {
                ScriptableObject newItem = CreateNew("Item Category", typeof(Category));
                if (newItem != null)
                {
                    myTarget.categories.Add((Category)newItem);
                    majorChange = true;
                }
            }

            for (int i = 0; i < list.arraySize; i++)
            {
                if (DrawItemCategory(list.GetArrayElementAtIndex(i), i, list.arraySize, lastGoodWidth - 150))
                {
                    majorChange = true;
                }
            }

            if(clearAtEnd)
            {
                list.ClearArray();
                majorChange = true;
            }

            return majorChange;
        }

        public bool DrawItemsList(GameObject prefabRoot)
        {
            bool majorChange = false;
            bool clearAtEnd = false;

            // Header
            bool createNew = false;
            Rect clickRect;
            Color restore = GUI.color;
            SerializedProperty list = serializedObject.FindProperty("availableItems");

            GUILayout.BeginVertical(BoxSub, GUILayout.ExpandWidth(true));
            GUILayout.BeginHorizontal();

            GUILayout.Label("Available Items");

            GUI.color = Color.black;
            GUILayout.Label(GetIcon("Drop", "Icons/tock-drop"), GUILayout.ExpandHeight(false), GUILayout.ExpandWidth(false), GUILayout.Height(16), GUILayout.Width(18));
            GUI.color = restore;

            GUILayout.FlexibleSpace();

            GUI.color = Color.black;
            GUILayout.Label(GetIcon("Wizard", "Icons/tock-wizard"), GUILayout.ExpandHeight(false), GUILayout.ExpandWidth(false), GUILayout.Height(16), GUILayout.Width(18));
            clickRect = GUILayoutUtility.GetLastRect();
            if (Event.current.type == EventType.MouseUp && Event.current.button == 0 && clickRect.Contains(Event.current.mousePosition))
            {
                InventoryItemWizard.ShowWindow(myTarget, this, prefabRoot);
            }

            GUI.color = Color.black;
            GUILayout.Label(GetIcon("Add", "Icons/tock-add"), GUILayout.ExpandHeight(false), GUILayout.ExpandWidth(false), GUILayout.Height(16), GUILayout.Width(18));
            clickRect = GUILayoutUtility.GetLastRect();
            if (Event.current.type == EventType.MouseUp && Event.current.button == 0 && clickRect.Contains(Event.current.mousePosition))
            {
                createNew = true;
            }

            GUILayout.Label(GetIcon("X", "Icons/tock-x"), GUILayout.ExpandHeight(false), GUILayout.ExpandWidth(false), GUILayout.Height(16), GUILayout.Width(18));
            clickRect = GUILayoutUtility.GetLastRect();
            if (Event.current.type == EventType.MouseUp && Event.current.button == 0 && clickRect.Contains(Event.current.mousePosition))
            {
                clearAtEnd = true;
            }
            GUI.color = restore;

            GUILayout.EndHorizontal();
            GUILayout.Space(4);
            GUILayout.EndVertical();

            if (HandleDragDrop(list, typeof(InventoryItem)))
            {
                majorChange = true;
            }

            clickRect = GUILayoutUtility.GetLastRect();

            if (clickRect.width > 1)
            {
                lastGoodWidth = clickRect.width;
            }

            for (int i = 0; i < list.arraySize; i++)
            {
                if (DrawItem(list.GetArrayElementAtIndex(i), i, list.arraySize, lastGoodWidth - 150))
                {
                    majorChange = true;
                }
            }

            if (createNew)
            {
                ScriptableObject newItem = CreateNew("Item", typeof(InventoryItem));
                if (newItem != null)
                {
                    myTarget.availableItems.Add((InventoryItem)newItem);
                    majorChange = true;
                }
            }

            if (clearAtEnd)
            {
                list.ClearArray();
                majorChange = true;
            }

            return majorChange;
        }

        private bool DrawRecipe(SerializedProperty theme, int index, int length, float maxWidth)
        {
            if (theme == null) return true;

            CraftingRecipe reference = (CraftingRecipe)theme.objectReferenceValue;
            if (reference == null)
            {
                myTarget.recipes.Remove(reference);
                return true;
            }

            Rect clickRect;

            GUILayout.Space(-4);

            GUILayout.BeginVertical(BoxBlue, GUILayout.ExpandWidth(true));
            GUILayout.BeginHorizontal();

            GUILayout.Label(reference.displayName, Skin.GetStyle("PanelText"), GUILayout.MaxWidth(maxWidth));
            GUILayout.FlexibleSpace();

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField(theme, new GUIContent(), GUILayout.MaxWidth(64));
            EditorGUI.EndDisabledGroup();

            bool delAtEnd, moveUp, moveDown;
            FoldoutNavigation(index > 0, out moveUp, index < length - 1, out moveDown, out delAtEnd);

            GUILayout.EndHorizontal();
            GUILayout.Space(4);
            GUILayout.EndVertical();
            clickRect = GUILayoutUtility.GetLastRect();

            if (expandedRecipes.Contains(index))
            {
                GUILayout.Space(-6);
                GUILayout.BeginVertical(BoxWhite, GUILayout.ExpandWidth(true));

                if (!recipeEditors.ContainsKey(reference))
                {
                    recipeEditors.Add(reference, (CraftingRecipeEditor)Editor.CreateEditor(reference, typeof(CraftingRecipeEditor)));
                }
                recipeEditors[reference].serializedObject.Update();
                recipeEditors[reference].DrawInspector(myTarget);
                recipeEditors[reference].serializedObject.ApplyModifiedProperties();

                GUILayout.Space(4);
                GUILayout.EndVertical();
            }

            if (!delAtEnd && !moveDown && !moveUp && Event.current.type == EventType.MouseUp && Event.current.button == 0 && clickRect.Contains(Event.current.mousePosition))
            {
                if (expandedRecipes.Contains(index))
                {
                    expandedRecipes.Remove(index);
                }
                else
                {
                    expandedRecipes.Add(index);
                }

                Repaint();
            }

            if (moveDown)
            {
                myTarget.recipes.Remove(reference);
                myTarget.recipes.Insert(index + 1, reference);

                for (int i = 0; i < expandedRecipes.Count; i++)
                {
                    if (expandedRecipes[i] == index + 1)
                    {
                        expandedRecipes[i] = index;
                    }
                    else if (expandedRecipes[i] == index)
                    {
                        expandedRecipes[i] += 1;
                    }
                }

                return true;
            }

            if (moveUp)
            {
                myTarget.recipes.Remove(reference);
                myTarget.recipes.Insert(index - 1, reference);

                for (int i = 0; i < expandedRecipes.Count; i++)
                {
                    if (expandedRecipes[i] == index - 1)
                    {
                        expandedRecipes[i] = index;
                    }
                    else if (expandedRecipes[i] == index)
                    {
                        expandedRecipes[i] -= 1;
                    }
                }

                return true;
            }

            if (delAtEnd)
            {
                myTarget.recipes.RemoveAt(index);
                return true;
            }

            return false;
        }

        public bool DrawRecipesList()
        {
            bool majorChange = false;
            bool clearAtEnd = false;

            // Header
            bool createNew = false;
            Rect clickRect;
            Color restore = GUI.color;
            SerializedProperty list = serializedObject.FindProperty("recipes");

            GUILayout.BeginVertical(BoxSub, GUILayout.ExpandWidth(true));
            GUILayout.BeginHorizontal();

            GUILayout.Label("Available Recipes");

            GUI.color = Color.black;
            GUILayout.Label(GetIcon("Drop", "Icons/tock-drop"), GUILayout.ExpandHeight(false), GUILayout.ExpandWidth(false), GUILayout.Height(16), GUILayout.Width(18));
            GUI.color = restore;

            GUILayout.FlexibleSpace();

            GUI.color = Color.black;
            GUILayout.Label(GetIcon("Add", "Icons/tock-add"), GUILayout.ExpandHeight(false), GUILayout.ExpandWidth(false), GUILayout.Height(16), GUILayout.Width(18));
            clickRect = GUILayoutUtility.GetLastRect();
            if (Event.current.type == EventType.MouseUp && Event.current.button == 0 && clickRect.Contains(Event.current.mousePosition))
            {
                createNew = true;
            }

            GUILayout.Label(GetIcon("X", "Icons/tock-x"), GUILayout.ExpandHeight(false), GUILayout.ExpandWidth(false), GUILayout.Height(16), GUILayout.Width(18));
            clickRect = GUILayoutUtility.GetLastRect();
            if (Event.current.type == EventType.MouseUp && Event.current.button == 0 && clickRect.Contains(Event.current.mousePosition))
            {
                clearAtEnd = true;
            }
            GUI.color = restore;

            GUILayout.EndHorizontal();
            GUILayout.Space(4);
            GUILayout.EndVertical();

            if (HandleDragDrop(list, typeof(CraftingCategory)))
            {
                majorChange = true;
            }

            clickRect = GUILayoutUtility.GetLastRect();

            if (clickRect.width > 1)
            {
                lastGoodWidth = clickRect.width;
            }

            for (int i = 0; i < list.arraySize; i++)
            {
                if (DrawRecipe(list.GetArrayElementAtIndex(i), i, list.arraySize, lastGoodWidth - 150))
                {
                    majorChange = true;
                }
            }

            if (createNew)
            {
                ScriptableObject newItem = CreateNew("Recipie", typeof(CraftingRecipe));
                if (newItem != null)
                {
                    myTarget.recipes.Add((CraftingRecipe)newItem);
                    majorChange = true;
                }
            }

            if(clearAtEnd)
            {
                list.ClearArray();
                majorChange = true;
            }

            return majorChange;
        }

        private bool DrawTheme(SerializedProperty theme, int index, int length, float maxWidth)
        {
            if (theme == null) return true;

            InventoryTheme reference = (InventoryTheme)theme.objectReferenceValue;
            if (reference == null)
            {
                myTarget.themes.Remove(reference);
                return true;
            }

            Rect clickRect;

            GUILayout.Space(-4);

            GUILayout.BeginVertical(BoxBlue, GUILayout.ExpandWidth(true));
            GUILayout.BeginHorizontal();

            GUILayout.Label(reference.displayName, Skin.GetStyle("PanelText"), GUILayout.MaxWidth(maxWidth));
            GUILayout.FlexibleSpace();

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField(theme, new GUIContent(), GUILayout.MaxWidth(64));
            EditorGUI.EndDisabledGroup();

            bool delAtEnd, moveUp, moveDown;
            FoldoutNavigation(index > 0, out moveUp, index < length - 1, out moveDown, out delAtEnd);

            GUILayout.EndHorizontal();
            GUILayout.Space(4);
            GUILayout.EndVertical();
            clickRect = GUILayoutUtility.GetLastRect();

            if(expandedThemes.Contains(index))
            {
                GUILayout.Space(-6);
                GUILayout.BeginVertical(BoxWhite, GUILayout.ExpandWidth(true));

                if (!themeEditors.ContainsKey(reference))
                {
                    themeEditors.Add(reference, (InventoryThemeEditor)Editor.CreateEditor(reference, typeof(InventoryThemeEditor)));
                }
                themeEditors[reference].serializedObject.Update();
                themeEditors[reference].DrawInspector();
                themeEditors[reference].serializedObject.ApplyModifiedProperties();

                GUILayout.Space(4);
                GUILayout.EndVertical();
            }

            if (!delAtEnd && !moveDown && !moveUp && Event.current.type == EventType.MouseUp && Event.current.button == 0 && clickRect.Contains(Event.current.mousePosition))
            {
                if (expandedThemes.Contains(index))
                {
                    expandedThemes.Remove(index);
                }
                else
                {
                    expandedThemes.Add(index);
                }

                Repaint();
            }

            if (moveDown)
            {
                myTarget.themes.Remove(reference);
                myTarget.themes.Insert(index + 1, reference);

                for (int i = 0; i < expandedThemes.Count; i++)
                {
                    if (expandedThemes[i] == index + 1)
                    {
                        expandedThemes[i] = index;
                    }
                    else if (expandedThemes[i] == index)
                    {
                        expandedThemes[i] += 1;
                    }
                }

                return true;
            }

            if (moveUp)
            {
                myTarget.themes.Remove(reference);
                myTarget.themes.Insert(index - 1, reference);

                for (int i = 0; i < expandedThemes.Count; i++)
                {
                    if (expandedThemes[i] == index - 1)
                    {
                        expandedThemes[i] = index;
                    }
                    else if (expandedThemes[i] == index)
                    {
                        expandedThemes[i] -= 1;
                    }
                }

                return true;
            }

            if(delAtEnd)
            {
                myTarget.themes.RemoveAt(index);
                return true;
            }

            return false;
        }

        public bool DrawThemeList()
        {
            bool majorChange = false;
            bool clearAtEnd = false;

            // Header
            bool createNew = false;
            Rect clickRect;
            Color restore = GUI.color;
            SerializedProperty list = serializedObject.FindProperty("themes");

            GUILayout.BeginVertical(BoxSub, GUILayout.ExpandWidth(true));
            GUILayout.BeginHorizontal();

            GUILayout.Label("Available Themes");

            GUI.color = Color.black;
            GUILayout.Label(GetIcon("Drop", "Icons/tock-drop"), GUILayout.ExpandHeight(false), GUILayout.ExpandWidth(false), GUILayout.Height(16), GUILayout.Width(18));
            GUI.color = restore;

            GUILayout.FlexibleSpace();

            GUI.color = Color.black;
            GUILayout.Label(GetIcon("Add", "Icons/tock-add"), GUILayout.ExpandHeight(false), GUILayout.ExpandWidth(false), GUILayout.Height(16), GUILayout.Width(18));
            clickRect = GUILayoutUtility.GetLastRect();
            if (Event.current.type == EventType.MouseUp && Event.current.button == 0 && clickRect.Contains(Event.current.mousePosition))
            {
                createNew = true;
            }

            GUILayout.Label(GetIcon("X", "Icons/tock-x"), GUILayout.ExpandHeight(false), GUILayout.ExpandWidth(false), GUILayout.Height(16), GUILayout.Width(18));
            clickRect = GUILayoutUtility.GetLastRect();
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && clickRect.Contains(Event.current.mousePosition))
            {
                clearAtEnd = true;
            }
            GUI.color = restore;

            GUILayout.EndHorizontal();
            GUILayout.Space(4);
            GUILayout.EndVertical();

            if(HandleDragDrop(list, typeof(InventoryTheme)))
            {
                majorChange = true;
            }

            clickRect = GUILayoutUtility.GetLastRect();

            if (clickRect.width > 1)
            {
                lastGoodWidth = clickRect.width;
            }

            for (int i = 0; i < list.arraySize; i++)
            {
                if(DrawTheme(list.GetArrayElementAtIndex(i), i, list.arraySize, lastGoodWidth - 150))
                {
                    majorChange = true;
                }
            }

            if(createNew)
            {
                ScriptableObject newItem = CreateNew("Theme", typeof(InventoryTheme));
                if (newItem != null)
                {
                    myTarget.themes.Add((InventoryTheme)newItem);
                    majorChange = true;
                }
            }

            if(clearAtEnd)
            {
                list.ClearArray();
                majorChange = true;
            }

            return majorChange;
        }

        #endregion

    }
}
