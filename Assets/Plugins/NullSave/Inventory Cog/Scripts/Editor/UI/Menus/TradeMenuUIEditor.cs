using UnityEditor;

namespace NullSave.TOCK.Inventory
{
    [CanEditMultipleObjects()]
    [CustomEditor(typeof(TradeMenuUI))]
    public class TradeMenuUIEditor : TOCKEditorV2
    {

        #region Unity Methods

        public override void OnInspectorGUI()
        {
            MainContainerBegin("Trade Menu", "Icons/tock-menu");

            SectionHeader("UI Elements");
            SimpleProperty("minCount");
            SimpleProperty("maxCount");
            SimpleProperty("curCount");
            SimpleProperty("totalCost");
            SimpleProperty("tradeType");
            SimpleProperty("buyText");
            SimpleProperty("sellText");
            SimpleProperty("countSlider");

            SectionHeader("Colors");
            SimpleProperty("fundsAvailable");
            SimpleProperty("insufficientFunds");

            SectionHeader("Events");
            SimpleProperty("onHasFunds");
            SimpleProperty("onDoesNotHaveFunds");
            SimpleProperty("onHideCount");
            SimpleProperty("onCannotSell");

            MainContainerEnd();
        }

        #endregion

    }
}