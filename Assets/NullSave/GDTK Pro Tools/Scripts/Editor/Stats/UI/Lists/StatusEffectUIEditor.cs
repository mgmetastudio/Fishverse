#if GDTK
using UnityEditor;
using UnityEngine;

namespace NullSave.GDTK.Stats
{
    [CustomEditor(typeof(StatusEffectUI))]
    public class StatusEffectUIEditor : GDTKEditor
    {

        #region Unity Methods

        public override void OnInspectorGUI()
        {
            MainContainerBegin();

            SectionHeader("Info UI");
            SimpleProperty("image");
            SimpleProperty("colorize");

            DrawFormattedLabelList(serializedObject.FindProperty("labels"));

            GUILayout.Space(12);
            SimpleProperty("showHelp");
            if (SimpleValue<bool>("showHelp"))
            {
                EditorGUILayout.HelpBox(
                    "{id} - Id of Effect\r\n" +
                    "{title} - Title of Effect\r\n" +
                    "{abbr} - Abbreviation of Effect\r\n" +
                    "{description} - Description of Effect\r\n" +
                    "{group} - Group Name of Effect\r\n" +
                    "{life} - Remaining Life of Effect"
                    , MessageType.Info);

            }

            SectionHeader("Lifespan UI");
            SimpleProperty("remainingLife");
            SimpleProperty("remainingLifeFormat");
            SimpleProperty("lifeProgressbar");
            SimpleProperty("useColors");

            SectionHeader("Child UI");
            SimpleProperty("showAttributes");
            if (SimpleValue<bool>("showAttributes"))
            {
                SimpleProperty("attributePrefab");
            }
            SimpleProperty("showModifiers");
            if(SimpleValue<bool>("showModifiers"))
            {
                SimpleProperty("modifierPrefab");
            }

            MainContainerEnd();
        }

        #endregion

    }
}
#endif