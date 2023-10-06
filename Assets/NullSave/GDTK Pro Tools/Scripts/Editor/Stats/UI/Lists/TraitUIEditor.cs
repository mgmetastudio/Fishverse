#if GDTK
using UnityEditor;
using UnityEngine;

namespace NullSave.GDTK.Stats
{
    [CustomEditor(typeof(TraitUI))]
    public class TraitUIEditor : GDTKEditor
    {

        #region Unity Methods

        public override void OnInspectorGUI()
        {
            MainContainerBegin();

            SectionHeader("Behavior");
            SimpleProperty("image");
            SimpleProperty("colorize");

            DrawFormattedLabelList(serializedObject.FindProperty("labels"));

            GUILayout.Space(12);
            SimpleProperty("showHelp");
            if (SimpleValue<bool>("showHelp"))
            {
                EditorGUILayout.HelpBox("Available formatting options:"
                    + "\r\n{id} - Id of the Trait"
                    + "\r\n{title} - Title of the Trait"
                    + "\r\n{abbr} - Abbreviation of the Trait"
                    + "\r\n{description} - Description of the Trait"
                    + "\r\n{groupName} - Group name of the Trait"
                    , MessageType.Info);
            }

            SectionHeader("Events");
            SimpleProperty("onClick");

            MainContainerEnd();
        }

        #endregion

    }
}
#endif