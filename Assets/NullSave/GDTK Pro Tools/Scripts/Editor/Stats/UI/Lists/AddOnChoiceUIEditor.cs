#if GDTK
using UnityEditor;
using UnityEngine;

namespace NullSave.GDTK.Stats
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(AddOnChoiceUI))]
    public class AddOnChoiceUIEditor : GDTKEditor
    {

        #region Unity Methods

        public override void OnInspectorGUI()
        {
            MainContainerBegin();

            SectionHeader("Behavior");
            SimpleProperty("image");
            SimpleProperty("colorize");
            SimpleProperty("clickToSelect");

            DrawFormattedLabelList(serializedObject.FindProperty("labels"));

            GUILayout.Space(12);
            SimpleProperty("showHelp");
            if (SimpleValue<bool>("showHelp"))
            {
                EditorGUILayout.HelpBox("Available formatting options:"
                    + "\r\n{id} - Id of the Choice"
                    + "\r\n{title} - Title of the Choice"
                    + "\r\n{abbr} - Abbreviation of the Choice"
                    + "\r\n{description} - Description of the Choice"
                    + "\r\n{groupName} - Group name of the Choice"
                    + "\r\n{level} - Current level of the PChoice"
                    , MessageType.Info);
            }

            SectionHeader("Events");
            SimpleProperty("onClick");
            SimpleProperty("onSelected");
            SimpleProperty("onDeselected");

            MainContainerEnd();
        }

        #endregion

    }
}
#endif