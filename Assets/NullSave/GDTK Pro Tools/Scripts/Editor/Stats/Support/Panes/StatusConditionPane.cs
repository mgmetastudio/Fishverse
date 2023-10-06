#if GDTK
using UnityEditor;
using UnityEngine;

namespace NullSave.GDTK.Stats
{
    public class StatusConditionPane : TemplatePane
    {

        #region Fields

        private EditorInfoList _smList;

        #endregion

        #region Properties

        public override string Name { get { return "Status Conditions"; } }

        public override string Description { get { return "Responses to natural effects such as thirst or encumberance"; } }

        #endregion

        #region Constructors

        public StatusConditionPane(SerializedObject obj, string fieldName) : base(obj, fieldName, null)
        {
            _smList = new EditorInfoList();
        }

        #endregion

        #region Public Methods

        public override void AddEntry(GDTKStatsEditor editor, StatsDatabase database, ref int entryIndex, ref SerializedProperty editing, ref bool skipFrame)
        {
            base.AddEntry(editor, database, ref entryIndex, ref editing, ref skipFrame);
            editor.ClearLastStatusConditionItem(list);
        }

        public override void DuplicateEntry(StatsDatabase database, ref int entryIndex, ref SerializedProperty editing)
        {
            database.statusConditions.Insert(entryIndex, database.statusConditions[entryIndex].Clone());
            base.DuplicateEntry(database, ref entryIndex, ref editing);
        }

        public override void EditEntry(GDTKStatsEditor editor, StatsDatabase database, ref int entryIndex, SerializedProperty editing)
        {
            EditBasicInfo(editor, editing, editing.FindPropertyRelative("info"), entryIndex);

            editor.SectionHeader("Activation");
            editor.SimpleProperty(editing, "activateWhen");
            switch ((ConditionStartMode)GDTKEditor.SimpleValue<int>(editing, "activateWhen"))
            {
                case ConditionStartMode.ConditionTrue:
                    editor.SimpleProperty(editing, "startCondition");
                    break;
                case ConditionStartMode.EventTriggered:
                    editor.SimpleProperty(editing, "startEvent");
                    break;
            }

            editor.SectionHeader("Deactivation");
            editor.SimpleProperty(editing, "deactivateWhen");
            switch ((ConditionEndMode)GDTKEditor.SimpleValue<int>(editing, "deactivateWhen"))
            {
                case ConditionEndMode.ConditionTrue:
                    editor.SimpleProperty(editing, "endCondition");
                    break;
                case ConditionEndMode.EventTriggered:
                    editor.SimpleProperty(editing, "endEvent");
                    break;
                case ConditionEndMode.TimeElapsed:
                case ConditionEndMode.TurnsElapsed:
                    editor.SimpleProperty(editing, "endTime");
                    break;
            }

            GUILayout.Space(8);
            editor.DrawStatModifierList(editing.FindPropertyRelative("statModifiers"), _smList);

            GUILayout.Space(8);
            editor.SectionHeader("Spawn when Activated", GDTKEditor.GetIcon("icons/object"));
            editor.SimpleProperty(editing, "spawnInfo");
        }

        #endregion

    }
}
#endif