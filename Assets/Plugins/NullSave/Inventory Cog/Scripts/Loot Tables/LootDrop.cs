﻿using System.Collections.Generic;

namespace NullSave.TOCK.Inventory
{
    [System.Serializable]
    public class LootDrop
    {

        #region Variables

        public bool usePlayerCondition;
        public string playerCondition;
        public float weight;
        public List<LootItem> items;
        public List<ItemReference> itemReferences;

        #endregion

    }
}