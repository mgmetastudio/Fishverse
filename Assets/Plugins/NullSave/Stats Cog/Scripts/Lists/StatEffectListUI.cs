using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NullSave.TOCK.Stats
{
    public class StatEffectListUI : MonoBehaviour
    {

        #region Variables

        public StatEffectUI uiPrefab;

        private List<StatEffectUI> loaded;

        #endregion

        #region Public Methods

        public void Clear()
        {
            if(loaded == null)
            {
                loaded = new List<StatEffectUI>();
                return;
            }

            foreach(StatEffectUI ui in loaded)
            {
                if(ui != null && ui.gameObject != null)
                {
                    Destroy(ui.gameObject);
                }
            }
            loaded.Clear();
        }

#if INVENTORY_COG
        public void LoadEffects(Inventory.InventoryItem item)
        {
            Clear();

            foreach(StatEffect effect in item.statEffects)
            {
                StatEffectUI ui = Instantiate(uiPrefab, transform);
                ui.SetEffect(effect);
                loaded.Add(ui);
            }
        }
#endif

        #endregion

    }
}