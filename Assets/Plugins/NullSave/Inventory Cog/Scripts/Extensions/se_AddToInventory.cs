using NullSave.TOCK.Stats;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace NullSave.TOCK.Inventory
{
    [CreateAssetMenu(menuName = "TOCK/Stats Cog/Extensions/Add To Inventory", fileName = "New Add Inventory Extension")]
    public class se_AddToInventory : StatExtension
    {

        #region Variables

        public List<ItemReference> items;

        #endregion

        #region Public Methods

        public override bool AllowAddToParent(Type parentType)
        {
            if (typeof(Trait) == parentType) return true;
            if (typeof(StatEffect) == parentType) return true;
            return false;
        }

        public override void OnAdded(GameObject host)
        {
            InventoryCog ic = host.GetComponentInChildren<InventoryCog>();
            if (ic == null) return;

            foreach(ItemReference item in items)
            {
                ic.AddToInventory(item.item, item.count);
            }
        }

        public override void OnEnded(GameObject host) { OnRemoved(host); }

        public override void OnRemoved(GameObject host)
        {
            InventoryCog ic = host.GetComponentInChildren<InventoryCog>();
            if (ic == null) return;

            foreach (ItemReference item in items)
            {
                ic.RemoveItem(item.item, item.count);
            }
        }

        #endregion

    }
}