#if GDTK
using UnityEditor;

namespace NullSave.GDTK.Stats
{
    public interface IStatEditorPane
    {

        #region Properties

        public string Description { get; }

        public string Name { get; }

        #endregion

        #region Methods

        void AddEntry(GDTKStatsEditor editor, StatsDatabase database, ref int entryIndex, ref SerializedProperty editing, ref bool skipFrame);

        void DeleteEntry(StatsDatabase database, SerializedObject obj, ref int entryIndex, ref SerializedProperty editing);

        void DuplicateEntry(StatsDatabase database, ref int entryIndex, ref SerializedProperty editing);

        void EditEntry(GDTKStatsEditor editor, StatsDatabase database, ref int entryIndex, SerializedProperty editing);

        void ListEntries(StatsDatabase database, ref int entryIndex, string searchValue, ref SerializedProperty editing, bool needsRefresh);

        bool IsValidEntry(int entryIndex, SerializedProperty editing);

        void SortEntries(StatsDatabase database, ref int entryIndex);

        #endregion

    }
}
#endif