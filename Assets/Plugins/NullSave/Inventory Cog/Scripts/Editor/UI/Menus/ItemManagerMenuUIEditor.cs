using UnityEditor;

namespace NullSave.TOCK.Inventory
{
    [CanEditMultipleObjects()]
    [CustomEditor(typeof(ItemManagerMenuUI))]
    public class ItemManagerMenuUIEditor : TOCKEditorV2
    {

        #region Unity Methods

        public override void OnInspectorGUI()
        {
            MainContainerBegin("Item Manager Menu", "Icons/tock-menu");

            SectionHeader("Behaviour");
            SimpleProperty("closeMode");
            switch ((NavigationType)SimpleInt("closeMode"))
            {
                case NavigationType.ByButton:
                    SimpleProperty("closeButton");
                    break;
                case NavigationType.ByKey:
                    SimpleProperty("closeKey");
                    break;
            }

            MainContainerEnd();
        }

        #endregion

    }
}