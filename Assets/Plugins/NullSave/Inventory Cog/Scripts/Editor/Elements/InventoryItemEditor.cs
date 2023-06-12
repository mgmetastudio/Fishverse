using NullSave.TOCK.Stats;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace NullSave.TOCK.Inventory
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(InventoryItem))]
    public class InventoryItemEditor : TOCKEditorV2
    {

        #region Enumerations

        private enum DisplayFlags
        {
            None = 0,
            Behaviour = 1,
            Stacking = 2,
            Equipping = 4,
            Animation = 8,
            UI = 16,
            Breakdown = 32,
            Repair = 64,
            Stas = 128,
            Tags = 256,
            Preview = 512,
            Attachment = 1024,
            Unlocking = 2048,
            Ammo = 4096
        }

        #endregion

        #region Variables

        private DisplayFlags displayFlags;
        private ReorderableList repair, breakdown, equipPoints, attachPoints, uiTags;
        private Texture2D behaviourIcon, stackingIcon, equipIcon, animIcon, uiIcon, breakdownIcon, repairIcon, statsIcon, tagsIcon, previewIcon, attachmentIcon, unlockIcon;
        private int sel1, sel2, sel3;

        #endregion

        #region Properties

        private Texture2D AnimateIcon
        {
            get
            {
                if (animIcon == null)
                {
                    animIcon = (Texture2D)Resources.Load("Icons/tock-animate", typeof(Texture2D));
                }

                return animIcon;
            }
        }

        private Texture2D AttachmentIcon
        {
            get
            {
                if (attachmentIcon == null)
                {
                    attachmentIcon = (Texture2D)Resources.Load("Icons/attachment", typeof(Texture2D));
                }

                return attachmentIcon;
            }
        }

        private Texture2D BehaviourIcon
        {
            get
            {
                if (behaviourIcon == null)
                {
                    behaviourIcon = (Texture2D)Resources.Load("Icons/tock-behaviour", typeof(Texture2D));
                }

                return behaviourIcon;
            }
        }

        private Texture2D BreakdownIcon
        {
            get
            {
                if (breakdownIcon == null)
                {
                    breakdownIcon = (Texture2D)Resources.Load("Icons/damage", typeof(Texture2D));
                }

                return breakdownIcon;
            }
        }

        private Texture2D EquipIcon
        {
            get
            {
                if (equipIcon == null)
                {
                    equipIcon = (Texture2D)Resources.Load("Icons/tock-equip", typeof(Texture2D));
                }

                return equipIcon;
            }
        }

        private Texture2D PreviewIcon
        {
            get
            {
                if (previewIcon == null)
                {
                    previewIcon = (Texture2D)Resources.Load("Icons/view", typeof(Texture2D));
                }

                return previewIcon;
            }
        }

        private Texture2D RepairIcon
        {
            get
            {
                if (repairIcon == null)
                {
                    repairIcon = (Texture2D)Resources.Load("Icons/tock-repair", typeof(Texture2D));
                }

                return repairIcon;
            }
        }

        private Texture2D StackingIcon
        {
            get
            {
                if (stackingIcon == null)
                {
                    stackingIcon = (Texture2D)Resources.Load("Icons/tock-stack", typeof(Texture2D));
                }

                return stackingIcon;
            }
        }

        private Texture2D StatsIcon
        {
            get
            {
                if (statsIcon == null)
                {
                    statsIcon = (Texture2D)Resources.Load("Icons/tock-stats", typeof(Texture2D));
                }

                return statsIcon;
            }
        }

        private Texture2D TagsIcon
        {
            get
            {
                if (tagsIcon == null)
                {
                    tagsIcon = (Texture2D)Resources.Load("Icons/tock-tag", typeof(Texture2D));
                }

                return tagsIcon;
            }
        }

        private Texture2D UIIcon
        {
            get
            {
                if (uiIcon == null)
                {
                    uiIcon = (Texture2D)Resources.Load("Icons/tock-ui", typeof(Texture2D));
                }

                return uiIcon;
            }
        }

        private Texture2D UnlockIcon
        {
            get
            {
                if (unlockIcon == null)
                {
                    unlockIcon = (Texture2D)Resources.Load("Icons/inventory-unlock", typeof(Texture2D));
                }

                return unlockIcon;
            }
        }

        #endregion

        #region Unity Methods

        public void OnEnable()
        {
            if (target == null || serializedObject == null) return;

            repair = new ReorderableList(serializedObject, serializedObject.FindProperty("incrementComponents"), true, true, true, true);
            repair.elementHeight = (EditorGUIUtility.singleLineHeight * 2) + 4;
            repair.drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, "Increment Components"); };
            SetupComponentsCallback(repair);

            breakdown = new ReorderableList(serializedObject, serializedObject.FindProperty("breakdownResult"), true, true, true, true);
            breakdown.elementHeight = (EditorGUIUtility.singleLineHeight * 2) + 4;
            breakdown.drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, "Breakdown Result"); };
            SetupComponentsCallback(breakdown);

            equipPoints = new ReorderableList(serializedObject, serializedObject.FindProperty("equipPoints"), true, true, true, true);
            equipPoints.elementHeight = EditorGUIUtility.singleLineHeight + 2;
            equipPoints.drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, "Equip Points"); };
            equipPoints.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var element = equipPoints.serializedProperty.GetArrayElementAtIndex(index);
                EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element);
            };

            attachPoints = new ReorderableList(serializedObject, serializedObject.FindProperty("attachPoints"), true, true, true, true);
            attachPoints.elementHeight = EditorGUIUtility.singleLineHeight + 2;
            attachPoints.drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, "Attach Points"); };
            attachPoints.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var element = attachPoints.serializedProperty.GetArrayElementAtIndex(index);
                EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element);
            };


            uiTags = new ReorderableList(serializedObject, serializedObject.FindProperty("uiTags"), true, true, true, true);
            uiTags.elementHeight = EditorGUIUtility.singleLineHeight + 2;
            uiTags.drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, "Item Tags"); };
            uiTags.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var element = uiTags.serializedProperty.GetArrayElementAtIndex(index);
                EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element);
            };
        }

        public override void OnInspectorGUI()
        {
            displayFlags = (DisplayFlags)serializedObject.FindProperty("z_display_flags").intValue;
            MainContainerBegin("Inventory Item", "Icons/item_icon", false);

            DrawGeneral();
            DrawUI();
            if ((ItemType)SimpleInt("itemType") != ItemType.Skill)
            {
                if ((ItemType)SimpleInt("itemType") != ItemType.Attachment)
                {
                    DrawAttachments();
                }
                else
                {
                    DrawAttachmentSettings(false);
                }
                DrawRepairBreakdown();
            }
            DrawStats();
            DrawTags();

            MainContainerEnd();
        }

        #endregion

        #region Private Methods

        private void ComponentsAddCallback(ReorderableList list)
        {
            var index = list.serializedProperty.arraySize;
            list.serializedProperty.arraySize++;
            list.index = index;
            list.serializedProperty.GetArrayElementAtIndex(list.serializedProperty.arraySize - 1).FindPropertyRelative("count").intValue = 1;

            var element = list.serializedProperty.GetArrayElementAtIndex(index);
        }

        private void DrawAttachments()
        {
            if (SectionToggle((int)displayFlags, (int)DisplayFlags.Attachment, "Attachments", AttachmentIcon))
            {
                if (!SimpleBool("canEquip"))
                {
                    GUILayout.Label("Only items that can be equipped can have attachments.", Skin.GetStyle("WrapText"));
                }
                else if (serializedObject.FindProperty("equipObject").objectReferenceValue == null)
                {
                    GUILayout.Label("An equip object needs to be supplied to support attachments.", Skin.GetStyle("WrapText"));
                }
                else
                {
                    SimpleProperty("attachRequirement");
                    switch ((AttachRequirement)SimpleInt("attachRequirement"))
                    {
                        case AttachRequirement.InCategory:
                            SubHeader("Categories", "attachCatsFilter", typeof(Category));
                            SimpleList("attachCatsFilter");
                            DrawSlots();
                            break;
                        case AttachRequirement.InItemList:
                            SubHeader("Items", "attachItemsFilter", typeof(InventoryItem));
                            SimpleList("attachItemsFilter");
                            DrawSlots();
                            break;
                    }
                }
            }
        }

        private void DrawAttachmentSettings(bool autoAttach)
        {
            if (SectionToggle((int)displayFlags, (int)DisplayFlags.Attachment, "Attachment Settings", AttachmentIcon))
            {
                if (autoAttach)
                {
                    GUI.enabled = false;
                    EditorGUILayout.TextField("Attach Object", "{Auto Creating}");
                    GUI.enabled = true;
                }
                else
                {
                    SimpleProperty("attachObject");
                }
                SimpleProperty("modifyName", "Modify Parent Name");
                if (SimpleBool("modifyName"))
                {
                    SimpleProperty("nameModifier");
                    SimpleProperty("modifierOrder");
                }
                attachPoints.DoLayoutList();
            }
        }

        private void DrawCategoryFromDb(InventoryDB db)
        {
            if (db.categories == null || db.categories.Count == 0)
            {
                EditorGUILayout.Popup("Item Category", 0, new string[] { "No Categories Available" });
                return;
            }

            string[] catNames = new string[db.categories.Count];
            for (int i = 0; i < catNames.Length; i++)
            {
                catNames[i] = db.categories[i].displayName;
            }

            int selIndex = 0;
            Category selCat = (Category)serializedObject.FindProperty("category").objectReferenceValue;
            if (selCat != null)
            {
                for (int i = 0; i < catNames.Length; i++)
                {
                    if (catNames[i] == selCat.displayName)
                    {
                        selIndex = i;
                        break;
                    }
                }
            }

            selIndex = EditorGUILayout.Popup("Item Category", selIndex, catNames);
            serializedObject.FindProperty("category").objectReferenceValue = db.categories[selIndex];
        }

        private void DrawGeneral(bool autoLoot = false, bool autoEquip = false, InventoryDB db = null)
        {
            bool isSkill = (ItemType)SimpleInt("itemType") == ItemType.Skill;

            if (SectionToggle((int)displayFlags, (int)DisplayFlags.Behaviour, "Behaviour", BehaviourIcon))
            {
                if (db == null)
                {
                    SimpleProperty("category");
                }
                else
                {
                    DrawCategoryFromDb(db);
                }
                SimpleProperty("itemType");
                if ((ItemType)serializedObject.FindProperty("itemType").intValue == ItemType.Weapon)
                {
                    SimpleProperty("usesAmmo");
                    if (serializedObject.FindProperty("usesAmmo").boolValue)
                    {
                        SimpleProperty("ammoType");
                        SimpleProperty("ammoPerUse");
                    }
                }
                else if ((ItemType)SimpleInt("itemType") == ItemType.Ammo)
                {
                    SimpleProperty("ammoType");
                    SimpleProperty("projectile");
                }

                SimpleProperty("canDrop");
                if (serializedObject.FindProperty("canDrop").boolValue)
                {
                    if (autoLoot)
                    {
                        GUI.enabled = false;
                        EditorGUILayout.TextField("Drop Object", "{Auto Creating}");
                        GUI.enabled = true;
                    }
                    else
                    {
                        SimpleProperty("dropObject");
                    }
                }
                SimpleProperty("displayInInventory", "Display in List");
                SimpleProperty("useSlotId");
                if (serializedObject.FindProperty("useSlotId").boolValue)
                {
                    SimpleProperty("slotId");
                }
                if (!isSkill)
                {
                    SimpleProperty("allowCustomName");
                }
                SimpleProperty("canSell");
                SimpleProperty("allowHotbar");

                if ((ItemType)serializedObject.FindProperty("itemType").intValue == ItemType.Container)
                {
                    SectionHeader("Storage");
                    SimpleProperty("mustEmptyToHold");
                    SimpleProperty("hasMaxStoreSlots", "Has Max Slots");
                    if (serializedObject.FindProperty("hasMaxStoreSlots").boolValue)
                    {
                        SimpleProperty("maxStoreSlots", "Max Slots");
                    }
                    SimpleProperty("hasMaxStoreWeight", "Has Max Weight");
                    if (serializedObject.FindProperty("hasMaxStoreWeight").boolValue)
                    {
                        SimpleProperty("maxStoreWeight", "Max Weight");
                    }
                }

                switch ((ItemType)SimpleInt("itemType"))
                {
                    case ItemType.Component:
                    case ItemType.Consumable:
                    case ItemType.Ingredient:
                        SimpleProperty("autoUse");
                        if (SectionToggle((int)displayFlags, (int)DisplayFlags.Unlocking, "Unlock on Use/Consume", UnlockIcon))
                        {
                            if (db == null)
                            {
                                SubHeader("Categories", "unlockCategories", typeof(Category));
                                SimpleList("unlockCategories");
                                GUILayout.Space(6);
                                SubHeader("Crafting Categories", "unlockCraftingCategories", typeof(CraftingCategory));
                                SimpleList("unlockCraftingCategories");
                                GUILayout.Space(6);
                                SubHeader("Crafting Recipes", "unlockRecipes", typeof(CraftingRecipe));
                                SimpleList("unlockRecipes");
                            }
                            else
                            {
                                DrawUnlockFromDb(db);
                            }
                        }
                        break;
                }
            }

            if (SectionToggle((int)displayFlags, (int)DisplayFlags.Stacking, "Stacking", StackingIcon))
            {
                if ((ItemType)serializedObject.FindProperty("itemType").intValue == ItemType.Container)
                {
                    GUILayout.Label("Containers cannot be stacked", Skin.GetStyle("WrapText"));
                    serializedObject.FindProperty("canStack").boolValue = false;
                }
                else
                {
                    SimpleProperty("canStack", "Stackable");
                    if (serializedObject.FindProperty("canStack").boolValue)
                    {
                        SimpleProperty("countPerStack");
                        SimpleProperty("hasMaxStacks");
                        if (serializedObject.FindProperty("hasMaxStacks").boolValue)
                        {
                            SimpleProperty("maxStacks");
                        }
                    }
                }
            }

            if (!isSkill)
            {
                if ((ItemType)SimpleInt("itemType") != ItemType.Attachment)
                {
                    if (SectionToggle((int)displayFlags, (int)DisplayFlags.Equipping, "Equipping", EquipIcon))
                    {
                        StatsBoolToggle("canEquip", "Can Equip", "equipSource", "equipExpression");
                        if (serializedObject.FindProperty("canEquip").boolValue)
                        {
                            SimpleProperty("canStore");
                            SimpleProperty("freeSlotWhenEquipped");
                            if (autoEquip)
                            {
                                GUI.enabled = false;
                                EditorGUILayout.TextField("Equip Object", "{Auto Creating}");
                                GUI.enabled = true;
                            }
                            else
                            {
                                SimpleProperty("equipObject");
                            }
                            SimpleProperty("autoEquip");

                            SimpleProperty("equipLocation");
                            GUILayout.Space(4);
                            equipPoints.DoLayoutList();

                            if (SectionToggle((int)displayFlags, (int)DisplayFlags.Animation, "Animator Mods", AnimateIcon))
                            {
                                SectionHeader("On Equip");
                                SimpleList("equipAnimatorMods");

                                SectionHeader("On Un-equip");
                                SimpleList("unequipAnimatorMods");
                            }
                        }
                    }
                }
            }
            else
            {
                if (SectionToggle((int)displayFlags, (int)DisplayFlags.Animation, "Animator Mods", AnimateIcon))
                {
                    SectionHeader("On Skill Equip");
                    SimpleList("equipAnimatorMods");

                    SectionHeader("On Skill Un-equip");
                    SimpleList("unequipAnimatorMods");
                }
            }
        }

        private void DrawRepairBreakdown()
        {
            if (SectionToggle((int)displayFlags, (int)DisplayFlags.Repair, "Repair", RepairIcon))
            {
                SimpleProperty("condition");
                StatsBoolToggle("canRepair", "Can Repair", "repairSource", "repairExpression");
                if (serializedObject.FindProperty("canRepair").boolValue)
                {
                    SimpleProperty("repairIncrement");
                    SimpleProperty("incrementCost");
                    repair.DoLayoutList();
                }
            }

            if (SectionToggle((int)displayFlags, (int)DisplayFlags.Breakdown, "Breakdown", BreakdownIcon))
            {
                StatsBoolToggle("canBreakdown", "Can Breakdown", "breakdownSource", "breakdownExpression");
                if (serializedObject.FindProperty("canBreakdown").boolValue)
                {
                    SimpleProperty("breakdownCategory", "Category");
                    breakdown.DoLayoutList();
                }
            }
        }

        private void DrawSlots()
        {
            InventoryItem myTarget = null;
            AttachPoint[] points = null;

            if (target is InventoryItem) myTarget = (InventoryItem)target;
            if (myTarget != null && myTarget.equipObject != null) points = myTarget.equipObject.GetComponentsInChildren<AttachPoint>();

            SubHeader("Slots");
            //GUI.enabled = false;
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(8);
            if (points == null || points.Length == 0)
            {
                GUILayout.Label("None", Skin.GetStyle("WrapText"), GUILayout.ExpandWidth(true));
            }
            else
            {
                EditorGUILayout.BeginVertical();
                foreach (AttachPoint point in points)
                {
                    GUILayout.Label(point.pointId, GUILayout.ExpandWidth(true));

                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(6);
        }

        private void DrawStats()
        {
            if (SectionToggle((int)displayFlags, (int)DisplayFlags.Stas, "Stats", StatsIcon))
            {
                SimpleProperty("rarity");
                SimpleProperty("weight");
                SimpleProperty("value");
                SectionHeader("Effects", "statEffects", typeof(StatEffect));
                SimpleList("statEffects");
            }
        }

        private void DrawTags()
        {
            if (SectionToggle((int)displayFlags, (int)DisplayFlags.Tags, "Custom Tags", TagsIcon))
            {
                SimpleList("customTags");
            }
        }

        private void DrawUI(bool autoPreview = false)
        {
            if (SectionToggle((int)displayFlags, (int)DisplayFlags.UI, "UI", UIIcon))
            {
                SimpleProperty("icon");
                SimpleProperty("displayName");
                SimpleProperty("subtext");
                SimpleProperty("description");
                SimpleProperty("displayRecipe");

                GUILayout.Space(12);
                DragBox(serializedObject.FindProperty("uiTags"), typeof(InventoryItemUITag), "Drag & Drop UI Tags Here");
                uiTags.DoLayoutList();
            }

            if (SectionToggle((int)displayFlags, (int)DisplayFlags.Preview, "Preview", PreviewIcon))
            {
                if (autoPreview)
                {
                    GUI.enabled = false;
                    EditorGUILayout.TextField("Object", "{Auto Creating}");
                    GUI.enabled = true;
                }
                else
                {
                    SimpleProperty("previewObject", "Object");
                }
                SimpleProperty("previewScale", "Scale");
            }
        }

        private void DrawUnlockFromDb(InventoryDB db)
        {
            string[] itemCats, craftCats, recipes;
            int i;

            if (db.categories == null || db.categories.Count == 0)
            {
                itemCats = new string[] { "No Categories Available" };
            }
            else
            {
                itemCats = new string[db.categories.Count];
                for (i = 0; i < itemCats.Length; i++)
                {
                    itemCats[i] = db.categories[i].displayName;
                }
            }

            if (db.craftingCategories == null || db.craftingCategories.Count == 0)
            {
                craftCats = new string[] { "No Categories Available" };
            }
            else
            {
                craftCats = new string[db.craftingCategories.Count];
                for (i = 0; i < craftCats.Length; i++)
                {
                    if (db.craftingCategories[i] != null)
                    {
                        craftCats[i] = db.craftingCategories[i].displayName;
                    }
                    else
                    {
                        craftCats[i] = string.Empty;
                    }
                }
            }

            if (db.recipes == null || db.recipes.Count == 0)
            {
                recipes = new string[] { "No Recipes Available" };
            }
            else
            {
                recipes = new string[db.recipes.Count];
                for (i = 0; i < recipes.Length; i++)
                {
                    recipes[i] = db.recipes[i].displayName;
                }
            }


            SubHeader("Categories", "unlockCategories");
            GUILayout.BeginHorizontal();
            sel1 = EditorGUILayout.Popup(sel1, itemCats);
            if (GUILayout.Button("Add"))
            {
                ((InventoryItem)target).unlockCategories.Add(db.categories[sel1]);
            }
            GUILayout.EndHorizontal();
            SimpleList("unlockCategories", false);

            GUILayout.Space(6);

            SubHeader("Crafting Categories", "unlockCraftingCategories");
            GUILayout.BeginHorizontal();
            sel2 = EditorGUILayout.Popup(sel2, craftCats);
            if (GUILayout.Button("Add"))
            {
                ((InventoryItem)target).unlockCraftingCategories.Add(db.craftingCategories[sel2]);
            }
            GUILayout.EndHorizontal();
            SimpleList("unlockCraftingCategories", false);

            GUILayout.Space(6);

            SubHeader("Crafting Recipes", "unlockRecipes");
            GUILayout.BeginHorizontal();
            sel3 = EditorGUILayout.Popup(sel3, recipes);
            if (GUILayout.Button("Add"))
            {
                ((InventoryItem)target).unlockRecipes.Add(db.recipes[sel3]);
            }
            GUILayout.EndHorizontal();
            SimpleList("unlockRecipes");
        }

        private void SetupComponentsCallback(ReorderableList list)
        {
            list.onAddCallback += ComponentsAddCallback;

            // Elements
            list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var element = list.serializedProperty.GetArrayElementAtIndex(index);

                EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("item"), new GUIContent("Item", null, string.Empty));
                rect.y += EditorGUIUtility.singleLineHeight + 2;

                EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("count"), new GUIContent("Count", null, string.Empty));
                rect.y += EditorGUIUtility.singleLineHeight + 2;
            };
        }

        private void StatsBoolToggle(string baseValue, string displayName, string sourceValue, string expressionValue)
        {
            if ((BooleanSource)serializedObject.FindProperty(sourceValue).intValue == BooleanSource.Static)
            {
                EditorGUILayout.BeginHorizontal();
                SimpleProperty(sourceValue, displayName);
                serializedObject.FindProperty(baseValue).boolValue = EditorGUILayout.Toggle(serializedObject.FindProperty(baseValue).boolValue);
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.BeginVertical();
                SimpleProperty(sourceValue, displayName);
                SimpleProperty(expressionValue, " ");
                EditorGUILayout.EndVertical();
            }
        }

        #endregion

        #region Window Methods

        internal void DrawInspector(InventoryDB db, bool autoLoot, bool autoEquip, bool autoPreview, bool autoAttach)
        {
            displayFlags = (DisplayFlags)serializedObject.FindProperty("z_display_flags").intValue;
            DrawGeneral(autoLoot, autoEquip, db);
            DrawUI(autoPreview);
            if ((ItemType)SimpleInt("itemType") != ItemType.Skill)
            {
                if ((ItemType)SimpleInt("itemType") != ItemType.Attachment)
                {
                    DrawAttachments();
                }
                else
                {
                    DrawAttachmentSettings(autoAttach);
                }
                DrawRepairBreakdown();
            }
            DrawStats();
            DrawTags();
            GUILayout.Space(6);
        }

        #endregion

    }
}