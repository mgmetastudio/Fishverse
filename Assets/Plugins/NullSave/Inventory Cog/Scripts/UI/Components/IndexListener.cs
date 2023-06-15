using UnityEngine.Events;

namespace NullSave.TOCK.Inventory
{
    [System.Serializable]
    public class IndexListener
    {

        #region Variables

        public int index;
        public UnityEvent onIndexSelected;

        #endregion

    }
}