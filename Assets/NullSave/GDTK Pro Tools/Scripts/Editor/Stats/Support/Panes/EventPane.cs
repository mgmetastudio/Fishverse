#if GDTK
using UnityEditor;
using UnityEngine;

namespace NullSave.GDTK.Stats
{
    public class EventPane : TemplatePane
    {

        #region Fields

        private EditorInfoList _smList;

        #endregion

        #region Properties

        public override string Name { get { return "Events"; } }

        public override string Description { get { return "Custom strings used to raise and respond to game events"; } }

        #endregion

        #region Constructors

        public EventPane(SerializedObject obj, string fieldName) : base(obj, fieldName, null)
        {
            _smList = new EditorInfoList();
        }

        #endregion

        #region Public Methods

        public override void AddEntry(GDTKStatsEditor editor, StatsDatabase database, ref int entryIndex, ref SerializedProperty editing, ref bool skipFrame)
        {
            base.AddEntry(editor, database, ref entryIndex, ref editing, ref skipFrame);
            editor.ClearLastEventItem(list);
        }

        public override void DuplicateEntry(StatsDatabase database, ref int entryIndex, ref SerializedProperty editing)
        {
            database.events.Insert(entryIndex, database.events[entryIndex].Clone());
            base.DuplicateEntry(database, ref entryIndex, ref editing);
        }

        public override void EditEntry(GDTKStatsEditor editor, StatsDatabase database, ref int entryIndex, SerializedProperty editing)
        {
            EditBasicInfo(editor, editing, editing.FindPropertyRelative("info"), entryIndex);

            editor.SectionHeader("Behavior", GDTKEditor.GetIcon("icons/behavior"));
            editor.SimpleProperty(editing, "raiseTokenHeartbeat");
            if (GDTKEditor.SimpleValue<bool>(editing, "raiseTokenHeartbeat"))
            {
                editor.SimpleProperty(editing, "tokens");
            }

            GUILayout.Space(8);
            editor.DrawStatModifierList(editing.FindPropertyRelative("statModifiers"), _smList);
        }

        #endregion

    }
}
#endif