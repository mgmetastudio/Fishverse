#if GDTK
using UnityEditor;
using UnityEngine;

namespace NullSave.GDTK.Stats
{
    [CustomEditor(typeof(StatusConditionUI))]
    public class StatusConditionUIEditor : GDTKEditor
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
                EditorGUILayout.HelpBox(
                    "{id} - Id of Condition\r\n" +
                    "{title} - Title of Condition\r\n" +
                    "{abbr} - Abbreviation of Condition\r\n" +
                    "{description} - Description of Condition\r\n" +
                    "{group} - Group Name of Condition"
                    , MessageType.Info);

            }

            SectionHeader("Events");
            SimpleProperty("onActivated");
            SimpleProperty("onDeactivated");

            MainContainerEnd();
        }

        #endregion

    }
}
#endif