#if GDTK
using UnityEditor;
using UnityEngine;

namespace NullSave.GDTK.Stats
{
    [CustomEditor(typeof(AddOnChoiceListUI))]
    public class AddOnChoiceListUIEditor : GDTKEditor
    {

        #region Unity Methods

        public override void OnInspectorGUI()
        {
            MainContainerBegin();


            SectionHeader("Behavior");
            SimpleProperty("uiPrefab", "UI Prefab");
            SimpleProperty("content");
            SimpleProperty("autoClose");
            SimpleProperty("setTimeScale");
            if(SimpleValue<bool>("setTimeScale"))
            {
                SimpleProperty("timeScale");
            }

            DrawFormattedLabelList(serializedObject.FindProperty("labels"));

            GUILayout.Space(12);
            SimpleProperty("showHelp");
            if (SimpleValue<bool>("showHelp"))
            {
                EditorGUILayout.HelpBox("Available formatting options:"
                    + "\r\n{id} - Id of the List's source"
                    + "\r\n{title} - Title of the List's source"
                    + "\r\n{abbr} - Abbreviation of the List's source"
                    + "\r\n{description} - Description of the List's source"
                    + "\r\n{groupName} - Group name of the List's source"
                    , MessageType.Info);
            }

            SectionHeader("Events");
            SimpleProperty("onCanSubmit");
            SimpleProperty("onCannotSubmit");

            MainContainerEnd();
        }

        #endregion

    }
}
#endif