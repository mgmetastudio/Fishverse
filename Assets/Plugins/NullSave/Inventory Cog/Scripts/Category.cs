﻿using NullSave.TOCK.Stats;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

namespace NullSave.TOCK.Inventory
{
    [CreateAssetMenu(menuName = "TOCK/Inventory/Item Category", order = 3)]
    public class Category : ScriptableObject
    {

        #region Variables

        [Tooltip("Icon to display for category in UI")] public Sprite icon;
        [Tooltip("Name to display for category in UI")] public string displayName;
        [Tooltip("Description of category"), TextArea(2,5)] public string description;

        [Tooltip("Is this category unlocked and available")] public bool catUnlocked = true;
        [Tooltip("Should this category display in the UI")] public bool displayInList = true;

#if PHOTON_UNITY_NETWORKING
        [Tooltip("What path under Resources should we use to find objects for PUN instancing?")] public string punRelativePath;
#endif


        // Slots
        [Tooltip("Does this category have a maximum number of slots")] public bool hasMaxSlots = false;
        [Tooltip("Maximum number of slots")] public int maxSlots = 10;
        [Tooltip("Can slots in this category be locked")] public bool hasLockingSlots = false;
        [Tooltip("How many slots are unlocked in this cateogry")] public int unlockedSlots = 4;
        [Tooltip("Should ammo be ignored when calculating used slots")] public bool ammoIsSlotless = true;

        // Stats Cog Slots
        [Tooltip("Source of maximum slot number")] public ValueSource maxSlotSource = ValueSource.Static;
        [Tooltip("Stat Value used to determine maximum slots")] public string maxSlotStat = "MaxSlots";
        private StatValue statMaxSlot;
        [Tooltip("Source of unlocked slot number")] public ValueSource unlockedSource = ValueSource.Static;
        [Tooltip("Stat Value used to determine unlocked slots")] public string unlockedStat = "UnlockedSlots";
        private StatValue statUnlockedSlots;

        public UnityEvent onItemsChanged;

        private int displayCount;

        #endregion

        #region Properties

        public List<InventoryItem> AssignedAmmo { get; private set; }

        public List<InventoryItem> AssignedItems { get; private set; }

        public StatsCog StatsCog { get; set; }

        public int MaximumSlots
        {
            get
            {
                if (maxSlotSource == ValueSource.Static)
                {
                    return maxSlots;
                }

                if (StatsCog == null)
                {
                    Debug.Log(name + ".MaximumSlots has not StatsCog");
                    return 0;
                }

                if (statMaxSlot == null)
                {
                    statMaxSlot = StatsCog.FindStat(maxSlotStat);
                    if (statMaxSlot == null)
                    {
                        Debug.Log(name + ".MaximumSlots cannot find stat " + maxSlotStat);
                        return 0;
                    }
                }

                return Mathf.CeilToInt(statMaxSlot.CurrentValue);
            }
        }

        public int UnlockedSlots
        {
            get
            {
                if (unlockedSource == ValueSource.Static)
                {
                    return unlockedSlots;
                }

                if (StatsCog == null)
                {
                    Debug.Log(name + ".UnlockedSlots has no StatsCog");
                    return 0;
                }

                if (statUnlockedSlots == null)
                {
                    statUnlockedSlots = StatsCog.FindStat(unlockedStat);
                    if (statUnlockedSlots == null)
                    {
                        Debug.Log(name + ".UnlockedSlots cannot find stat " + unlockedStat);
                        return 0;
                    }
                }

                return Mathf.CeilToInt(statUnlockedSlots.CurrentValue);
            }
        }

        public int UsedSlots { get; private set; }

        #endregion

        #region Public Methods

        public void AddItem(InventoryItem item)
        {
            if (!catUnlocked)
            {
                Debug.LogWarning("Category '" + name + "' is not unlocked, add item refused.");
                return;
            }

            if (ammoIsSlotless && item.itemType == ItemType.Ammo)
            {
                AssignedAmmo.Add(item);
                if (item.displayInInventory) displayCount += 1;
                onItemsChanged?.Invoke();
                return;
            }

            AssignedItems.Add(item);
            if (item.displayInInventory)
            {
                displayCount += 1;
                UsedSlots += 1;
            }

            onItemsChanged?.Invoke();
        }

        public void Clear()
        {
            AssignedAmmo.Clear();
            while (AssignedItems.Count > 0)
            {
                RemoveItem(AssignedItems[0]);
            }
        }

        public void InsertItem(InventoryItem item, int index)
        {
            if (!catUnlocked)
            {
                Debug.LogWarning(name + " is not unlocked, add item refused.");
                return;
            }

            if (ammoIsSlotless && item.itemType == ItemType.Ammo)
            {
                AssignedAmmo.Add(item);
                if (item.displayInInventory) displayCount += 1;
                onItemsChanged?.Invoke();
                return;
            }

            AssignedItems.Insert(index, item);
            if (item.displayInInventory)
            {
                displayCount += 1;
                UsedSlots += 1;
            }

            onItemsChanged?.Invoke();
        }

        public int GetItemUsedStacks(InventoryItem item)
        {
            int count = 0;
            if (ammoIsSlotless && item.itemType == ItemType.Ammo)
            {
                foreach (InventoryItem usedItem in AssignedAmmo)
                {
                    if (usedItem.name == item.name)
                    {
                        count += 1;
                    }
                }
            }
            else
            {
                foreach (InventoryItem usedItem in AssignedItems)
                {
                    if (usedItem.name == item.name)
                    {
                        count += 1;
                    }
                }
            }
            return count;
        }

        public int GetUnlockedPages(int slotsPerPage)
        {
            int max = hasLockingSlots ? UnlockedSlots : MaximumSlots;
            return Mathf.CeilToInt(max / (float)slotsPerPage);
        }

        public int GetUsedPages(int slotsPerPage)
        {
            return Mathf.CeilToInt((AssignedItems.Count + AssignedAmmo.Count) / (float)slotsPerPage);
        }

        public int GetUsedPagesUI(int slotsPerPage, bool includeLocked)
        {
            if (hasLockingSlots && includeLocked)
            {
                return Mathf.CeilToInt((displayCount + (MaximumSlots - UnlockedSlots))  / (float)slotsPerPage);
            }
            else
            {
                return Mathf.CeilToInt(displayCount / (float)slotsPerPage);
            }
        }

        public void RemoveItem(InventoryItem item)
        {
            if (AssignedItems.Contains(item))
            {
                AssignedItems.Remove(item);
                if (item.displayInInventory)
                {
                    displayCount -= 1;
                    UsedSlots -= 1;
                }
                onItemsChanged?.Invoke();
            }
            else if (AssignedAmmo.Contains(item))
            {
                AssignedAmmo.Remove(item);
                if (item.displayInInventory) displayCount -= 1;
                onItemsChanged?.Invoke();
            }
        }

        /// <summary>
        /// Resort the inventory
        /// </summary>
        /// <param name="sortOrder"></param>
        public void Sort(InventorySortOrder sortOrder)
        {
            switch (sortOrder)
            {
                case InventorySortOrder.ConditionAsc:
                    AssignedItems.Sort((p1, p2) => p1.condition.CompareTo(p2.condition));
                    break;
                case InventorySortOrder.ConditionDesc:
                    AssignedItems.Sort((p1, p2) => p2.condition.CompareTo(p1.condition));
                    break;
                case InventorySortOrder.DisplayNameAsc:
                    AssignedItems.Sort((p1, p2) => p1.DisplayName.CompareTo(p2.DisplayName));
                    break;
                case InventorySortOrder.DisplayNameDesc:
                    AssignedItems.Sort((p1, p2) => p2.DisplayName.CompareTo(p1.DisplayName));
                    break;
                case InventorySortOrder.ItemCountAsc:
                    AssignedItems.Sort((p1, p2) => p1.CurrentCount.CompareTo(p2.CurrentCount));
                    break;
                case InventorySortOrder.ItemCountDesc:
                    AssignedItems.Sort((p1, p2) => p2.CurrentCount.CompareTo(p1.CurrentCount));
                    break;
                case InventorySortOrder.ItemTypeAsc:
                    AssignedItems.Sort((p1, p2) => p1.itemType.CompareTo(p2.itemType));
                    break;
                case InventorySortOrder.ItemTypeDesc:
                    AssignedItems.Sort((p1, p2) => p2.itemType.CompareTo(p1.itemType));
                    break;
                case InventorySortOrder.RarityAsc:
                    AssignedItems.Sort((p1, p2) => p1.rarity.CompareTo(p2.rarity));
                    break;
                case InventorySortOrder.RarityDesc:
                    AssignedItems.Sort((p1, p2) => p2.rarity.CompareTo(p1.rarity));
                    break;
                case InventorySortOrder.ValueAsc:
                    AssignedItems.Sort((p1, p2) => p1.value.CompareTo(p2.value));
                    break;
                case InventorySortOrder.ValueDesc:
                    AssignedItems.Sort((p1, p2) => p2.value.CompareTo(p1.value));
                    break;
                case InventorySortOrder.WeightAsc:
                    AssignedItems.Sort((p1, p2) => p1.weight.CompareTo(p2.weight));
                    break;
                case InventorySortOrder.WeightDesc:
                    AssignedItems.Sort((p1, p2) => p2.weight.CompareTo(p1.weight));
                    break;
            }
        }

        public void StateLoad(Stream stream, InventoryCog inventory, float version)
        {
            // generic data
            unlockedSlots = stream.ReadInt();
            UsedSlots = stream.ReadInt();
            displayCount = stream.ReadInt();
            if(version >= 1.6f)
            {
                catUnlocked = stream.ReadBool();
            }

            // ammo data
            AssignedAmmo = new List<InventoryItem>();
            int count = stream.ReadInt();
            for (int i = 0; i < count; i++)
            {
                AssignedAmmo.Add(inventory.GetItemByInstanceId(stream.ReadStringPacket()));
            }

            //  item data
            AssignedItems = new List<InventoryItem>();
            count = stream.ReadInt();
            for (int i = 0; i < count; i++)
            {
                AssignedItems.Add(inventory.GetItemByInstanceId(stream.ReadStringPacket()));
            }
        }

        public void StateSave(Stream stream, float version)
        {
            // Write generic data
            stream.WriteInt(unlockedSlots);
            stream.WriteInt(UsedSlots);
            stream.WriteInt(displayCount);
            if (version >= 1.6f)
            {
                stream.WriteBool(catUnlocked);
            }

            // Write ammo data
            if (AssignedAmmo == null)
            {
                stream.WriteInt(0);
            }
            else
            {
                stream.WriteInt(AssignedAmmo.Count);
                foreach (InventoryItem item in AssignedAmmo)
                {
                    stream.WriteStringPacket(item.InstanceId);
                }
            }

            // Write item data
            if (AssignedItems == null)
            {
                stream.WriteInt(0);
            }
            else
            {
                stream.WriteInt(AssignedItems.Count);
                foreach (InventoryItem item in AssignedItems)
                {
                    stream.WriteStringPacket(item.InstanceId);
                }
            }
        }

        public void Initialize()
        {
            displayCount = 0;
            UsedSlots = 0;
            AssignedItems = new List<InventoryItem>();
            AssignedAmmo = new List<InventoryItem>();
        }

        #endregion

    }
}