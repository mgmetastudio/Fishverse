#if GDTK
using UnityEditor;

namespace NullSave.GDTK.Stats
{
    public class AttributePane : TemplatePane
    {

        #region Properties

        public override string Name { get { return "Attributes"; } }

        public override string Description { get { return "Simple values that a creature either has or doesn't"; } }

        #endregion

        #region Constructors

        public AttributePane(SerializedObject obj, string fieldName) : base(obj, fieldName, null) { }

        #endregion

        #region Public Methods

        public override void DuplicateEntry(StatsDatabase database, ref int entryIndex, ref SerializedProperty editing)
        {
            database.attributes.Insert(entryIndex, database.attributes[entryIndex].Clone());
            base.DuplicateEntry(database, ref entryIndex, ref editing);
        }

        public override void EditEntry(GDTKStatsEditor editor, StatsDatabase database, ref int entryIndex, SerializedProperty editing)
        {
            InfoSection(editor, ref entryIndex, editing);
        }

        #endregion

    }
}
#endif