using NullSave.TOCK.Stats;
using System.Collections.Generic;
using UnityEngine;

namespace NullSave.TOCK.Inventory
{
    public class LootTable : MonoBehaviour
    {

        #region Variables

        public List<LootDrop> dropList;

        #endregion

        #region Public Methods

        public void AddRandomLootToInventory(InventoryCog target)
        {
            List<LootItem> loot = GetRandomLoot(target.gameObject.GetComponent<StatsCog>());
            foreach(LootItem item in loot)
            {
                target.AddToInventory(item);
            }
        }

        public void DropRandomLoot()
        {
            List<LootItem> loot = GetRandomLoot(null);
            foreach (LootItem item in loot)
            {
                GameObject goItem = Instantiate(item).gameObject;
                goItem.transform.position = transform.position;
            }
        }

        public void DropRandomLoot(StatsCog sourceStats)
        {
            List<LootItem> loot = GetRandomLoot(sourceStats);
            foreach (LootItem item in loot)
            {
                GameObject goItem = Instantiate(item).gameObject;
                goItem.transform.position = transform.position;
            }
        }

        public void DropRandomLootFromLastAttacker()
        {
            List<LootItem> loot = GetRandomLoot(gameObject.GetComponent<StatsCog>().LastDamageSource);
            foreach (LootItem item in loot)
            {
                GameObject goItem = Instantiate(item).gameObject;
                goItem.transform.position = transform.position;
            }
        }

        public List<LootItem> GetRandomLoot(StatsCog sourceStats)
        {
            float totalWeight = 0;

            List<LootDrop> tempList = new List<LootDrop>();
            foreach(LootDrop drop in dropList)
            {
                if(!drop.usePlayerCondition || (sourceStats != null && sourceStats.EvaluateCondition(drop.playerCondition)))
                {
                    tempList.Add(drop);
                    totalWeight += drop.weight;
                }
            }

            // Sort items by weight
            tempList.Sort((p1, p2) => p1.weight.CompareTo(p2.weight));

            // Get roll
            float roll = Random.Range(0, totalWeight);

            // Get reward
            foreach (LootDrop dropItem in tempList)
            {
                if(roll <= dropItem.weight)
                {
                    return dropItem.items;
                }
                else
                {
                    roll -= dropItem.weight;
                }
            }

            return null;
        }

        #endregion

    }
}