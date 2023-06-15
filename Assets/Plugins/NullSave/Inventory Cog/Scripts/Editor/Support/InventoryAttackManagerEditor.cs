using UnityEditor;

namespace NullSave.TOCK.Inventory
{
    [CustomEditor(typeof(InventoryAttackManager))]
    public class InventoryAttackManagerEditor : TOCKEditorV2
    {

        public override void OnInspectorGUI()
        {
            MainContainerBeginSlim();

            SectionHeader("Behaviour");
            SimpleProperty("equipPointName", "Equip Point");
            SimpleProperty("useButton");
            if(SimpleBool("useButton"))
            {
                SimpleProperty("attackButton");
            }
            SimpleProperty("useKey");
            if (SimpleBool("useKey"))
            {
                SimpleProperty("attackKey");
            }

            SectionHeader("Require Animator Values");
            SimpleList("requireMods");

            SectionHeader("Set Animator Values");
            SimpleList("applyMods");

            SectionHeader("Events");
            SimpleProperty("onAttackTriggered");

            MainContainerEnd();

        }

    }
}