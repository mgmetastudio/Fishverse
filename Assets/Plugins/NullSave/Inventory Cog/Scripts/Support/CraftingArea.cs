using System.Collections.Generic;
using UnityEngine;

namespace NullSave.TOCK.Inventory
{
    public class CraftingArea : MonoBehaviour
    {

        #region Variables

        public string displayName = "Crafting Bench";
        public string overrideActionName;
        public bool showAllCategories = true;
        public List<CraftingCategory> allowedCategories;

        #endregion

    }
}