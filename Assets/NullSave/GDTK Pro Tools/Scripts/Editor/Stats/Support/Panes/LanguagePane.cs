#if GDTK
using UnityEditor;

namespace NullSave.GDTK.Stats
{
    public class LanguagePane : TemplatePane
    {

        #region Properties

        public override string Name { get { return "Languages"; } }

        public override string Description { get { return "Verbal and written communication between creatures"; } }

        #endregion

        #region Constructors

        public LanguagePane(SerializedObject obj, string fieldName) : base(obj, fieldName, null) { }

        #endregion

        #region Public Methods

        public override void AddEntry(GDTKStatsEditor editor, StatsDatabase database, ref int entryIndex, ref SerializedProperty editing, ref bool skipFrame)
        {
            base.AddEntry(editor, database, ref entryIndex, ref editing, ref skipFrame);
            editor.ClearLastLanguageItem(list);
        }

        public override void DuplicateEntry(StatsDatabase database, ref int entryIndex, ref SerializedProperty editing)
        {
            database.languages.Insert(entryIndex, database.languages[entryIndex].Clone());
            base.DuplicateEntry(database, ref entryIndex, ref editing);
        }

        public override void EditEntry(GDTKStatsEditor editor, StatsDatabase database, ref int entryIndex, SerializedProperty editing)
        {
            InfoSection(editor, ref entryIndex, editing);

            editor.SectionHeader("Behavior", GDTKEditor.GetIcon("icons/behavior"));
            editor.SimpleProperty(editing, "script");
            editor.StringList(editing, "typicalSpeakers");
        }

        #endregion

    }
}
#endif