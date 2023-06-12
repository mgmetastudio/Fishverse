using UnityEditor;
using UnityEngine;

namespace NullSave.TOCK.Inventory
{
    [CanEditMultipleObjects()]
    [CustomEditor(typeof(InventoryMenuUI))]
    public class InventoryMenuUIEditor : TOCKEditorV2
    {

        #region Unity Methods

        public override void OnInspectorGUI()
        {
            MainContainerBegin("Inventory Menu UI", "Icons/tock-menu", false);

            SectionHeader("Behaviour");

            SimpleProperty("overrideSpawn");
            if(SimpleBool("overrideSpawn"))
            {
                SimpleProperty("overrideTag");
            }

            SimpleProperty("openMode");
            switch ((NavigationType)serializedObject.FindProperty("openMode").intValue)
            {
                case NavigationType.ByButton:
                    SimpleProperty("openButton", "Button");
                    break;
                case NavigationType.ByKey:
                    SimpleProperty("openKey", "Key");
                    break;
            }

            GUILayout.Space(6);

            SimpleProperty("closeMode");
            switch ((NavigationType)serializedObject.FindProperty("closeMode").intValue)
            {
                case NavigationType.ByButton:
                    SimpleProperty("closeButton", "Button");
                    break;
                case NavigationType.ByKey:
                    SimpleProperty("closeKey", "Key");
                    break;
            }

            SectionHeader("Events");
            SimpleProperty("onOpen");

            MainContainerEnd();
        }

        #endregion

    }
}