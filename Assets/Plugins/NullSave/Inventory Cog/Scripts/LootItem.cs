using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace NullSave.TOCK.Inventory
{
    [HierarchyIcon("loot_item", false)]
    public class LootItem : MonoBehaviour
    {

        #region Variables

        [Tooltip("Item to add to inventory")] public InventoryItem item;
        [Tooltip("If checked item will automatically be picked up without UI")] public bool autoPickup;
        [Tooltip("Item will not honor 'Auto Equip' settings on loot unless this option is checked")] public bool autoEquip;
        [Tooltip("Condition required to automatically consume item")] public string autoConsumeWhen = "1 > 2";
        [Tooltip("Count of item to add to inventory")] public int count = 1;
        [Tooltip("Currency to add to inventory")] public float currency = 0;

        [Tooltip("Override name to display in prompt UI")] public string overrideName;
        [Tooltip("Override action text to display in prompt UI")] public string overrideAction;

        [Tooltip("Rarity generation")] public GenerationType rarityGen;
        public int rarity1;
        public int rarity2;

        [Tooltip("Condition generation")] public GenerationType conditionGen;
        [Range(0, 0)] public float condition1;
        [Range(0, 1)] public int condition2;

        [Tooltip("Value generation")] public GenerationType valueGen;
        public float value1;
        public float value2;

        [Tooltip("Multiplies defualt rarity by above value when checked")] public bool mulByRarity;
        [Tooltip("Multiplies defualt condition by above value when checked")] public bool mulByCondition;

        public UnityEvent onLoot, onPlayerEnter, onPlayerExit;

        #endregion

        #region Properties

        /// <summary>
        /// When a container item is dropped this is populated with all the items in the container
        /// </summary>
        public List<ItemReference> ContainedItems { get; set; }

        public InventoryCog PlayerInventory { get; private set; }

        #endregion

        #region Unity Methods

        private void OnTriggerEnter(Collider other)
        {
            InventoryCog inventoryCog = other.GetComponentInChildren<InventoryCog>();
            if (inventoryCog != null)
            {
                PlayerInventory = inventoryCog;
                onPlayerEnter?.Invoke();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            InventoryCog inventoryCog = other.GetComponentInChildren<InventoryCog>();
            if (inventoryCog != null)
            {
                PlayerInventory = null;
                onPlayerExit.Invoke();
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            InventoryCog inventoryCog = other.GetComponentInChildren<InventoryCog>();
            if (inventoryCog != null)
            {
                PlayerInventory = inventoryCog;
                onPlayerEnter?.Invoke();
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            InventoryCog inventoryCog = other.GetComponentInChildren<InventoryCog>();
            if (inventoryCog != null)
            {
                PlayerInventory = null;
                onPlayerExit.Invoke();
            }
        }

        #endregion

        #region Public Methods

        public virtual void AddToInventory()
        {
            if (PlayerInventory == null)
            {
                GameObject go = GameObject.FindGameObjectWithTag("Player");
                if (go != null)
                {
                    PlayerInventory = go.GetComponent<InventoryCog>();
                }
            }

            if (PlayerInventory == null)
            {
                Debug.LogError(name + ".LootItem.AddToInventory no Player present in trigger");
            }
            else
            {
                count = PlayerInventory.AddToInventory(this, autoEquip);
                if (count == 0)
                {
                    Destroy(gameObject);
                }
            }
        }

        public InventoryItem GenerateValues()
        {
            InventoryItem result = Instantiate(item);
            result.name = item.name;
            result.CurrentCount = count;
            switch (rarityGen)
            {
                case GenerationType.Constant:
                    item.rarity = rarity1;
                    break;
                case GenerationType.RandomBetweenConstants:
                    item.rarity = Random.Range(rarity1, rarity2);
                    break;
            }
            switch (conditionGen)
            {
                case GenerationType.Constant:
                    item.condition = condition1;
                    break;
                case GenerationType.RandomBetweenConstants:
                    item.condition = Random.Range(condition1, condition2);
                    break;
            }
            switch (valueGen)
            {
                case GenerationType.Constant:
                    item.value = value1;
                    break;
                case GenerationType.RandomBetweenConstants:
                    item.value = Random.Range(value1, value2);
                    break;
            }
            if (mulByCondition) item.value *= item.condition;
            if (mulByRarity) item.value *= item.rarity;

            return item;
        }

        #endregion

    }
}