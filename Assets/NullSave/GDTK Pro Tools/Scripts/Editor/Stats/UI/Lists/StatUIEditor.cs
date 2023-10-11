#if GDTK
using UnityEditor;
using UnityEngine;

namespace NullSave.GDTK.Stats
{
    [CustomEditor(typeof(StatUI))]
    public class StatUIEditor : GDTKEditor
    {

        #region Unity Methods

        public override void OnInspectorGUI()
        {
            MainContainerBegin();

            SectionHeader("Stats");
            SimpleProperty("source");
            switch ((StatSourceReference)SimpleValue<int>("source"))
            {
                case StatSourceReference.DirectReference:
                    SimpleProperty("m_stats", "Reference");
                    break;
                case StatSourceReference.FindInRegistry:
                    SimpleProperty("key");
                    break;
            }
            SimpleProperty("m_statId");
            SimpleProperty("treatAsInt");

            SectionHeader("Behavior");
            SimpleProperty("image");
            SimpleProperty("colorize");

            DrawFormattedLabelList(serializedObject.FindProperty("labels"));

            GUILayout.Space(12);
            SimpleProperty("showHelp");
            if (SimpleValue<bool>("showHelp"))
            {
                EditorGUILayout.HelpBox("Available formatting options:"
                    + "\r\n{id} - Id of the Stat"
                    + "\r\n{title} - Title of the Stat"
                    + "\r\n{abbr} - Abbreviation of the Stat"
                    + "\r\n{description} - Description of the Stat"
                    + "\r\n{groupName} - Group name of the Stat"
                    + "\r\n{value} - Current value of the Stat"
                    + "\r\n{maximum} - Maximum value of the Stat"
                    + "\r\n{minimum} - Minimum value of the Stat"
                    + "\r\n{special} - Special value of the Stat"
                    + "\r\n{valueMod} - Total of modifiers applied to value"
                    + "\r\n{maximumMod} - Total of modifiers applied to maximum"
                    + "\r\n{minimumMod} - Total of modifiers applied to minimum"
                    + "\r\n{regenDelay} - Regeneration Delay of the Stat"
                    + "\r\n{regenRate} - Regeneration Rate of the Stat"
                    , MessageType.Info);
            }

            MainContainerEnd();
        }

        #endregion

    }
}
#endif