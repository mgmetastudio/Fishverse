#if GDTK
using UnityEditor;
using UnityEngine;

namespace NullSave.GDTK.Stats
{
    [CustomEditor(typeof(ClassUI))]
    public class ClassUIEditor : GDTKEditor
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
                    + "\r\n{id} - Id of the Player Class"
                    + "\r\n{title} - Title of the Player Class"
                    + "\r\n{abbr} - Abbreviation of the Player Class"
                    + "\r\n{description} - Description of the Player Class"
                    + "\r\n{groupName} - Group name of the Player Class"
                    + "\r\n{level} - Current level of the Player Class"
                    , MessageType.Info);
            }

            MainContainerEnd();
        }

        #endregion

    }
}
#endif