using UnityEditor;

namespace NullSave.TOCK.Inventory
{
    [CanEditMultipleObjects()]
    [CustomEditor(typeof(ItemTextAppend))]
    public class ItemTextAppendEditor : TOCKEditorV2
    {

        #region Unity Methods

        public override void OnInspectorGUI()
        {
            MainContainerBeginSlim();

            SectionHeader("Behaviour");
            SimpleProperty("targetProperty");
            if ((ItemTextTarget)SimpleInt("targetProperty") == ItemTextTarget.RarityName)
            {
                SimpleProperty("useColor");
            }
            SimpleProperty("format");

            switch((ItemTextTarget)SimpleInt("targetProperty"))
            {
                case ItemTextTarget.Condition:
                case ItemTextTarget.Count:
                case ItemTextTarget.CountPerStack:
                case ItemTextTarget.Rarity:
                case ItemTextTarget.RepairIncrement:
                case ItemTextTarget.RepairIncrementCost:
                case ItemTextTarget.Value:
                case ItemTextTarget.Weight:
                    SimpleProperty("hasMinValue");
                    if(SimpleBool("hasMinValue"))
                    {
                        SimpleProperty("minValue");
                    }
                    SimpleProperty("hasMaxValue");
                    if (SimpleBool("hasMaxValue"))
                    {
                        SimpleProperty("maxValue");
                    }
                    break;
                default:
                    SimpleBool("hasMinValue", false);
                    SimpleBool("hasMaxValue", false);
                    break;
            }

            MainContainerEnd();
        }

        #endregion

    }
}