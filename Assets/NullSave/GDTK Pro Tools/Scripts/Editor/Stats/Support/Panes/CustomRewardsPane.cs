#if GDTK
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NullSave.GDTK.Stats
{
    public class CustomRewardsPane : TemplatePane
    {

        #region Fields

        private UniversalObjectEditorInfo _addOnPlugins;
        private EditorInfoList _smList;

        #endregion

        #region Properties

        public override string Name { get { return "Custom Rewards"; } }

        public override string Description { get { return "Rewards you can give to your player at any time"; } }

        #endregion

        #region Constructors

        public CustomRewardsPane(SerializedObject obj, string fieldName, object inventoryDB) : base(obj, fieldName, inventoryDB)
        {
            _addOnPlugins = new UniversalObjectEditorInfo();
            _smList = new EditorInfoList();
        }

        #endregion

        #region Public Methods

        public override void AddEntry(GDTKStatsEditor editor, StatsDatabase database, ref int entryIndex, ref SerializedProperty editing, ref bool skipFrame)
        {
            base.AddEntry(editor, database, ref entryIndex, ref editing, ref skipFrame);
            editor.ClearLastLevelRewardItem(list);
        }

        public override void DuplicateEntry(StatsDatabase database, ref int entryIndex, ref SerializedProperty editing)
        {
            database.customRewards.Insert(entryIndex, database.customRewards[entryIndex].Clone());
            base.DuplicateEntry(database, ref entryIndex, ref editing);
        }

        public override void EditEntry(GDTKStatsEditor editor, StatsDatabase database, ref int entryIndex, SerializedProperty editing)
        {
            List<UniversalPluginWrapper<AddOnPlugin>> addonList = database.customRewards[entryIndex].addOnPlugins;

            EditBasicInfo(editor, editing, editing.FindPropertyRelative("info"), entryIndex);

            editor.SectionHeader("Behavior", GDTKEditor.GetIcon("icons/behavior"));
            editor.SimpleProperty(editing, "requirements");

            // New AddOn Plugins
            GUILayout.Space(8);
            editor.SectionHeader("Add-On Plugins", GDTKEditor.GetIcon("icons/plugin"));
            GUILayout.BeginHorizontal();
            GUILayout.Space(EditorGUIUtility.labelWidth + 4);
            GUILayout.BeginVertical();
            editor.DrawAddOnPluginList(addonList, _addOnPlugins);
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            // Show Add Button
            Rect r = EditorGUILayout.GetControlRect(true, GUILayout.ExpandWidth(true));
            GUILayout.Space(-EditorGUIUtility.singleLineHeight);
            GUILayout.BeginHorizontal();
            GUILayout.Space(EditorGUIUtility.labelWidth + 4);
            if (GUILayout.Button(new GUIContent("Add Option", GDTKEditor.GetIcon("icons/add-small")), GUILayout.Height(24)))
            {
                if (addonList == null) addonList = new List<UniversalPluginWrapper<AddOnPlugin>>();
                GUI.FocusControl(null);
                PopupWindow.Show(r, new AddOnPluginPicker() { targetList = editing.FindPropertyRelative("addOnPlugins"), target = addonList });
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(8);
            editor.DrawAddOnChoicePluginList(editing.FindPropertyRelative("pluginChoices"), database.customRewards[entryIndex].pluginChoices, database, _addOnPlugins, inventoryDB);

            GUILayout.Space(8);
            editor.DrawStatModifierList(editing.FindPropertyRelative("statModifiers"), _smList);
        }

        #endregion

    }
}
#endif