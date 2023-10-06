#if GDTK
using UnityEditor;
using UnityEngine;

namespace NullSave.GDTK.Stats
{
    public class BackgroundPane : TemplatePane
    {

        #region Fields

        private EditorInfoList _traitsList;

        private GDTKStatsEditor editor;
        private StatsDatabase database;
        private int entryIndex;

        #endregion

        #region Properties

        public override string Name { get { return "Backgrounds"; } }

        public override string Description { get { return "The history of a creature can affect its abilities"; } }

        #endregion

        #region Constructors

        public BackgroundPane(SerializedObject obj, string fieldName, object inventoryDB) : base(obj, fieldName, inventoryDB)
        {
            _traitsList = new EditorInfoList();
        }

        #endregion

        #region Public Methods

        public override void AddEntry(GDTKStatsEditor editor, StatsDatabase database, ref int entryIndex, ref SerializedProperty editing, ref bool skipFrame)
        {
            base.AddEntry(editor, database, ref entryIndex, ref editing, ref skipFrame);
            editor.ClearLastBackgroundItem(list);
        }

        public override void DuplicateEntry(StatsDatabase database, ref int entryIndex, ref SerializedProperty editing)
        {
            database.backgrounds.Insert(entryIndex, database.backgrounds[entryIndex].Clone());
            EditorUtility.SetDirty(database);
            base.DuplicateEntry(database, ref entryIndex, ref editing);
        }

        public override void EditEntry(GDTKStatsEditor editor, StatsDatabase database, ref int entryIndex, SerializedProperty editing)
        {
            this.editor = editor;
            this.database = database;
            this.entryIndex = entryIndex;

            EditBasicInfo(editor, editing, editing.FindPropertyRelative("info"), entryIndex);
            DrawTraits(editing);
        }

        #endregion

        #region Private Methods

        private void DrawTraits(SerializedProperty editing)
        {
            SerializedProperty list = editing.FindPropertyRelative("traits");
            editor.DrawListHeader("Traits", list.arraySize, GDTKEditor.GetIcon("icons/animate"));

            GUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(" ");
            GUILayout.BeginVertical();
            editor.SimpleObjectDragList(list, DrawTraitTitle, _traitsList, DrawTrait, true);
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(" ");
            if (GUILayout.Button(new GUIContent("Add Trait", GDTKEditor.GetIcon("icons/add-small")), GUILayout.Height(24)))
            {
                GUI.FocusControl(null);
                database.backgrounds[entryIndex].traits.Add(new GDTKTrait());
                EditorUtility.SetDirty(database);
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        private void DrawTraitTitle(SerializedProperty item, int index)
        {
            EditorGUILayout.LabelField(item.FindPropertyRelative("info").FindPropertyRelative("title").stringValue);
        }

        private void DrawTrait(SerializedProperty item, int index)
        {
            GDTKTrait trait = database.backgrounds[entryIndex].traits[index];
            if (trait.addOnPlugList == null) trait.addOnPlugList = new UniversalObjectEditorInfo();
            if (trait.modifierList == null) trait.modifierList = new EditorInfoList();
            editor.DrawTrait(item, trait, (UniversalObjectEditorInfo)trait.addOnPlugList, (EditorInfoList)trait.modifierList, database, inventoryDB, BuildClassIds);
        }

        #endregion

    }
}
#endif