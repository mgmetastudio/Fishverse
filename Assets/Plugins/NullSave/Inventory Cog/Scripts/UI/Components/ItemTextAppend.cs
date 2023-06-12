using UnityEngine;

namespace NullSave.TOCK.Inventory
{
    [RequireComponent(typeof(ItemText))]
    public class ItemTextAppend : MonoBehaviour
    {

        #region Variables

        public ItemTextTarget targetProperty;
        public string format = "{0}";
        public bool useColor;

        public bool hasMinValue;
        public float minValue;

        public bool hasMaxValue;
        public float maxValue;

        #endregion

    }
}