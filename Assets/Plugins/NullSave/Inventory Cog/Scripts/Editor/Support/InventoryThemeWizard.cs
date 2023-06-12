using NullSave.TOCK.Stats;
using System;
using System.IO;
using System.Reflection;
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace NullSave.TOCK.Inventory
{
    public class InventoryThemeWizard : TOCKEditorWindow
    {

        #region Constants

        private const string TITLE = "Theme Wizard";
        private static GUIStyle container;

        #endregion

        #region Variables

        private static Texture2D windowIcon;
        private int step;

        // Construction variables
        private InventoryTheme theme;
        private InventoryThemeEditor themeEditor;

        // Inventory Window
        private int windowMode;
        private bool includeCrafting;
        private int invListType;
        private bool invDragDrop;
        private bool invCatList;
        private bool invOverrideSpawn;
        private string invOverrideTag;
        private int invOpenMode;
        private string invOpenButton;
        private KeyCode invOpenKey;
        private int invCloseMode;
        private string invCloseButton;
        private KeyCode invCloseKey;
        private bool useItemManager;
        private bool invContextBtns;
        private bool invItemPreview;
        private bool invItemDetails;
        private bool invCharPreview;

        // Item Manager
        private int immCloseMode;
        private string immCloseButton;
        private KeyCode immCloseKey;
        private bool immRepairCosts;

        // Item Container Menu
        private int icmListType;
        private bool icmDragDrop;
        private bool icmCatList;
        private bool icmContextBtns;
        private bool icmItemPreview;
        private bool icmItemDetails;
        private bool icmCharPreview;
        private bool icmPlShow;
        private int icmPlListType;
        private bool icmPlDragDrop;
        private bool icmPlCatList;
        private bool icmPlContextBtns;

        // Crafting
        private int cmCraftType;
        private bool cmShowQueue;
        private bool cmShowContext;
        private bool cmRecipeDetails;
        private bool cmCraftDetailMenu;
        private bool cmDestoryNo;
        private bool cmAlwaysSingle;

        // Merchant Menu
        private int mmListType;
        private bool mmDragDrop;
        private bool mmCatList;
        private bool mmItemPreview;
        private bool mmItemDetails;
        private bool mmTradeDetail;
        private bool mmPlShow;
        private int mmPlListType;
        private bool mmPlDragDrop;
        private bool mmPlCatList;

        // Environment Container Menu
        private int ecmListType;
        private bool ecmDragDrop;
        private bool ecmCatList;
        private bool ecmContextBtns;
        private bool ecmItemPreview;
        private bool ecmItemDetails;
        private bool ecmPlShow;
        private int ecmPlListType;
        private bool ecmPlDragDrop;
        private bool ecmPlCatList;
        private bool ecmPlContextBtns;

        // Enum lists
        private string[] windowModes = new string[] { "Inventory Only", "Multi-Screen" };
        private string[] listTypes = new string[] { "Scroll Grid", "Paged Grid", "Scroll List" };
        private string[] navModes = new string[] { "Manual", "By Button", "By Key" };
        private string[] craftTypes = new string[] { "Normal", "Blind" };
        
        #endregion

        #region Properties

        private static GUIStyle Container
        {
            get
            {
                if (container == null)
                {
                    container = new GUIStyle(EditorStyles.label)
                    {
                        margin = new RectOffset(10, 10, 10, 10)
                    };
                }

                return container;
            }
        }

        private static Texture2D WindowIcon
        {
            get
            {
                if (windowIcon == null)
                {
                    windowIcon = (Texture2D)Resources.Load("Icons/tock-ui", typeof(Texture2D));
                }

                return windowIcon;
            }
        }

        #endregion

        #region Unity Methods

        private void OnGUI()
        {
            if (theme == null)
            {
                theme = ScriptableObject.CreateInstance<InventoryTheme>();
                theme.displayName = "New Theme";
            }

            if(themeEditor == null)
            {
                themeEditor = (InventoryThemeEditor)Editor.CreateEditor(theme);
            }

            switch (step)
            {
                case 0:
                    DrawStep0();
                    break;
                case 1:
                    DrawStep1();
                    break;
                case 2:
                    DrawStep2();
                    break;
                case 3:
                    DrawStep3();
                    break;
                case 4:
                    DrawStep4();
                    break;
                case 5:
                    DrawStep5();
                    break;
                case 6:
                    DrawStep6();
                    break;
                case 7:
                    DrawStep7();
                    break;
                case 8:
                    DrawStep8();
                    break;
            }
        }

        private void OnInspectorUpdate()
        {
            Repaint();
        }

        #endregion

        #region Public Methods

        [MenuItem("Tools/NullSave/Inventory Theme Wizard", false, 2)]
        public static void ShowWindow()
        {
            InventoryThemeWizard w = GetWindow<InventoryThemeWizard>(TITLE);
            w.titleContent = new GUIContent(TITLE, WindowIcon);
            w.minSize = new Vector2(800, 600);
            w.maxSize = new Vector2(800, 600);
            float scale = 1;
            if (Screen.dpi >= 144)
            {
                scale = 0.5f;
            }
            else if (Screen.dpi >= 120)
            {
                scale = 0.75f;
            }
            w.position = new Rect((Screen.currentResolution.width * scale - 800 * scale) / 2, (Screen.currentResolution.height* scale - 600 * scale) / 2, 800, 600);
            w.wantsMouseMove = true;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Basics
        /// </summary>
        private void DrawStep0()
        {
            GUILayout.BeginVertical(Container, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
            MainContainerBegin("Theme Wizard (Basics)", "Icons/tock-ui");
            GUILayout.Space(16);
            GUILayout.Label("Welcome to the Theme Wizard! This window will collect some information from you in order to build a template for your new theme.\r\n" +
                "Folders and prefabs will be automatically generated for you at the end of this wizard, at which point you can add your new theme to\r\n" +
                "the Inventory DB.");

            themeEditor.DrawStep0();

            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Next", GUILayout.Height(32), GUILayout.Width(120)))
            {
                step = 1;
            }
            GUILayout.EndHorizontal();

            MainContainerEnd(false);
            GUILayout.EndVertical();
        }

        /// <summary>
        /// Prompts & Rarity
        /// </summary>
        private void DrawStep1()
        {
            GUILayout.BeginVertical(Container, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
            MainContainerBegin("Theme Wizard (Prompts & Rarity)", "Icons/tock-ui");
            GUILayout.Space(16);

            themeEditor.DrawStep1();

            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Next", GUILayout.Height(32), GUILayout.Width(120)))
            {
                step = 2;
            }
            GUILayout.EndHorizontal();

            MainContainerEnd(false);
            GUILayout.EndVertical();
        }

        /// <summary>
        /// Inventory Menu
        /// </summary>
        private void DrawStep2()
        {
            GUILayout.BeginVertical(Container, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
            MainContainerBegin("Theme Wizard (Inventory Menu)", "Icons/tock-ui");
            GUILayout.Space(16);
            GUILayout.Label("The Inventory Menu is your main window to interact with inventory.\r\n" +
                "This can act on its own or have other options embedded in the same window.");

            SectionHeader("UI");
            //windowMode = EditorGUILayout.Popup("Layout", windowMode, windowModes);
            //if(windowMode == 1)
            //{
            //    includeCrafting = EditorGUILayout.Toggle("Include Crafting", includeCrafting);
            //}
            invListType = EditorGUILayout.Popup("List Type", invListType, listTypes);
            invCatList = EditorGUILayout.Toggle("Include Category List", invCatList);

            SectionHeader("Menu Behaviour");
            invOverrideSpawn = EditorGUILayout.Toggle("Override Spawn", invOverrideSpawn);
            if(invOverrideSpawn)
            {
                invOverrideTag = InlineProperty(invOverrideTag, "Override Tag");
            }

            invOpenMode = EditorGUILayout.Popup("Open Mode", invOpenMode, navModes);
            switch(invOpenMode)
            {
                case 1: // By button
                    invOpenButton = InlineProperty(invOpenButton, "Button");
                    break;
                case 2: // By key
                    invOpenKey = (KeyCode)EditorGUILayout.EnumPopup("Run Key", invOpenKey);
                    break;
            }

            invCloseMode = EditorGUILayout.Popup("Close Mode", invCloseMode, navModes);
            switch (invCloseMode)
            {
                case 1: // By button
                    invCloseButton = InlineProperty(invCloseButton, "Button");
                    break;
                case 2: // By key
                    invCloseKey = (KeyCode)EditorGUILayout.EnumPopup("Run Key", invCloseKey);
                    break;
            }

            SectionHeader("List Behaviour");
            invDragDrop = EditorGUILayout.Toggle("Enable Drag & Drop", invDragDrop);
            useItemManager = EditorGUILayout.Toggle("Use Item Manager", useItemManager);
            invContextBtns = EditorGUILayout.Toggle("Show Context Buttons", invContextBtns);
            invItemDetails = EditorGUILayout.Toggle("Item Details", invItemDetails);
            invItemPreview = EditorGUILayout.Toggle("Item Preview", invItemPreview);
            invCharPreview = EditorGUILayout.Toggle("Character Preview", invCharPreview);

            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Next", GUILayout.Height(32), GUILayout.Width(120)))
            {
                step = 3;
            }
            GUILayout.EndHorizontal();

            MainContainerEnd(false);
            GUILayout.EndVertical();
        }

        /// <summary>
        /// Item Manager
        /// </summary>
        private void DrawStep3()
        {
            GUILayout.BeginVertical(Container, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
            MainContainerBegin("Theme Wizard (Item Manager)", "Icons/tock-ui");
            GUILayout.Space(16);
            GUILayout.Label("The Item Manager is mostly used for mobile/casual games but can be handy for desktop/console as well. This UI allows you to \r\n"
                + "manage a specifc inventory item on its own.");


            SectionHeader("Menu Behaviour");
            immCloseMode = EditorGUILayout.Popup("Close Mode", immCloseMode, navModes);
            switch (immCloseMode)
            {
                case 1: // By button
                    immCloseButton = InlineProperty(immCloseButton, "Button");
                    break;
                case 2: // By key
                    immCloseKey = (KeyCode)EditorGUILayout.EnumPopup("Run Key", immCloseKey);
                    break;
            }

            SectionHeader("UI");
            immRepairCosts = EditorGUILayout.Toggle("Repair Costs", immRepairCosts);

            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Next", GUILayout.Height(32), GUILayout.Width(120)))
            {
                step = 4;
            }
            GUILayout.EndHorizontal();

            MainContainerEnd(false);
            GUILayout.EndVertical();
        }

        /// <summary>
        /// Item Container Menu
        /// </summary>
        private void DrawStep4()
        {
            GUILayout.BeginVertical(Container, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
            MainContainerBegin("Theme Wizard (Item Container Menu)", "Icons/tock-ui");
            GUILayout.Space(16);
            GUILayout.Label("The Item Container Menu controls containers inside of your inventory.");

            SectionHeader("Container UI");
            icmListType = EditorGUILayout.Popup("List Type", icmListType, listTypes);
            icmCatList = EditorGUILayout.Toggle("Include Category List", icmCatList);
            icmDragDrop = EditorGUILayout.Toggle("Enable Drag & Drop", icmDragDrop);
            icmContextBtns = EditorGUILayout.Toggle("Show Context Buttons", icmContextBtns);
            icmItemDetails = EditorGUILayout.Toggle("Item Details", icmItemDetails);
            icmItemPreview = EditorGUILayout.Toggle("Item Preview", icmItemPreview);
            icmCharPreview = EditorGUILayout.Toggle("Character Preview", icmCharPreview);

            SectionHeader("Player UI");
            icmPlShow = EditorGUILayout.Toggle("Show Player Inventory", icmPlShow);
            if (icmPlShow)
            {
                icmPlListType = EditorGUILayout.Popup("List Type", icmPlListType, listTypes);
                icmPlCatList = EditorGUILayout.Toggle("Include Category List", icmPlCatList);
                icmPlDragDrop = EditorGUILayout.Toggle("Enable Drag & Drop", icmPlDragDrop);
                icmPlContextBtns = EditorGUILayout.Toggle("Show Context Buttons", icmPlContextBtns);
            }

            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Next", GUILayout.Height(32), GUILayout.Width(120)))
            {
                step = 5;
            }
            GUILayout.EndHorizontal();

            MainContainerEnd(false);
            GUILayout.EndVertical();
        }

        /// <summary>
        /// Crafting
        /// </summary>
        private void DrawStep5()
        {
            GUILayout.BeginVertical(Container, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
            MainContainerBegin("Theme Wizard (Crafting)", "Icons/tock-ui");
            GUILayout.Space(16);
            GUILayout.Label("The Crafting Menu allows you to craft new items.");

            SectionHeader("UI");
            cmCraftType = EditorGUILayout.Popup("Crafting Type", cmCraftType, craftTypes);
            if(cmCraftType == 1)
            {
                cmDestoryNo = EditorGUILayout.Toggle("Destroy If No Match", cmDestoryNo);
                cmAlwaysSingle = EditorGUILayout.Toggle("Always Use Single", cmAlwaysSingle);
            }
            else
            {
                cmCraftDetailMenu = EditorGUILayout.Toggle("Use Craft Detail Menu", cmCraftDetailMenu);
            }
            cmShowQueue = EditorGUILayout.Toggle("Show Crafting Queue", cmShowQueue);
            cmShowContext = EditorGUILayout.Toggle("Show Context Buttons", cmShowContext);
            cmRecipeDetails = EditorGUILayout.Toggle("Show Recipe Details", cmRecipeDetails);

            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Next", GUILayout.Height(32), GUILayout.Width(120)))
            {
                step = 6;
            }
            GUILayout.EndHorizontal();

            MainContainerEnd(false);
            GUILayout.EndVertical();
        }

        /// <summary>
        /// Merchant Menu
        /// </summary>
        private void DrawStep6()
        {
            GUILayout.BeginVertical(Container, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
            MainContainerBegin("Theme Wizard (Merchant Menu)", "Icons/tock-ui");
            GUILayout.Space(16);
            GUILayout.Label("The Merchant Menu controls buying/selling items.");

            SectionHeader("Merchant UI");
            mmListType = EditorGUILayout.Popup("List Type", mmListType, listTypes);
            mmCatList = EditorGUILayout.Toggle("Include Category List", mmCatList);
            mmDragDrop = EditorGUILayout.Toggle("Enable Drag & Drop", mmDragDrop);
            mmItemDetails = EditorGUILayout.Toggle("Item Details", mmItemDetails);
            mmItemPreview = EditorGUILayout.Toggle("Item Preview", mmItemPreview);
            mmTradeDetail = EditorGUILayout.Toggle("Trade Detail", mmTradeDetail);

            SectionHeader("Player UI");
            mmPlShow = EditorGUILayout.Toggle("Show Player Inventory", mmPlShow);
            if (mmPlShow)
            {
                mmPlListType = EditorGUILayout.Popup("List Type", mmPlListType, listTypes);
                mmPlCatList = EditorGUILayout.Toggle("Include Category List", mmPlCatList);
                mmPlDragDrop = EditorGUILayout.Toggle("Enable Drag & Drop", mmPlDragDrop);
            }

            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Next", GUILayout.Height(32), GUILayout.Width(120)))
            {
                step = 7;
            }
            GUILayout.EndHorizontal();

            MainContainerEnd(false);
            GUILayout.EndVertical();
        }

        /// <summary>
        /// Environment Container Menu
        /// </summary>
        private void DrawStep7()
        {
            GUILayout.BeginVertical(Container, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
            MainContainerBegin("Theme Wizard (Container Menu)", "Icons/tock-ui");
            GUILayout.Space(16);
            GUILayout.Label("The Container Menu controls containers found in the environment.");

            SectionHeader("Container UI");
            ecmListType = EditorGUILayout.Popup("List Type", ecmListType, listTypes);
            ecmCatList = EditorGUILayout.Toggle("Include Category List", ecmCatList);
            ecmDragDrop = EditorGUILayout.Toggle("Enable Drag & Drop", ecmDragDrop);
            ecmContextBtns = EditorGUILayout.Toggle("Show Context Buttons", ecmContextBtns);
            ecmItemDetails = EditorGUILayout.Toggle("Item Details", ecmItemDetails);
            ecmItemPreview = EditorGUILayout.Toggle("Item Preview", ecmItemPreview);

            SectionHeader("Player UI");
            ecmPlShow = EditorGUILayout.Toggle("Show Player Inventory", ecmPlShow);
            if (ecmPlShow)
            {
                ecmPlListType = EditorGUILayout.Popup("List Type", ecmPlListType, listTypes);
                ecmPlCatList = EditorGUILayout.Toggle("Include Category List", ecmPlCatList);
                ecmPlDragDrop = EditorGUILayout.Toggle("Enable Drag & Drop", ecmPlDragDrop);
                ecmPlContextBtns = EditorGUILayout.Toggle("Show Context Buttons", ecmPlContextBtns);
            }

            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Finalize", GUILayout.Height(32), GUILayout.Width(120)))
            {
                CreateTheme();
            }
            GUILayout.EndHorizontal();

            MainContainerEnd(false);
            GUILayout.EndVertical();
        }

        private void DrawStep8()
        {
            GUILayout.BeginVertical(Container, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
            MainContainerBegin("Theme Wizard (Finalize)", "Icons/tock-ui");
            GUILayout.Space(16);
            GUILayout.Label("The wizard is now creating your theme.");

            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();

            if (GUILayout.Button("Create", GUILayout.Height(32), GUILayout.Width(120)))
            {
                CreateTheme();
            }

            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();


            MainContainerEnd(false);
            GUILayout.EndVertical();
        }

        #endregion

        #region Creation Methods

        private void CreateComponents(string basePath, string themeName)
        {
            string componentPath = Path.Combine(basePath, "Components");
            string listPath = Path.Combine(basePath, "Lists");
            string promptPath = Path.Combine(basePath, "Prompts");
            string renderTexPath = Path.Combine(basePath, "Render Textures");
            string windowsPath = Path.Combine(basePath, "Windows");

            // Create Temporary Canvas
            GameObject canvas = new GameObject("__THEME WIZARD CANVAS__");
            canvas.AddComponent<Canvas>();
            canvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;

            // Create Button
            GameObject go = new GameObject("(" + themeName + ") Action Button");
            go.transform.SetParent(canvas.transform);
            SetFullSize(go);
            Image img = go.AddComponent<Image>();
            img.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            img.type = Image.Type.Sliced;
            Button btn = go.AddComponent<Button>();
            GameObject subGO = new GameObject("Text");
            subGO.transform.SetParent(go.transform);
            TextMeshProUGUI tmpro = CreateTextMeshPro(subGO, "Text");
            SetFullSize(subGO);
            GameObject prefabButton = PrefabUtility.SaveAsPrefabAsset(go, Path.Combine(componentPath, go.name + ".prefab"));
            DestroyImmediate(go);
            AssetDatabase.Refresh();

            // Create Page Indicator
            go = new GameObject("(" + themeName + ") Page Indicator");
            RectTransform rt = go.AddComponent<RectTransform>();
            go.transform.SetParent(canvas.transform);
            rt.anchorMax = rt.anchorMin = Vector2.zero;
            rt.sizeDelta = new Vector2(6, 6);
            img = go.AddComponent<Image>();
            img.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            img.preserveAspect = true;
            go.AddComponent<PageIndicatorUI>();
            GameObject prefabPageIndicator = PrefabUtility.SaveAsPrefabAsset(go, Path.Combine(componentPath, go.name + ".prefab"));
            DestroyImmediate(go);

            // Create Category UI
            go = new GameObject("(" + themeName + ") Category UI");
            go.transform.SetParent(canvas.transform);
            SetFullSize(go);
            CategoryUI categoryUI = go.AddComponent<CategoryUI>();
            GameObject imgGO = new GameObject("Icon");
            imgGO.transform.SetParent(go.transform);
            img = imgGO.AddComponent<Image>();
            img.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            img.preserveAspect = true;
            rt = imgGO.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0, 0);
            rt.anchorMax = new Vector2(1, 1);
            rt.offsetMin = new Vector2(0, 30);
            rt.offsetMax = new Vector2(0, 0);
            categoryUI.categoryImage = img;
            GameObject txtGO = new GameObject("Category Name");
            txtGO.transform.SetParent(go.transform);
            tmpro = CreateTextMeshPro(txtGO, "Display Name");
            rt = txtGO.GetComponent<RectTransform>();
            rt.pivot = new Vector2(0, 0);
            rt.anchorMin = new Vector2(0, 0);
            rt.anchorMax = new Vector2(1, 0);
            rt.sizeDelta = new Vector2(0, 30);
            categoryUI.categoryName = tmpro;
            imgGO = new GameObject("Selection Indicator");
            imgGO.transform.SetParent(go.transform);
            SetFullSize(imgGO);
            img = imgGO.AddComponent<Image>();
            img.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            img.preserveAspect = true;
            imgGO.SetActive(false);
            categoryUI.selectionIndicator = imgGO;
            GameObject prefabCategorUI = PrefabUtility.SaveAsPrefabAsset(go, Path.Combine(componentPath, go.name + ".prefab"));
            DestroyImmediate(go);

            // Create Category List
            go = new GameObject("(" + themeName + ") Category List");
            go.transform.SetParent(canvas.transform);
            SetFullSize(go);
            CategoryList categoryList = go.AddComponent<CategoryList>();
            categoryList.allowClickSelect = true;
            categoryList.categoryPrefab = prefabCategorUI.GetComponent<CategoryUI>();
            categoryList.prefabContainer = go.transform;
            HorizontalLayoutGroup hlg = go.AddComponent<HorizontalLayoutGroup>();
            hlg.childForceExpandHeight = true;
            hlg.childForceExpandWidth = false;
            hlg.childControlHeight = true;
            hlg.childControlWidth = false;
            GameObject prefabCategoryList = PrefabUtility.SaveAsPrefabAsset(go, Path.Combine(listPath, go.name + ".prefab"));
            DestroyImmediate(go);

            // Create Category UI (Paged)
            go = new GameObject("(" + themeName + ") Category UI Paged");
            go.transform.SetParent(canvas.transform);
            SetFullSize(go);
            categoryUI = go.AddComponent<CategoryUI>();
            GameObject pic = CreateHorizontalContainer("Page Indicators", go.transform, 2);
            rt = pic.GetComponent<RectTransform>();
            rt.pivot = new Vector2(0, 1);
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(1, 1);
            rt.sizeDelta = new Vector2(0, 6);
            imgGO = new GameObject("Icon");
            imgGO.transform.SetParent(go.transform);
            img = imgGO.AddComponent<Image>();
            img.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            img.preserveAspect = true;
            rt = imgGO.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0, 0);
            rt.anchorMax = new Vector2(1, 1);
            rt.offsetMin = new Vector2(0, 30);
            rt.offsetMax = new Vector2(0, -6);
            categoryUI.categoryImage = img;
            txtGO = new GameObject("Category Name");
            txtGO.transform.SetParent(go.transform);
            tmpro = CreateTextMeshPro(txtGO, "Display Name");
            rt.pivot = new Vector2(0, 0);
            rt.anchorMin = new Vector2(0, 0);
            rt.anchorMax = new Vector2(1, 0);
            rt.sizeDelta = new Vector2(0, 30);
            categoryUI.categoryName = tmpro;
            categoryUI.indicatorParent = pic.transform;
            imgGO = new GameObject("Selection Indicator");
            imgGO.transform.SetParent(go.transform);
            SetFullSize(imgGO);
            img = imgGO.AddComponent<Image>();
            img.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            img.preserveAspect = true;
            imgGO.SetActive(false);
            categoryUI.selectionIndicator = imgGO;
            categoryUI.pageIndicator = prefabPageIndicator.GetComponent<PageIndicatorUI>();
            GameObject prefabCategorUIPaged = PrefabUtility.SaveAsPrefabAsset(go, Path.Combine(componentPath, go.name + ".prefab"));
            DestroyImmediate(go);

            // Create Category List (Paged)
            go = new GameObject("(" + themeName + ") Category List Paged");
            go.transform.SetParent(canvas.transform);
            SetFullSize(go);
            categoryList = go.AddComponent<CategoryList>();
            categoryList.allowClickSelect = true;
            categoryList.categoryPrefab = prefabCategorUIPaged.GetComponent<CategoryUI>();
            categoryList.prefabContainer = go.transform;
            hlg = go.AddComponent<HorizontalLayoutGroup>();
            hlg.childForceExpandHeight = true;
            hlg.childForceExpandWidth = true;
            hlg.childControlHeight = true;
            hlg.childControlWidth = true;
            GameObject prefabCategoryListPaged = PrefabUtility.SaveAsPrefabAsset(go, Path.Combine(listPath, go.name + ".prefab"));
            DestroyImmediate(go);

            // Create Attachment Slot Indicator
            go = new GameObject("(" + themeName + ") Attachment Slot Indicator");
            go.transform.SetParent(canvas.transform);
            SetFullSize(go);
            AttachmentSlotUI attachmentSlotUI = go.AddComponent<AttachmentSlotUI>();
            img = go.AddComponent<Image>();
            img.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            img.preserveAspect = true;
            imgGO = new GameObject("Equipped Indicator");
            imgGO.transform.SetParent(go.transform);
            SetFullSize(imgGO);
            img = imgGO.AddComponent<Image>();
            img.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            img.preserveAspect = true;
            imgGO.SetActive(false);
            attachmentSlotUI.equipedIcon = imgGO.GetComponent<Image>();
            GameObject prefabAttachSlotInd = PrefabUtility.SaveAsPrefabAsset(go, Path.Combine(componentPath, go.name + ".prefab"));
            DestroyImmediate(go);

            // Create Attachment Slot UI
            go = new GameObject("(" + themeName + ") Attachment Slot UI");
            go.transform.SetParent(canvas.transform);
            SetFullSize(go);
            attachmentSlotUI = go.AddComponent<AttachmentSlotUI>();
            imgGO = new GameObject("Slot Icon");
            imgGO.transform.SetParent(go.transform);
            img = imgGO.AddComponent<Image>();
            img.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            img.preserveAspect = true;
            attachmentSlotUI.slotIcon = img;
            SetFullSize(imgGO);
            imgGO = new GameObject("Icon");
            imgGO.transform.SetParent(go.transform);
            img = imgGO.AddComponent<Image>();
            img.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            img.preserveAspect = true;
            rt = imgGO.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0, 0);
            rt.anchorMax = new Vector2(1, 1);
            rt.offsetMin = new Vector2(0, 30);
            rt.offsetMax = new Vector2(0, 0);
            attachmentSlotUI.icon = img;
            txtGO = new GameObject("Display Name");
            txtGO.transform.SetParent(go.transform);
            tmpro = CreateTextMeshPro(txtGO, "Display Name");
            rt = tmpro.GetComponent<RectTransform>();
            rt.pivot = new Vector2(0, 0);
            rt.anchorMin = new Vector2(0, 0);
            rt.anchorMax = new Vector2(1, 0);
            rt.offsetMax = rt.offsetMin = Vector2.zero;
            rt.sizeDelta = new Vector2(rt.sizeDelta.x, 30);
            attachmentSlotUI.displayName = tmpro;
            imgGO = new GameObject("Equipped Icon");
            imgGO.transform.SetParent(go.transform);
            img = imgGO.AddComponent<Image>();
            img.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            img.preserveAspect = true;
            imgGO.SetActive(false);
            attachmentSlotUI.equipedIcon = img;
            imgGO = new GameObject("Selected Icon");
            imgGO.transform.SetParent(go.transform);
            img = imgGO.AddComponent<Image>();
            img.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            img.preserveAspect = true;
            imgGO.SetActive(false);
            attachmentSlotUI.selectionIndicator = imgGO;
            GameObject prefabAttachSlotUI = PrefabUtility.SaveAsPrefabAsset(go, Path.Combine(componentPath, go.name + ".prefab"));
            DestroyImmediate(go);

            // Create Item Tag UI
            go = new GameObject("(" + themeName + ") Item Tag UI");
            go.transform.SetParent(canvas.transform);
            SetFullSize(go);
            ItemTagUI itemTag = go.AddComponent<ItemTagUI>();
            imgGO = new GameObject("Tag Icon");
            imgGO.transform.SetParent(go.transform);
            SetFullSize(imgGO).offsetMin = new Vector2(0, 20);
            img = imgGO.AddComponent<Image>();
            img.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            img.preserveAspect = true;
            itemTag.icon = img;
            txtGO = new GameObject("Tag Text");
            txtGO.transform.SetParent(go.transform);
            tmpro = CreateTextMeshPro(txtGO, "Tag");
            rt = CreateOrGetRectTransform(txtGO);
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = new Vector2(1, 0);
            rt.pivot = new Vector2(0.5f, 0);
            rt.sizeDelta = new Vector2(0, 20);
            itemTag.displayText = tmpro;
            GameObject prefabItemTagUI = PrefabUtility.SaveAsPrefabAsset(go, Path.Combine(componentPath, go.name + ".prefab"));
            DestroyImmediate(go);

            // Create Item UI (Grid)
            go = new GameObject("(" + themeName + ") Item UI (Grid)");
            go.transform.SetParent(canvas.transform);
            SetFullSize(go);
            ItemUI itemUI = go.AddComponent<ItemUI>();
            subGO = new GameObject("Has Item Background");
            subGO.transform.SetParent(go.transform);
            SetFullSize(subGO);
            img = subGO.AddComponent<Image>();
            img.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
            img.type = Image.Type.Sliced;
            img.fillCenter = true;
            itemUI.bkgItem = img;
            subGO = new GameObject("No Item Background");
            subGO.transform.SetParent(go.transform);
            SetFullSize(subGO);
            img = subGO.AddComponent<Image>();
            img.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
            img.color = new Color(1, 1, 1, 0.2f);
            img.type = Image.Type.Sliced;
            img.fillCenter = true;
            subGO.SetActive(false);
            itemUI.bkgNoItem = img;
            itemUI.enableDragDrop = true;
            subGO = new GameObject("Equipable Indicator");
            subGO.transform.SetParent(go.transform);
            SetFullSize(subGO);
            img = subGO.AddComponent<Image>();
            img.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
            img.color = new Color(0, 1, 0, 0.25f);
            img.type = Image.Type.Sliced;
            img.fillCenter = true;
            subGO.SetActive(false);
            itemUI.equipableIndicator = subGO;
            subGO = new GameObject("Equipped Indicator");
            subGO.transform.SetParent(go.transform);
            SetFullSize(subGO);
            img = subGO.AddComponent<Image>();
            img.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
            img.color = new Color(0, 1, 0, 0.5f);
            img.type = Image.Type.Sliced;
            img.fillCenter = true;
            subGO.SetActive(false);
            itemUI.equippedIndicator = subGO;
            subGO = new GameObject("Item Icon");
            subGO.transform.SetParent(go.transform);
            rt = SetFullSize(subGO);
            img = subGO.AddComponent<Image>();
            img.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            img.preserveAspect = true;
            rt.offsetMin = new Vector2(10, 10);
            rt.offsetMax = new Vector2(-10, -10);
            itemUI.itemImage = img;
            txtGO = new GameObject("Item Name");
            txtGO.transform.SetParent(go.transform);
            tmpro = CreateTextMeshPro(txtGO, "Item Name");
            rt = CreateOrGetRectTransform(txtGO);
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(1, 1);
            rt.pivot = new Vector2(0.5f, 1);
            rt.sizeDelta = new Vector2(0, 20);
            itemUI.displayName = tmpro;
            txtGO = new GameObject("Item Description");
            txtGO.transform.SetParent(go.transform);
            tmpro = CreateTextMeshPro(txtGO, "Description");
            rt = CreateOrGetRectTransform(txtGO);
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(1, 1);
            rt.pivot = new Vector2(0.5f, 1);
            rt.sizeDelta = new Vector2(0, 20);
            rt.anchoredPosition = new Vector2(0, -20);
            itemUI.description = tmpro;
            itemUI.tagPrefab = prefabItemTagUI.GetComponent<ItemTagUI>();
            txtGO = new GameObject("Item Weight");
            txtGO.transform.SetParent(go.transform);
            tmpro = CreateTextMeshPro(txtGO, "0lb");
            rt = CreateOrGetRectTransform(txtGO);
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = new Vector2(0.33f, 0);
            rt.pivot = Vector2.zero;
            rt.sizeDelta = new Vector2(0, 20);
            itemUI.weight = tmpro;
            txtGO = new GameObject("Item Value");
            txtGO.transform.SetParent(go.transform);
            tmpro = CreateTextMeshPro(txtGO, "$0");
            rt = CreateOrGetRectTransform(txtGO);
            rt.anchorMin = new Vector2(0.33f, 0);
            rt.anchorMax = new Vector2(0.64f, 0);
            rt.pivot = Vector2.zero;
            rt.sizeDelta = new Vector2(0, 20);
            itemUI.value = tmpro;
            subGO = new GameObject("Count Container");
            subGO.transform.SetParent(go.transform);
            rt = CreateOrGetRectTransform(subGO);
            rt.anchorMin = new Vector2(0.64f, 0);
            rt.anchorMax = new Vector2(1, 0);
            rt.pivot = Vector2.zero;
            rt.sizeDelta = new Vector2(0, 20);
            img = subGO.AddComponent<Image>();
            img.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            img.type = Image.Type.Sliced;
            img.fillCenter = true;
            txtGO = new GameObject("Item Count");
            txtGO.transform.SetParent(subGO.transform);
            tmpro = CreateTextMeshPro(txtGO, "0");
            SetFullSize(txtGO);
            itemUI.count = tmpro;
            itemUI.countPrefix = string.Empty;
            itemUI.hideIfCountSub2 = subGO;
            subGO = new GameObject("Locked Indicator");
            subGO.transform.SetParent(go.transform);
            SetFullSize(subGO);
            img = subGO.AddComponent<Image>();
            img.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
            img.color = new Color(1, 0, 0, 0.5f);
            img.type = Image.Type.Sliced;
            img.fillCenter = true;
            subGO.SetActive(false);
            itemUI.lockedIndicator = img;
            subGO = new GameObject("Rarity Color Indicator");
            subGO.transform.SetParent(go.transform);
            rt = CreateOrGetRectTransform(subGO);
            rt.pivot = rt.anchorMin = rt.anchorMax = new Vector2(1, 1);
            rt.anchoredPosition = new Vector2(-5, -5);
            rt.sizeDelta = new Vector2(5, 5);
            img = subGO.AddComponent<Image>();
            img.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            img.preserveAspect = true;
            img.color = Color.yellow;
            RarityColorIndicator rarityColor = subGO.AddComponent<RarityColorIndicator>();
            itemUI.rarityColorIndicator = rarityColor;
            subGO = new GameObject("Rarity Slider");
            subGO.transform.SetParent(go.transform);
            Slider raritySlider = CreateProgressbar(subGO);
            rt = CreateOrGetRectTransform(subGO);
            rt.anchorMin = new Vector2(0, 0);
            rt.anchorMax = new Vector2(0.5f, 0);
            rt.pivot = Vector2.zero;
            rt.sizeDelta = new Vector2(0, 8);
            rt.anchoredPosition = new Vector2(0, 20);
            itemUI.raritySlider = raritySlider;
            subGO = new GameObject("Condition Slider");
            subGO.transform.SetParent(go.transform);
            Slider conditionSlider = CreateProgressbar(subGO);
            rt = CreateOrGetRectTransform(subGO);
            rt.anchorMin = new Vector2(0.5f, 0);
            rt.anchorMax = new Vector2(1, 0);
            rt.pivot = Vector2.zero;
            rt.sizeDelta = new Vector2(0, 8);
            rt.anchoredPosition = new Vector2(0, 20);
            itemUI.conditionSlider = conditionSlider;
            itemUI.slotPrefab = prefabAttachSlotUI.GetComponent<AttachmentSlotUI>();
            subGO = new GameObject("Attachment Slot Container");
            subGO.transform.SetParent(go.transform);
            rt = CreateOrGetRectTransform(subGO);
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = new Vector2(0.5f, 1);
            rt.pivot = new Vector2(0, 0.5f);
            rt.offsetMin = new Vector2(0, 30);
            rt.offsetMax = new Vector2(0, -40);
            subGO.AddComponent<VerticalLayoutGroup>();
            itemUI.slotContainer = subGO.transform;
            subGO = new GameObject("Tag Container");
            subGO.transform.SetParent(go.transform);
            rt = CreateOrGetRectTransform(subGO);
            rt.anchorMin = new Vector2(0.5f, 0);
            rt.anchorMax = new Vector2(1, 1);
            rt.pivot = new Vector2(1, 0.5f);
            rt.offsetMin = new Vector2(0, 30);
            rt.offsetMax = new Vector2(0, -40);
            subGO.AddComponent<VerticalLayoutGroup>();
            itemUI.tagContainer = subGO.transform;
            subGO = new GameObject("Selection Indicator");
            subGO.transform.SetParent(go.transform);
            SetFullSize(subGO);
            img = subGO.AddComponent<Image>();
            img.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
            img.color = new Color(0, 0, 1, 0.25f);
            img.type = Image.Type.Sliced;
            img.fillCenter = true;
            subGO.SetActive(false);
            itemUI.selectedIndicator = subGO;
            GameObject prefabItemUIGrid = PrefabUtility.SaveAsPrefabAsset(go, Path.Combine(componentPath, go.name + ".prefab"));
            DestroyImmediate(go);

            // Create Inventory Scroll Grid
            go = new GameObject("(" + themeName + ") Item Scroll Grid");
            go.transform.SetParent(canvas.transform);
            SetFullSize(go);
            ScrollRect scrollRect = CreateScrollRect(go);
            GameObject content = scrollRect.content.gameObject;
            GridLayoutGroup glg = content.AddComponent<GridLayoutGroup>();
            glg.cellSize = new Vector2(80, 80);
            glg.spacing = new Vector2(8, 8);
            InventoryItemScrollGrid itemScrollGrid = content.AddComponent<InventoryItemScrollGrid>();
            itemScrollGrid.itemUIPrefab = prefabItemUIGrid.GetComponent<ItemUI>();
            GameObject prefabItemScrollGrid = PrefabUtility.SaveAsPrefabAsset(go, Path.Combine(listPath, go.name + ".prefab"));
            DestroyImmediate(go);

            // Create Inventory Paged Grid
            go = new GameObject("(" + themeName + ") Item Paged Grid");
            go.transform.SetParent(canvas.transform);
            SetFullSize(go);
            scrollRect = CreateScrollRect(go);
            content = scrollRect.content.gameObject;
            glg = content.AddComponent<GridLayoutGroup>();
            glg.cellSize = new Vector2(80, 80);
            glg.spacing = new Vector2(8, 8);
            InventoryItemGrid itemGrid = content.AddComponent<InventoryItemGrid>();
            itemGrid.itemUIPrefab = prefabItemUIGrid.GetComponent<ItemUI>();
            GameObject prefabItemPagedGrid = PrefabUtility.SaveAsPrefabAsset(go, Path.Combine(listPath, go.name + ".prefab"));
            DestroyImmediate(go);

            // Create General UI
            go = new GameObject("(" + themeName + ") General UI");
            go.transform.SetParent(canvas.transform);
            SetFullSize(go);
            go.AddComponent<GeneralUI>();
            GameObject prefabGeneralUI = PrefabUtility.SaveAsPrefabAsset(go, Path.Combine(componentPath, go.name + ".prefab"));
            DestroyImmediate(go);
            themeEditor.serializedObject.FindProperty("generalUI").objectReferenceValue = prefabGeneralUI.GetComponent<GeneralUI>();

            // Create Item UI (List)
            go = new GameObject("(" + themeName + ") Item UI (List)");
            go.transform.SetParent(canvas.transform);
            SetFullSize(go);
            itemUI = go.AddComponent<ItemUI>();
            subGO = new GameObject("Has Item Background");
            subGO.transform.SetParent(go.transform);
            SetFullSize(subGO);
            img = subGO.AddComponent<Image>();
            img.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
            img.type = Image.Type.Sliced;
            img.fillCenter = true;
            itemUI.bkgItem = img;
            subGO = new GameObject("No Item Background");
            subGO.transform.SetParent(go.transform);
            SetFullSize(subGO);
            img = subGO.AddComponent<Image>();
            img.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
            img.color = new Color(1, 1, 1, 0.2f);
            img.type = Image.Type.Sliced;
            img.fillCenter = true;
            subGO.SetActive(false);
            itemUI.bkgNoItem = img;
            itemUI.enableDragDrop = true;
            subGO = new GameObject("Equipable Indicator");
            subGO.transform.SetParent(go.transform);
            SetFullSize(subGO);
            img = subGO.AddComponent<Image>();
            img.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
            img.color = new Color(0, 1, 0, 0.25f);
            img.type = Image.Type.Sliced;
            img.fillCenter = true;
            subGO.SetActive(false);
            itemUI.equipableIndicator = subGO;
            subGO = new GameObject("Equipped Indicator");
            subGO.transform.SetParent(go.transform);
            SetFullSize(subGO);
            img = subGO.AddComponent<Image>();
            img.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
            img.color = new Color(0, 1, 0, 0.5f);
            img.type = Image.Type.Sliced;
            img.fillCenter = true;
            subGO.SetActive(false);
            itemUI.equippedIndicator = subGO;
            subGO = new GameObject("Item Icon");
            subGO.transform.SetParent(go.transform);
            rt = SetFullSize(subGO);
            img = subGO.AddComponent<Image>();
            img.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            img.preserveAspect = true;
            rt.offsetMin = new Vector2(10, 10);
            rt.offsetMax = new Vector2(-10, -10);
            itemUI.itemImage = img;
            txtGO = new GameObject("Item Name");
            txtGO.transform.SetParent(go.transform);
            tmpro = CreateTextMeshPro(txtGO, "Item Name", true, TextAlignmentOptions.MidlineLeft);
            rt = CreateOrGetRectTransform(txtGO);
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(1, 1);
            rt.pivot = new Vector2(0.5f, 1);
            rt.sizeDelta = new Vector2(0, 20);
            itemUI.displayName = tmpro;
            txtGO = new GameObject("Item Description");
            txtGO.transform.SetParent(go.transform);
            tmpro = CreateTextMeshPro(txtGO, "Description", true, TextAlignmentOptions.MidlineLeft);
            rt = CreateOrGetRectTransform(txtGO);
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(1, 1);
            rt.pivot = new Vector2(0.5f, 1);
            rt.sizeDelta = new Vector2(0, 20);
            rt.anchoredPosition = new Vector2(0, -20);
            itemUI.description = tmpro;
            itemUI.tagPrefab = prefabItemTagUI.GetComponent<ItemTagUI>();
            txtGO = new GameObject("Item Weight");
            txtGO.transform.SetParent(go.transform);
            tmpro = CreateTextMeshPro(txtGO, "0lb");
            rt = CreateOrGetRectTransform(txtGO);
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = new Vector2(0.33f, 0);
            rt.pivot = Vector2.zero;
            rt.sizeDelta = new Vector2(0, 20);
            itemUI.weight = tmpro;
            txtGO = new GameObject("Item Value");
            txtGO.transform.SetParent(go.transform);
            tmpro = CreateTextMeshPro(txtGO, "$0");
            rt = CreateOrGetRectTransform(txtGO);
            rt.anchorMin = new Vector2(0.33f, 0);
            rt.anchorMax = new Vector2(0.64f, 0);
            rt.pivot = Vector2.zero;
            rt.sizeDelta = new Vector2(0, 20);
            itemUI.value = tmpro;
            subGO = new GameObject("Count Container");
            subGO.transform.SetParent(go.transform);
            rt = CreateOrGetRectTransform(subGO);
            rt.anchorMin = new Vector2(0.64f, 0);
            rt.anchorMax = new Vector2(1, 0);
            rt.pivot = Vector2.zero;
            rt.sizeDelta = new Vector2(0, 20);
            img = subGO.AddComponent<Image>();
            img.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            img.type = Image.Type.Sliced;
            img.fillCenter = true;
            txtGO = new GameObject("Item Count");
            txtGO.transform.SetParent(subGO.transform);
            tmpro = CreateTextMeshPro(txtGO, "0");
            SetFullSize(txtGO);
            itemUI.count = tmpro;
            itemUI.countPrefix = string.Empty;
            itemUI.hideIfCountSub2 = subGO;
            subGO = new GameObject("Locked Indicator");
            subGO.transform.SetParent(go.transform);
            SetFullSize(subGO);
            img = subGO.AddComponent<Image>();
            img.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
            img.color = new Color(1, 0, 0, 0.5f);
            img.type = Image.Type.Sliced;
            img.fillCenter = true;
            subGO.SetActive(false);
            itemUI.lockedIndicator = img;
            subGO = new GameObject("Rarity Color Indicator");
            subGO.transform.SetParent(go.transform);
            rt = CreateOrGetRectTransform(subGO);
            rt.pivot = rt.anchorMin = rt.anchorMax = new Vector2(1, 1);
            rt.anchoredPosition = new Vector2(-5, -5);
            rt.sizeDelta = new Vector2(5, 5);
            img = subGO.AddComponent<Image>();
            img.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            img.preserveAspect = true;
            img.color = Color.yellow;
            rarityColor = subGO.AddComponent<RarityColorIndicator>();
            itemUI.rarityColorIndicator = rarityColor;
            subGO = new GameObject("Rarity Slider");
            subGO.transform.SetParent(go.transform);
            raritySlider = CreateProgressbar(subGO);
            rt = CreateOrGetRectTransform(subGO);
            rt.anchorMin = new Vector2(0, 0);
            rt.anchorMax = new Vector2(0.5f, 0);
            rt.pivot = Vector2.zero;
            rt.sizeDelta = new Vector2(0, 8);
            rt.anchoredPosition = new Vector2(0, 20);
            itemUI.raritySlider = raritySlider;
            subGO = new GameObject("Condition Slider");
            subGO.transform.SetParent(go.transform);
            conditionSlider = CreateProgressbar(subGO);
            rt = CreateOrGetRectTransform(subGO);
            rt.anchorMin = new Vector2(0.5f, 0);
            rt.anchorMax = new Vector2(1, 0);
            rt.pivot = Vector2.zero;
            rt.sizeDelta = new Vector2(0, 8);
            rt.anchoredPosition = new Vector2(0, 20);
            itemUI.conditionSlider = conditionSlider;
            itemUI.slotPrefab = prefabAttachSlotUI.GetComponent<AttachmentSlotUI>();
            subGO = new GameObject("Attachment Slot Container");
            subGO.transform.SetParent(go.transform);
            rt = CreateOrGetRectTransform(subGO);
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = new Vector2(0.5f, 1);
            rt.pivot = new Vector2(0, 0.5f);
            rt.offsetMin = new Vector2(0, 30);
            rt.offsetMax = new Vector2(0, -40);
            subGO.AddComponent<VerticalLayoutGroup>();
            itemUI.slotContainer = subGO.transform;
            subGO = new GameObject("Tag Container");
            subGO.transform.SetParent(go.transform);
            rt = CreateOrGetRectTransform(subGO);
            rt.anchorMin = new Vector2(0.5f, 0);
            rt.anchorMax = new Vector2(1, 1);
            rt.pivot = new Vector2(1, 0.5f);
            rt.offsetMin = new Vector2(0, 30);
            rt.offsetMax = new Vector2(0, -40);
            subGO.AddComponent<VerticalLayoutGroup>();
            itemUI.tagContainer = subGO.transform;
            subGO = new GameObject("Selection Indicator");
            subGO.transform.SetParent(go.transform);
            SetFullSize(subGO);
            img = subGO.AddComponent<Image>();
            img.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
            img.color = new Color(0, 0, 1, 0.25f);
            img.type = Image.Type.Sliced;
            img.fillCenter = true;
            subGO.SetActive(false);
            itemUI.selectedIndicator = subGO;
            GameObject prefabItemUIList = PrefabUtility.SaveAsPrefabAsset(go, Path.Combine(componentPath, go.name + ".prefab"));
            DestroyImmediate(go);

            // Create Inventory Scroll List
            go = new GameObject("(" + themeName + ") Item Scroll List");
            go.transform.SetParent(canvas.transform);
            SetFullSize(go);
            scrollRect = CreateScrollRect(go);
            content = scrollRect.content.gameObject;
            VerticalLayoutGroup vlg = content.AddComponent<VerticalLayoutGroup>();
            InventoryItemScrollList itemList = go.AddComponent<InventoryItemScrollList>();
            itemList.itemUIPrefab = prefabItemUIList.GetComponent<ItemUI>();
            GameObject prefabItemScrollList = PrefabUtility.SaveAsPrefabAsset(go, Path.Combine(listPath, go.name + ".prefab"));
            DestroyImmediate(go);

            // Create Blind Scroll List
            go = new GameObject("(" + themeName + ") Blind Scroll List");
            go.transform.SetParent(canvas.transform);
            SetFullSize(go);
            scrollRect = CreateScrollRect(go);
            content = scrollRect.content.gameObject;
            vlg = content.AddComponent<VerticalLayoutGroup>();
            BlindItemScrollList blindList = go.AddComponent<BlindItemScrollList>();
            blindList.itemUIPrefab = prefabItemUIGrid.GetComponent<ItemUI>();
            GameObject prefabBlindScrollList = PrefabUtility.SaveAsPrefabAsset(go, Path.Combine(listPath, go.name + ".prefab"));
            DestroyImmediate(go);

            // Create Blind Scroll Grid
            go = new GameObject("(" + themeName + ") Blind Scroll Grid");
            go.transform.SetParent(canvas.transform);
            SetFullSize(go);
            scrollRect = CreateScrollRect(go);
            content = scrollRect.content.gameObject;
            glg = content.AddComponent<GridLayoutGroup>();
            glg.cellSize = new Vector2(80, 80);
            glg.spacing = new Vector2(8, 8);
            BlindItemScrollGrid blindScrollGrid = content.AddComponent<BlindItemScrollGrid>();
            blindScrollGrid.itemUIPrefab = prefabItemUIGrid.GetComponent<ItemUI>();
            GameObject prefabBlindScrollGrid = PrefabUtility.SaveAsPrefabAsset(go, Path.Combine(listPath, go.name + ".prefab"));
            DestroyImmediate(go);

            // Create Render Texture
            RenderTexture renderTexture = new RenderTexture(400, 400, 24, UnityEngine.Experimental.Rendering.GraphicsFormat.R32G32B32A32_SFloat);
            renderTexture.Create();
            AssetDatabase.CreateAsset(renderTexture, Path.Combine(renderTexPath, "Player Camera Render Texture.asset"));

            // Create Render Texture
            renderTexture = new RenderTexture(400, 400, 24, UnityEngine.Experimental.Rendering.GraphicsFormat.R32G32B32A32_SFloat);
            renderTexture.Create();
            AssetDatabase.CreateAsset(renderTexture, Path.Combine(renderTexPath, "Item Preview Render Texture.asset"));

            // Create Player Preview Camera
            go = new GameObject("(" + themeName + ") Player Preview Camera");
            subGO = new GameObject("Camera");
            subGO.transform.SetParent(go.transform);
            Camera camera = subGO.AddComponent<Camera>();
            camera.targetTexture = renderTexture;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = Color.black;
            PrefabUtility.SaveAsPrefabAsset(go, Path.Combine(componentPath, go.name + ".prefab"));
            DestroyImmediate(go);

            // Create Effect UI
            go = new GameObject("(" + themeName + ") Effect UI");
            go.transform.SetParent(canvas.transform);
            SetFullSize(go);
            StatEffectUI effectUI = go.AddComponent<StatEffectUI>();
            hlg = go.AddComponent<HorizontalLayoutGroup>();
            hlg.childControlHeight = hlg.childControlWidth = hlg.childForceExpandHeight = hlg.childForceExpandWidth = true;
            subGO = new GameObject("Effect Name");
            subGO.transform.SetParent(go.transform);
            tmpro = CreateTextMeshPro(subGO, "Name");
            effectUI.displayName = tmpro;
            subGO = new GameObject("Category");
            subGO.transform.SetParent(go.transform);
            tmpro = CreateTextMeshPro(subGO, "Category");
            effectUI.category = tmpro;
            subGO = new GameObject("Time Remaining");
            subGO.transform.SetParent(go.transform);
            tmpro = CreateTextMeshPro(subGO, "2sec");
            effectUI.remainingTime = tmpro;
            GameObject prefabEffectUI = PrefabUtility.SaveAsPrefabAsset(go, Path.Combine(componentPath, go.name + ".prefab"));
            DestroyImmediate(go);

            // Recipe Component UI
            go = new GameObject("(" + themeName + ") Recipe Component UI");
            go.transform.SetParent(canvas.transform);
            SetFullSize(go);
            RecipeComponentUI recipeComponent = go.AddComponent<RecipeComponentUI>();
            subGO = new GameObject("Item Icon");
            subGO.transform.SetParent(go.transform);
            rt = SetFullSize(subGO);
            img = subGO.AddComponent<Image>();
            img.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            img.preserveAspect = true;
            rt.offsetMin = new Vector2(10, 10);
            rt.offsetMax = new Vector2(-10, -10);
            recipeComponent.itemImage = img;
            txtGO = new GameObject("Item Name");
            txtGO.transform.SetParent(go.transform);
            tmpro = CreateTextMeshPro(txtGO, "Item Name", true, TextAlignmentOptions.MidlineLeft);
            rt = CreateOrGetRectTransform(txtGO);
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(1, 1);
            rt.pivot = new Vector2(0.5f, 1);
            rt.sizeDelta = new Vector2(0, 20);
            recipeComponent.displayName = tmpro;
            txtGO = new GameObject("Item Description");
            txtGO.transform.SetParent(go.transform);
            tmpro = CreateTextMeshPro(txtGO, "Description", true, TextAlignmentOptions.MidlineLeft);
            rt = CreateOrGetRectTransform(txtGO);
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(1, 1);
            rt.pivot = new Vector2(0.5f, 1);
            rt.sizeDelta = new Vector2(0, 20);
            rt.anchoredPosition = new Vector2(0, -20);
            recipeComponent.description = tmpro;
            txtGO = new GameObject("Count Available");
            txtGO.transform.SetParent(go.transform);
            tmpro = CreateTextMeshPro(txtGO, "0");
            rt = CreateOrGetRectTransform(txtGO);
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = new Vector2(0.5f, 0);
            rt.pivot = Vector2.zero;
            rt.sizeDelta = new Vector2(0, 20);
            recipeComponent.countAvailable = tmpro;
            txtGO = new GameObject("Count Needed");
            txtGO.transform.SetParent(go.transform);
            tmpro = CreateTextMeshPro(txtGO, "0");
            rt = CreateOrGetRectTransform(txtGO);
            rt.anchorMin = new Vector2(0.5f, 0);
            rt.anchorMax = new Vector2(1, 0);
            rt.pivot = Vector2.zero;
            rt.sizeDelta = new Vector2(0, 20);
            recipeComponent.countNeeded = tmpro;
            subGO = new GameObject("Rarity Color Indicator");
            subGO.transform.SetParent(go.transform);
            rt = CreateOrGetRectTransform(subGO);
            rt.pivot = rt.anchorMin = rt.anchorMax = new Vector2(1, 1);
            rt.anchoredPosition = new Vector2(-5, -5);
            rt.sizeDelta = new Vector2(5, 5);
            img = subGO.AddComponent<Image>();
            img.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            img.preserveAspect = true;
            img.color = Color.yellow;
            rarityColor = subGO.AddComponent<RarityColorIndicator>();
            recipeComponent.rarityColorIndicator = rarityColor;
            subGO = new GameObject("Rarity Slider");
            subGO.transform.SetParent(go.transform);
            raritySlider = CreateProgressbar(subGO);
            rt = subGO.GetComponent<RectTransform>();
            if (rt == null) rt = subGO.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(0, 0);
            rt.anchorMax = new Vector2(0.5f, 0);
            rt.pivot = Vector2.zero;
            rt.sizeDelta = new Vector2(0, 8);
            rt.anchoredPosition = new Vector2(0, 20);
            recipeComponent.raritySlider = raritySlider;
            subGO = new GameObject("Condition Slider");
            subGO.transform.SetParent(go.transform);
            conditionSlider = CreateProgressbar(subGO);
            rt = CreateOrGetRectTransform(subGO);
            rt.anchorMin = new Vector2(0.5f, 0);
            rt.anchorMax = new Vector2(1, 0);
            rt.pivot = Vector2.zero;
            rt.sizeDelta = new Vector2(0, 8);
            rt.anchoredPosition = new Vector2(0, 20);
            recipeComponent.conditionSlider = conditionSlider;
            GameObject prefabRecipeComponentUI = PrefabUtility.SaveAsPrefabAsset(go, Path.Combine(componentPath, go.name + ".prefab"));
            DestroyImmediate(go);

            // Repair Component UI
            go = new GameObject("(" + themeName + ") Repair Component UI");
            go.transform.SetParent(canvas.transform);
            SetFullSize(go);
            ComponentUI repairComponent = go.AddComponent<ComponentUI>();
            subGO = new GameObject("Item Icon");
            subGO.transform.SetParent(go.transform);
            rt = SetFullSize(subGO);
            img = subGO.AddComponent<Image>();
            img.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            img.preserveAspect = true;
            rt.offsetMin = new Vector2(10, 10);
            rt.offsetMax = new Vector2(-10, -10);
            repairComponent.itemImage = img;
            txtGO = new GameObject("Item Name");
            txtGO.transform.SetParent(go.transform);
            tmpro = CreateTextMeshPro(txtGO, "Item Name", true, TextAlignmentOptions.MidlineLeft);
            rt = CreateOrGetRectTransform(subGO);
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(1, 1);
            rt.pivot = new Vector2(0.5f, 1);
            rt.sizeDelta = new Vector2(0, 20);
            repairComponent.displayName = tmpro;
            txtGO = new GameObject("Item Description");
            txtGO.transform.SetParent(go.transform);
            tmpro = CreateTextMeshPro(txtGO, "Description", true, TextAlignmentOptions.MidlineLeft);
            rt = CreateOrGetRectTransform(txtGO);
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(1, 1);
            rt.pivot = new Vector2(0.5f, 1);
            rt.sizeDelta = new Vector2(0, 20);
            rt.anchoredPosition = new Vector2(0, -20);
            repairComponent.description = tmpro;
            txtGO = new GameObject("Count Available");
            txtGO.transform.SetParent(go.transform);
            tmpro = CreateTextMeshPro(txtGO, "0");
            rt = CreateOrGetRectTransform(txtGO);
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = new Vector2(0.5f, 0);
            rt.pivot = Vector2.zero;
            rt.sizeDelta = new Vector2(0, 20);
            repairComponent.countAvailable = tmpro;
            txtGO = new GameObject("Count Needed");
            txtGO.transform.SetParent(go.transform);
            tmpro = CreateTextMeshPro(txtGO, "0");
            rt = CreateOrGetRectTransform(txtGO);
            rt.anchorMin = new Vector2(0.5f, 0);
            rt.anchorMax = new Vector2(1, 0);
            rt.pivot = Vector2.zero;
            rt.sizeDelta = new Vector2(0, 20);
            repairComponent.countNeeded = tmpro;
            subGO = new GameObject("Rarity Color Indicator");
            subGO.transform.SetParent(go.transform);
            rt = CreateOrGetRectTransform(subGO);
            rt.pivot = rt.anchorMin = rt.anchorMax = new Vector2(1, 1);
            rt.anchoredPosition = new Vector2(-5, -5);
            rt.sizeDelta = new Vector2(5, 5);
            img = subGO.AddComponent<Image>();
            img.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            img.preserveAspect = true;
            img.color = Color.yellow;
            rarityColor = subGO.AddComponent<RarityColorIndicator>();
            repairComponent.rarityColorIndicator = rarityColor;
            subGO = new GameObject("Rarity Slider");
            subGO.transform.SetParent(go.transform);
            raritySlider = CreateProgressbar(subGO);
            rt = CreateOrGetRectTransform(subGO);
            rt.anchorMin = new Vector2(0, 0);
            rt.anchorMax = new Vector2(0.5f, 0);
            rt.pivot = Vector2.zero;
            rt.sizeDelta = new Vector2(0, 8);
            rt.anchoredPosition = new Vector2(0, 20);
            repairComponent.raritySlider = raritySlider;
            subGO = new GameObject("Condition Slider");
            subGO.transform.SetParent(go.transform);
            conditionSlider = CreateProgressbar(subGO);
            rt = CreateOrGetRectTransform(subGO);
            rt.anchorMin = new Vector2(0.5f, 0);
            rt.anchorMax = new Vector2(1, 0);
            rt.pivot = Vector2.zero;
            rt.sizeDelta = new Vector2(0, 8);
            rt.anchoredPosition = new Vector2(0, 20);
            repairComponent.conditionSlider = conditionSlider;
            GameObject prefabRepairComponentUI = PrefabUtility.SaveAsPrefabAsset(go, Path.Combine(componentPath, go.name + ".prefab"));
            DestroyImmediate(go);

            // Create Recipe UI
            go = new GameObject("(" + themeName + ") Recipe UI");
            go.transform.SetParent(canvas.transform);
            SetFullSize(go);
            RecipeUI recipeUI = go.AddComponent<RecipeUI>();
            subGO = new GameObject("Craftable Indicator");
            subGO.transform.SetParent(go.transform);
            SetFullSize(subGO);
            img = subGO.AddComponent<Image>();
            img.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
            img.color = new Color(0, 1, 0, 0.25f);
            img.type = Image.Type.Sliced;
            img.fillCenter = true;
            subGO.SetActive(false);
            recipeUI.craftableIndicator = subGO;
            subGO = new GameObject("Item Icon");
            subGO.transform.SetParent(go.transform);
            rt = SetFullSize(subGO);
            img = subGO.AddComponent<Image>();
            img.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            img.preserveAspect = true;
            rt.offsetMin = new Vector2(10, 10);
            rt.offsetMax = new Vector2(-10, -10);
            recipeUI.icon = img;
            txtGO = new GameObject("Item Name");
            txtGO.transform.SetParent(go.transform);
            tmpro = CreateTextMeshPro(txtGO, "Item Name");
            rt = CreateOrGetRectTransform(txtGO);
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(1, 1);
            rt.pivot = new Vector2(0.5f, 1);
            rt.sizeDelta = new Vector2(0, 20);
            recipeUI.displayName = tmpro;
            txtGO = new GameObject("Item Description");
            txtGO.transform.SetParent(go.transform);
            tmpro = CreateTextMeshPro(txtGO, "Description");
            rt = CreateOrGetRectTransform(txtGO);
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(1, 1);
            rt.pivot = new Vector2(0.5f, 1);
            rt.sizeDelta = new Vector2(0, 20);
            rt.anchoredPosition = new Vector2(0, -20);
            recipeUI.description = tmpro;
            txtGO = new GameObject("Craft Duration");
            txtGO.transform.SetParent(go.transform);
            tmpro = CreateTextMeshPro(txtGO, "0sec");
            rt = CreateOrGetRectTransform(txtGO);
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = new Vector2(1, 0);
            rt.pivot = Vector2.zero;
            rt.sizeDelta = new Vector2(0, 20);
            recipeUI.duration = tmpro;
            recipeUI.hideIfInstant = txtGO;
            subGO = new GameObject("Locked Indicator");
            subGO.transform.SetParent(go.transform);
            SetFullSize(subGO);
            img = subGO.AddComponent<Image>();
            img.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
            img.color = new Color(1, 0, 0, 0.5f);
            img.type = Image.Type.Sliced;
            img.fillCenter = true;
            subGO.SetActive(false);
            recipeUI.lockedIndicator = subGO;
            subGO = new GameObject("Rarity Color Indicator");
            subGO.transform.SetParent(go.transform);
            rt = CreateOrGetRectTransform(subGO);
            rt.pivot = rt.anchorMin = rt.anchorMax = new Vector2(1, 1);
            rt.anchoredPosition = new Vector2(-5, -5);
            rt.sizeDelta = new Vector2(5, 5);
            img = subGO.AddComponent<Image>();
            img.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            img.preserveAspect = true;
            img.color = Color.yellow;
            rarityColor = subGO.AddComponent<RarityColorIndicator>();
            recipeUI.rarityColorIndicator = rarityColor;
            subGO = new GameObject("Rarity Slider");
            subGO.transform.SetParent(go.transform);
            raritySlider = CreateProgressbar(subGO);
            rt = CreateOrGetRectTransform(subGO);
            rt.anchorMin = new Vector2(0, 0);
            rt.anchorMax = new Vector2(1, 0);
            rt.pivot = Vector2.zero;
            rt.sizeDelta = new Vector2(0, 8);
            rt.anchoredPosition = new Vector2(0, 20);
            recipeUI.raritySlider = raritySlider;
            subGO = new GameObject("Selection Indicator");
            subGO.transform.SetParent(go.transform);
            SetFullSize(subGO);
            img = subGO.AddComponent<Image>();
            img.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
            img.color = new Color(0, 0, 1, 0.25f);
            img.type = Image.Type.Sliced;
            img.fillCenter = true;
            subGO.SetActive(false);
            recipeUI.selectedIndicator = subGO;
            GameObject prefabRecipeUI = PrefabUtility.SaveAsPrefabAsset(go, Path.Combine(componentPath, go.name + ".prefab"));
            DestroyImmediate(go);

            // Create Skill Slot UI
            go = new GameObject("(" + themeName + ") Skill Slot UI");
            go.transform.SetParent(canvas.transform);
            SetFullSize(go);
            SkillSlotUI skillSlotUI = go.AddComponent<SkillSlotUI>();
            subGO = new GameObject("Skill Icon");
            subGO.transform.SetParent(go.transform);
            rt = SetFullSize(subGO);
            img = subGO.AddComponent<Image>();
            img.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            img.preserveAspect = true;
            rt.offsetMin = new Vector2(10, 10);
            rt.offsetMax = new Vector2(-10, -10);
            skillSlotUI.skillIcon = img;
            subGO = new GameObject("Equipped Indicator");
            subGO.transform.SetParent(go.transform);
            SetFullSize(subGO);
            img = subGO.AddComponent<Image>();
            img.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
            img.color = new Color(0, 1, 0, 0.5f);
            img.type = Image.Type.Sliced;
            img.fillCenter = true;
            subGO.SetActive(false);
            skillSlotUI.equippedIcon = img;
            GameObject prefabSkillSlotUI = PrefabUtility.SaveAsPrefabAsset(go, Path.Combine(componentPath, go.name + ".prefab"));
            DestroyImmediate(go);

            // Create Craft Queue UI
            go = new GameObject("(" + themeName + ") Craft Queue UI");
            go.transform.SetParent(canvas.transform);
            SetFullSize(go);
            CraftQueueUI craftQueueUI = go.AddComponent<CraftQueueUI>();
            recipeUI = go.AddComponent<RecipeUI>();
            craftQueueUI.recipeUI = recipeUI;
            subGO = new GameObject("Recipe Icon");
            subGO.transform.SetParent(go.transform);
            rt = SetFullSize(subGO);
            img = subGO.AddComponent<Image>();
            img.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            img.preserveAspect = true;
            rt.offsetMin = new Vector2(10, 10);
            rt.offsetMax = new Vector2(-10, -10);
            recipeUI.icon = img;
            txtGO = new GameObject("Item Name");
            txtGO.transform.SetParent(go.transform);
            tmpro = CreateTextMeshPro(txtGO, "Item Name");
            rt = CreateOrGetRectTransform(txtGO);
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(1, 1);
            rt.pivot = new Vector2(0.5f, 1);
            rt.sizeDelta = new Vector2(0, 20);
            recipeUI.displayName = tmpro;
            txtGO = new GameObject("Item Count");
            txtGO.transform.SetParent(go.transform);
            tmpro = CreateTextMeshPro(txtGO, "000");
            rt = CreateOrGetRectTransform(txtGO);
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(1, 1);
            rt.pivot = new Vector2(0.5f, 1);
            rt.sizeDelta = new Vector2(0, 20);
            rt.anchoredPosition = new Vector2(0, -20);
            craftQueueUI.count = tmpro;
            txtGO = new GameObject("Time Remaining");
            txtGO.transform.SetParent(go.transform);
            tmpro = CreateTextMeshPro(txtGO, "0 sec");
            rt = CreateOrGetRectTransform(txtGO);
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = new Vector2(1, 0);
            rt.pivot = Vector2.zero;
            rt.sizeDelta = new Vector2(0, 20);
            craftQueueUI.timeRemaining = tmpro;
            subGO = new GameObject("Progress");
            subGO.transform.SetParent(go.transform);
            raritySlider = CreateProgressbar(subGO);
            rt = CreateOrGetRectTransform(subGO);
            rt.anchorMin = new Vector2(0, 0);
            rt.anchorMax = new Vector2(1, 0);
            rt.pivot = Vector2.zero;
            rt.sizeDelta = new Vector2(0, 8);
            rt.anchoredPosition = new Vector2(0, 20);
            craftQueueUI.progress = raritySlider;
            GameObject prefabCraftQueueUI = PrefabUtility.SaveAsPrefabAsset(go, Path.Combine(componentPath, go.name + ".prefab"));
            DestroyImmediate(go);

            // Create Inventory Scroll List
            go = new GameObject("(" + themeName + ") Craft Queue Scroll List");
            go.transform.SetParent(canvas.transform);
            SetFullSize(go);
            scrollRect = CreateScrollRect(go);
            content = scrollRect.content.gameObject;
            vlg = content.AddComponent<VerticalLayoutGroup>();
            CraftQueueScrollList queueList = go.AddComponent<CraftQueueScrollList>();
            queueList.itemPrefab = prefabCraftQueueUI.GetComponent<CraftQueueUI>();
            GameObject prefabQueueScrollList = PrefabUtility.SaveAsPrefabAsset(go, Path.Combine(listPath, go.name + ".prefab"));
            DestroyImmediate(go);

            // Create Prompt
            go = new GameObject("(" + themeName + ") Prompt (By Button)");
            go.transform.SetParent(canvas.transform);
            SetFullSize(go);
            PromptUI promptUI = go.AddComponent<PromptUI>();
            TriggerByButton triggerByButton = go.AddComponent<TriggerByButton>();
            triggerByButton.buttonName = "Submit";
            if (triggerByButton.onButtonDown == null) triggerByButton.onButtonDown = new UnityEvent();
            MethodInfo targetinfo = UnityEvent.GetValidMethodInfo(promptUI, "ConfirmPrompt", new Type[0]);
            UnityAction action = Delegate.CreateDelegate(typeof(UnityAction), promptUI, targetinfo, false) as UnityAction;
            UnityEventTools.AddPersistentListener(triggerByButton.onButtonDown, action);
            txtGO = new GameObject("Prompt");
            txtGO.transform.SetParent(go.transform);
            tmpro = CreateTextMeshPro(txtGO, "{Prompt}");
            rt = CreateOrGetRectTransform(txtGO);
            rt.anchorMin = new Vector2(0, 0.263f);
            rt.anchorMax = new Vector2(1, 0.3f);
            rt.offsetMax = rt.offsetMin = Vector2.zero;
            promptUI.promptText = tmpro;
            promptUI.textFormat = "{1} {0}";
            GameObject prefabPromptUI = PrefabUtility.SaveAsPrefabAsset(go, Path.Combine(promptPath, go.name + ".prefab"));
            DestroyImmediate(go);
            themeEditor.serializedObject.FindProperty("promptUI").objectReferenceValue = prefabPromptUI.GetComponent<PromptUI>();

            // Create Prompt
            go = new GameObject("(" + themeName + ") Prompt (By Key)");
            go.transform.SetParent(canvas.transform);
            SetFullSize(go);
            promptUI = go.AddComponent<PromptUI>();
            TriggerByKey triggerByKey = go.AddComponent<TriggerByKey>();
            triggerByKey.key = KeyCode.F;
            if (triggerByKey.onKeyDown == null) triggerByKey.onKeyDown = new UnityEvent();
            targetinfo = UnityEvent.GetValidMethodInfo(promptUI, "ConfirmPrompt", new Type[0]);
            action = Delegate.CreateDelegate(typeof(UnityAction), promptUI, targetinfo, false) as UnityAction;
            UnityEventTools.AddPersistentListener(triggerByKey.onKeyDown, action);
            txtGO = new GameObject("Prompt");
            txtGO.transform.SetParent(go.transform);
            tmpro = CreateTextMeshPro(txtGO, "{Prompt}");
            rt = CreateOrGetRectTransform(txtGO);
            rt.anchorMin = new Vector2(0, 0.263f);
            rt.anchorMax = new Vector2(1, 0.3f);
            rt.offsetMax = rt.offsetMin = Vector2.zero;
            promptUI.promptText = tmpro;
            promptUI.textFormat = "{1} {0}";
            PrefabUtility.SaveAsPrefabAsset(go, Path.Combine(promptPath, go.name + ".prefab"));
            DestroyImmediate(go);

            // Create Count Selection
            go = new GameObject("(" + themeName + ") Count Selection");
            go.transform.SetParent(canvas.transform);
            SetFullSize(go);
            subGO = new GameObject("Window");
            subGO.transform.SetParent(go.transform);
            rt = CreateOrGetRectTransform(subGO);
            rt.anchorMin = new Vector2(0.3f, 0.3f);
            rt.anchorMax = new Vector2(0.7f, 0.7f);
            rt.offsetMax = rt.offsetMin = Vector2.zero;
            img = subGO.AddComponent<Image>();
            img.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            img.type = Image.Type.Sliced;
            img.fillCenter = true;
            CountSelectUI countSelectUI = go.AddComponent<CountSelectUI>();
            txtGO = new GameObject("Title");
            txtGO.transform.SetParent(subGO.transform);
            tmpro = CreateTextMeshPro(txtGO, "{Title}");
            countSelectUI.title = tmpro;
            rt = CreateOrGetRectTransform(txtGO);
            rt.anchorMin = new Vector2(0, 0.8190001f);
            rt.anchorMax = new Vector2(1, 0.9548612f);
            rt.offsetMax = rt.offsetMin = Vector2.zero;
            GameObject pnlGO = new GameObject("Details");
            pnlGO.transform.SetParent(subGO.transform);
            rt = CreateOrGetRectTransform(pnlGO);
            rt.anchorMin = new Vector2(0.048f, 0.377f);
            rt.anchorMax = new Vector2(0.952f, 0.763f);
            rt.offsetMax = rt.offsetMin = Vector2.zero;
            txtGO = new GameObject("Prompt");
            txtGO.transform.SetParent(pnlGO.transform);
            tmpro = CreateTextMeshPro(txtGO, "{Prompt}");
            rt = CreateOrGetRectTransform(txtGO);
            rt.anchorMin = new Vector2(0, 0.554f);
            rt.anchorMax = new Vector2(1, 1);
            rt.offsetMax = rt.offsetMin = Vector2.zero;
            countSelectUI.prompt = tmpro;
            txtGO = new GameObject("Slider");
            txtGO.transform.SetParent(pnlGO.transform);
            Slider slider = CreateSlider(txtGO);
            rt = CreateOrGetRectTransform(txtGO);
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = new Vector2(1, 0.41f);
            rt.offsetMin = new Vector2(30, 0);
            rt.offsetMax = new Vector2(-30, 0);
            countSelectUI.countSlider = slider;
            txtGO = new GameObject("Min Count");
            txtGO.transform.SetParent(pnlGO.transform);
            tmpro = CreateTextMeshPro(txtGO, "0");
            rt = CreateOrGetRectTransform(txtGO);
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = new Vector2(0, 0.41f);
            rt.offsetMax = rt.offsetMin = Vector2.zero;
            rt.pivot = new Vector2(0, 0.5f);
            rt.sizeDelta = new Vector2(30, 0);
            countSelectUI.minCount = tmpro;
            txtGO = new GameObject("Max Count");
            txtGO.transform.SetParent(pnlGO.transform);
            tmpro = CreateTextMeshPro(txtGO, "0");
            rt = CreateOrGetRectTransform(txtGO);
            rt.anchorMin = new Vector2(1, 0);
            rt.anchorMax = new Vector2(1, 0.41f);
            rt.offsetMax = rt.offsetMin = Vector2.zero;
            rt.pivot = new Vector2(1, 0.5f);
            rt.sizeDelta = new Vector2(30, 0);
            countSelectUI.maxCount = tmpro;
            txtGO = new GameObject("Current Count");
            txtGO.transform.SetParent(slider.handleRect);
            tmpro = CreateTextMeshPro(txtGO, "0");
            SetFullSize(txtGO);
            countSelectUI.curCount = tmpro;
            pnlGO = new GameObject("Buttons");
            pnlGO.transform.SetParent(subGO.transform);
            rt = CreateOrGetRectTransform(pnlGO);
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = new Vector2(1, 0.2655289f);
            rt.offsetMin = new Vector2(16, 16);
            rt.offsetMax = new Vector2(-16, 0);
            hlg = pnlGO.AddComponent<HorizontalLayoutGroup>();
            hlg.spacing = 64;
            hlg.childForceExpandHeight = hlg.childForceExpandWidth = hlg.childControlHeight = hlg.childControlWidth = true;
            txtGO = PrefabUtility.InstantiatePrefab(prefabButton, pnlGO.transform) as GameObject;
            txtGO.GetComponentInChildren<TextMeshProUGUI>().text = "OK";
            targetinfo = UnityEvent.GetValidMethodInfo(countSelectUI, "ConfirmCount", new Type[0]);
            action = Delegate.CreateDelegate(typeof(UnityAction), countSelectUI, targetinfo, false) as UnityAction;
            UnityEventTools.AddPersistentListener(txtGO.GetComponentInChildren<Button>().onClick, action);
            txtGO = PrefabUtility.InstantiatePrefab(prefabButton, pnlGO.transform) as GameObject;
            txtGO.GetComponentInChildren<TextMeshProUGUI>().text = "Cancel";
            targetinfo = UnityEvent.GetValidMethodInfo(countSelectUI, "CancelCount", new Type[0]);
            action = Delegate.CreateDelegate(typeof(UnityAction), countSelectUI, targetinfo, false) as UnityAction;
            UnityEventTools.AddPersistentListener(txtGO.GetComponentInChildren<Button>().onClick, action);
            GameObject prefabCountSelect = PrefabUtility.SaveAsPrefabAsset(go, Path.Combine(promptPath, go.name + ".prefab"));
            DestroyImmediate(go);
            themeEditor.serializedObject.FindProperty("countSelection").objectReferenceValue = prefabCountSelect.GetComponent<CountSelectUI>();

            // Create Rename Prompt
            go = new GameObject("(" + themeName + ") Rename Prompt");
            go.transform.SetParent(canvas.transform);
            SetFullSize(go);
            RenamePrompt renamePrompt = go.AddComponent<RenamePrompt>();
            subGO = new GameObject("Window");
            subGO.transform.SetParent(go.transform);
            rt = CreateOrGetRectTransform(subGO);
            rt.anchorMin = new Vector2(0.2648751f, 0.3401653f);
            rt.anchorMax = new Vector2(0.7337813f, 0.6597769f);
            rt.offsetMax = rt.offsetMin = Vector2.zero;
            img = subGO.AddComponent<Image>();
            img.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            img.type = Image.Type.Sliced;
            img.fillCenter = true;
            txtGO = new GameObject("Current Name");
            txtGO.transform.SetParent(subGO.transform);
            tmpro = CreateTextMeshPro(txtGO, "{Current Name}");
            renamePrompt.currentName = tmpro;
            rt = CreateOrGetRectTransform(txtGO);
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(1, 1);
            rt.pivot = new Vector2(0.5f, 1);
            rt.offsetMax = rt.offsetMin = Vector2.zero;
            rt.sizeDelta = new Vector2(0, 30);
            txtGO = new GameObject("New Name Input");
            txtGO.transform.SetParent(subGO.transform);
            renamePrompt.newName_TMP = CreateTextMeshInput(txtGO);
            rt = CreateOrGetRectTransform(txtGO);
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(1, 1);
            rt.pivot = new Vector2(0.5f, 1);
            rt.offsetMin = new Vector2(12, 0);
            rt.offsetMax = new Vector2(-12, -40);
            rt.sizeDelta = new Vector2(rt.sizeDelta.x, 30);
            pnlGO = new GameObject("Buttons");
            pnlGO.transform.SetParent(subGO.transform);
            rt = CreateOrGetRectTransform(pnlGO);
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = new Vector2(1, 0);
            rt.pivot = new Vector2(0.5f, 0);
            rt.offsetMin = new Vector2(16, 16);
            rt.offsetMax = new Vector2(-16, 0);
            rt.sizeDelta = new Vector2(rt.sizeDelta.x, 38);
            hlg = pnlGO.AddComponent<HorizontalLayoutGroup>();
            hlg.spacing = 64;
            hlg.childForceExpandHeight = hlg.childForceExpandWidth = hlg.childControlHeight = hlg.childControlWidth = true;
            txtGO = PrefabUtility.InstantiatePrefab(prefabButton, pnlGO.transform) as GameObject;
            txtGO.GetComponentInChildren<TextMeshProUGUI>().text = "Rename";
            renamePrompt.okButton = txtGO.GetComponent<Button>();
            txtGO = PrefabUtility.InstantiatePrefab(prefabButton, pnlGO.transform) as GameObject;
            txtGO.GetComponentInChildren<TextMeshProUGUI>().text = "Cancel";
            renamePrompt.cancelButton = txtGO.GetComponent<Button>();
            GameObject prefabRenamePrompt = PrefabUtility.SaveAsPrefabAsset(go, Path.Combine(promptPath, go.name + ".prefab"));
            DestroyImmediate(go);
            themeEditor.serializedObject.FindProperty("renamePrompt").objectReferenceValue = prefabRenamePrompt.GetComponent<RenamePrompt>();

            // Create Attachment Item Grid
            go = new GameObject("(" + themeName + ") Attachment Item Grid");
            go.transform.SetParent(canvas.transform);
            SetFullSize(go);
            scrollRect = CreateScrollRect(go);
            content = scrollRect.content.gameObject;
            glg = content.AddComponent<GridLayoutGroup>();
            glg.cellSize = new Vector2(80, 80);
            glg.spacing = new Vector2(8, 8);
            AttachmentItemGrid attachmentItemGrid = go.AddComponent<AttachmentItemGrid>();
            attachmentItemGrid.itemUIPrefab = prefabItemUIGrid.GetComponent<ItemUI>();
            GameObject prefabAttachmentItemGrid = PrefabUtility.SaveAsPrefabAsset(go, Path.Combine(listPath, go.name + ".prefab"));
            DestroyImmediate(go);

            // Create Attachment Slot Grid
            go = new GameObject("(" + themeName + ") Attachment Slot Grid");
            go.transform.SetParent(canvas.transform);
            SetFullSize(go);
            scrollRect = CreateScrollRect(go);
            content = scrollRect.content.gameObject;
            glg = content.AddComponent<GridLayoutGroup>();
            glg.cellSize = new Vector2(80, 80);
            glg.spacing = new Vector2(8, 8);
            AttachmentSlotGrid attachmentSlotGrid = go.AddComponent<AttachmentSlotGrid>();
            attachmentSlotGrid.slotUIPrefab = prefabAttachSlotUI.GetComponent<AttachmentSlotUI>();
            GameObject prefabAttachmentSlotGrid = PrefabUtility.SaveAsPrefabAsset(go, Path.Combine(listPath, go.name + ".prefab"));
            DestroyImmediate(go);

            // Create Attachment UI Prompt
            go = new GameObject("(" + themeName + ") Attachment UI");
            go.transform.SetParent(canvas.transform);
            SetFullSize(go);
            AttachmentsUI attachmentsUI = go.AddComponent<AttachmentsUI>();
            subGO = new GameObject("Window");
            subGO.transform.SetParent(go.transform);
            rt = CreateOrGetRectTransform(subGO);
            rt.anchorMin = new Vector2(0.2648751f, 0.3401653f);
            rt.anchorMax = new Vector2(0.7337813f, 0.6597769f);
            rt.offsetMax = rt.offsetMin = Vector2.zero;
            img = subGO.AddComponent<Image>();
            img.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            img.type = Image.Type.Sliced;
            img.fillCenter = true;
            txtGO = new GameObject("Item Name");
            txtGO.transform.SetParent(subGO.transform);
            tmpro = CreateTextMeshPro(txtGO, "{Item Name}");
            attachmentsUI.itemTitle = tmpro;
            rt = CreateOrGetRectTransform(txtGO);
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(1, 1);
            rt.pivot = new Vector2(0.5f, 1);
            rt.offsetMax = rt.offsetMin = Vector2.zero;
            rt.sizeDelta = new Vector2(0, 30);
            txtGO = PrefabUtility.InstantiatePrefab(prefabAttachmentSlotGrid, subGO.transform) as GameObject;
            rt = CreateOrGetRectTransform(txtGO);
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = new Vector2(0.5f, 1);
            rt.offsetMin = new Vector2(16, 40);
            rt.offsetMax = new Vector2(0, -40);
            pnlGO = PrefabUtility.InstantiatePrefab(prefabAttachmentItemGrid, subGO.transform) as GameObject;
            rt = CreateOrGetRectTransform(pnlGO);
            rt.anchorMin = new Vector2(0.5f, 0);
            rt.anchorMax = new Vector2(1, 1);
            rt.offsetMin = new Vector2(0, 40);
            rt.offsetMax = new Vector2(-16, -40);
            txtGO.GetComponent<AttachmentSlotGrid>().attachmentItemList = pnlGO.GetComponent<AttachmentItemList>();
            txtGO = PrefabUtility.InstantiatePrefab(prefabButton, subGO.transform) as GameObject;
            txtGO.GetComponentInChildren<TextMeshProUGUI>().text = "Close";
            targetinfo = UnityEvent.GetValidMethodInfo(attachmentsUI, "Close", new Type[0]);
            action = Delegate.CreateDelegate(typeof(UnityAction), attachmentsUI, targetinfo, false) as UnityAction;
            UnityEventTools.AddPersistentListener(txtGO.GetComponentInChildren<Button>().onClick, action);
            rt = CreateOrGetRectTransform(txtGO);
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = new Vector2(1, 0);
            rt.pivot = new Vector2(0.5f, 0);
            rt.offsetMax = rt.offsetMin = Vector2.zero;
            rt.sizeDelta = new Vector2(0, 30);
            GameObject prefabAttachmentUI = PrefabUtility.SaveAsPrefabAsset(go, Path.Combine(promptPath, go.name + ".prefab"));
            DestroyImmediate(go);
            themeEditor.serializedObject.FindProperty("attachments").objectReferenceValue = prefabAttachmentUI.GetComponent<AttachmentsUI>();

            // Create Item Details
            go = new GameObject("(" + themeName + ") Item Detail UI");
            go.transform.SetParent(canvas.transform);
            SetFullSize(go);
            ItemDetailUI itemDetailUI  = go.AddComponent<ItemDetailUI>();
            subGO = new GameObject("Item Icon");
            subGO.transform.SetParent(go.transform);
            rt = SetFullSize(subGO);
            img = subGO.AddComponent<Image>();
            img.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            img.preserveAspect = true;
            rt.offsetMin = new Vector2(10, 10);
            rt.offsetMax = new Vector2(-10, -10);
            itemDetailUI.itemSprite = img;
            txtGO = new GameObject("Item Name");
            txtGO.transform.SetParent(go.transform);
            tmpro = CreateTextMeshPro(txtGO, "Item Name", true, TextAlignmentOptions.MidlineLeft);
            rt = CreateOrGetRectTransform(txtGO);
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(1, 1);
            rt.pivot = new Vector2(0.5f, 1);
            rt.offsetMax = rt.offsetMin = Vector2.zero;
            rt.sizeDelta = new Vector2(0, 20);
            itemDetailUI.itemName = tmpro;
            txtGO = new GameObject("Item Description");
            txtGO.transform.SetParent(go.transform);
            tmpro = CreateTextMeshPro(txtGO, "Description", true, TextAlignmentOptions.MidlineLeft);
            rt = CreateOrGetRectTransform(txtGO);
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(1, 1);
            rt.pivot = new Vector2(0.5f, 1);
            rt.sizeDelta = new Vector2(0, 20);
            rt.anchoredPosition = new Vector2(0, -20);
            itemDetailUI.itemDescription = tmpro;
            itemDetailUI.tagPrefab = prefabItemTagUI.GetComponent<ItemTagUI>();
            txtGO = new GameObject("Item Weight");
            txtGO.transform.SetParent(go.transform);
            tmpro = CreateTextMeshPro(txtGO, "0lb");
            rt = CreateOrGetRectTransform(txtGO);
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = new Vector2(0.33f, 0);
            rt.pivot = Vector2.zero;
            rt.offsetMax = rt.offsetMin = Vector2.zero;
            rt.sizeDelta = new Vector2(0, 20);
            itemDetailUI.weightText = tmpro;
            txtGO = new GameObject("Item Value");
            txtGO.transform.SetParent(go.transform);
            tmpro = CreateTextMeshPro(txtGO, "$0");
            rt = CreateOrGetRectTransform(txtGO);
            rt.anchorMin = new Vector2(0.33f, 0);
            rt.anchorMax = new Vector2(0.64f, 0);
            rt.offsetMax = rt.offsetMin = Vector2.zero;
            rt.pivot = Vector2.zero;
            rt.sizeDelta = new Vector2(0, 20);
            itemDetailUI.valueText = tmpro;
            subGO = new GameObject("Count Container");
            subGO.transform.SetParent(go.transform);
            rt = CreateOrGetRectTransform(subGO);
            rt.anchorMin = new Vector2(0.64f, 0);
            rt.anchorMax = new Vector2(1, 0);
            rt.offsetMax = rt.offsetMin = Vector2.zero;
            rt.pivot = Vector2.zero;
            rt.sizeDelta = new Vector2(0, 20);
            img = subGO.AddComponent<Image>();
            img.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            img.type = Image.Type.Sliced;
            img.fillCenter = true;
            subGO = new GameObject("Rarity Color Indicator");
            subGO.transform.SetParent(go.transform);
            rt = CreateOrGetRectTransform(subGO);
            rt.pivot = rt.anchorMin = rt.anchorMax = new Vector2(1, 1);
            rt.anchoredPosition = new Vector2(-5, -5);
            rt.sizeDelta = new Vector2(5, 5);
            img = subGO.AddComponent<Image>();
            img.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            img.preserveAspect = true;
            img.color = Color.yellow;
            rarityColor = subGO.AddComponent<RarityColorIndicator>();
            itemDetailUI.rarityIndicator = rarityColor;
            subGO = new GameObject("Rarity Slider");
            subGO.transform.SetParent(go.transform);
            raritySlider = CreateProgressbar(subGO);
            rt = CreateOrGetRectTransform(subGO);
            rt.anchorMin = new Vector2(0, 0);
            rt.anchorMax = new Vector2(0.5f, 0);
            rt.pivot = Vector2.zero;
            rt.sizeDelta = new Vector2(0, 8);
            rt.anchoredPosition = new Vector2(0, 20);
            itemDetailUI.raritySlider = raritySlider;
            subGO = new GameObject("Condition Slider");
            subGO.transform.SetParent(go.transform);
            conditionSlider = CreateProgressbar(subGO);
            rt = CreateOrGetRectTransform(subGO);
            rt.anchorMin = new Vector2(0.5f, 0);
            rt.anchorMax = new Vector2(1, 0);
            rt.pivot = Vector2.zero;
            rt.sizeDelta = new Vector2(0, 8);
            rt.anchoredPosition = new Vector2(0, 20);
            itemDetailUI.conditionSlider = conditionSlider;
            subGO = new GameObject("Tag Container");
            subGO.transform.SetParent(go.transform);
            rt = CreateOrGetRectTransform(subGO);
            rt.anchorMin = new Vector2(0.5f, 0);
            rt.anchorMax = new Vector2(1, 1);
            rt.pivot = new Vector2(1, 0.5f);
            rt.offsetMin = new Vector2(0, 30);
            rt.offsetMax = new Vector2(0, -40);
            subGO.AddComponent<VerticalLayoutGroup>();
            itemDetailUI.tagContainer = subGO.transform;
            GameObject prefabItemDetailUI = PrefabUtility.SaveAsPrefabAsset(go, Path.Combine(componentPath, go.name + ".prefab"));
            DestroyImmediate(go);

            // Create Component UI
            go = new GameObject("(" + themeName + ") Component UI");
            go.transform.SetParent(canvas.transform);
            SetFullSize(go);
            ComponentUI componentUI = go.AddComponent<ComponentUI>();
            subGO = new GameObject("Item Icon");
            subGO.transform.SetParent(go.transform);
            rt = SetFullSize(subGO);
            img = subGO.AddComponent<Image>();
            img.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            img.preserveAspect = true;
            rt.offsetMin = new Vector2(10, 10);
            rt.offsetMax = new Vector2(-10, -10);
            componentUI.itemImage = img;
            txtGO = new GameObject("Item Name");
            txtGO.transform.SetParent(go.transform);
            tmpro = CreateTextMeshPro(txtGO, "Item Name", true, TextAlignmentOptions.MidlineLeft);
            rt = CreateOrGetRectTransform(txtGO);
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(1, 1);
            rt.pivot = new Vector2(0.5f, 1);
            rt.offsetMax = rt.offsetMin = Vector2.zero;
            rt.sizeDelta = new Vector2(0, 20);
            componentUI.displayName = tmpro;
            txtGO = new GameObject("Item Description");
            txtGO.transform.SetParent(go.transform);
            tmpro = CreateTextMeshPro(txtGO, "Description", true, TextAlignmentOptions.MidlineLeft);
            rt = CreateOrGetRectTransform(txtGO);
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(1, 1);
            rt.pivot = new Vector2(0.5f, 1);
            rt.sizeDelta = new Vector2(0, 20);
            rt.anchoredPosition = new Vector2(0, -20);
            componentUI.description = tmpro;
            txtGO = new GameObject("Count Needed");
            txtGO.transform.SetParent(go.transform);
            tmpro = CreateTextMeshPro(txtGO, "0");
            rt = CreateOrGetRectTransform(txtGO);
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = new Vector2(0.5f, 0);
            rt.pivot = Vector2.zero;
            rt.offsetMax = rt.offsetMin = Vector2.zero;
            rt.sizeDelta = new Vector2(0, 20);
            componentUI.countNeeded = tmpro;
            txtGO = new GameObject("Count Available");
            txtGO.transform.SetParent(go.transform);
            tmpro = CreateTextMeshPro(txtGO, "0");
            rt = CreateOrGetRectTransform(txtGO);
            rt.anchorMin = new Vector2(0.5f, 0);
            rt.anchorMax = new Vector2(1, 0);
            rt.offsetMax = rt.offsetMin = Vector2.zero;
            rt.pivot = Vector2.zero;
            rt.sizeDelta = new Vector2(0, 20);
            componentUI.countAvailable = tmpro;
            subGO = new GameObject("Rarity Color Indicator");
            subGO.transform.SetParent(go.transform);
            rt = CreateOrGetRectTransform(subGO);
            rt.pivot = rt.anchorMin = rt.anchorMax = new Vector2(1, 1);
            rt.anchoredPosition = new Vector2(-5, -5);
            rt.sizeDelta = new Vector2(5, 5);
            img = subGO.AddComponent<Image>();
            img.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            img.preserveAspect = true;
            img.color = Color.yellow;
            rarityColor = subGO.AddComponent<RarityColorIndicator>();
            componentUI.rarityColorIndicator = rarityColor;
            subGO = new GameObject("Rarity Slider");
            subGO.transform.SetParent(go.transform);
            raritySlider = CreateProgressbar(subGO);
            rt = CreateOrGetRectTransform(subGO);
            rt.anchorMin = new Vector2(0, 0);
            rt.anchorMax = new Vector2(0.5f, 0);
            rt.pivot = Vector2.zero;
            rt.sizeDelta = new Vector2(0, 8);
            rt.anchoredPosition = new Vector2(0, 20);
            componentUI.raritySlider = raritySlider;
            subGO = new GameObject("Condition Slider");
            subGO.transform.SetParent(go.transform);
            conditionSlider = CreateProgressbar(subGO);
            rt = CreateOrGetRectTransform(subGO);
            rt.anchorMin = new Vector2(0.5f, 0);
            rt.anchorMax = new Vector2(1, 0);
            rt.pivot = Vector2.zero;
            rt.sizeDelta = new Vector2(0, 8);
            rt.anchoredPosition = new Vector2(0, 20);
            componentUI.conditionSlider = conditionSlider;
            GameObject prefabComponentUI = PrefabUtility.SaveAsPrefabAsset(go, Path.Combine(componentPath, go.name + ".prefab"));
            DestroyImmediate(go);

            // Create Component List
            go = new GameObject("(" + themeName + ") Component List");
            go.transform.SetParent(canvas.transform);
            SetFullSize(go);
            InventoryComponentList inventoryComponentList = go.AddComponent<InventoryComponentList>();
            inventoryComponentList.componentPrefab = prefabComponentUI.GetComponent<ComponentUI>();
            inventoryComponentList.componentContainer = go.transform;
            hlg = go.AddComponent<HorizontalLayoutGroup>();
            GameObject prefabComponentList = PrefabUtility.SaveAsPrefabAsset(go, Path.Combine(listPath, go.name + ".prefab"));
            DestroyImmediate(go);

            // Create Player Preview
            go = new GameObject("(" + themeName + ") Player Preview");
            go.transform.SetParent(canvas.transform);
            SetFullSize(go);
            RawImage rawImage = go.AddComponent<RawImage>();
            rawImage.texture = (Texture)AssetDatabase.LoadAssetAtPath(Path.Combine(renderTexPath, "Player Camera Render Texture.asset"), typeof(Texture));
            GameObject prefabPlayerPreview = PrefabUtility.SaveAsPrefabAsset(go, Path.Combine(componentPath, go.name + ".prefab"));
            DestroyImmediate(go);

            // Create Item Preview
            go = new GameObject("(" + themeName + ") Item Preview");
            go.transform.SetParent(canvas.transform);
            SetFullSize(go);
            rawImage = go.AddComponent<RawImage>();
            rawImage.texture = (Texture)AssetDatabase.LoadAssetAtPath(Path.Combine(renderTexPath, "Item Preview Render Texture.asset"), typeof(Texture));
            ItemPreviewUI itemPreviewUI = go.AddComponent<ItemPreviewUI>();
            subGO = new GameObject("Item Preview Camera");
            subGO.transform.SetParent(go.transform);
            camera = subGO.AddComponent<Camera>();
            camera.targetTexture = (RenderTexture)rawImage.texture;
            pnlGO = new GameObject("Item Preview Container");
            pnlGO.transform.SetParent(subGO.transform);
            itemPreviewUI.previewContainer = pnlGO.transform;
            GameObject prefabItemPreview = PrefabUtility.SaveAsPrefabAsset(go, Path.Combine(componentPath, go.name + ".prefab"));
            DestroyImmediate(go);

            // Create Inventory Context Buttons
            go = new GameObject("(" + themeName + ") Item List Context Buttons");
            go.transform.SetParent(canvas.transform);
            SetFullSize(go);
            hlg = go.AddComponent<HorizontalLayoutGroup>();
            hlg.childForceExpandHeight = true;
            hlg.childForceExpandWidth = true;
            hlg.childControlHeight = true;
            hlg.childControlWidth = true;
            CreateItemListContextButtons(go, prefabButton, null);
            GameObject prefabItemListButtons = PrefabUtility.SaveAsPrefabAsset(go, Path.Combine(componentPath, go.name + ".prefab"));
            DestroyImmediate(go);

            // Create Inventory Menu
            go = new GameObject("(" + themeName + ") Inventory Menu");
            go.transform.SetParent(canvas.transform);
            SetFullSize(go);
            InventoryMenuUI inventoryMenu = go.AddComponent<InventoryMenuUI>();
            inventoryMenu.openMode = (NavigationType)invOpenMode;
            inventoryMenu.openButton = invOpenButton;
            inventoryMenu.openKey = invOpenKey;
            inventoryMenu.closeMode = (NavigationType)invCloseMode;
            inventoryMenu.closeButton = invCloseButton;
            inventoryMenu.closeKey = invCloseKey;
            inventoryMenu.overrideSpawn = invOverrideSpawn;
            inventoryMenu.overrideTag = invOverrideTag;
            GameObject windowGO = new GameObject("Window");
            windowGO.transform.SetParent(go.transform);
            SetFullSize(windowGO);
            img = windowGO.AddComponent<Image>();
            img.color = Color.black;
            if(invCatList)
            {
                subGO = PrefabUtility.InstantiatePrefab(prefabCategoryListPaged, windowGO.transform) as GameObject;
                rt = CreateOrGetRectTransform(subGO);
                rt.pivot = new Vector2(0, 1);
                rt.anchorMin = new Vector2(0, 1);
                rt.anchorMax = new Vector2(1, 1);
                rt.offsetMax = rt.offsetMin = Vector2.zero;
                rt.sizeDelta = new Vector2(rt.sizeDelta.x, 60);
            }
            GameObject contentGO = new GameObject("Inventory");
            contentGO.transform.SetParent(windowGO.transform);
            SetFullSize(contentGO);
            hlg = contentGO.AddComponent<HorizontalLayoutGroup>();
            hlg.childForceExpandHeight = true;
            hlg.childForceExpandWidth = true;
            hlg.childControlHeight = true;
            hlg.childControlWidth = true;
            rt = CreateOrGetRectTransform(contentGO);
            rt.offsetMin = new Vector2(0, invContextBtns ? 60 : 0);
            rt.offsetMax = new Vector2(0, invCatList ? -60 : 0);
            InventoryItemList baseList = null;
            baseList = CreateListType(invListType, contentGO.transform, prefabItemScrollGrid, prefabItemPagedGrid, prefabItemScrollList, out rt);
            baseList.enableDragDrop = invDragDrop;
            if(invItemDetails)
            {
                subGO = PrefabUtility.InstantiatePrefab(prefabItemDetailUI, contentGO.transform) as GameObject;
                baseList.detailClient = subGO.GetComponent<ItemDetailUI>();
            }
            if (invCharPreview)
            {
                subGO = PrefabUtility.InstantiatePrefab(prefabPlayerPreview, contentGO.transform) as GameObject;
            }
            if(invItemPreview)
            {
                subGO = PrefabUtility.InstantiatePrefab(prefabItemPreview, contentGO.transform) as GameObject;
            }
            if(invContextBtns)
            {
                GameObject btnGO = PrefabUtility.InstantiatePrefab(prefabItemListButtons, windowGO.transform) as GameObject;
                rt = CreateOrGetRectTransform(btnGO);
                rt.pivot = rt.anchorMin = Vector2.zero;
                rt.anchorMax = new Vector2(1, 0);
                rt.offsetMax = rt.offsetMin = Vector2.zero;
                rt.sizeDelta = new Vector2(rt.sizeDelta.x, 60);
                btnGO.GetComponent<ItemListContextButtons>().targetList = baseList;
            }
            GameObject prefabInventoryMenuUI = PrefabUtility.SaveAsPrefabAsset(go, Path.Combine(windowsPath, go.name + ".prefab"));
            DestroyImmediate(go);
            themeEditor.serializedObject.FindProperty("inventoryMenu").objectReferenceValue = prefabInventoryMenuUI.GetComponent<InventoryMenuUI>();

            // Create Item Manager Context Buttons
            go = new GameObject("(" + themeName + ") Item Context Buttons");
            go.transform.SetParent(canvas.transform);
            SetFullSize(go);
            hlg = go.AddComponent<HorizontalLayoutGroup>();
            hlg.childForceExpandHeight = true;
            hlg.childForceExpandWidth = true;
            hlg.childControlHeight = true;
            hlg.childControlWidth = true;
            CreateItemContextButtons(go, prefabButton);
            GameObject prefabItemButtons = PrefabUtility.SaveAsPrefabAsset(go, Path.Combine(componentPath, go.name + ".prefab"));
            DestroyImmediate(go);

            // Create Item Manager
            go = new GameObject("(" + themeName + ") Item Manager Menu");
            go.transform.SetParent(canvas.transform);
            SetFullSize(go);
            ItemManagerMenuUI itemManager = go.AddComponent<ItemManagerMenuUI>();
            itemManager.closeMode = (NavigationType)immCloseMode;
            itemManager.closeButton = immCloseButton;
            itemManager.closeKey = immCloseKey;
            windowGO = new GameObject("Window");
            windowGO.transform.SetParent(go.transform);
            rt = CreateOrGetRectTransform(windowGO);
            rt.anchorMin = new Vector2(0.3036563f, 0.2932231f);
            rt.anchorMax = new Vector2(0.6955625f, 0.707f);
            rt.offsetMax = rt.offsetMin = Vector2.zero;
            img = windowGO.AddComponent<Image>();
            img.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            img.type = Image.Type.Sliced;
            img.fillCenter = true;
            pnlGO = PrefabUtility.InstantiatePrefab(prefabItemDetailUI, windowGO.transform) as GameObject;
            rt = CreateOrGetRectTransform(pnlGO);
            rt.offsetMin = new Vector2(0, immRepairCosts ? 120 : 60);
            rt.offsetMax = Vector2.zero;
            if(immRepairCosts)
            {
                pnlGO = PrefabUtility.InstantiatePrefab(prefabComponentList, windowGO.transform) as GameObject;
                pnlGO.transform.SetParent(windowGO.transform);
                rt = CreateOrGetRectTransform(pnlGO);
                rt.pivot = new Vector2(0.5f, 0);
                rt.anchorMin = Vector2.zero;
                rt.anchorMax = new Vector2(1, 0);
                rt.offsetMax = Vector2.zero;
                rt.offsetMin = new Vector2(0, 30);
                rt.sizeDelta = new Vector2(rt.sizeDelta.x, 30);
                inventoryComponentList = pnlGO.GetComponentInChildren<InventoryComponentList>();
                inventoryComponentList.listType = ComponentListType.Repair;
            }
            pnlGO = PrefabUtility.InstantiatePrefab(prefabItemButtons, windowGO.transform) as GameObject;
            pnlGO.transform.SetParent(windowGO.transform);
            rt = CreateOrGetRectTransform(pnlGO);
            rt.pivot = new Vector2(0.5f, 0);
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = new Vector2(1, 0);
            rt.offsetMax = rt.offsetMin = Vector2.zero;
            rt.sizeDelta = new Vector2(rt.sizeDelta.x, 30);
            pnlGO.GetComponentInChildren<ItemContextButtons>().itemManagerMenu = itemManager;
            GameObject prefabItemManager = PrefabUtility.SaveAsPrefabAsset(go, Path.Combine(windowsPath, go.name + ".prefab"));
            DestroyImmediate(go);
            themeEditor.serializedObject.FindProperty("itemManagerMenu").objectReferenceValue = prefabItemManager.GetComponent<ItemManagerMenuUI>();

            // Create Item Container Menu
            go = new GameObject("(" + themeName + ") Item Container Menu");
            go.transform.SetParent(canvas.transform);
            SetFullSize(go);
            ItemContainerMenuUI itemContainerMenu = go.AddComponent<ItemContainerMenuUI>();
            windowGO = new GameObject("Window");
            windowGO.transform.SetParent(go.transform);
            SetFullSize(windowGO);
            img = windowGO.AddComponent<Image>();
            img.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            img.type = Image.Type.Sliced;
            img.fillCenter = true;
            contentGO = new GameObject("Content");
            contentGO.transform.SetParent(windowGO.transform);
            SetFullSize(contentGO);
            hlg = contentGO.AddComponent<HorizontalLayoutGroup>();
            hlg.childForceExpandHeight = true;
            hlg.childForceExpandWidth = true;
            hlg.childControlHeight = true;
            hlg.childControlWidth = true;
            if(icmCatList || icmContextBtns)
            {
                subGO = new GameObject("Container UI");
                subGO.transform.SetParent(contentGO.transform);
                CreateOrGetRectTransform(subGO);

                if (icmCatList)
                {
                    txtGO = PrefabUtility.InstantiatePrefab(prefabCategoryListPaged, subGO.transform) as GameObject;
                    rt = CreateOrGetRectTransform(txtGO);
                    rt.anchorMin = new Vector2(0, 0.9f);
                    rt.anchorMax = new Vector2(1, 1);
                    rt.offsetMax = rt.offsetMin = Vector2.zero;
                    categoryList = txtGO.GetComponent<CategoryList>();
                }

                baseList = CreateListType(icmListType, subGO.transform, prefabItemScrollGrid, prefabItemPagedGrid, prefabItemScrollList, out rt);

                if (icmCatList) { categoryList.bindInventory = baseList; }
                rt.anchorMin = new Vector2(0, icmContextBtns ? 0.1f : 0);
                rt.anchorMax = new Vector2(1, icmCatList ? 0.9f : 1f);
                rt.offsetMax = rt.offsetMin = Vector2.zero;

                if(icmContextBtns)
                {
                    txtGO = PrefabUtility.InstantiatePrefab(prefabItemListButtons, subGO.transform) as GameObject;
                    rt = CreateOrGetRectTransform(txtGO);
                    rt.anchorMin = Vector2.zero;
                    rt.anchorMax = new Vector2(1, 0.1f);
                    rt.offsetMax = rt.offsetMin = Vector2.zero;
                    txtGO.GetComponent<ItemListContextButtons>().targetList = baseList;
                }
            }
            else
            {
                baseList = CreateListType(icmListType, contentGO.transform, prefabItemScrollGrid, prefabItemPagedGrid, prefabItemScrollList, out rt);
            }
            baseList.listSource = ListSource.ContainerItem;
            baseList.enableDragDrop = icmDragDrop;
            itemContainerMenu.itemList = baseList;
            if (icmItemDetails)
            {
                subGO = PrefabUtility.InstantiatePrefab(prefabItemDetailUI, contentGO.transform) as GameObject;
                baseList.detailClient = subGO.GetComponent<ItemDetailUI>();
            }
            if (icmCharPreview)
            {
                subGO = PrefabUtility.InstantiatePrefab(prefabPlayerPreview, contentGO.transform) as GameObject;
            }
            if (icmItemPreview)
            {
                subGO = PrefabUtility.InstantiatePrefab(prefabItemPreview, contentGO.transform) as GameObject;
            }
            if(icmPlShow)
            {
                if (icmPlCatList || icmPlContextBtns)
                {
                    subGO = new GameObject("Player UI");
                    subGO.transform.SetParent(contentGO.transform);
                    CreateOrGetRectTransform(subGO);
                    if (icmPlCatList)
                    {
                        txtGO = PrefabUtility.InstantiatePrefab(prefabCategoryListPaged, subGO.transform) as GameObject;
                        rt = CreateOrGetRectTransform(txtGO);
                        rt.anchorMin = new Vector2(0, 0.9f);
                        rt.anchorMax = new Vector2(1, 1);
                        rt.offsetMax = rt.offsetMin = Vector2.zero;
                        categoryList = txtGO.GetComponent<CategoryList>();
                    }
                    baseList = CreateListType(icmPlListType, subGO.transform, prefabItemScrollGrid, prefabItemPagedGrid, prefabItemScrollList, out rt);
                    if (icmPlCatList) { categoryList.bindInventory = baseList; }
                    rt.anchorMin = new Vector2(0, icmPlContextBtns ? 0.1f : 0);
                    rt.anchorMax = new Vector2(1, icmPlCatList ? 0.9f : 1f);
                    rt.offsetMax = rt.offsetMin = Vector2.zero;
                    if (icmPlContextBtns)
                    {
                        txtGO = PrefabUtility.InstantiatePrefab(prefabItemListButtons, subGO.transform) as GameObject;
                        rt = CreateOrGetRectTransform(txtGO);
                        rt.anchorMin = Vector2.zero;
                        rt.anchorMax = new Vector2(1, 0.1f);
                        rt.offsetMax = rt.offsetMin = Vector2.zero;
                        txtGO.GetComponent<ItemListContextButtons>().targetList = baseList;
                    }
                }
                else
                {
                    baseList = CreateListType(icmPlListType, contentGO.transform, prefabItemScrollGrid, prefabItemPagedGrid, prefabItemScrollList, out rt);
                }
                baseList.enableDragDrop = icmPlDragDrop;
                itemContainerMenu.inventoryList = baseList;
            }
            GameObject prefabItemContainerMenu = PrefabUtility.SaveAsPrefabAsset(go, Path.Combine(windowsPath, go.name + ".prefab"));
            DestroyImmediate(go);
            themeEditor.serializedObject.FindProperty("itemContainerMenu").objectReferenceValue = prefabItemContainerMenu.GetComponent<ItemContainerMenuUI>();

            // Create Environment Container Menu
            go = new GameObject("(" + themeName + ") Environment Container Menu");
            go.transform.SetParent(canvas.transform);
            SetFullSize(go);
            ContainerMenuUI containerMenu = go.AddComponent<ContainerMenuUI>();
            windowGO = new GameObject("Window");
            windowGO.transform.SetParent(go.transform);
            SetFullSize(windowGO);
            img = windowGO.AddComponent<Image>();
            img.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            img.type = Image.Type.Sliced;
            img.fillCenter = true;
            contentGO = new GameObject("Content");
            contentGO.transform.SetParent(windowGO.transform);
            SetFullSize(contentGO);
            hlg = contentGO.AddComponent<HorizontalLayoutGroup>();
            hlg.childForceExpandHeight = true;
            hlg.childForceExpandWidth = true;
            hlg.childControlHeight = true;
            hlg.childControlWidth = true;
            if (ecmCatList || ecmContextBtns)
            {
                subGO = new GameObject("Container UI");
                subGO.transform.SetParent(contentGO.transform);
                CreateOrGetRectTransform(subGO);

                if (ecmCatList)
                {
                    txtGO = PrefabUtility.InstantiatePrefab(prefabCategoryListPaged, subGO.transform) as GameObject;
                    rt = CreateOrGetRectTransform(txtGO);
                    rt.anchorMin = new Vector2(0, 0.9f);
                    rt.anchorMax = new Vector2(1, 1);
                    rt.offsetMax = rt.offsetMin = Vector2.zero;
                    categoryList = txtGO.GetComponent<CategoryList>();
                }

                baseList = CreateListType(ecmListType, subGO.transform, prefabItemScrollGrid, prefabItemPagedGrid, prefabItemScrollList, out rt);

                if (ecmCatList) { categoryList.bindInventory = baseList; }
                rt.anchorMin = new Vector2(0, ecmContextBtns ? 0.1f : 0);
                rt.anchorMax = new Vector2(1, ecmCatList ? 0.9f : 1f);
                rt.offsetMax = rt.offsetMin = Vector2.zero;

                if (ecmContextBtns)
                {
                    txtGO = PrefabUtility.InstantiatePrefab(prefabItemListButtons, subGO.transform) as GameObject;
                    rt = CreateOrGetRectTransform(txtGO);
                    rt.anchorMin = Vector2.zero;
                    rt.anchorMax = new Vector2(1, 0.1f);
                    rt.offsetMax = rt.offsetMin = Vector2.zero;
                    txtGO.GetComponent<ItemListContextButtons>().targetList = baseList;
                }
            }
            else
            {
                baseList = CreateListType(ecmListType, contentGO.transform, prefabItemScrollGrid, prefabItemPagedGrid, prefabItemScrollList, out rt);
            }
            baseList.listSource = ListSource.InventoryContainer;
            baseList.enableDragDrop = ecmDragDrop;
            containerMenu.containerInventory = baseList;
            if (ecmItemDetails)
            {
                subGO = PrefabUtility.InstantiatePrefab(prefabItemDetailUI, contentGO.transform) as GameObject;
                baseList.detailClient = subGO.GetComponent<ItemDetailUI>();
            }
            if (ecmItemPreview)
            {
                subGO = PrefabUtility.InstantiatePrefab(prefabItemPreview, contentGO.transform) as GameObject;
            }
            if (ecmPlShow)
            {
                if (ecmPlCatList || ecmPlContextBtns)
                {
                    subGO = new GameObject("Player UI");
                    subGO.transform.SetParent(contentGO.transform);
                    CreateOrGetRectTransform(subGO);
                    if (ecmPlCatList)
                    {
                        txtGO = PrefabUtility.InstantiatePrefab(prefabCategoryListPaged, subGO.transform) as GameObject;
                        rt = CreateOrGetRectTransform(txtGO);
                        rt.anchorMin = new Vector2(0, 0.9f);
                        rt.anchorMax = new Vector2(1, 1);
                        rt.offsetMax = rt.offsetMin = Vector2.zero;
                        categoryList = txtGO.GetComponent<CategoryList>();
                    }
                    baseList = CreateListType(ecmPlListType, subGO.transform, prefabItemScrollGrid, prefabItemPagedGrid, prefabItemScrollList, out rt);
                    if (ecmPlCatList) { categoryList.bindInventory = baseList; }
                    rt.anchorMin = new Vector2(0, ecmPlContextBtns ? 0.1f : 0);
                    rt.anchorMax = new Vector2(1, ecmPlCatList ? 0.9f : 1f);
                    rt.offsetMax = rt.offsetMin = Vector2.zero;
                    if (ecmPlContextBtns)
                    {
                        txtGO = PrefabUtility.InstantiatePrefab(prefabItemListButtons, subGO.transform) as GameObject;
                        rt = CreateOrGetRectTransform(txtGO);
                        rt.anchorMin = Vector2.zero;
                        rt.anchorMax = new Vector2(1, 0.1f);
                        rt.offsetMax = rt.offsetMin = Vector2.zero;
                        txtGO.GetComponent<ItemListContextButtons>().targetList = baseList;
                    }
                }
                else
                {
                    baseList = CreateListType(ecmPlListType, contentGO.transform, prefabItemScrollGrid, prefabItemPagedGrid, prefabItemScrollList, out rt);
                }
                baseList.enableDragDrop = ecmPlDragDrop;
                containerMenu.localInventory = baseList;
            }
            GameObject prefabContainerMenu = PrefabUtility.SaveAsPrefabAsset(go, Path.Combine(windowsPath, go.name + ".prefab"));
            DestroyImmediate(go);
            themeEditor.serializedObject.FindProperty("containerMenu").objectReferenceValue = prefabContainerMenu.GetComponent<ContainerMenuUI>();

            // Create Environment Container Menu
            go = new GameObject("(" + themeName + ") Merchant Menu");
            go.transform.SetParent(canvas.transform);
            SetFullSize(go);
            MerchantMenuUI merchantMenu = go.AddComponent<MerchantMenuUI>();
            windowGO = new GameObject("Window");
            windowGO.transform.SetParent(go.transform);
            SetFullSize(windowGO);
            img = windowGO.AddComponent<Image>();
            img.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            img.type = Image.Type.Sliced;
            img.fillCenter = true;
            contentGO = new GameObject("Content");
            contentGO.transform.SetParent(windowGO.transform);
            SetFullSize(contentGO);
            hlg = contentGO.AddComponent<HorizontalLayoutGroup>();
            hlg.childForceExpandHeight = true;
            hlg.childForceExpandWidth = true;
            hlg.childControlHeight = true;
            hlg.childControlWidth = true;
            if (mmCatList)
            {
                subGO = new GameObject("Merchant UI");
                subGO.transform.SetParent(contentGO.transform);
                CreateOrGetRectTransform(subGO);

                if (mmCatList)
                {
                    txtGO = PrefabUtility.InstantiatePrefab(prefabCategoryListPaged, subGO.transform) as GameObject;
                    rt = CreateOrGetRectTransform(txtGO);
                    rt.anchorMin = new Vector2(0, 0.9f);
                    rt.anchorMax = new Vector2(1, 1);
                    rt.offsetMax = rt.offsetMin = Vector2.zero;
                    categoryList = txtGO.GetComponent<CategoryList>();
                }

                baseList = CreateListType(mmListType, subGO.transform, prefabItemScrollGrid, prefabItemPagedGrid, prefabItemScrollList, out rt);

                if (mmCatList) { categoryList.bindInventory = baseList; }
                rt.anchorMin = Vector2.zero;
                rt.anchorMax = new Vector2(1, mmCatList ? 0.9f : 1f);
                rt.offsetMax = rt.offsetMin = Vector2.zero;
            }
            else
            {
                baseList = CreateListType(mmListType, contentGO.transform, prefabItemScrollGrid, prefabItemPagedGrid, prefabItemScrollList, out rt);
            }
            baseList.listSource = ListSource.InventoryMerchant;
            baseList.enableDragDrop = mmDragDrop;
            merchantMenu.merchantList = baseList;
            if (mmItemDetails)
            {
                subGO = PrefabUtility.InstantiatePrefab(prefabItemDetailUI, contentGO.transform) as GameObject;
                baseList.detailClient = subGO.GetComponent<ItemDetailUI>();
            }
            if (mmItemPreview)
            {
                subGO = PrefabUtility.InstantiatePrefab(prefabItemPreview, contentGO.transform) as GameObject;
            }
            if (mmPlShow)
            {
                if (mmPlCatList)
                {
                    subGO = new GameObject("Player UI");
                    subGO.transform.SetParent(contentGO.transform);
                    CreateOrGetRectTransform(subGO);
                    if (mmPlCatList)
                    {
                        txtGO = PrefabUtility.InstantiatePrefab(prefabCategoryListPaged, subGO.transform) as GameObject;
                        rt = CreateOrGetRectTransform(txtGO);
                        rt.anchorMin = new Vector2(0, 0.9f);
                        rt.anchorMax = new Vector2(1, 1);
                        rt.offsetMax = rt.offsetMin = Vector2.zero;
                        categoryList = txtGO.GetComponent<CategoryList>();
                    }
                    baseList = CreateListType(mmPlListType, subGO.transform, prefabItemScrollGrid, prefabItemPagedGrid, prefabItemScrollList, out rt);
                    if (mmPlCatList) { categoryList.bindInventory = baseList; }
                    rt.anchorMin = Vector2.zero;
                    rt.anchorMax = new Vector2(1, mmPlCatList ? 0.9f : 1f);
                    rt.offsetMax = rt.offsetMin = Vector2.zero;
                }
                else
                {
                    baseList = CreateListType(mmPlListType, contentGO.transform, prefabItemScrollGrid, prefabItemPagedGrid, prefabItemScrollList, out rt);
                }
                baseList.enableDragDrop = mmPlDragDrop;
                merchantMenu.playerList = baseList;
            }
            GameObject prefabMerchantMenu = PrefabUtility.SaveAsPrefabAsset(go, Path.Combine(windowsPath, go.name + ".prefab"));
            DestroyImmediate(go);
            themeEditor.serializedObject.FindProperty("merchantMenu").objectReferenceValue = prefabMerchantMenu.GetComponent<MerchantMenuUI>();

            // Create Trade Menu
            go = new GameObject("(" + themeName + ") Trade Menu");
            go.transform.SetParent(canvas.transform);
            SetFullSize(go);
            TradeMenuUI tradeMenu = go.AddComponent<TradeMenuUI>();
            subGO = new GameObject("Window");
            subGO.transform.SetParent(go.transform);
            rt = CreateOrGetRectTransform(subGO);
            rt.anchorMin = new Vector2(0.3f, 0.3f);
            rt.anchorMax = new Vector2(0.7f, 0.7f);
            rt.offsetMax = rt.offsetMin = Vector2.zero;
            img = subGO.AddComponent<Image>();
            img.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            img.type = Image.Type.Sliced;
            img.fillCenter = true;
            txtGO = new GameObject("Title");
            txtGO.transform.SetParent(subGO.transform);
            tmpro = CreateTextMeshPro(txtGO, "{Title}");
            tradeMenu.tradeType = tmpro;
            rt = CreateOrGetRectTransform(txtGO);
            rt.anchorMin = new Vector2(0, 0.8190001f);
            rt.anchorMax = new Vector2(1, 0.9548612f);
            rt.offsetMax = rt.offsetMin = Vector2.zero;
            pnlGO = new GameObject("Details");
            pnlGO.transform.SetParent(subGO.transform);
            rt = CreateOrGetRectTransform(pnlGO);
            rt.anchorMin = new Vector2(0.048f, 0.377f);
            rt.anchorMax = new Vector2(0.952f, 0.763f);
            rt.offsetMax = rt.offsetMin = Vector2.zero;
            txtGO = new GameObject("Total Cost");
            txtGO.transform.SetParent(pnlGO.transform);
            tmpro = CreateTextMeshPro(txtGO, "{Cost}");
            rt = CreateOrGetRectTransform(txtGO);
            rt.anchorMin = new Vector2(0, 0.554f);
            rt.anchorMax = new Vector2(1, 1);
            rt.offsetMax = rt.offsetMin = Vector2.zero;
            tradeMenu.totalCost = tmpro;
            txtGO = new GameObject("Slider");
            txtGO.transform.SetParent(pnlGO.transform);
            slider = CreateSlider(txtGO);
            rt = CreateOrGetRectTransform(txtGO);
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = new Vector2(1, 0.41f);
            rt.offsetMin = new Vector2(30, 0);
            rt.offsetMax = new Vector2(-30, 0);
            tradeMenu.countSlider = slider;
            txtGO = new GameObject("Min Count");
            txtGO.transform.SetParent(pnlGO.transform);
            tmpro = CreateTextMeshPro(txtGO, "0");
            rt = CreateOrGetRectTransform(txtGO);
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = new Vector2(0, 0.41f);
            rt.offsetMax = rt.offsetMin = Vector2.zero;
            rt.pivot = new Vector2(0, 0.5f);
            rt.sizeDelta = new Vector2(30, 0);
            tradeMenu.minCount = tmpro;
            txtGO = new GameObject("Max Count");
            txtGO.transform.SetParent(pnlGO.transform);
            tmpro = CreateTextMeshPro(txtGO, "0");
            rt = CreateOrGetRectTransform(txtGO);
            rt.anchorMin = new Vector2(1, 0);
            rt.anchorMax = new Vector2(1, 0.41f);
            rt.offsetMax = rt.offsetMin = Vector2.zero;
            rt.pivot = new Vector2(1, 0.5f);
            rt.sizeDelta = new Vector2(30, 0);
            tradeMenu.maxCount = tmpro;
            txtGO = new GameObject("Current Count");
            txtGO.transform.SetParent(slider.handleRect);
            tmpro = CreateTextMeshPro(txtGO, "0");
            SetFullSize(txtGO);
            tradeMenu.curCount = tmpro;
            pnlGO = new GameObject("Buttons");
            pnlGO.transform.SetParent(subGO.transform);
            rt = CreateOrGetRectTransform(pnlGO);
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = new Vector2(1, 0.2655289f);
            rt.offsetMin = new Vector2(16, 16);
            rt.offsetMax = new Vector2(-16, 0);
            hlg = pnlGO.AddComponent<HorizontalLayoutGroup>();
            hlg.spacing = 64;
            hlg.childForceExpandHeight = hlg.childForceExpandWidth = hlg.childControlHeight = hlg.childControlWidth = true;
            txtGO = PrefabUtility.InstantiatePrefab(prefabButton, pnlGO.transform) as GameObject;
            txtGO.GetComponentInChildren<TextMeshProUGUI>().text = "Buy";
            targetinfo = UnityEvent.GetValidMethodInfo(tradeMenu, "ConfirmTrade", new Type[0]);
            action = Delegate.CreateDelegate(typeof(UnityAction), tradeMenu, targetinfo, false) as UnityAction;
            UnityEventTools.AddPersistentListener(txtGO.GetComponentInChildren<Button>().onClick, action);
            txtGO = PrefabUtility.InstantiatePrefab(prefabButton, pnlGO.transform) as GameObject;
            txtGO.GetComponentInChildren<TextMeshProUGUI>().text = "Cancel";
            targetinfo = UnityEvent.GetValidMethodInfo(tradeMenu, "CancelTrade", new Type[0]);
            action = Delegate.CreateDelegate(typeof(UnityAction), tradeMenu, targetinfo, false) as UnityAction;
            UnityEventTools.AddPersistentListener(txtGO.GetComponentInChildren<Button>().onClick, action);
            GameObject prefabTradeMenu = PrefabUtility.SaveAsPrefabAsset(go, Path.Combine(windowsPath, go.name + ".prefab"));
            themeEditor.serializedObject.FindProperty("tradeMenu").objectReferenceValue = prefabTradeMenu.GetComponent<TradeMenuUI>();
            DestroyImmediate(go);

            // Create Craft Detail Menu
            go = new GameObject("(" + themeName + ") Craft Detail Menu");
            go.transform.SetParent(canvas.transform);
            SetFullSize(go);
            CraftDetailMenuUI craftDetailMenu = go.AddComponent<CraftDetailMenuUI>();
            recipeUI = go.AddComponent<RecipeUI>();
            craftDetailMenu.recipeUI = recipeUI;
            subGO = new GameObject("Window");
            subGO.transform.SetParent(go.transform);
            rt = CreateOrGetRectTransform(subGO);
            rt.anchorMin = new Vector2(0.3f, 0.3f);
            rt.anchorMax = new Vector2(0.7f, 0.7f);
            rt.offsetMax = rt.offsetMin = Vector2.zero;
            img = subGO.AddComponent<Image>();
            img.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            img.type = Image.Type.Sliced;
            img.fillCenter = true;
            txtGO = new GameObject("Display Name");
            txtGO.transform.SetParent(subGO.transform);
            tmpro = CreateTextMeshPro(txtGO, "{Display Name}");
            recipeUI.displayName = tmpro;
            rt = CreateOrGetRectTransform(txtGO);
            rt.anchorMin = new Vector2(0, 0.8190001f);
            rt.anchorMax = new Vector2(1, 0.9548612f);
            rt.offsetMax = rt.offsetMin = Vector2.zero;
            pnlGO = new GameObject("Details");
            pnlGO.transform.SetParent(subGO.transform);
            rt = CreateOrGetRectTransform(pnlGO);
            rt.anchorMin = new Vector2(0.048f, 0.377f);
            rt.anchorMax = new Vector2(0.952f, 0.763f);
            rt.offsetMax = rt.offsetMin = Vector2.zero;
            txtGO = new GameObject("Duration");
            txtGO.transform.SetParent(pnlGO.transform);
            tmpro = CreateTextMeshPro(txtGO, "{Duration}");
            rt = CreateOrGetRectTransform(txtGO);
            rt.anchorMin = new Vector2(0, 0.554f);
            rt.anchorMax = new Vector2(1, 1);
            rt.offsetMax = rt.offsetMin = Vector2.zero;
            recipeUI.duration = tmpro;
            txtGO = new GameObject("Slider");
            txtGO.transform.SetParent(pnlGO.transform);
            slider = CreateSlider(txtGO);
            rt = CreateOrGetRectTransform(txtGO);
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = new Vector2(1, 0.41f);
            rt.offsetMin = new Vector2(30, 0);
            rt.offsetMax = new Vector2(-30, 0);
            craftDetailMenu.countSlider = slider;
            txtGO = new GameObject("Min Count");
            txtGO.transform.SetParent(pnlGO.transform);
            tmpro = CreateTextMeshPro(txtGO, "0");
            rt = CreateOrGetRectTransform(txtGO);
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = new Vector2(0, 0.41f);
            rt.offsetMax = rt.offsetMin = Vector2.zero;
            rt.pivot = new Vector2(0, 0.5f);
            rt.sizeDelta = new Vector2(30, 0);
            craftDetailMenu.minCount = tmpro;
            txtGO = new GameObject("Max Count");
            txtGO.transform.SetParent(pnlGO.transform);
            tmpro = CreateTextMeshPro(txtGO, "0");
            rt = CreateOrGetRectTransform(txtGO);
            rt.anchorMin = new Vector2(1, 0);
            rt.anchorMax = new Vector2(1, 0.41f);
            rt.offsetMax = rt.offsetMin = Vector2.zero;
            rt.pivot = new Vector2(1, 0.5f);
            rt.sizeDelta = new Vector2(30, 0);
            craftDetailMenu.maxCount = tmpro;
            txtGO = new GameObject("Current Count");
            txtGO.transform.SetParent(slider.handleRect);
            tmpro = CreateTextMeshPro(txtGO, "0");
            SetFullSize(txtGO);
            craftDetailMenu.curCount = tmpro;
            pnlGO = new GameObject("Buttons");
            pnlGO.transform.SetParent(subGO.transform);
            rt = CreateOrGetRectTransform(pnlGO);
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = new Vector2(1, 0.2655289f);
            rt.offsetMin = new Vector2(16, 16);
            rt.offsetMax = new Vector2(-16, 0);
            hlg = pnlGO.AddComponent<HorizontalLayoutGroup>();
            hlg.spacing = 64;
            hlg.childForceExpandHeight = hlg.childForceExpandWidth = hlg.childControlHeight = hlg.childControlWidth = true;
            txtGO = PrefabUtility.InstantiatePrefab(prefabButton, pnlGO.transform) as GameObject;
            txtGO.GetComponentInChildren<TextMeshProUGUI>().text = "Craft";
            targetinfo = UnityEvent.GetValidMethodInfo(craftDetailMenu, "ConfirmCraft", new Type[0]);
            action = Delegate.CreateDelegate(typeof(UnityAction), craftDetailMenu, targetinfo, false) as UnityAction;
            UnityEventTools.AddPersistentListener(txtGO.GetComponentInChildren<Button>().onClick, action);
            txtGO = PrefabUtility.InstantiatePrefab(prefabButton, pnlGO.transform) as GameObject;
            txtGO.GetComponentInChildren<TextMeshProUGUI>().text = "Cancel";
            targetinfo = UnityEvent.GetValidMethodInfo(craftDetailMenu, "CancelCraft", new Type[0]);
            action = Delegate.CreateDelegate(typeof(UnityAction), craftDetailMenu, targetinfo, false) as UnityAction;
            UnityEventTools.AddPersistentListener(txtGO.GetComponentInChildren<Button>().onClick, action);
            GameObject prefabCraftDetailMenu = PrefabUtility.SaveAsPrefabAsset(go, Path.Combine(windowsPath, go.name + ".prefab"));
            themeEditor.serializedObject.FindProperty("craftDetailMenu").objectReferenceValue = prefabCraftDetailMenu.GetComponent<CraftDetailMenuUI>();
            DestroyImmediate(go);

            // Recipe Item Grid
            go = new GameObject("(" + themeName + ") Recipe Item Grid");
            go.transform.SetParent(canvas.transform);
            SetFullSize(go);
            RecipeItemGrid recipeItemGrid = go.AddComponent<RecipeItemGrid>();
            recipeItemGrid.itemUIPrefab = prefabRecipeUI.GetComponentInChildren<RecipeUI>();
            GameObject prefabRecipeItemGrid = PrefabUtility.SaveAsPrefabAsset(go, Path.Combine(listPath, go.name + ".prefab"));
            DestroyImmediate(go);

            // Crafting Menu
            go = new GameObject("(" + themeName + ") Crafting Menu");
            go.transform.SetParent(canvas.transform);
            SetFullSize(go);
            CraftMenuUI craftMenu = go.AddComponent<CraftMenuUI>();
            craftMenu.craftingType = (CraftMenuUI.CraftingType)cmCraftType;
            windowGO = new GameObject("Window");
            windowGO.transform.SetParent(go.transform);
            SetFullSize(windowGO);
            img = windowGO.AddComponent<Image>();
            img.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            img.type = Image.Type.Sliced;
            img.fillCenter = true;
            contentGO = new GameObject("Content");
            contentGO.transform.SetParent(windowGO.transform);
            SetFullSize(contentGO);
            hlg = contentGO.AddComponent<HorizontalLayoutGroup>();
            hlg.childForceExpandHeight = true;
            hlg.childForceExpandWidth = true;
            hlg.childControlHeight = true;
            hlg.childControlWidth = true;
            if (craftMenu.craftingType == CraftMenuUI.CraftingType.Normal)
            {
                // Normal
                if (cmShowContext)
                {
                    subGO = new GameObject("Craft List");
                    subGO.transform.SetParent(contentGO.transform);
                    rt = CreateOrGetRectTransform(subGO);

                    pnlGO = PrefabUtility.InstantiatePrefab(prefabRecipeItemGrid, subGO.transform) as GameObject;
                    rt = CreateOrGetRectTransform(pnlGO);
                    rt.anchorMin = new Vector2(0, 0.1f);
                    rt.anchorMax = new Vector2(1, 1);
                    rt.offsetMax = rt.offsetMin = Vector2.zero;
                    craftMenu.recipeGrid = pnlGO.GetComponentInChildren<RecipeItemGrid>();

                    pnlGO = new GameObject("Context Buttons");
                    pnlGO.transform.SetParent(subGO.transform);
                    rt = CreateOrGetRectTransform(pnlGO);
                    rt.anchorMin = Vector2.zero;
                    rt.anchorMax = new Vector2(1, 0.1f);
                    rt.offsetMax = rt.offsetMin = Vector2.zero;
                    hlg = pnlGO.AddComponent<HorizontalLayoutGroup>();
                    hlg.childForceExpandHeight = true;
                    hlg.childForceExpandWidth = true;
                    hlg.childControlHeight = true;
                    hlg.childControlWidth = true;

                    txtGO = PrefabUtility.InstantiatePrefab(prefabButton, pnlGO.transform) as GameObject;
                    txtGO.GetComponentInChildren<TextMeshProUGUI>().text = "Craft";
                    targetinfo = UnityEvent.GetValidMethodInfo(craftMenu.recipeGrid, "CraftSelected", new Type[0]);
                    action = Delegate.CreateDelegate(typeof(UnityAction), craftMenu.recipeGrid, targetinfo, false) as UnityAction;
                    UnityEventTools.AddPersistentListener(txtGO.GetComponentInChildren<Button>().onClick, action);

                    txtGO = PrefabUtility.InstantiatePrefab(prefabButton, pnlGO.transform) as GameObject;
                    txtGO.GetComponentInChildren<TextMeshProUGUI>().text = "Cancel";
                    targetinfo = UnityEvent.GetValidMethodInfo(craftMenu, "Close", new Type[0]);
                    action = Delegate.CreateDelegate(typeof(UnityAction), craftMenu, targetinfo, false) as UnityAction;
                    UnityEventTools.AddPersistentListener(txtGO.GetComponentInChildren<Button>().onClick, action);
                }
                else
                {
                    pnlGO = PrefabUtility.InstantiatePrefab(prefabRecipeItemGrid, contentGO.transform) as GameObject;
                    craftMenu.recipeGrid = pnlGO.GetComponentInChildren<RecipeItemGrid>();
                }

                if(cmRecipeDetails)
                {
                    pnlGO = PrefabUtility.InstantiatePrefab(prefabRecipeUI, contentGO.transform) as GameObject;
                    craftMenu.recipeGrid.recipeDetail = pnlGO.GetComponentInChildren<RecipeUI>();
                }
            }
            else
            {
                pnlGO = PrefabUtility.InstantiatePrefab(prefabItemScrollGrid, contentGO.transform) as GameObject;
                craftMenu.playerInventory = pnlGO.GetComponentInChildren<InventoryItemList>();
                craftMenu.destroyIfNoMatch = cmDestoryNo;
                craftMenu.alwaysUseSingle = cmAlwaysSingle;

                // Blind
                if (cmShowContext)
                {
                    subGO = new GameObject("Craft List");
                    subGO.transform.SetParent(contentGO.transform);
                    rt = CreateOrGetRectTransform(subGO);

                    pnlGO = PrefabUtility.InstantiatePrefab(prefabBlindScrollGrid, subGO.transform) as GameObject;
                    rt = CreateOrGetRectTransform(pnlGO);
                    rt.anchorMin = new Vector2(0, 0.1f);
                    rt.anchorMax = new Vector2(1, 1);
                    rt.offsetMax = rt.offsetMin = Vector2.zero;
                    craftMenu.blindList = pnlGO.GetComponentInChildren<BlindItemList>();

                    pnlGO = new GameObject("Context Buttons");
                    pnlGO.transform.SetParent(subGO.transform);
                    rt = CreateOrGetRectTransform(pnlGO);
                    rt.anchorMin = Vector2.zero;
                    rt.anchorMax = new Vector2(1, 0.1f);
                    rt.offsetMax = rt.offsetMin = Vector2.zero;
                    hlg = pnlGO.AddComponent<HorizontalLayoutGroup>();
                    hlg.childForceExpandHeight = true;
                    hlg.childForceExpandWidth = true;
                    hlg.childControlHeight = true;
                    hlg.childControlWidth = true;

                    txtGO = PrefabUtility.InstantiatePrefab(prefabButton, pnlGO.transform) as GameObject;
                    txtGO.GetComponentInChildren<TextMeshProUGUI>().text = "Add";
                    targetinfo = UnityEvent.GetValidMethodInfo(craftMenu, "AddSelectedToBlindCraft", new Type[0]);
                    action = Delegate.CreateDelegate(typeof(UnityAction), craftMenu, targetinfo, false) as UnityAction;
                    UnityEventTools.AddPersistentListener(txtGO.GetComponentInChildren<Button>().onClick, action);

                    txtGO = PrefabUtility.InstantiatePrefab(prefabButton, pnlGO.transform) as GameObject;
                    txtGO.GetComponentInChildren<TextMeshProUGUI>().text = "Craft";
                    targetinfo = UnityEvent.GetValidMethodInfo(craftMenu, "BlindCraft", new Type[0]);
                    action = Delegate.CreateDelegate(typeof(UnityAction), craftMenu, targetinfo, false) as UnityAction;
                    UnityEventTools.AddPersistentListener(txtGO.GetComponentInChildren<Button>().onClick, action);

                    txtGO = PrefabUtility.InstantiatePrefab(prefabButton, pnlGO.transform) as GameObject;
                    txtGO.GetComponentInChildren<TextMeshProUGUI>().text = "Cancel";
                    targetinfo = UnityEvent.GetValidMethodInfo(craftMenu, "Close", new Type[0]);
                    action = Delegate.CreateDelegate(typeof(UnityAction), craftMenu, targetinfo, false) as UnityAction;
                    UnityEventTools.AddPersistentListener(txtGO.GetComponentInChildren<Button>().onClick, action);
                }
                else
                {
                    pnlGO = PrefabUtility.InstantiatePrefab(prefabRecipeItemGrid, contentGO.transform) as GameObject;
                    craftMenu.recipeGrid = pnlGO.GetComponentInChildren<RecipeItemGrid>();
                }


            }
            if(cmShowQueue)
            {
                pnlGO = PrefabUtility.InstantiatePrefab(prefabCraftQueueUI, contentGO.transform) as GameObject;
            }
            GameObject prefabCraftMenu = PrefabUtility.SaveAsPrefabAsset(go, Path.Combine(windowsPath, go.name + ".prefab"));
            themeEditor.serializedObject.FindProperty("craftingMenu").objectReferenceValue = prefabCraftMenu.GetComponent<CraftMenuUI>();
            DestroyImmediate(go);

            // Save Theme
            string themePath = Path.Combine(basePath, themeName + ".asset");
            if (File.Exists(themePath)) File.Delete(themePath);
            themeEditor.serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(themeEditor);
            themeEditor.serializedObject.Update();
            AssetDatabase.CreateAsset(theme, themePath);
            AssetDatabase.Refresh();

            // Destroy canvas
            DestroyImmediate(canvas);
        }

        private void CreateDirectories(string basePath, string themeName)
        {
            string path = Path.Combine(basePath, themeName);
            string subPath;

            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            subPath = Path.Combine(path, "Components");
            if (!Directory.Exists(subPath)) Directory.CreateDirectory(subPath);

            subPath = Path.Combine(path, "Lists");
            if (!Directory.Exists(subPath)) Directory.CreateDirectory(subPath);

            subPath = Path.Combine(path, "Prompts");
            if (!Directory.Exists(subPath)) Directory.CreateDirectory(subPath);

            subPath = Path.Combine(path, "Render Textures");
            if (!Directory.Exists(subPath)) Directory.CreateDirectory(subPath);

            subPath = Path.Combine(path, "Textures");
            if (!Directory.Exists(subPath)) Directory.CreateDirectory(subPath);

            subPath = Path.Combine(path, "Windows");
            if (!Directory.Exists(subPath)) Directory.CreateDirectory(subPath);

            AssetDatabase.Refresh();
        }

        private GameObject CreateHorizontalContainer(string name, Transform parent, float spacing)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent);
            HorizontalLayoutGroup hlg = go.AddComponent<HorizontalLayoutGroup>();
            hlg.spacing = spacing;
            hlg.childForceExpandHeight = true;
            hlg.childForceExpandWidth = false;
            hlg.childControlHeight = true;
            hlg.childControlWidth = false;
            return go;
        }

        private void CreateItemContextButtons(GameObject go, GameObject prefabButton)
        {
            ItemContextButtons icb = go.AddComponent<ItemContextButtons>();

            GameObject btnGO = PrefabUtility.InstantiatePrefab(prefabButton, go.transform) as GameObject;
            btnGO.GetComponentInChildren<TextMeshProUGUI>().text = "Equip";
            icb.btnEquip = btnGO.GetComponent<Button>();

            btnGO = PrefabUtility.InstantiatePrefab(prefabButton, go.transform) as GameObject;
            btnGO.GetComponentInChildren<TextMeshProUGUI>().text = "Remove";
            icb.btnRemove = btnGO.GetComponent<Button>();

            btnGO = PrefabUtility.InstantiatePrefab(prefabButton, go.transform) as GameObject;
            btnGO.GetComponentInChildren<TextMeshProUGUI>().text = "Modify";
            icb.btnModify = btnGO.GetComponent<Button>();

            btnGO = PrefabUtility.InstantiatePrefab(prefabButton, go.transform) as GameObject;
            btnGO.GetComponentInChildren<TextMeshProUGUI>().text = "Repair";
            icb.btnRepair = btnGO.GetComponent<Button>();

            btnGO = PrefabUtility.InstantiatePrefab(prefabButton, go.transform) as GameObject;
            btnGO.GetComponentInChildren<TextMeshProUGUI>().text = "Drop";
            icb.btnDrop = btnGO.GetComponent<Button>();

            btnGO = PrefabUtility.InstantiatePrefab(prefabButton, go.transform) as GameObject;
            btnGO.GetComponentInChildren<TextMeshProUGUI>().text = "Breakdown";
            icb.btnBreakdown = btnGO.GetComponent<Button>();

            btnGO = PrefabUtility.InstantiatePrefab(prefabButton, go.transform) as GameObject;
            btnGO.GetComponentInChildren<TextMeshProUGUI>().text = "Attach";
            icb.btnAttach = btnGO.GetComponent<Button>();

            btnGO = PrefabUtility.InstantiatePrefab(prefabButton, go.transform) as GameObject;
            btnGO.GetComponentInChildren<TextMeshProUGUI>().text = "Un-attach";
            icb.btnUnattach = btnGO.GetComponent<Button>();

            btnGO = PrefabUtility.InstantiatePrefab(prefabButton, go.transform) as GameObject;
            btnGO.GetComponentInChildren<TextMeshProUGUI>().text = "Rename";
            icb.btnRename = btnGO.GetComponent<Button>();

            btnGO = PrefabUtility.InstantiatePrefab(prefabButton, go.transform) as GameObject;
            btnGO.GetComponentInChildren<TextMeshProUGUI>().text = "Open";
            icb.btnOpen = btnGO.GetComponent<Button>();

            btnGO = PrefabUtility.InstantiatePrefab(prefabButton, go.transform) as GameObject;
            btnGO.GetComponentInChildren<TextMeshProUGUI>().text = "Consume";
            icb.btnConsumable = btnGO.GetComponent<Button>();
        }

        private void CreateItemListContextButtons(GameObject go, GameObject prefabButton, InventoryItemList baseList)
        {
            ItemListContextButtons ilm = go.AddComponent<ItemListContextButtons>();
            ilm.targetList = baseList;

            GameObject btnGO = PrefabUtility.InstantiatePrefab(prefabButton, go.transform) as GameObject;
            btnGO.GetComponentInChildren<TextMeshProUGUI>().text = "Equip";
            ilm.btnEquip = btnGO.GetComponent<Button>();

            btnGO = PrefabUtility.InstantiatePrefab(prefabButton, go.transform) as GameObject;
            btnGO.GetComponentInChildren<TextMeshProUGUI>().text = "Remove";
            ilm.btnRemove = btnGO.GetComponent<Button>();

            btnGO = PrefabUtility.InstantiatePrefab(prefabButton, go.transform) as GameObject;
            btnGO.GetComponentInChildren<TextMeshProUGUI>().text = "Modify";
            ilm.btnModify = btnGO.GetComponent<Button>();

            btnGO = PrefabUtility.InstantiatePrefab(prefabButton, go.transform) as GameObject;
            btnGO.GetComponentInChildren<TextMeshProUGUI>().text = "Repair";
            ilm.btnRepair = btnGO.GetComponent<Button>();

            btnGO = PrefabUtility.InstantiatePrefab(prefabButton, go.transform) as GameObject;
            btnGO.GetComponentInChildren<TextMeshProUGUI>().text = "Drop";
            ilm.btnDrop = btnGO.GetComponent<Button>();

            btnGO = PrefabUtility.InstantiatePrefab(prefabButton, go.transform) as GameObject;
            btnGO.GetComponentInChildren<TextMeshProUGUI>().text = "Breakdown";
            ilm.btnBreakdown = btnGO.GetComponent<Button>();

            btnGO = PrefabUtility.InstantiatePrefab(prefabButton, go.transform) as GameObject;
            btnGO.GetComponentInChildren<TextMeshProUGUI>().text = "Attach";
            ilm.btnAttach = btnGO.GetComponent<Button>();

            btnGO = PrefabUtility.InstantiatePrefab(prefabButton, go.transform) as GameObject;
            btnGO.GetComponentInChildren<TextMeshProUGUI>().text = "Un-attach";
            ilm.btnUnattach = btnGO.GetComponent<Button>();

            btnGO = PrefabUtility.InstantiatePrefab(prefabButton, go.transform) as GameObject;
            btnGO.GetComponentInChildren<TextMeshProUGUI>().text = "Rename";
            ilm.btnRename = btnGO.GetComponent<Button>();

            btnGO = PrefabUtility.InstantiatePrefab(prefabButton, go.transform) as GameObject;
            btnGO.GetComponentInChildren<TextMeshProUGUI>().text = "Open";
            ilm.btnContainer = btnGO.GetComponent<Button>();
        }

        private InventoryItemList CreateListType(int listType, Transform parent, UnityEngine.Object prefabItemScrollGrid, UnityEngine.Object prefabItemPagedGrid, UnityEngine.Object prefabItemScrollList, out RectTransform rt)
        {
            GameObject pnlGO;
            switch (listType)
            {
                case 0: // Scroll Grid
                    pnlGO = PrefabUtility.InstantiatePrefab(prefabItemScrollGrid, parent) as GameObject;
                    rt = CreateOrGetRectTransform(pnlGO);
                    return pnlGO.GetComponentInChildren<InventoryItemScrollGrid>();
                case 1: // Paged Grid
                    pnlGO = PrefabUtility.InstantiatePrefab(prefabItemPagedGrid, parent) as GameObject;
                    rt = CreateOrGetRectTransform(pnlGO);
                    return pnlGO.GetComponentInChildren<InventoryItemGrid>();
                case 2: // Scroll List
                    pnlGO = PrefabUtility.InstantiatePrefab(prefabItemScrollList, parent) as GameObject;
                    rt = CreateOrGetRectTransform(pnlGO);
                    return pnlGO.GetComponentInChildren<InventoryItemScrollList>();
            }
            rt = null;
            return null;
        }

        private Slider CreateProgressbar(GameObject go)
        {
            Slider slider = go.AddComponent<Slider>();
            slider.interactable = false;
            slider.transition = Selectable.Transition.None;

            GameObject subGO = new GameObject("Background");
            subGO.transform.SetParent(go.transform);
            SetFullSize(subGO);
            Image img = subGO.AddComponent<Image>();
            img.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
            img.type = Image.Type.Sliced;
            img.fillCenter = true;

            subGO = new GameObject("Fill Area");
            subGO.transform.SetParent(go.transform);
            RectTransform rt = SetFullSize(subGO);
            rt.offsetMin = new Vector2(5, 0);
            rt.offsetMax = new Vector2(-5, 0);

            GameObject subGO2 = new GameObject("Fill");
            subGO2.transform.SetParent(subGO.transform);
            img = subGO2.AddComponent<Image>();
            img.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            img.type = Image.Type.Sliced;
            img.fillCenter = true;
            rt = CreateOrGetRectTransform(subGO2);
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = new Vector2(0, 1);
            rt.sizeDelta = new Vector2(10, 0);

            slider.fillRect = rt;

            return slider;
        }

        private RectTransform CreateOrGetRectTransform(GameObject go)
        {
            RectTransform rt = go.GetComponent<RectTransform>();
            if (rt == null) rt = go.AddComponent<RectTransform>();
            return rt;
        }

        private Slider CreateSlider(GameObject go)
        {
            Slider slider = go.AddComponent<Slider>();
            slider.interactable = false;
            slider.transition = Selectable.Transition.None;

            GameObject subGO = new GameObject("Background");
            subGO.transform.SetParent(go.transform);
            SetFullSize(subGO);
            Image img = subGO.AddComponent<Image>();
            img.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
            img.type = Image.Type.Sliced;
            img.fillCenter = true;

            subGO = new GameObject("Fill Area");
            subGO.transform.SetParent(go.transform);
            RectTransform rt = SetFullSize(subGO);
            rt.offsetMin = new Vector2(0, 0);
            rt.offsetMax = new Vector2(-10, 0);

            GameObject subGO2 = new GameObject("Fill");
            subGO2.transform.SetParent(subGO.transform);
            img = subGO2.AddComponent<Image>();
            img.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            img.type = Image.Type.Sliced;
            img.fillCenter = true;
            rt = CreateOrGetRectTransform(subGO2);
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = new Vector2(0, 1);
            rt.sizeDelta = new Vector2(10, 0);
            slider.fillRect = rt;

            subGO = new GameObject("Handle Slide Area");
            subGO.transform.SetParent(go.transform);
            rt = SetFullSize(subGO);
            rt.offsetMin = new Vector2(10, 0);
            rt.offsetMax = new Vector2(-10, 0);

            subGO2 = new GameObject("Handle");
            subGO2.transform.SetParent(subGO.transform);
            img = subGO2.AddComponent<Image>();
            img.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            img.type = Image.Type.Sliced;
            img.fillCenter = true;
            rt = CreateOrGetRectTransform(subGO2);
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = new Vector2(0, 1);
            rt.offsetMin = rt.offsetMax = Vector2.zero;
            rt.sizeDelta = new Vector2(20, 0);
            slider.handleRect = rt;

            return slider;
        }

        private Scrollbar CreateScrollbar(GameObject go)
        {
            go.AddComponent<CanvasRenderer>();

            Image img = go.AddComponent<Image>();
            img.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
            img.type = Image.Type.Sliced;
            img.fillCenter = true;

            GameObject area = new GameObject("Sliding Area");
            area.transform.SetParent(go.transform);
            SetFullSize(area);

            GameObject handle = new GameObject("Handle");
            handle.transform.SetParent(area.transform);
            SetFullSize(handle);
            img = handle.AddComponent<Image>();
            img.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            img.type = Image.Type.Sliced;
            img.fillCenter = true;

            Scrollbar scrollbar = go.AddComponent<Scrollbar>();
            scrollbar.handleRect = handle.GetComponent<RectTransform>();

            return scrollbar;
        }

        private ScrollRect CreateScrollRect(GameObject go)
        {
            ScrollRect scrollRect = go.AddComponent<ScrollRect>();
            GameObject viewport = new GameObject("Viewport");
            viewport.transform.SetParent(go.transform);
            SetFullSize(viewport);
            Mask mask = viewport.AddComponent<Mask>();
            mask.showMaskGraphic = false;
            viewport.AddComponent<CanvasRenderer>();
            Image img = viewport.AddComponent<Image>();
            img.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UIMask.psd");
            img.type = Image.Type.Sliced;
            img.fillCenter = true;
            GameObject content = new GameObject("Content");
            content.transform.SetParent(viewport.transform);
            RectTransform rt = content.GetComponent<RectTransform>();
            if (rt == null) rt = content.AddComponent<RectTransform>();
            rt.pivot = new Vector2(0, 1);
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(1, 1);
            rt.offsetMin = new Vector2(0, 100);
            rt.offsetMax = new Vector2(0, 300);
            scrollRect.content = rt;
            scrollRect.viewport = viewport.GetComponent<RectTransform>();

            // Horizontal Scrollbar
            GameObject hs = new GameObject("Scrollbar Horizontal");
            hs.transform.SetParent(go.transform);
            Scrollbar hScroll = CreateScrollbar(hs);
            rt = hs.GetComponent<RectTransform>();
            rt.pivot = Vector2.zero;
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = new Vector2(1, 0);
            rt.offsetMax = rt.offsetMin = Vector2.zero;
            rt.sizeDelta = new Vector2(rt.sizeDelta.x, 20);

            // Vertical Scrollbar
            GameObject vs = new GameObject("Scrollbar Vertical");
            vs.transform.SetParent(go.transform);
            Scrollbar vScroll = CreateScrollbar(vs);
            rt = vs.GetComponent<RectTransform>();
            rt.pivot = new Vector2(1, 1);
            rt.anchorMin = new Vector2(1, 0);
            rt.anchorMax = new Vector2(1, 1);
            rt.offsetMax = rt.offsetMin = Vector2.zero;
            rt.sizeDelta = new Vector2(20, rt.sizeDelta.y);
            vScroll.direction = Scrollbar.Direction.BottomToTop;
            vScroll.value = 0.1452992f;
            vScroll.size = 0.61f;

            scrollRect.horizontalScrollbar = hScroll;
            scrollRect.horizontalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
            scrollRect.horizontalScrollbarSpacing = -3;

            scrollRect.verticalScrollbar = vScroll;
            scrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
            scrollRect.verticalScrollbarSpacing = -3;

            go.AddComponent<CanvasRenderer>();
            img = go.AddComponent<Image>();
            img.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
            img.type = Image.Type.Sliced;
            img.fillCenter = true;
            img.color = new Color(1, 1, 1, 0.3921569f);

            return scrollRect;
        }

        private TMP_InputField CreateTextMeshInput(GameObject go)
        {
            Image img = go.AddComponent<Image>();
            img.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/InputFieldBackground.psd");
            img.type = Image.Type.Sliced;
            img.fillCenter = true;

            TMP_InputField input = go.AddComponent<TMP_InputField>();
            input.targetGraphic = img;

            GameObject pnlGO = new GameObject("Text Area");
            pnlGO.transform.SetParent(go.transform);
            pnlGO.AddComponent<RectMask2D>();
            RectTransform rt = SetFullSize(pnlGO);
            rt.offsetMin = new Vector2(10, 7);
            rt.offsetMax = new Vector2(-10, -6);
            input.textViewport = rt;

            GameObject txtGO = new GameObject("Placeholder");
            txtGO.transform.SetParent(pnlGO.transform);
            input.placeholder = CreateTextMeshPro(txtGO, "Enter text...");
            SetFullSize(txtGO);

            txtGO = new GameObject("Text");
            txtGO.transform.SetParent(pnlGO.transform);
            input.textComponent = CreateTextMeshPro(txtGO, string.Empty);
            input.fontAsset = input.textComponent.font;
            SetFullSize(txtGO);

            return input;
        }

        private TextMeshProUGUI CreateTextMeshPro(GameObject go, string text, bool autoSize = true, TextAlignmentOptions align = TextAlignmentOptions.Center)
        {
            TextMeshProUGUI tmpro = go.AddComponent<TextMeshProUGUI>();
            tmpro.text = text;
            tmpro.alignment = align;
            tmpro.enableAutoSizing = autoSize;
            tmpro.color = Color.black;
            return tmpro;
        }

        private void CreateTheme()
        {
            string themeName = themeEditor.serializedObject.FindProperty("displayName").stringValue;
            string path = EditorUtility.SaveFilePanelInProject("Themes Location", themeName, "asset", "Select your top-level themes folder", "Assets/");
            path = Path.GetDirectoryName(path);

            // Create 
            CreateDirectories(path, themeName);
            CreateComponents(Path.Combine(path, themeName), themeName);

            InventoryThemeWizard w = GetWindow<InventoryThemeWizard>(TITLE);
            w.Close();
        }

        private RectTransform SetFullSize(GameObject go)
        {
            RectTransform rt = go.GetComponent<RectTransform>();
            if (rt == null) rt = go.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(0, 0);
            rt.anchorMax = new Vector2(1, 1);
            rt.offsetMax = rt.offsetMin = Vector2.zero;
            return rt;
        }

        #endregion

    }
}