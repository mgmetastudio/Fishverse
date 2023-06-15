using System.Collections.Generic;
using UnityEngine;

namespace NullSave.TOCK.Inventory
{
    public class CraftQueueList : MonoBehaviour
    {

        #region Variables

        // Load type
        public ListLoadMode loadMode = ListLoadMode.OnEnable;

        public InventoryCog inventoryCog;

        // Filtering
        public List<string> categories;

        // Editor
        public int z_display_flags = 4095;

        #endregion

        #region Properties

        public InventoryCog Inventory { get { return inventoryCog; } set { inventoryCog = value; } }

        #endregion

        #region Public Methods

        public virtual void LoadQueue() { throw new System.NotImplementedException(); }

        #endregion

    }
}