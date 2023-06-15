using UnityEditor;
using UnityEngine;

namespace NullSave.TOCK.Inventory
{
    [CustomEditor(typeof(PromptUI))]
    public class PromptUIditor : TOCKEditorV2
    {

        #region Unity Methods

        public override void OnInspectorGUI()
        {
            MainContainerBeginSlim();
            SectionHeader("Behaviour");
            DrawPropertiesExcluding(serializedObject, "m_Script");

            SubHeader("Key");
            GUILayout.Label("{0} - Item Name\r\n{1} - Action Prompt", Skin.GetStyle("WrapText"));
            GUILayout.Space(4);

            MainContainerEnd();
        }

        #endregion

    }
}