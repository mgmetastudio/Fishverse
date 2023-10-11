#if GDTK
using UnityEditor;
using UnityEngine;

namespace NullSave.GDTK.Stats
{
    [CustomEditor(typeof(BackgroundUI))]
    public class BackgroundUIEditor : GDTKEditor
    {

        #region Unity Methods

        public override void OnInspectorGUI()
        {
            MainContainerBegin();

            SectionHeader("Stat Source");
            SimpleProperty("source");
            switch ((StatSourceReference)SimpleValue<int>("source"))
            {
                case StatSourceReference.DirectReference:
                    SimpleProperty("m_stats");
                    break;
                case StatSourceReference.FindInRegistry:
                    SimpleProperty("key");
                    break;
            }

            SectionHeader("Behavior");
            SimpleProperty("image");
            SimpleProperty("colorize");
            SimpleProperty("showTraits");
            if(SimpleValue<bool>("showTraits"))
            {
                SimpleProperty("traitList");
            }

            DrawFormattedLabelList(serializedObject.FindProperty("labels"));

            GUILayout.Space(12);
            SimpleProperty("showHelp");
            if (SimpleValue<bool>("showHelp"))
            {
                EditorGUILayout.HelpBox("Available formatting options:"
                    + "\r\n{id} - Id of the Background"
                    + "\r\n{title} - Title of the Background"
                    + "\r\n{abbr} - Abbreviation of the Background"
                    + "\r\n{description} - Description of the Background"
                    + "\r\n{groupName} - Group name of the Background"
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