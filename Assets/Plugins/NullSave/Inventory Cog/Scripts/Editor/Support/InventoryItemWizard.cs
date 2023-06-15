using NullSave.TOCK.Stats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace NullSave.TOCK.Inventory
{
    public class InventoryItemWizard : TOCKEditorWindow
    {

        #region Constants

        private const string TITLE = "Item Wizard";

        #endregion

        #region Variables

        private static GUIStyle container;
        private static Texture2D windowIcon;

        private InventoryDB db;
        private InventoryDBEditor dbEditor;
        private GameObject prefabRoot;
        private Vector2 itemPos;

        // Construction variables
        private InventoryItem item;
        private InventoryItemEditor itemEditor;

        // Auto variables
        private GameObject itemBase;
        private bool autoLoot, autoEquip, autoPreview, autoAttachment;
        private LayerMask lootLayer = 0;
        private LayerMask equipLayer = 0;
        private LayerMask previewLayer = 0;
        private LayerMask attachLayer = 0;

        // Layer masks
        static List<int> layerNumbers = new List<int>();

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
                    windowIcon = (Texture2D)Resources.Load("Icons/db-window", typeof(Texture2D));
                }

                return windowIcon;
            }
        }

        #endregion

        #region Unity Methods

        private void OnGUI()
        {
            if (item == null)
            {
                item = ScriptableObject.CreateInstance<InventoryItem>();
                item.displayName = "New Item";
            }

            if (itemEditor == null)
            {
                itemEditor = (InventoryItemEditor)Editor.CreateEditor(item);
            }

            GUILayout.BeginVertical(Container, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
            MainContainerBegin("Item Wizard", "Icons/tock-sword");
            GUILayout.Space(16);
            GUILayout.Label("Welcome to the Item Wizard! This window will collect some information from you in order to build your new item.");

            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical();
            itemEditor.serializedObject.Update();
            SectionHeader("Create Item");

            itemPos = GUILayout.BeginScrollView(itemPos);
            itemEditor.DrawInspector(db, autoLoot, autoEquip, autoPreview, autoAttachment);
            GUILayout.EndScrollView();

            itemEditor.serializedObject.ApplyModifiedProperties();
            GUILayout.EndVertical();

            GUILayout.BeginVertical(GUILayout.Width(300));
            SectionHeader("Auto Create Prefabs");

            itemBase = SimpleEditorGameObject("Base Model", itemBase);
            autoLoot = SimpleEditorBool("Loot Item", autoLoot);
            if(autoLoot)
            {
                lootLayer = LayerMaskField("Loot Layer", lootLayer);
            }
            autoEquip = SimpleEditorBool("Equip Item", autoEquip);
            if (autoEquip)
            {
                equipLayer = LayerMaskField("Equip Layer", equipLayer);
            }
            autoPreview = SimpleEditorBool("Item Preview", autoPreview);
            if (autoPreview)
            {
                previewLayer = LayerMaskField("Preview Layer", previewLayer);
            }
            autoAttachment = SimpleEditorBool("Attachment Item", autoAttachment);
            if (autoAttachment)
            {
                attachLayer = LayerMaskField("Attachment Layer", attachLayer);
            }
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();


            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button(db == null ? "Create" : "Create and Add", GUILayout.Height(32), GUILayout.Width(120)))
            {
                if (itemBase == null && (autoEquip || autoLoot || autoPreview || autoAttachment))
                {
                    EditorUtility.DisplayDialog("Inventory Cog", "You have selected at least one auto-create prefab but have not assigned an object to use.\r\n\r\nPlease provide one or turn off auto-create prefabs.", "OK");
                }
                else
                {
                    string path = EditorUtility.SaveFilePanelInProject("Save ", item.displayName, "asset", "Select a location to save the item.", PlayerPrefs.GetString("ItemPath", Application.dataPath));
                    if (path.Length != 0)
                    {
                        PlayerPrefs.SetString("ItemPath", Path.GetDirectoryName(path) + "/");

                        // Create new item
                        string baseFilename = Path.GetFileNameWithoutExtension(path);
                        item.name = baseFilename;

                        AssetDatabase.CreateAsset(item, path);

                        // Create Loot item
                        if (autoLoot)
                        {
                            GameObject loot = Instantiate(itemBase);
                            loot.name = item.name + " (Loot)";
                            loot.layer = lootLayer;
                            LootItem lootItem = loot.AddComponent<LootItem>();
                            lootItem.item = item;
                            lootItem.count = 1;
                            BoxCollider bc = loot.AddComponent<BoxCollider>();
                            bc.isTrigger = true;
                            item.dropObject = (PrefabUtility.SaveAsPrefabAsset(loot, path + loot.name + ".prefab")).GetComponent<LootItem>();
                            DestroyImmediate(loot);
                        }

                        // Create equip item
                        if (autoEquip)
                        {
                            GameObject equip = new GameObject
                            {
                                name = item.name + " (Equip)",
                                layer = equipLayer
                            };
                            Instantiate(itemBase, equip.transform);
                            item.equipObject = PrefabUtility.SaveAsPrefabAsset(equip, path + equip.name + ".prefab");
                            DestroyImmediate(equip);
                        }

                        // Create preview item
                        if (autoPreview)
                        {
                            GameObject preview = new GameObject
                            {
                                name = item.name + " (Preview)",
                                layer = previewLayer
                            };
                            Instantiate(itemBase, preview.transform);
                            item.previewObject = PrefabUtility.SaveAsPrefabAsset(preview, path + preview.name + ".prefab");
                            DestroyImmediate(preview);
                        }

                        // Create attachment item
                        if (autoAttachment)
                        {
                            GameObject attach = new GameObject
                            {
                                name = item.name + " (Attachment)",
                                layer = attachLayer
                            };
                            Instantiate(itemBase, attach.transform);
                            item.attachObject = PrefabUtility.SaveAsPrefabAsset(attach, path + attach.name + ".prefab");
                            DestroyImmediate(attach);
                        }

                        AssetDatabase.SaveAssets();

                        if(dbEditor != null)
                        {
                            dbEditor.AddItem(AssetDatabase.LoadAssetAtPath(path, typeof(InventoryItem)) as InventoryItem);
                            dbEditor.serializedObject.ApplyModifiedProperties();
                            if (prefabRoot != null)
                            {
                                PrefabUtility.ApplyPrefabInstance(prefabRoot, InteractionMode.AutomatedAction);
                            }
                        }

                        item = null;
                        itemBase = null;
                        autoEquip = autoLoot = autoPreview = autoAttachment = false;
                        this.Close();
                    }
                }
            }
            GUILayout.EndHorizontal();

            MainContainerEnd(false);
            GUILayout.EndVertical();

        }

        private void OnInspectorUpdate()
        {
            Repaint();
        }

        #endregion

        #region Public Methods

        [MenuItem("Tools/NullSave/Inventory Item Wizard", false, 2)]
        public static void ShowWindow()
        {
            ShowWindow(null, null, null);
        }

        public static void ShowWindow(InventoryDB db, InventoryDBEditor dbEditor, GameObject prefabRoot)
        {
            InventoryItemWizard w = GetWindow<InventoryItemWizard>(TITLE);
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
            w.position = new Rect((Screen.currentResolution.width * scale - 800 * scale) / 2, (Screen.currentResolution.height * scale - 600 * scale) / 2, 800, 600);
            w.wantsMouseMove = true;
            w.db = db;
            w.dbEditor = dbEditor;
            w.prefabRoot = prefabRoot;
        }

        #endregion

        #region Private Methods

        static LayerMask LayerMaskField(string label, LayerMask layerMask)
        {
            layerMask = EditorGUILayout.LayerField(label, layerMask);
            return layerMask;
        }

        #endregion

    }
}