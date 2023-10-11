#if GDTK
using UnityEditor;
using UnityEngine;

namespace NullSave.GDTK.Stats
{
    [CustomEditor(typeof(PerkUI))]
    public class PerkUIEditor : GDTKEditor
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
                    + "\r\n{id} - Id of the Perk"
                    + "\r\n{title} - Title of the Perk"
                    + "\r\n{abbr} - Abbreviation of the Perk"
                    + "\r\n{description} - Description of the Perk"
                    + "\r\n{groupName} - Group name of the Perk"
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