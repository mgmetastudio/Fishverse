using UnityEditor;

namespace NullSave.TOCK.Inventory
{
    [CanEditMultipleObjects()]
    [CustomEditor(typeof(EquipPointUI))]
    public class EquipPointUIEditor : TOCKEditorV2
    {

        #region Unity Methods

        public override void OnInspectorGUI()
        {
            MainContainerBegin("Equip Point UI", "Icons/tock-equip");

            SectionHeader("Behaviour");
            SimpleProperty("inventoryCog");
            SimpleProperty("equipPointId");
            SimpleProperty("findByTag");
            if (serializedObject.FindProperty("findByTag").boolValue)
            {
                SimpleProperty("targetTag");
            }
            SimpleProperty("emptyIcon");

            SectionHeader("UI Elements");
            SimpleProperty("itemIcon");
            SimpleProperty("itemName");
            SimpleProperty("rarityColor");

            SectionHeader("Ammo UI Elements");
            SimpleProperty("ammoContainer");
            SimpleProperty("ammoIcon");
            SimpleProperty("ammoCount");
            SimpleProperty("ammoCountFormat", "Format");

            MainContainerEnd();
        }

        #endregion

    }
}