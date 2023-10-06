#if GDTK
using UnityEditor;

namespace NullSave.GDTK.Stats
{
    public class StatusEffectPane : TemplatePane
    {

        #region Fields

        private EditorInfoList _smList;

        #endregion

        #region Properties

        public override string Name { get { return "Status Effects"; } }

        public override string Description { get { return "Conditions applied directly from an external source"; } }

        #endregion

        #region Constructors

        public StatusEffectPane(SerializedObject obj, string fieldName) : base(obj, fieldName, null)
        {
            _smList = new EditorInfoList();
        }

        #endregion

        #region Public Methods

        public override void AddEntry(GDTKStatsEditor editor, StatsDatabase database, ref int entryIndex, ref SerializedProperty editing, ref bool skipFrame)
        {
            base.AddEntry(editor, database, ref entryIndex, ref editing, ref skipFrame);
            editor.ClearLastEffectItem(list);
        }

        public override void DuplicateEntry(StatsDatabase database, ref int entryIndex, ref SerializedProperty editing)
        {
            database.effects.Insert(entryIndex, database.effects[entryIndex].Clone());
            base.DuplicateEntry(database, ref entryIndex, ref editing);
        }

        public override void EditEntry(GDTKStatsEditor editor, StatsDatabase database, ref int entryIndex, SerializedProperty editing)
        {
            EditBasicInfo(editor, editing, editing.FindPropertyRelative("info"), entryIndex);

            editor.DrawEffectData(editing, _smList, BuildEffectIds, BuildAttributeIds, false);
        }

        #endregion

    }
}
#endif