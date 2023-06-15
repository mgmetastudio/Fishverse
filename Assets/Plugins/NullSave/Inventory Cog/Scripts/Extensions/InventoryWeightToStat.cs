using NullSave.TOCK.Stats;
using UnityEngine;

namespace NullSave.TOCK.Inventory
{
    [RequireComponent(typeof(InventoryCog)), RequireComponent(typeof(StatsCog))]
    public class InventoryWeightToStat : MonoBehaviour
    {

        #region Variables

        public string weightStatName = "Weight";

        private InventoryCog inventory;
        private StatsCog stats;
        private StatValue weightStat;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            inventory = GetComponent<InventoryCog>();
            stats = GetComponent<StatsCog>();
            weightStat = stats.FindStat(weightStatName);

            if (weightStat == null)
            {
                Debug.LogError(name + ".InventoryWeightToStat cannot find stat named '" + weightStatName + "'.");
                return;
            }

            inventory.onItemAdded.AddListener(UpdateStat);
            inventory.onItemDropped.AddListener(UpdateStat);
            inventory.onItemRemoved.AddListener(UpdateStat);
        }

        #endregion

        #region Private Methods

        private void UpdateStat(InventoryItem item, int count)
        {
            weightStat.SetValue(inventory.TotalWeight);
        }

        #endregion

    }
}