using TMPro;
using UnityEngine;

namespace NullSave.TOCK.Inventory
{
    public class LoadoutEntryUI : MonoBehaviour
    {

        #region Variables

        public TextMeshProUGUI displayName;
#if GAME_COG
        public UI.SimpleMenuItem menuItem;
#endif

        #endregion

        #region Properties

        public InventoryCog Inventory { get; private set; }

        public Loadout Loadout { get; private set; }

        #endregion

        #region Public Methods

        public void ApplyLoadout()
        {
            Inventory.Loadouts.ActivateLoadout(Loadout);
        }

        public void SaveLoadout()
        {
            Inventory.Loadouts.ActivateLoadout(Loadout);
            SaveLoadoutId();
        }

        public void Load(InventoryCog inventory, Loadout loadout)
        {
            Inventory = inventory;
            Loadout = loadout;
            if (displayName != null)
            {
                displayName.text = loadout.displayName;
            }
#if GAME_COG
            if (menuItem != null)
            {
                menuItem.menuText = loadout.displayName;
            }
#endif
        }

        #endregion

        #region Private Methods

        private void SaveLoadoutId()
        {
            for (int i = 0; i < Inventory.Loadouts.availableLoadouts.Count; i++)
            {
                if (Inventory.Loadouts.availableLoadouts[i] == Loadout)
                {
                    PlayerPrefs.SetInt("LoadoutIndex", i);
                    return;
                }
            }
        }

        #endregion

    }
}