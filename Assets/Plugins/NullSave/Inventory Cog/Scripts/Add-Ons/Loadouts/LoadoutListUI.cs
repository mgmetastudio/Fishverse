using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NullSave.TOCK.Inventory
{
    public class LoadoutListUI : MonoBehaviour
    {

        #region Variables

        public InventoryCog inventoryCog;
        public LoadoutEntryUI entryPrefab;
        public Transform listContainer;

        private List<LoadoutEntryUI> loadedItems;


        #endregion

        #region Unity Methods

        private void OnEnable()
        {
            Clear();

            if (inventoryCog != null)
            {
                LoadList();
            }
            else
            {
                StartCoroutine("WaitForCog");
            }
        }

        #endregion

        #region Public Methods

        public void Reload()
        {
            LoadList();
        }

        #endregion

        #region Private Methods

        private void Clear()
        {
            if (loadedItems != null)
            {
                foreach (LoadoutEntryUI item in loadedItems)
                {
                    Destroy(item.gameObject);
                }
                loadedItems.Clear();
            }
            else
            {
                loadedItems = new List<LoadoutEntryUI>();
            }
        }

        private void LoadList()
        {
            if (inventoryCog.Loadouts == null) return;
            Clear();

            foreach (Loadout loadout in inventoryCog.Loadouts.availableLoadouts)
            {
                LoadoutEntryUI instance = Instantiate(entryPrefab, listContainer);
                instance.Load(inventoryCog, loadout);
                loadedItems.Add(instance);
            }

#if GAME_COG
            UI.SimpleMenu menu = GetComponent<UI.SimpleMenu>();
            if(menu != null)
            {
                menu.ResetMenu();
            }
#endif
        }

        private IEnumerator WaitForCog()
        {
            int tryCount = 0;
            while (tryCount < 30)
            {
                yield return new WaitForEndOfFrame();
                if (inventoryCog != null)
                {
                    LoadList();
                    break;
                }
            }
        }

        #endregion

    }
}