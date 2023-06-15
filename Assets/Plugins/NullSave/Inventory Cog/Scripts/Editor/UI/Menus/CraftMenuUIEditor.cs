using UnityEditor;

namespace NullSave.TOCK.Inventory
{
    [CustomEditor(typeof(CraftMenuUI))]
    public class CraftMenuUIEditor : TOCKEditorV2
    {

        #region Unity Methods

        public override void OnInspectorGUI()
        {
            MainContainerBegin("Craft Menu", "Icons/tock-menu");

            SectionHeader("Behaviour");
            SimpleProperty("craftingType");
            switch((CraftMenuUI.CraftingType)SimpleInt("craftingType"))
            {
                case CraftMenuUI.CraftingType.Blind:
                    SimpleProperty("playerInventory");
                    SimpleProperty("blindList");
                    SimpleProperty("destroyIfNoMatch");
                    SimpleProperty("alwaysUseSingle");
                    break;
                case CraftMenuUI.CraftingType.Normal:
                    SimpleProperty("recipeGrid");
                    break;
            }

            MainContainerEnd();
        }

        #endregion

    }
}
