using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace NullSave.TOCK.Inventory
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(InventoryHotbar))]
    public class InventoryHotbarEditor : TOCKEditorV2
    {

        #region Unity Methods

        public override void OnInspectorGUI()
        {
            MainContainerBeginSlim();

            SectionHeader("Behaviour");
            SimpleProperty("hotbarSize", "Slot Count");
            SimpleProperty("hotbarFreeSlot", "Free Slots");
            SimpleProperty("hotbarDragDrop", "Enable Drag & Drop");

            GUILayout.Space(6);
            GUILayout.BeginVertical("box");
            GUILayout.Label("If 'Free Slots' is checked items placed in the hotbar will be removed from inventory, freeing up their slot(s).", Skin.GetStyle("BodyText"));
            GUILayout.EndVertical();

            MainContainerEnd();
        }

        #endregion

    }
}