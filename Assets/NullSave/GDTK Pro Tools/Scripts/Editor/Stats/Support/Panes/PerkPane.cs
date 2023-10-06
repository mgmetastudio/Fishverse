#if GDTK
using UnityEditor;
using UnityEngine;

namespace NullSave.GDTK.Stats
{
    public class PerkPane : TemplatePane
    {

        #region Fields

        private EditorInfoList _smList;

        #endregion

        #region Properties

        public override string Name { get { return "Perks"; } }

        public override string Description { get { return "Special abilities granted over time and effort"; } }

        #endregion

        #region Constructors

        public PerkPane(SerializedObject obj, string fieldName) : base(obj, fieldName, null)
        {
            _smList = new EditorInfoList();
        }

        #endregion

        #region Public Methods

        public override void AddEntry(GDTKStatsEditor editor, StatsDatabase database, ref int entryIndex, ref SerializedProperty editing, ref bool skipFrame)
        {
            base.AddEntry(editor, database, ref entryIndex, ref editing, ref skipFrame);
            editor.ClearLastPerkItem(list);
        }

        public override void DuplicateEntry(StatsDatabase database, ref int entryIndex, ref SerializedProperty editing)
        {
            database.perks.Insert(entryIndex, database.perks[entryIndex].Clone());
            base.DuplicateEntry(database, ref entryIndex, ref editing);
        }

        public override void EditEntry(GDTKStatsEditor editor, StatsDatabase database, ref int entryIndex, SerializedProperty editing)
        {
            EditBasicInfo(editor, editing, editing.FindPropertyRelative("info"), entryIndex);

            editor.SectionHeader("Behavior", GDTKEditor.GetIcon("icons/behavior"));
            editor.DrawUnlocking(editing.FindPropertyRelative("unlocking"), BuildClassIds);

            GUILayout.Space(8);
            editor.SimpleStringSearchHeaderListProperty(editing.FindPropertyRelative("attributeIds"), "Attributes", GDTKEditor.GetIcon("icons/tag"), BuildAttributeIds);
            GUILayout.Space(8);
            editor.SimpleStringSearchHeaderListProperty(editing.FindPropertyRelative("conditionIds"), "Status Conditions", GDTKEditor.GetIcon("icons/animate"), BuildConditionIds);
            GUILayout.Space(8);
            editor.DrawStatModifierList(editing.FindPropertyRelative("statModifiers"), _smList);
        }

        #endregion

    }
}
#endif