using UnityEditor;
using UnityEngine;

namespace NullSave.TOCK.Inventory
{
    [CanEditMultipleObjects()]
    [CustomEditor(typeof(GeneralUI))]
    public class GeneralUIEditor : TOCKEditorV2
    {

        #region Unity Methods

        public override void OnInspectorGUI()
        {
            MainContainerBeginSlim();

            SectionHeader("General UI");
            GUILayout.Label("This component allows your Game Object to interact with Inventory Cog's themes. No special configuration is required.", Skin.GetStyle("BodyText"));
            GUILayout.Space(8);

            SectionHeader("Events");
            SimpleProperty("onInventoryOpened");
            SimpleProperty("onInventoryClosed");

            MainContainerEnd();
        }

        #endregion

    }
}