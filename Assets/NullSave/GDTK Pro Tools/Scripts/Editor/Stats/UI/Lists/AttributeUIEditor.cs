#if GDTK
using UnityEditor;
using UnityEngine;

namespace NullSave.GDTK.Stats
{
    [CustomEditor(typeof(AttributeUI))]
    public class AttributeUIEditor : GDTKEditor
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
                    + "\r\n{id} - Id of the Attribute"
                    + "\r\n{title} - Title of the Attribute"
                    + "\r\n{abbr} - Abbreviation of the Attribute"
                    + "\r\n{description} - Description of the Attribute"
                    + "\r\n{groupName} - Group name of the Attribute"
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