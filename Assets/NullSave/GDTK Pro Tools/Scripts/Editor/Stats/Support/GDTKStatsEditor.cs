#if GDTK
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace NullSave.GDTK.Stats
{
    public class GDTKStatsEditor : GDTKEditor
    {

        #region Enumerations

        [Flags]
        private enum TraitExpansionFlags
        {
            Attributes = 2,
            Languages = 4,
            StatusConditions = 8,
            ForbidEquipTypes = 32,
        }

        #endregion

        #region Public Methods

        public void DebugStat(GDTKStat stat, ref bool expanded)
        {
            expanded = DrawToggleBarTitle(expanded, stat.info.id + " (" + stat.value + "/" + stat.maximum + ") [" + stat.special + "]");
            if (expanded)
            {
                GUI.enabled = false;
                EditorGUILayout.TextField("Id", stat.info.id);
                GUI.enabled = true;

                SubHeader("Current Values");
                DebugStatValue("Maximum", stat.expressions.maximum);
                DebugStatValue("Minimum", stat.expressions.minimum);
                DebugStatValue("Value", stat.expressions.value);
                DebugStatValue("Special", stat.expressions.special);

                SubHeader("Regeneration");
                GUI.enabled = false;
                EditorGUILayout.Toggle("Enabled", stat.regeneration.isEnabled);
                GUI.enabled = true;
                DebugStatValue("Delay", stat.regeneration.delay);
                DebugStatValue("Rate", stat.regeneration.rate);
            }
        }

        public void DebugStatList(BasicStats source, ref bool[] expandedList)
        {
            if (source.stats == null) return;
            int index = 0;
            if (expandedList == null)
            {
                expandedList = new bool[source.stats.Count];
            }
            foreach (var entry in source.stats)
            {
                DebugStat(entry.Value, ref expandedList[index++]);
            }
        }

        public void DebugStatValue(string title, GDTKStatValue statValue)
        {
            switch (statValue.valueType)
            {
                case ValueType.Conditional:
                    GUI.enabled = false;
                    EditorGUILayout.DoubleField(title, statValue.value);
                    GUI.enabled = true;
                    break;
                case ValueType.RandomRange:
                    statValue.value = EditorGUILayout.DoubleField(title, statValue.value);
                    if (statValue.value != statValue.valueWithModifiers)
                    {
                        GUI.enabled = false;
                        EditorGUILayout.DoubleField("(with modifiers)", statValue.valueWithModifiers);
                        GUI.enabled = true;
                    }
                    break;
                case ValueType.Standard:
                    if (statValue.isExpressionNumeric)
                    {
                        statValue.value = EditorGUILayout.DoubleField(title, statValue.value);
                        if (statValue.value != statValue.valueWithModifiers)
                        {
                            GUI.enabled = false;
                            EditorGUILayout.DoubleField("(with modifiers)", statValue.valueWithModifiers);
                            GUI.enabled = true;
                        }
                    }
                    else
                    {
                        statValue.valueExpression = EditorGUILayout.TextField(title, statValue.valueExpression);
                        GUI.enabled = false;
                        EditorGUILayout.DoubleField(" ", statValue.value);
                        if (statValue.value != statValue.valueWithModifiers)
                        {
                            EditorGUILayout.DoubleField("(with modifiers)", statValue.valueWithModifiers);
                        }
                        GUI.enabled = true;
                    }
                    break;
            }
        }

        public void DrawAddOnPluginList(List<UniversalPluginWrapper<AddOnPlugin>> actions, UniversalObjectEditorInfo plugins)
        {
            Color resColor = GUI.contentColor;
            GUIStyle style;
            UniversalObjectEditorItemInfo itemInfo;
            int toggleIndex = -1;
            int removeAt = -1;

            float maxWidth = EditorGUIUtility.currentViewWidth - 24 - 40;

            for (int i = 0; i < actions.Count; i++)
            {
                UniversalPluginWrapper<AddOnPlugin> wrapper = actions[i];
                if (wrapper.plugin != null)
                {
                    itemInfo = plugins.GetInfo(wrapper.instanceId);
                    if (itemInfo.editor == null)
                    {
                        itemInfo.editor = UniversalPluginEditor.CreateEditor(wrapper.plugin);
                        wrapper.serializationData = SimpleJson.ToJSON(wrapper.plugin);
                    }

                    // Check drag
                    if (plugins.isDragging && plugins.curIndex == i)
                    {
                        GUILayout.Space(2);
                        GUILayout.Label(string.Empty, Styles.Redline, GUILayout.Height(1), GUILayout.ExpandWidth(true));
                        GUILayout.Space(2);
                    }

                    // Draw plugin bar
                    GUILayout.BeginHorizontal();
                    EditorGUI.BeginDisabledGroup(itemInfo.isDragging && itemInfo.isDragging);

                    // Item drag handle
                    GUI.contentColor = Styles.EditorColor;
                    GUILayout.Label(GetIcon("Burger", "icons/burger"), Styles.ButtonLeft, GUILayout.Width(22), GUILayout.Height(22));
                    GUI.contentColor = resColor;
                    if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDrag)
                    {
                        plugins.BeginDrag(i, this, ref itemInfo);
                        GUIUtility.hotControl = GUIUtility.GetControlID(FocusType.Passive);
                    }

                    // Title
                    style = new GUIStyle(itemInfo.isExpanded ? Styles.ButtonMidPressed : Styles.ButtonMid);
                    GUILayout.BeginHorizontal(style, GUILayout.Height(22), GUILayout.MaxWidth(maxWidth));
                    GUILayout.Space(2);
                    GUILayout.Label(wrapper.plugin.titlebarText, Styles.TitleTextStyle, GUILayout.Height(15));
                    GUILayout.EndHorizontal();
                    if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDown)
                    {
                        toggleIndex = i;
                    }

                    // Delete
                    GUI.contentColor = Styles.EditorColor;
                    GUILayout.Label(GetIcon("Trash", "icons/trash"), Styles.ButtonRight, GUILayout.Width(22), GUILayout.Height(22));
                    GUI.contentColor = resColor;
                    if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDown)
                    {
                        removeAt = i;
                    }

                    EditorGUI.EndDisabledGroup();
                    GUILayout.EndHorizontal();

                    if (itemInfo.isExpanded)
                    {
                        GUILayout.Space(-5);
                        GUILayout.BeginVertical("box");
                        itemInfo.editor.OnInspectorGUI();
                        GUILayout.EndVertical();
                    }

                    if (itemInfo.editor.IsDirty)
                    {
                        itemInfo.editor.ApplyUpdates(ref wrapper.serializationData);
                        EditorUtility.SetDirty(target);
                    }
                }
                else
                {
                    removeAt = i;
                }
            }

            if (toggleIndex > -1)
            {
                plugins.items[actions[toggleIndex].instanceId].isExpanded = !plugins.items[actions[toggleIndex].instanceId].isExpanded;
                if (plugins.items[actions[toggleIndex].instanceId].isExpanded)
                {
                    plugins.items[actions[toggleIndex].instanceId].editor.OnEnable();
                }
            }

            if (removeAt > -1)
            {
                if (plugins.items != null)
                {
                    plugins.items.Remove(actions[removeAt].instanceId);
                }
                actions.RemoveAt(removeAt);
                EditorUtility.SetDirty(target);
            }

            plugins.UpdateDragPosition(actions, this, GDTKEditor.Styles.Redline);
        }

        public bool DrawAddOnChoicePlugin(SerializedProperty target, AddOnPluginChoice choice, UniversalObjectEditorInfo plugins, string title, StatsDatabase database, object inventoryDatabase)
        {
            bool expanded = target.FindPropertyRelative("z_expanded").boolValue;
            bool result;
            bool wantsDelete = false;

            // Draw plugin bar
            GUILayout.BeginHorizontal();

            result = DrawToggleBarTitleLeft(
                expanded, title
                );
            if (result != expanded)
            {
                target.FindPropertyRelative("z_expanded").boolValue = result;
            }

            // Delete
            if (DrawBarIcon(Styles.ButtonRight, GetIcon("icons/trash")))
            {
                wantsDelete = true;
            }

            GUILayout.EndHorizontal();

            // Draw Expanded item
            if (expanded)
            {
                GUILayout.Space(-4);
                GUILayout.BeginVertical("box");

                bool infoExpanded = target.FindPropertyRelative("info").FindPropertyRelative("z_expanded").boolValue;
                target.FindPropertyRelative("info").FindPropertyRelative("z_expanded").boolValue = SectionToggle(infoExpanded, "Info", GetIcon("Icons/list"));
                if (infoExpanded)
                {
                    SimpleProperty(target, "info");
                }

                // New AddOn Plugins
                GUILayout.Space(8);
                SectionHeader("Add-On Plugins", GetIcon("icons/plugin"));
                GUILayout.BeginHorizontal();
                GUILayout.Space(EditorGUIUtility.labelWidth + 4);
                GUILayout.BeginVertical();
                DrawAddOnPluginList(choice.pickFrom, plugins);
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();

                // Show Add Button
                Rect r = EditorGUILayout.GetControlRect(true, GUILayout.ExpandWidth(true));
                GUILayout.Space(-EditorGUIUtility.singleLineHeight);
                GUILayout.BeginHorizontal();
                GUILayout.Space(EditorGUIUtility.labelWidth + 4);
                if (GUILayout.Button(new GUIContent("Add Option", GetIcon("icons/add-small")), GUILayout.Height(24)))
                {
                    GUI.FocusControl(null);
                    PopupWindow.Show(r, new AddOnPluginPicker() { targetList = target, target = choice.pickFrom });
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.EndVertical();
            }

            return wantsDelete;
        }

        public void DrawAddOnChoicePluginList(SerializedProperty list, List<AddOnPluginChoice> choices, StatsDatabase database, UniversalObjectEditorInfo plugins, object inventoryDatabase)
        {
            int removeAt = -1;

            DrawListHeader("Add-On Choices", list.arraySize, GetIcon("icons/prompt"));

            GUILayout.BeginHorizontal();
            GUILayout.Space(EditorGUIUtility.labelWidth);
            GUILayout.BeginVertical();
            for (int i = 0; i < choices.Count; i++)
            {
                if (DrawAddOnChoicePlugin(list.GetArrayElementAtIndex(i), choices[i], plugins, "Option " + (i + 1), database, inventoryDatabase))
                {
                    removeAt = i;
                }
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            if (removeAt > -1)
            {
                int preSize = list.arraySize;
                list.DeleteArrayElementAtIndex(removeAt);

                // This is an object so sometimes we need to delete twice to really clear
                if (list.arraySize == preSize)
                {
                    list.DeleteArrayElementAtIndex(removeAt);
                    GUI.FocusControl(null);
                }
            }

            GUILayout.BeginHorizontal();
            GUILayout.Space(EditorGUIUtility.labelWidth);
            if (GUILayout.Button(new GUIContent("Add Option", GetIcon("icons/add-small")), GUILayout.Height(24)))
            {
                list.arraySize++;
                ClearLastAddOnChoiceItem(list);
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        public bool DrawEffect(SerializedProperty effect, EditorInfoList modifierList, Action<List<string>> effectBuildOptions, Action<List<string>> attributeBuildOptions)
        {
            bool wantsDelete = false;
            bool expanded = effect.FindPropertyRelative("z_expanded").boolValue;

            // Draw plugin bar
            GUILayout.BeginHorizontal();

            effect.FindPropertyRelative("z_expanded").boolValue = DrawToggleBarTitleLeft(
                expanded, effect.FindPropertyRelative("info").FindPropertyRelative("id").stringValue
                );

            ////Copy
            //if (DrawBarIcon(Styles.ButtonMid, GetIcon("icons/duplicate")))
            //{
            //    CopyStatToClipboard(stat);
            //}

            // Delete
            if (DrawBarIcon(Styles.ButtonRight, GetIcon("icons/trash")))
            {
                wantsDelete = true;
            }

            GUILayout.EndHorizontal();

            // Draw Expanded item
            if (expanded)
            {
                GUILayout.Space(-4);
                GUILayout.BeginVertical("box");
                GUILayout.Space(4);

                DrawEffectData(effect, modifierList, effectBuildOptions, attributeBuildOptions);

                GUILayout.EndVertical();
            }

            return wantsDelete;
        }

        public void DrawEffectData(SerializedProperty effect, EditorInfoList modifierList, Action<List<string>> effectBuildOptions, Action<List<string>> attributeBuildOptions, bool includeInfo = true)
        {
            if (includeInfo)
            {
                bool infoExpanded = effect.FindPropertyRelative("info").FindPropertyRelative("z_expanded").boolValue;
                effect.FindPropertyRelative("info").FindPropertyRelative("z_expanded").boolValue = SectionToggle(infoExpanded, "Info", GetIcon("Icons/list"));
                if (infoExpanded)
                {
                    SimpleProperty(effect, "info");
                }
            }

            SectionHeader("Behavior", GetIcon("icons/behavior"));
            SimpleProperty(effect, "maxStack");
            SimpleProperty(effect, "expires");
            if ((EffectExpiry)SimpleValue<int>(effect, "expires") != EffectExpiry.Automatically)
            {
                SimpleProperty(effect, "expiryTime", "Expires After");
            }

            GUILayout.Space(8);
            DrawStatModifierList(effect.FindPropertyRelative("modifiers"), modifierList);

            SimpleStringSearchHeaderListProperty(effect.FindPropertyRelative("cancelEffectIds"), "Prevent Effects", GetIcon("icons/prevent"), effectBuildOptions);

            SimpleStringSearchHeaderListProperty(effect.FindPropertyRelative("attributeIds"), "Attributes", GetIcon("icons/tag"), attributeBuildOptions);

            GUILayout.Space(8);
            SectionHeader("Spawn with Effect", GetIcon("icons/object"));
            SimpleProperty(effect, "spawnInfo");
        }

        public bool DrawStat(SerializedProperty stat, out bool duplicate)
        {
            bool wantsDelete = false;
            bool expanded = stat.FindPropertyRelative("z_expanded").boolValue;

            duplicate = false;

            // Draw plugin bar
            GUILayout.BeginHorizontal();

            stat.FindPropertyRelative("z_expanded").boolValue = DrawToggleBarTitleLeft(
                expanded, stat.FindPropertyRelative("info").FindPropertyRelative("id").stringValue
                );

            //Copy
            if (DrawBarIcon(Styles.ButtonMid, GetIcon("icons/duplicate"), "Duplicate"))
            {
                //CopyStatToClipboard(stat);
                duplicate = true;
            }

            // Delete
            if (DrawBarIcon(Styles.ButtonRight, GetIcon("icons/trash"), "Delete"))
            {
                wantsDelete = true;
            }

            GUILayout.EndHorizontal();

            // Draw Expanded item
            if (expanded)
            {
                GUILayout.Space(-4);
                GUILayout.BeginVertical("box");

                bool infoExpanded = stat.FindPropertyRelative("info").FindPropertyRelative("z_expanded").boolValue;
                stat.FindPropertyRelative("info").FindPropertyRelative("z_expanded").boolValue = SectionToggle(infoExpanded, "Info", GetIcon("Icons/list"));
                if (infoExpanded)
                {
                    SimpleProperty(stat, "info");
                }

                SerializedProperty subProp = stat.FindPropertyRelative("expressions");
                infoExpanded = subProp.FindPropertyRelative("z_expanded").boolValue;
                subProp.FindPropertyRelative("z_expanded").boolValue = SectionToggle(infoExpanded, "Values", GetIcon("icons/meter-small"));
                if (infoExpanded)
                {
                    SimpleProperty(subProp, "minimum");
                    SimpleProperty(subProp, "maximum");
                    SimpleProperty(stat, "startMaxed");
                    if (!SimpleValue<bool>(stat, "startMaxed"))
                    {
                        SimpleProperty(subProp, "value");
                    }
                    SimpleProperty(subProp, "special");
                }

                subProp = stat.FindPropertyRelative("regeneration");
                infoExpanded = subProp.FindPropertyRelative("z_expanded").boolValue;
                subProp.FindPropertyRelative("z_expanded").boolValue = SectionToggle(infoExpanded, "Regeneration", GetIcon("icons/wait"));
                if (infoExpanded)
                {
                    SimpleProperty(subProp, "enabled");
                    if (ShowConditionalBool(subProp.FindPropertyRelative("enabled")))
                    {
                        SimpleProperty(subProp, "delay");
                        SimpleProperty(subProp, "rate");
                        SimpleProperty(subProp, "wholeIncrements");
                    }
                }

                subProp = stat.FindPropertyRelative("incrementation");
                infoExpanded = subProp.FindPropertyRelative("z_expanded").boolValue;
                subProp.FindPropertyRelative("z_expanded").boolValue = SectionToggle(infoExpanded, "Incrementing", GetIcon("icons/upgrade"));
                if (infoExpanded)
                {
                    SimpleProperty(subProp, "enabled");
                    if (ShowConditionalBool(subProp.FindPropertyRelative("enabled")))
                    {
                        SimpleProperty(subProp, "incrementWhen");
                        SimpleProperty(subProp, "incrementAmount");
                    }

                }
                GUILayout.EndVertical();
            }

            return wantsDelete;
        }

        public void DrawStatList(SerializedProperty list)
        {
            int removeAt = -1;
            int dupAt = -1;

            DrawListHeader("Stats", list.arraySize, GetIcon("icons/stats"));

            GUILayout.BeginHorizontal();
            GUILayout.Space(EditorGUIUtility.labelWidth);
            GUILayout.BeginVertical();
            for (int i = 0; i < list.arraySize; i++)
            {
                if (DrawStat(list.GetArrayElementAtIndex(i), out bool duplicate))
                {
                    removeAt = i;
                }

                if (duplicate)
                {
                    dupAt = i;
                }
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            if (removeAt > -1)
            {
                int preSize = list.arraySize;
                list.DeleteArrayElementAtIndex(removeAt);

                // This is an object so sometimes we need to delete twice to really clear
                if (list.arraySize == preSize)
                {
                    list.DeleteArrayElementAtIndex(removeAt);
                    GUI.FocusControl(null);
                }
            }

            if (dupAt > -1)
            {
                list.InsertArrayElementAtIndex(dupAt);
            }

            GUILayout.BeginHorizontal();
            GUILayout.Space(EditorGUIUtility.labelWidth);
            if (GUILayout.Button(new GUIContent("Add Stat", GetIcon("icons/add-small")), GUILayout.Height(24)))
            {
                list.arraySize++;
                ClearLastStatItem(list);
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        public void DrawStatListNoIndent(SerializedProperty list)
        {
            int removeAt = -1;
            int dupAt = -1;

            for (int i = 0; i < list.arraySize; i++)
            {
                if (DrawStat(list.GetArrayElementAtIndex(i), out bool duplicate))
                {
                    removeAt = i;
                }

                if (duplicate)
                {
                    dupAt = i;
                }
            }

            if (removeAt > -1)
            {
                int preSize = list.arraySize;
                list.DeleteArrayElementAtIndex(removeAt);

                // This is an object so sometimes we need to delete twice to really clear
                if (list.arraySize == preSize)
                {
                    list.DeleteArrayElementAtIndex(removeAt);
                    GUI.FocusControl(null);
                }
            }

            if (dupAt > -1)
            {
                list.InsertArrayElementAtIndex(dupAt);
            }

            if (GUILayout.Button(new GUIContent("Add Stat", GetIcon("icons/add-small")), GUILayout.Height(24)))
            {
                list.arraySize++;
                ClearLastStatItem(list);
            }
        }

        public bool DrawStatModifier(SerializedProperty modifier)
        {
            bool expanded = modifier.FindPropertyRelative("z_expanded").boolValue;
            bool result;

            bool wantsDelete = false;

            // Draw plugin bar
            GUILayout.BeginHorizontal();

            result = DrawToggleBarTitleLeft(expanded, GetModifierDescription(modifier));
            if (result != expanded)
            {
                modifier.FindPropertyRelative("z_expanded").boolValue = result;
            }

            // Delete
            if (DrawBarIcon(Styles.ButtonRight, GetIcon("icons/trash")))
            {
                wantsDelete = true;
            }

            GUILayout.EndHorizontal();

            // Draw Expanded item
            if (expanded)
            {
                ModifierApplication application = (ModifierApplication)SimpleValue<int>(modifier, "applies");

                GUILayout.Space(-4);
                GUILayout.BeginVertical("box");

                SubHeader("Behavior");
                SimpleProperty(modifier, "affectsStatId");
                SimpleProperty(modifier, "requirements");
                SimpleProperty(modifier, "applies");
                if (application == ModifierApplication.RecurringOverSeconds || application == ModifierApplication.RecurringOverTurns)
                {
                    SimpleProperty(modifier, "lifespan");
                    SimpleProperty(modifier, "wholeIncrements");
                }

                SimpleProperty(modifier, "target");
                if (application != ModifierApplication.SetValueOnce && application != ModifierApplication.SetValueUntilRemoved)
                {
                    SimpleProperty(modifier, "changeType");
                }
                SimpleProperty(modifier, "value");


                GUILayout.EndVertical();
            }

            return wantsDelete;
        }

        public void DrawStatModifierList(SerializedProperty list, bool indent = true)
        {
            int removeAt = -1;

            DrawListHeader("Stat Modifiers", list.arraySize, GetIcon("icons/book"));

            if (indent)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(EditorGUIUtility.labelWidth);
                GUILayout.BeginVertical();
            }
            for (int i = 0; i < list.arraySize; i++)
            {
                if (DrawStatModifier(list.GetArrayElementAtIndex(i)))
                {
                    removeAt = i;
                }
            }
            if (indent)
            {
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
            }

            if (removeAt > -1)
            {
                int preSize = list.arraySize;
                list.DeleteArrayElementAtIndex(removeAt);

                // This is an object so sometimes we need to delete twice to really clear
                if (list.arraySize == preSize)
                {
                    list.DeleteArrayElementAtIndex(removeAt);
                    GUI.FocusControl(null);
                }
            }

            if (indent)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(EditorGUIUtility.labelWidth);
            }
            if (GUILayout.Button(new GUIContent("Add Modifier", GetIcon("icons/add-small")), GUILayout.Height(24)))
            {
                list.arraySize++;
                ClearLastModifierItem(list);
            }
            if (indent)
            {
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
        }

        public void DrawStatModifierList(SerializedProperty list, EditorInfoList statModifiers, bool indent = true)
        {
            int removeAt = -1;

            DrawListHeader("Stat Modifiers", list.arraySize, GetIcon("icons/book"));

            if (indent)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(EditorGUIUtility.labelWidth);
                GUILayout.BeginVertical();
            }

            //for (int i = 0; i < list.arraySize; i++)
            //{
            //    if (DrawStatModifier(list.GetArrayElementAtIndex(i)))
            //    {
            //        removeAt = i;
            //    }
            //}

            SimpleObjectDragList(list, DrawStatModifierTitle, statModifiers, DrawStatModifier, true);

            if (indent)
            {
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
            }

            if (removeAt > -1)
            {
                int preSize = list.arraySize;
                list.DeleteArrayElementAtIndex(removeAt);

                // This is an object so sometimes we need to delete twice to really clear
                if (list.arraySize == preSize)
                {
                    list.DeleteArrayElementAtIndex(removeAt);
                    GUI.FocusControl(null);
                }
            }

            if (indent)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(EditorGUIUtility.labelWidth);
            }
            if (GUILayout.Button(new GUIContent("Add Modifier", GetIcon("icons/add-small")), GUILayout.Height(24)))
            {
                list.arraySize++;
                ClearLastModifierItem(list);
            }
            if (indent)
            {
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
        }

        public void DrawStatModifierList(SerializedProperty list, string title, string tooltip, bool indent = true)
        {
            int removeAt = -1;

            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical(GUILayout.Width(EditorGUIUtility.labelWidth));
            GUILayout.Label(new GUIContent(title, tooltip));
            GUILayout.EndVertical();

            GUILayout.Label("(" + list.arraySize + ")");

            GUILayout.EndHorizontal();

            if (indent)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(EditorGUIUtility.labelWidth);
                GUILayout.BeginVertical();
            }
            for (int i = 0; i < list.arraySize; i++)
            {
                if (DrawStatModifier(list.GetArrayElementAtIndex(i)))
                {
                    removeAt = i;
                }
            }
            if (indent)
            {
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
            }

            if (removeAt > -1)
            {
                int preSize = list.arraySize;
                list.DeleteArrayElementAtIndex(removeAt);

                // This is an object so sometimes we need to delete twice to really clear
                if (list.arraySize == preSize)
                {
                    list.DeleteArrayElementAtIndex(removeAt);
                    GUI.FocusControl(null);
                }
            }

            if (indent)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(EditorGUIUtility.labelWidth);
            }
            if (GUILayout.Button(new GUIContent("Add Modifier", GetIcon("icons/add-small")), GUILayout.Height(24)))
            {
                list.arraySize++;
                ClearLastModifierItem(list);
            }
            if (indent)
            {
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
        }

        public void DrawTrait(SerializedProperty target, GDTKTrait trait, UniversalObjectEditorInfo plugins, EditorInfoList statModifiers, StatsDatabase database, object inventoryDatabase, Action<List<string>> buildOptions)
        {
            GUILayout.Space(-4);
            GUILayout.BeginVertical("box");

            if (DrawCollapableBasicInfo(target.FindPropertyRelative("info")))
            {
                SimpleProperty(target, "info");
            }

            GUILayout.Space(8);
            SectionHeader("Behavior", GetIcon("icons/behavior"));
            DrawUnlocking(target.FindPropertyRelative("unlocking"), buildOptions);

            // New AddOn Plugins
            GUILayout.Space(8);
            SectionHeader("Add-On Plugins", GetIcon("icons/plugin"));
            GUILayout.BeginHorizontal();
            GUILayout.Space(EditorGUIUtility.labelWidth + 4);
            GUILayout.BeginVertical();
            DrawAddOnPluginList(trait.addOnPlugins, plugins);
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            // Show Add Button
            Rect r = EditorGUILayout.GetControlRect(true, GUILayout.ExpandWidth(true));
            GUILayout.Space(-EditorGUIUtility.singleLineHeight);
            GUILayout.BeginHorizontal();
            GUILayout.Space(EditorGUIUtility.labelWidth + 4);
            if (GUILayout.Button(new GUIContent("Add Option", GetIcon("icons/add-small")), GUILayout.Height(24)))
            {
                GUI.FocusControl(null);
                PopupWindow.Show(r, new AddOnPluginPicker() { targetList = target, target = trait.addOnPlugins });
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(8);
            DrawStatModifierList(target.FindPropertyRelative("statModifiers"), statModifiers);

            GUILayout.EndVertical();
        }

        #endregion

        #region Private Methods

        private void DrawStatModifierTitle(SerializedProperty item, int index)
        {
            EditorGUILayout.LabelField(GetModifierDescription(item));
        }

        private void DrawStatModifier(SerializedProperty modifier, int index)
        {
            ModifierApplication application = (ModifierApplication)SimpleValue<int>(modifier, "applies");

            GUILayout.Space(-4);
            GUILayout.BeginVertical("box");

            SubHeader("Behavior");
            SimpleProperty(modifier, "affectsStatId");
            SimpleProperty(modifier, "requirements");
            SimpleProperty(modifier, "applies");
            if (application == ModifierApplication.RecurringOverSeconds || application == ModifierApplication.RecurringOverTurns)
            {
                SimpleProperty(modifier, "lifespan");
                SimpleProperty(modifier, "wholeIncrements");
            }

            SimpleProperty(modifier, "target");
            if (application != ModifierApplication.SetValueOnce && application != ModifierApplication.SetValueUntilRemoved)
            {
                SimpleProperty(modifier, "changeType");
            }
            SimpleProperty(modifier, "value");

            GUILayout.EndVertical();
        }


        public void BuildAttributesList(List<string> options, StatsDatabase database)
        {
            foreach (GDTKAttribute attrib in database.attributes)
            {
                options.Add(attrib.info.id);
            }
        }

        public void BuildClassList(List<string> options, StatsDatabase database)
        {
            foreach (GDTKClass pc in database.classes)
            {
                options.Add(pc.info.id);
            }
        }

#if GDTK_Inventory2
        public void BuildCurrencyList(List<string> options, Inventory.InventoryDatabase database)
        {
            options.AddRange(database.currencies.Select(x => x.info.id));
        }
#endif

        public void BuildLangugesList(List<string> options, StatsDatabase database)
        {
            foreach (GDTKLanguage value in database.languages)
            {
                options.Add(value.info.id);
            }
        }

        public void BuildStatusConditionsList(List<string> options, StatsDatabase database)
        {
            foreach (GDTKStatusCondition value in database.statusConditions)
            {
                options.Add(value.info.id);
            }
        }

        public void ClearLastAddOnItem(SerializedProperty list)
        {
            SerializedProperty target = list.GetArrayElementAtIndex(list.arraySize - 1);
            SimpleValue(target, "z_expanded", true);
            target.FindPropertyRelative("type").intValue = 0;
            target.FindPropertyRelative("mode").intValue = 0;
            target.FindPropertyRelative("addCount").intValue = 1;
            target.FindPropertyRelative("groupName").stringValue = string.Empty;
            target.FindPropertyRelative("optionIds").ClearArray();
            target.FindPropertyRelative("m_used").intValue = 0;
            target.FindPropertyRelative("selectedIds").ClearArray();
            SimpleValue(target, "excludeOwned", true);
        }

        public void ClearLastAddOnChoiceItem(SerializedProperty list)
        {
            SerializedProperty target = list.GetArrayElementAtIndex(list.arraySize - 1);
            SimpleValue(target, "z_expanded", true);
            ClearBasicInfo(target.FindPropertyRelative("info"));
            target.FindPropertyRelative("pickFrom").ClearArray();
        }

        public void ClearLastBackgroundItem(SerializedProperty list)
        {
            SerializedProperty target = list.GetArrayElementAtIndex(list.arraySize - 1);
            ClearBasicInfo(target.FindPropertyRelative("info"));
            target.FindPropertyRelative("traits").ClearArray();
        }

        private void ClearBasicInfo(SerializedProperty target)
        {
            SimpleValue(target, "id", string.Empty);
            SimpleValue(target, "autoGenId", true);
            SimpleValue(target, "title", string.Empty);
            SimpleValue(target, "abbr", string.Empty);
            SimpleValue(target, "description", string.Empty);
            SimpleValue(target, "groupName", string.Empty);
            SimpleValue(target, "hidden", false);
            SimpleValue(target, "z_expanded", true);
            target.FindPropertyRelative("color").colorValue = Color.white;
            ClearImageInfo(target.FindPropertyRelative("image"));
            target.FindPropertyRelative("tags").ClearArray();
        }

        public void ClearLastClassItem(SerializedProperty list)
        {
            SerializedProperty target = list.GetArrayElementAtIndex(list.arraySize - 1);
            ClearBasicInfo(target.FindPropertyRelative("info"));
            ClearUnlockingItem(target.FindPropertyRelative("unlocking"));
            SimpleValue(target, "playable", true);
            target.FindPropertyRelative("parentId").stringValue = string.Empty;
            target.FindPropertyRelative("traits").ClearArray();
        }

        private void ClearConditionalBool(SerializedProperty value)
        {
            SimpleValue(value, "value", false);
            SimpleValue(value, "useExpression", false);
            SimpleValue(value, "expression", "1 = 0");
        }

        private void ClearImageInfo(SerializedProperty target)
        {
            target.FindPropertyRelative("sprite").objectReferenceValue = null;
            SimpleValue(target, "source", 0);
            SimpleValue(target, "path", string.Empty);
            SimpleValue(target, "bundleName", string.Empty);
            SimpleValue(target, "assetName", string.Empty);
            SimpleValue(target, "z_imageError", false);
        }

        public void ClearLastActionSequenceItem(SerializedProperty list)
        {
            SerializedProperty target = list.GetArrayElementAtIndex(list.arraySize - 1);
            SimpleValue(target, "id", string.Empty);
            target.FindPropertyRelative("actions").ClearArray();
        }

        public void ClearLastBasicInfo(SerializedProperty list)
        {
            ClearBasicInfo(list.GetArrayElementAtIndex(list.arraySize - 1));
        }

        public void ClearLastChildBasicInfo(SerializedProperty list)
        {
            ClearBasicInfo(list.GetArrayElementAtIndex(list.arraySize - 1).FindPropertyRelative("info"));
        }

        public void ClearLastCreatureSizeItem(SerializedProperty list)
        {
            SerializedProperty target = list.GetArrayElementAtIndex(list.arraySize - 1);
            ClearBasicInfo(target.FindPropertyRelative("info"));
            SimpleValue(target, "size", 0f);
        }

        public void ClearLastEffectItem(SerializedProperty list)
        {
            SerializedProperty target = list.GetArrayElementAtIndex(list.arraySize - 1);
            ClearBasicInfo(target.FindPropertyRelative("info"));
            target.FindPropertyRelative("modifiers").ClearArray();
            SimpleValue(target, "maxStack", 1);
            target.FindPropertyRelative("cancelEffectIds").ClearArray();
            SimpleValue(target, "expires", 0);
            SimpleValue(target, "expiryTime", 1f);
            target.FindPropertyRelative("attributeIds").ClearArray();
            ClearSpawnInfo(target.FindPropertyRelative("spawnInfo"));
        }

        public void ClearLastEventItem(SerializedProperty list)
        {
            SerializedProperty target = list.GetArrayElementAtIndex(list.arraySize - 1);
            ClearBasicInfo(target.FindPropertyRelative("info"));
            SimpleValue(target, "raiseTokenHeartbeat", false);
            SimpleValue(target, "tokens", 1f);
            target.FindPropertyRelative("statModifiers").ClearArray();
        }

        public void ClearLastLanguageItem(SerializedProperty list)
        {
            SerializedProperty target = list.GetArrayElementAtIndex(list.arraySize - 1);
            ClearBasicInfo(target.FindPropertyRelative("info"));
            target.FindPropertyRelative("typicalSpeakers").ClearArray();
            SimpleValue<string>(target, "script", null);
        }

        public void ClearLastLevelRewardItem(SerializedProperty list)
        {
            SerializedProperty target = list.GetArrayElementAtIndex(list.arraySize - 1);
            ClearBasicInfo(target.FindPropertyRelative("info"));
            SimpleValue<string>(target, "requirements", null);
            target.FindPropertyRelative("jsonAddOnPlugins").ClearArray();
            target.FindPropertyRelative("pluginChoices").ClearArray();
            target.FindPropertyRelative("statModifiers").ClearArray();
        }

        private void ClearLastModifierItem(SerializedProperty list)
        {
            SerializedProperty stat = list.GetArrayElementAtIndex(list.arraySize - 1);
            SimpleValue(stat, "z_expanded", true);

            SimpleValue(stat, "affectsStatId", string.Empty);
            SimpleValue(stat, "requirements", string.Empty);
            SimpleValue(stat, "applies", 0);
            SimpleValue(stat, "target", 0);
            SimpleValue(stat, "changeType", 0);
            SimpleValue(stat, "lifespan", 0f);
            SimpleValue(stat, "wholeIncrements", false);
            ClearStatValue(stat.FindPropertyRelative("value"), "0");
        }

        public void ClearLastPerkItem(SerializedProperty list)
        {
            SerializedProperty target = list.GetArrayElementAtIndex(list.arraySize - 1);
            ClearBasicInfo(target.FindPropertyRelative("info"));
            ClearUnlockingItem(target.FindPropertyRelative("unlocking"));
            target.FindPropertyRelative("attributeIds").ClearArray();
            target.FindPropertyRelative("conditionIds").ClearArray();
            target.FindPropertyRelative("statModifiers").ClearArray();
        }

        public void ClearLastRaceItem(SerializedProperty list)
        {
            SerializedProperty target = list.GetArrayElementAtIndex(list.arraySize - 1);
            ClearBasicInfo(target.FindPropertyRelative("info"));
            SimpleValue(target, "playable", true);
            target.FindPropertyRelative("parentId").stringValue = string.Empty;
            target.FindPropertyRelative("traits").ClearArray();
        }

        private void ClearLastStatItem(SerializedProperty list)
        {
            SerializedProperty stat = list.GetArrayElementAtIndex(list.arraySize - 1);
            SimpleValue(stat, "startMaxed", false);
            SimpleValue(stat, "z_expanded", true);

            ClearBasicInfo(stat.FindPropertyRelative("info"));

            SerializedProperty subProp = stat.FindPropertyRelative("expressions");
            ClearStatValue(subProp.FindPropertyRelative("minimum"), "0");
            ClearStatValue(subProp.FindPropertyRelative("maximum"), "100");
            ClearStatValue(subProp.FindPropertyRelative("value"), "0");
            ClearStatValue(subProp.FindPropertyRelative("special"), "0");
            SimpleValue(subProp, "z_expanded", true);

            subProp = stat.FindPropertyRelative("regeneration");
            ClearConditionalBool(subProp.FindPropertyRelative("enabled"));
            ClearStatValue(subProp.FindPropertyRelative("delay"), "1");
            ClearStatValue(subProp.FindPropertyRelative("rate"), "1");
            SimpleValue(subProp, "wholeIncrements", false);

            subProp = stat.FindPropertyRelative("incrementation");
            ClearConditionalBool(subProp.FindPropertyRelative("enabled"));
            SimpleValue(subProp, "incrementWhen", "1 = 0");
            ClearStatValue(subProp.FindPropertyRelative("incrementAmount"), "1");
        }

        public void ClearLastStatusConditionItem(SerializedProperty list)
        {
            SerializedProperty target = list.GetArrayElementAtIndex(list.arraySize - 1);
            ClearBasicInfo(target.FindPropertyRelative("info"));

            SimpleValue(target, "activateWhen", 0);
            SimpleValue(target, "deactivateWhen", 0);

            SimpleValue(target, "startCondition", "1 = 0");
            SimpleValue(target, "endCondition", "1 = 0");

            SimpleValue(target, "startEvent", string.Empty);
            SimpleValue(target, "endEvent", string.Empty);
            SimpleValue(target, "endTime", 1f);
            ClearSpawnInfo(target.FindPropertyRelative("spawnInfo"));

            target.FindPropertyRelative("statModifiers").ClearArray();
        }

        private void ClearSpawnInfo(SerializedProperty target)
        {
            SimpleValue(target, "source", 0);
            SimpleValue(target, "path", string.Empty);
            SimpleValue(target, "bundleName", string.Empty);
            SimpleValue(target, "assetName", string.Empty);
            SimpleValue(target, "parent", false);
            target.FindPropertyRelative("gameObject").objectReferenceValue = null;
            target.FindPropertyRelative("offset").vector3Value = Vector3.zero;
            SimpleValue(target, "z_spawnError", false);
        }

        private void ClearStatValue(SerializedProperty statValue, string defaultValue)
        {
            SimpleValue(statValue, "m_valueType", 0);
            SimpleValue(statValue, "m_value", defaultValue);
            SimpleValue(statValue, "m_randomMin", "0");
            SimpleValue(statValue, "m_randomMax", "1");
            statValue.FindPropertyRelative("m_conditions").ClearArray();
        }

        private void ClearUnlockingItem(SerializedProperty target)
        {
            SimpleValue(target, "unlock", 0);
            SimpleValue(target, "level", 0);
            SimpleValue(target, "expression", string.Empty);
            SimpleValue(target, "classId", string.Empty);
            SimpleValue(target, "z_expanded", true);
        }

        private bool DrawCollapableBasicInfo(SerializedProperty target)
        {
            bool infoExpanded = target.FindPropertyRelative("z_expanded").boolValue;
            bool result = SectionToggle(infoExpanded, "Info", GetIcon("Icons/list"));
            if (result != infoExpanded)
            {
                target.FindPropertyRelative("z_expanded").boolValue = result;
            }

            return infoExpanded;
        }

        public void DrawListHeader(string title, int count, Texture2D icon = null)
        {
            GUILayout.BeginHorizontal();

            GUILayout.BeginHorizontal(GUILayout.Width(EditorGUIUtility.labelWidth - 4));
            if (icon != null)
            {
                Color res = GUI.color;
                GUILayout.BeginVertical();
                GUILayout.Label(icon, GUILayout.Width(18), GUILayout.Height(18));
                GUILayout.EndVertical();
                GUI.color = res;
            }

            GUILayout.BeginVertical();
            if (icon != null)
            {
                GUILayout.Space(2);
            }

            GUILayout.Label(title, Styles.SectionHeaderStyle);
            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();

            GUILayout.EndHorizontal();

            GUILayout.Label("(" + count + ")");

            GUILayout.EndHorizontal();
        }

        public void DrawUnlocking(SerializedProperty target, Action<List<string>> buildOptions)
        {
            SimpleProperty(target, "unlock");
            switch ((Unlocking)SimpleValue<int>(target, "unlock"))
            {
                case Unlocking.AtCharacterLevel:
                    SimpleProperty(target, "level");
                    break;
                case Unlocking.AtClassLevel:
                    SimpleStringSearchProperty(target.FindPropertyRelative("classId"), "Class Id", buildOptions);
                    SimpleProperty(target, "level");
                    break;
                case Unlocking.ByExpression:
                    SimpleProperty(target, "expression");
                    break;
            }
        }

        private string GetModifierDescription(SerializedProperty target)
        {
            StringBuilder sb = new StringBuilder();

            ModifierApplication application = (ModifierApplication)SimpleValue<int>(target, "applies");

            sb.Append(SimpleValue<string>(target, "affectsStatId"));

            switch ((ModifierTarget)SimpleValue<int>(target, "target"))
            {
                case ModifierTarget.Maximum:
                    sb.Append(".maximum");
                    break;
                case ModifierTarget.Minimum:
                    sb.Append(".minimum");
                    break;
                case ModifierTarget.RegenerationDelay:
                    sb.Append(".regenerationDelay");
                    break;
                case ModifierTarget.RegenerationRate:
                    sb.Append(".regenerationRate");
                    break;
                case ModifierTarget.Special:
                    sb.Append(".special");
                    break;
                case ModifierTarget.Value:
                    sb.Append(".value");
                    break;
            }

            switch (application)
            {
                case ModifierApplication.SetValueOnce:
                    sb.Append(" = ");
                    break;
                case ModifierApplication.SetValueUntilRemoved:
                    sb.Append(" = ");
                    break;
                default:
                    switch ((ModifierChangeType)SimpleValue<int>(target, "changeType"))
                    {
                        case ModifierChangeType.Add:
                            sb.Append(" += ");
                            break;
                        case ModifierChangeType.AddMultiplier:
                            sb.Append(" += ");
                            sb.Append(SimpleValue<string>(target, "affectsStatId"));
                            sb.Append(" * ");
                            break;
                        case ModifierChangeType.Subtract:
                            sb.Append(" -= ");
                            break;
                        case ModifierChangeType.SubtractMultiplier:
                            sb.Append(" -= ");
                            sb.Append(SimpleValue<string>(target, "affectsStatId"));
                            sb.Append(" * ");
                            break;
                    }
                    break;
            }

            sb.Append(GetStatValueDescription(target.FindPropertyRelative("value")));

            switch (application)
            {
                case ModifierApplication.SetValueOnce:
                case ModifierApplication.Immediately:
                    sb.Append(" at once");
                    break;
                case ModifierApplication.RecurringOverSeconds:
                    sb.Append(" over ");
                    sb.Append(SimpleValue<float>(target, "lifespan"));
                    sb.Append(" second(s)");
                    break;
                case ModifierApplication.RecurringOverTurns:
                    sb.Append(" over ");
                    sb.Append(SimpleValue<float>(target, "lifespan"));
                    sb.Append(" turn(s)");
                    break;
                case ModifierApplication.SetValueUntilRemoved:
                case ModifierApplication.UntilRemoved:
                    sb.Append(" until removed");
                    break;
            }

            return sb.ToString();
        }

        private string GetStatValueDescription(SerializedProperty target)
        {
            return (ValueType)SimpleValue<int>(target, "m_valueType") switch
            {
                ValueType.Conditional => "(conditionalValue)",
                ValueType.RandomRange => "random(" + SimpleValue<string>(target, "m_randomMin") + ", " + SimpleValue<string>(target, "m_randomMax"),
                _ => SimpleValue<string>(target, "m_value"),
            };
        }

        private void PasteBasicInfo(SerializedProperty target, BasicInfo source)
        {
            if (source == null) return;

            target.FindPropertyRelative("abbr").stringValue = source.abbr;
            target.FindPropertyRelative("autoGenId").boolValue = source.autoGenId;
            target.FindPropertyRelative("color").colorValue = source.color;
            target.FindPropertyRelative("description").stringValue = source.description;
            target.FindPropertyRelative("hidden").boolValue = source.hidden;
            target.FindPropertyRelative("id").stringValue = source.id;
            target.FindPropertyRelative("title").stringValue = source.title;

            PasteImageInfo(target.FindPropertyRelative("image"), source.image);
        }

        private void PasteConditionalBool(SerializedProperty target, GDTKConditionalBool source)
        {
            target.FindPropertyRelative("expression").stringValue = source.expression;
            target.FindPropertyRelative("useExpression").boolValue = source.useExpression;
            target.FindPropertyRelative("value").boolValue = source.value;
        }

        private void PasteImageInfo(SerializedProperty target, ImageInfo source)
        {
            if (source == null) return;

            target.FindPropertyRelative("bundleName").stringValue = source.bundleName;
            target.FindPropertyRelative("assetName").stringValue = source.assetName;
            target.FindPropertyRelative("path").stringValue = source.path;
            target.FindPropertyRelative("source").intValue = (int)source.source;
        }

        private void PasteIncrementSetting(SerializedProperty target, GDTKIncrementSettings source)
        {
            PasteConditionalBool(target.FindPropertyRelative("enabled"), source.enabled);
            target.FindPropertyRelative("incrementWhen").stringValue = source.incrementWhen;
            source.incrementAmount.ToProp(target.FindPropertyRelative("incrementAmount"));
        }

        private void PasteRegenSettings(SerializedProperty target, GDTKRegenerationSettings source)
        {
            PasteConditionalBool(target.FindPropertyRelative("enabled"), source.enabled);
            source.delay.ToProp(target.FindPropertyRelative("delay"));
            source.rate.ToProp(target.FindPropertyRelative("rate"));
            target.FindPropertyRelative("wholeIncrements").boolValue = source.wholeIncrements;
        }

        private void PasteStatExpressions(SerializedProperty target, GDTKStatExpressionSet source)
        {
            source.minimum.ToProp(target.FindPropertyRelative("minimum"));
            source.maximum.ToProp(target.FindPropertyRelative("maximum"));
            source.value.ToProp(target.FindPropertyRelative("value"));
            source.special.ToProp(target.FindPropertyRelative("special"));
        }

        public void PasteStatFromClipboard(SerializedProperty list)
        {
            try
            {
                GDTKStat source = SimpleJson.FromJSON<GDTKStat>(GUIUtility.systemCopyBuffer);

                list.serializedObject.Update();
                list.arraySize++;
                SerializedProperty stat = list.GetArrayElementAtIndex(list.arraySize - 1);
                PasteBasicInfo(stat.FindPropertyRelative("info"), source.info);
                PasteStatExpressions(stat.FindPropertyRelative("expressions"), source.expressions);
                PasteRegenSettings(stat.FindPropertyRelative("regeneration"), source.regeneration);
                PasteIncrementSetting(stat.FindPropertyRelative("incrementation"), source.incrementation);
                stat.FindPropertyRelative("startMaxed").boolValue = source.startMaxed;
                list.serializedObject.ApplyModifiedProperties();

            }
            catch { }

        }

        private bool ShowConditionalBool(SerializedProperty value)
        {
            if (SimpleValue<bool>(value, "useExpression")) return true;
            return SimpleValue<bool>(value, "value");
        }

        #endregion

        #region Internal Methods

        private void BuildStatIds(List<string> output)
        {
            SerializedProperty list = serializedObject.FindProperty("m_stats");
            for (int i = 0; i < list.arraySize; i++)
            {
                output.Add(list.GetArrayElementAtIndex(i).FindPropertyRelative("info").FindPropertyRelative("id").stringValue);
            }
            output.Sort();
        }

        internal void CopyBasicInfoData(SerializedProperty source, SerializedProperty dest)
        {
            dest.FindPropertyRelative("id").stringValue = source.FindPropertyRelative("id").stringValue;
            dest.FindPropertyRelative("autoGenId").boolValue = source.FindPropertyRelative("autoGenId").boolValue;
            dest.FindPropertyRelative("title").stringValue = source.FindPropertyRelative("title").stringValue;
            dest.FindPropertyRelative("abbr").stringValue = source.FindPropertyRelative("abbr").stringValue;
            dest.FindPropertyRelative("description").stringValue = source.FindPropertyRelative("description").stringValue;
            dest.FindPropertyRelative("groupName").stringValue = source.FindPropertyRelative("groupName").stringValue;
            CopyImageInfo(source.FindPropertyRelative("image"), dest.FindPropertyRelative("image"));
            dest.FindPropertyRelative("color").colorValue = source.FindPropertyRelative("color").colorValue;
            dest.FindPropertyRelative("hidden").boolValue = source.FindPropertyRelative("hidden").boolValue;
            CopyStringList(source.FindPropertyRelative("tags"), dest.FindPropertyRelative("tags"));
        }

        internal void CopyConditionalBoolData(SerializedProperty source, SerializedProperty dest)
        {
            dest.FindPropertyRelative("value").boolValue = source.FindPropertyRelative("value").boolValue;
            dest.FindPropertyRelative("useExpression").boolValue = source.FindPropertyRelative("useExpression").boolValue;
            dest.FindPropertyRelative("expression").stringValue = source.FindPropertyRelative("expression").stringValue;
        }

        internal void CopyConditionalValueList(SerializedProperty source, SerializedProperty dest)
        {
            SerializedProperty si, di;

            dest.ClearArray();
            dest.arraySize = source.arraySize;
            for(int i=0; i < source.arraySize; i++)
            {
                si = source.GetArrayElementAtIndex(i);
                di = dest.GetArrayElementAtIndex(i);

                di.FindPropertyRelative("condition").stringValue = si.FindPropertyRelative("condition").stringValue;
                di.FindPropertyRelative("value").stringValue = si.FindPropertyRelative("value").stringValue;
            }
        }

        internal void CopyImageInfo(SerializedProperty source, SerializedProperty dest)
        {
            dest.FindPropertyRelative("sprite").objectReferenceValue = source.FindPropertyRelative("sprite").objectReferenceValue;
            dest.FindPropertyRelative("source").intValue = source.FindPropertyRelative("source").intValue;
            dest.FindPropertyRelative("path").stringValue = source.FindPropertyRelative("path").stringValue;
            dest.FindPropertyRelative("bundleName").stringValue = source.FindPropertyRelative("bundleName").stringValue;
            dest.FindPropertyRelative("assetName").stringValue = source.FindPropertyRelative("assetName").stringValue;
            dest.FindPropertyRelative("z_imageError").boolValue = source.FindPropertyRelative("z_imageError").boolValue;
        }

        internal void CopyStatData(SerializedProperty source, SerializedProperty dest)
        {
            CopyBasicInfoData(source.FindPropertyRelative("info"), dest.FindPropertyRelative("info"));
            CopyStatExpressionSetData(source.FindPropertyRelative("expressions"), dest.FindPropertyRelative("expressions"));
            CopyStatRegenerationData(source.FindPropertyRelative("regeneration"), dest.FindPropertyRelative("regeneration"));
            CopyStatIncrementData(source.FindPropertyRelative("incrementation"), dest.FindPropertyRelative("incrementation"));
            dest.FindPropertyRelative("startMaxed").boolValue = source.FindPropertyRelative("startMaxed").boolValue;

        }

        internal void CopyStatExpressionSetData(SerializedProperty source, SerializedProperty dest)
        {
            CopyStatValueData(source.FindPropertyRelative("minimum"), dest.FindPropertyRelative("minimum"));
            CopyStatValueData(source.FindPropertyRelative("maximum"), dest.FindPropertyRelative("maximum"));
            CopyStatValueData(source.FindPropertyRelative("value"), dest.FindPropertyRelative("value"));
            CopyStatValueData(source.FindPropertyRelative("special"), dest.FindPropertyRelative("special"));
            dest.FindPropertyRelative("z_expanded").boolValue = source.FindPropertyRelative("z_expanded").boolValue;
        }

        internal void CopyStatIncrementData(SerializedProperty source, SerializedProperty dest)
        {
            CopyConditionalBoolData(source.FindPropertyRelative("enabled"), dest.FindPropertyRelative("enabled"));
            dest.FindPropertyRelative("incrementWhen").stringValue = source.FindPropertyRelative("incrementWhen").stringValue;
            CopyStatValueData(source.FindPropertyRelative("incrementAmount"), dest.FindPropertyRelative("incrementAmount"));
            dest.FindPropertyRelative("z_expanded").boolValue = source.FindPropertyRelative("z_expanded").boolValue;
        }

        internal void CopyStatRegenerationData(SerializedProperty source, SerializedProperty dest)
        {
            CopyConditionalBoolData(source.FindPropertyRelative("enabled"), dest.FindPropertyRelative("enabled"));
            CopyStatValueData(source.FindPropertyRelative("delay"), dest.FindPropertyRelative("delay"));
            CopyStatValueData(source.FindPropertyRelative("rate"), dest.FindPropertyRelative("rate"));
            dest.FindPropertyRelative("wholeIncrements").boolValue = source.FindPropertyRelative("wholeIncrements").boolValue;
            dest.FindPropertyRelative("z_expanded").boolValue = source.FindPropertyRelative("z_expanded").boolValue;
        }

        internal void CopyStatValueData(SerializedProperty source, SerializedProperty dest)
        {
            dest.FindPropertyRelative("m_valueType").intValue = source.FindPropertyRelative("m_valueType").intValue;
            dest.FindPropertyRelative("m_value").stringValue = source.FindPropertyRelative("m_value").stringValue;
            dest.FindPropertyRelative("m_randomMin").stringValue = source.FindPropertyRelative("m_randomMin").stringValue;
            dest.FindPropertyRelative("m_randomMax").stringValue = source.FindPropertyRelative("m_randomMax").stringValue;
            CopyConditionalValueList(source.FindPropertyRelative("m_conditions"), dest.FindPropertyRelative("m_conditions"));
        }

        internal void CopyStringList(SerializedProperty source, SerializedProperty dest)
        {
            dest.ClearArray();
            dest.arraySize = source.arraySize;
            for(int i =0; i < source.arraySize; i++)
            {
                dest.GetArrayElementAtIndex(i).stringValue = source.GetArrayElementAtIndex(i).stringValue;
            }
        }    

        internal void DesignTimeAttributes(int displayFlags, int flag, StatsDatabase db, List<string> attributeList)
        {
            SimpleSection(flag, "Attributes", GetIcon("icons/tag"), () =>
            {
                if (db != null)
                {
                    if (attributeList.Count == 0)
                    {
                        GUILayout.Label("There are no attributes in the database.");
                    }
                    else
                    {
                        SimpleStringSearchListProperty(serializedObject.FindProperty("m_attributeIds"), "Entries", attributeList);
                    }
                }
                else
                {
                    StringList("m_attributeIds");
                }
            });
        }

        internal void DesignTimeData(int displayFlags, int flag, ref bool import, ref bool export)
        {
            if (SectionToggle(displayFlags, flag, "Data Migration", GetIcon("icons/db")))
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button(new GUIContent("Import", GetIcon("icons/import-small")), GUILayout.Height(24)))
                {
                    import = true;
                }
                if (GUILayout.Button(new GUIContent("Export", GetIcon("icons/export-small")), GUILayout.Height(24)))
                {
                    export = true;
                }
                GUILayout.EndHorizontal();
            }
        }

        internal void DesignTimeLevel(int displayFlags, int flag)
        {
            SimpleSection(flag, "Leveling", GetIcon("icons/level-up"), () =>
            {
                SimpleStringSearchProperty(serializedObject.FindProperty("m_levelId"), "Level Stat", BuildStatIds);
                SimpleProperty("m_preventLevelGain");
            });
        }

        internal void DesignTimePlugins(int displayFlags, int flag, BasicStats myTarget, UniversalObjectEditorInfo _plugins, ref Rect buttonRect)
        {
            if (SectionToggle(displayFlags, flag, "Plugins", GetIcon("icons/plugin")))
            {
                Color resColor = GUI.contentColor;
                GUIStyle style;
                UniversalObjectEditorItemInfo itemInfo;
                int toggleIndex = -1;
                int removeAt = -1;

                float maxWidth = EditorGUIUtility.currentViewWidth - 24 - 40;

                for (int i = 0; i < myTarget.plugins.Count; i++)
                {
                    if (myTarget.plugins[i] == null || myTarget.plugins[i].plugin == null)
                    {
                        removeAt = i;
                        continue;
                    }
                    UniversalPluginWrapper<StatsPlugin> wrapper = myTarget.plugins[i];
                    itemInfo = _plugins.GetInfo(wrapper.instanceId);
                    if (itemInfo.editor == null)
                    {
                        itemInfo.editor = UniversalPluginEditor.CreateEditor(wrapper.plugin);
                        wrapper.serializationData = SimpleJson.ToJSON(wrapper.plugin);
                    }

                    // Check drag
                    if (_plugins.isDragging && _plugins.curIndex == i)
                    {
                        GUILayout.Space(2);
                        GUILayout.Label(string.Empty, Styles.Redline, GUILayout.Height(1), GUILayout.ExpandWidth(true));
                        GUILayout.Space(2);
                    }

                    // Draw plugin bar
                    GUILayout.BeginHorizontal();
                    EditorGUI.BeginDisabledGroup(itemInfo.isDragging && itemInfo.isDragging);

                    // Item drag handle
                    GUI.contentColor = Styles.EditorColor;
                    GUILayout.Label(GetIcon("Burger", "icons/burger"), Styles.ButtonLeft, GUILayout.Width(22), GUILayout.Height(22));
                    GUI.contentColor = resColor;
                    if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDrag)
                    {
                        _plugins.BeginDrag(i, this, ref itemInfo);
                        GUIUtility.hotControl = GUIUtility.GetControlID(FocusType.Passive);
                    }

                    // Title
                    style = new GUIStyle(itemInfo.isExpanded ? Styles.ButtonMidPressed : Styles.ButtonMid);
                    GUILayout.BeginHorizontal(style, GUILayout.Height(22), GUILayout.MaxWidth(maxWidth));
                    GUILayout.Space(2);
                    GUILayout.Label(wrapper.plugin.icon, GUILayout.Height(16), GUILayout.Width(16));
                    GUILayout.Space(2);
                    GUILayout.Label(wrapper.plugin.titlebarText, Styles.TitleTextStyle, GUILayout.Height(15));
                    GUILayout.EndHorizontal();
                    if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDown)
                    {
                        toggleIndex = i;
                    }

                    // Delete
                    GUI.contentColor = Styles.EditorColor;
                    GUILayout.Label(GetIcon("Trash", "icons/trash"), Styles.ButtonRight, GUILayout.Width(22), GUILayout.Height(22));
                    GUI.contentColor = resColor;
                    if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDown)
                    {
                        removeAt = i;
                    }

                    EditorGUI.EndDisabledGroup();
                    GUILayout.EndHorizontal();

                    if (itemInfo.isExpanded)
                    {
                        GUILayout.Space(-5);
                        GUILayout.BeginVertical("box");
                        itemInfo.editor.OnInspectorGUI();
                        GUILayout.EndVertical();
                    }

                    if (itemInfo.editor.IsDirty)
                    {
                        itemInfo.editor.ApplyUpdates(ref wrapper.serializationData);
                        EditorUtility.SetDirty(target);
                    }
                }

                if (toggleIndex > -1)
                {
                    _plugins.items[myTarget.plugins[toggleIndex].instanceId].isExpanded = !_plugins.items[myTarget.plugins[toggleIndex].instanceId].isExpanded;
                    if (_plugins.items[myTarget.plugins[toggleIndex].instanceId].isExpanded)
                    {
                        _plugins.items[myTarget.plugins[toggleIndex].instanceId].editor.OnEnable();
                    }
                }

                if (removeAt > -1)
                {
                    if (_plugins.items != null)
                    {
                        _plugins.items.Remove(myTarget.plugins[removeAt].instanceId);
                    }
                    myTarget.plugins.RemoveAt(removeAt);
                    EditorUtility.SetDirty(target);
                }

                if (GUILayout.Button("Add Plugin"))
                {
                    PopupWindow.Show(buttonRect, new StatPluginPicker() { target = myTarget.plugins, width = buttonRect.width });
                }
                if (Event.current.type == EventType.Repaint) buttonRect = GUILayoutUtility.GetLastRect();
            }
        }

        internal void DesignTimeRaceAndClasses(int displayFlags, int flag, StatsDatabase db, List<string> raceList, List<string> backgroundList, List<string> classList)
        {
            SimpleSection(flag, "Race & Classes", GetIcon("icons/character-class"), () =>
            {
                if (db != null)
                {
                    if (raceList.Count == 0)
                    {
                        GUILayout.Label("There are no races in the database.");
                    }
                    else
                    {
                        SimpleStringSearchProperty(serializedObject.FindProperty("m_raceId"), "Race", raceList);
                    }

                    if (backgroundList.Count == 0)
                    {
                        GUILayout.Label("There are no backgrounds in the database.");
                    }
                    else
                    {
                        SimpleStringSearchProperty(serializedObject.FindProperty("m_backgroundId"), "Background", backgroundList);
                    }

                    if (classList.Count == 0)
                    {
                        GUILayout.Label("There are no clases in the database.");
                    }
                    else
                    {
                        SimpleStringSearchProperty(serializedObject.FindProperty("m_classId"), "Class", classList);
                    }
                }
                else
                {
                    SimpleProperty("m_raceId", "Race Id");
                    SimpleProperty("m_classId", "Class Id");
                }
            });
        }

        internal void DesignTimeRespawn(int displayFlags, int flag)
        {
            if (SectionToggle(displayFlags, flag, "Respawning", GetIcon("icons/skull")))
            {
                SimpleProperty("respawnCondition");
                SimpleProperty("saveFilename");
                SimpleProperty("savePosition");
                SimpleProperty("saveRotation");

                DrawStatModifierList(serializedObject.FindProperty("respawnModifiers"));
            }
        }

        internal void DesignTimeStats(int displayFlags, int flag)
        {
            if (SectionToggle(displayFlags, flag, "Stats", GetIcon("icons/stats")))
            {
                // Copy old stats
                SerializedProperty oldStats = serializedObject.FindProperty("m_stats");
                SerializedProperty newStats = serializedObject.FindProperty("statSource").FindPropertyRelative("m_stats");

                if (oldStats.arraySize > 0)
                {
                    newStats.ClearArray();
                    newStats.arraySize = oldStats.arraySize;
                    for (int i = 0; i < oldStats.arraySize; i++)
                    {
                        CopyStatData(oldStats.GetArrayElementAtIndex(i), newStats.GetArrayElementAtIndex(i));
                    }
                    oldStats.ClearArray();
                }

                DrawStatListNoIndent(newStats);
            }
        }

        internal void DesignTimeStatusConditions(int displayFlags, int flag, StatsDatabase db, List<string> conditionList)
        {
            if (SectionToggle(displayFlags, flag, "Status Conditions", GetIcon("icons/animate")))
            {
                if (db != null)
                {
                    if (conditionList.Count == 0)
                    {
                        GUILayout.Label("There are no status conditions in the database.");
                    }
                    else
                    {
                        SimpleStringSearchListProperty(serializedObject.FindProperty("m_conditionIds"), "Entries", conditionList);
                    }
                }
                else
                {
                    StringList("m_conditionIds");
                }
            }
        }

        internal void DesignTimeToolRegistry(int displayFlags, int flag)
        {
            GUILayout.Space(8);
            if (SectionToggle(displayFlags, flag, "Tool Registry", GetIcon("icons/navigation")))
            {
                SimpleProperty("register");
                if (SimpleValue<bool>("register"))
                {
                    SimpleProperty("registryKey");
                }
            }
        }

        internal void DesignTimeUI(int displayFlags, int flag)
        {
            if (SectionToggle(displayFlags, flag, "UI Options", GetIcon("icons/ui")))
            {
                SimpleProperty("selectOptionUI");
            }
        }

        internal void RunTimeActiveEffects(int displayFlags, int flag, StatsAndEffects target)
        {
            if (SectionToggle(displayFlags, flag, "Active Effects", GetIcon("icons/8bit-potion")))
            {
                foreach (GDTKStatusEffect effect in target.activeEffects)
                {
                    GUILayout.BeginVertical("box");
                    GUILayout.Label(effect.info.id + (effect.expires != EffectExpiry.Automatically ? " (remaining " + effect.lifeRemaining + ")" : ""));
                    GUILayout.EndVertical();
                    foreach (GDTKStatModifier mod in effect.livingModifiers)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Space(16);
                        GUILayout.BeginVertical("box");
                        GUILayout.Label(mod.GetDescription() + ((mod.applies == ModifierApplication.RecurringOverSeconds || mod.applies == ModifierApplication.RecurringOverTurns) ? " (remaining " + mod.lifeRemaining + ")" : ""));
                        GUILayout.EndVertical();
                        GUILayout.EndHorizontal();
                    }
                }
            }
        }

        internal void RunTimeAttributes(int displayFlags, int flag, BasicStats target)
        {
            if (SectionToggle(displayFlags, flag, "Attributes", GetIcon("icons/tag")))
            {
                foreach (GDTKAttribute attrib in target.attributes)
                {
                    GUILayout.BeginVertical("box");
                    GUILayout.Label(attrib.info.hidden ? "(hidden) " + attrib.info.id : attrib.info.id);
                    GUILayout.EndVertical();
                }
            }
        }

        internal void RunTimeLevel(int displayFlags, int flag, PlayerCharacterStats target)
        {
            if (SectionToggle(displayFlags, flag, "Leveling", GetIcon("icons/level-up")))
            {
                GUI.enabled = false;
                SimpleProperty("m_levelId");
                GUI.enabled = true;
                target.preventLevelGain = EditorGUILayout.Toggle("Prevent Level Gain", target.preventLevelGain);
            }
        }

        internal void RunTimeRaceAndClasses(int displayFlags, int flag, PlayerCharacterStats target)
        {
            if (SectionToggle(displayFlags, flag, "Race & Classes", GetIcon("icons/character-class")))
            {
                GUI.enabled = false;
                if (target.race == null)
                {
                    EditorGUILayout.TextField("Race", "{none}");
                }
                else
                {
                    EditorGUILayout.TextField("Race", target.race.info.id);
                }

                if (target.background == null)
                {
                    EditorGUILayout.TextField("Background", "{none}");
                }
                else
                {
                    EditorGUILayout.TextField("Background", target.background.info.id);
                }
                GUI.enabled = true;

                GUILayout.Label("Classes");
                if (target.classes != null)
                {
                    foreach (GDTKClass pc in target.classes)
                    {
                        GUILayout.BeginVertical("box");
                        GUILayout.Label(pc.info.id + " (Level " + pc.level + ")");
                        GUILayout.EndVertical();
                    }
                }
            }
        }

        internal void RunTimeStats(int displayFlags, int flag, BasicStats target, ref bool[] expanded)
        {
            if (SectionToggle(displayFlags, flag, "Stats", GetIcon("icons/stats")))
            {
                DebugStatList(target, ref expanded);
            }
        }

        internal void RunTimeStatusConditions(int displayFlags, int flag, BasicStats target)
        {
            if (SectionToggle(displayFlags, flag, "Status Conditions", GetIcon("icons/animate")))
            {
                foreach (GDTKStatusCondition condition in target.statusConditions)
                {
                    GUILayout.BeginVertical("box");
                    GUILayout.Label((condition.active ? "(Active) " : "(Inactive) ") + condition.info.id);
                    GUILayout.EndVertical();
                }
            }
        }

        internal void RunTimeToolRegistry(int displayFlags, int flag)
        {
            GUILayout.Space(8);
            if (SectionToggle(displayFlags, flag, "Tool Registry", GetIcon("icons/navigation")))
            {
                GUI.enabled = false;
                SimpleProperty("register");
                if (SimpleValue<bool>("register"))
                {
                    SimpleProperty("registryKey");
                }
                GUI.enabled = true;
            }
        }

        internal void RunTimeUI(int displayFlags, int flag)
        {
            GUILayout.Space(8);
            if (SectionToggle(displayFlags, flag, "UI Options", GetIcon("icons/ui")))
            {
                SimpleProperty("selectOptionUI");
            }
        }

        #endregion

    }
}
#endif