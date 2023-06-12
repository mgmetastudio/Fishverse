﻿using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NullSave.TOCK.Inventory
{
    [HierarchyIcon("tock-ui", false)]
    public class EquipPointUI : MonoBehaviour, IDropHandler
    {

        #region Variables

        public InventoryCog inventoryCog;
        public bool findByTag;
        public string targetTag;

        public string equipPointId;
        public Sprite emptyIcon;
        public Image itemIcon;
        public TextMeshProUGUI itemName;
        public RarityColorIndicator rarityColor;

        // Ammo Elements
        public GameObject ammoContainer;
        public Image ammoIcon;
        public TextMeshProUGUI ammoCount;
        public string ammoCountFormat = "{0}";

        private EquipPoint point;

        #endregion

        #region Unity Methods

        private void OnEnable()
        {
            Refresh();
        }

        private void OnDisable()
        {
            if (point != null)
            {
                Unsubscribe();
                point = null;
            }
        }

        #endregion

        #region Public Methods

        public void OnDrop(PointerEventData eventData)
        {
            ItemUI draggableItem = eventData.pointerDrag.gameObject.GetComponentInChildren<ItemUI>();
            if (draggableItem != null)
            {
                if (draggableItem != null)
                {
                    inventoryCog.EquipItem(draggableItem.Item, equipPointId);
                }
            }
        }

        public void Refresh()
        {
            if (inventoryCog == null && findByTag)
            {
                GameObject[] gos = GameObject.FindGameObjectsWithTag(targetTag);
                foreach (GameObject go in gos)
                {
                    InventoryCog inventory = go.GetComponent<InventoryCog>();
                    if (inventory != null)
                    {
                        inventoryCog = inventory;
                        break;
                    }
                }

                if (inventoryCog == null)
                {
                    StartCoroutine("RetryFind");
                }
            }

            if (inventoryCog != null)
            {
                if (point != null)
                {
                    Unsubscribe();
                }

                point = inventoryCog.GetPointById(equipPointId);
                if (point != null)
                {
                    Subscribe();

                    if (point.IsItemEquipped)
                    {
                        UpdateUI(point.EquipedOrStoredItem);
                    }
                    else
                    {
                        UpdateUI(null);
                    }
                }
                else
                {
                    Debug.Log("No point found");
                    UpdateUI(null);
                }
            }
        }

        #endregion

        #region Private Methods

        private IEnumerator RetryFind()
        {
            yield return new WaitForEndOfFrame();
            GameObject[] gos = GameObject.FindGameObjectsWithTag(targetTag);
            foreach (GameObject go in gos)
            {
                InventoryCog inventory = go.GetComponent<InventoryCog>();
                if (inventory != null)
                {
                    inventoryCog = inventory;
                    break;
                }
            }

            if (inventoryCog != null)
            {
                if (point != null)
                {
                    Unsubscribe();
                }

                point = inventoryCog.GetPointById(equipPointId);
                if (point != null)
                {
                    Subscribe();

                    if (point.IsItemEquipped)
                    {
                        UpdateUI(point.EquipedOrStoredItem);
                    }
                    else
                    {
                        UpdateUI(null);
                    }
                }
                else
                {
                    UpdateUI(null);
                }
            }
            else
            {
                UpdateUI(null);
            }
        }

        private void Subscribe()
        {
            point.onItemEquipped.AddListener(UpdateUI);
            point.onItemStored.AddListener(UpdateUI);
            point.onItemUnequipped.AddListener((InventoryItem item) => UpdateUI(null));
            point.onItemAmmoChanged.AddListener(UpdateUI);
        }

        private void Unsubscribe()
        {
            point.onItemEquipped.RemoveListener(UpdateUI);
            point.onItemStored.RemoveListener(UpdateUI);
            point.onItemUnequipped.RemoveListener(UpdateUI);
            point.onItemAmmoChanged.RemoveListener(UpdateUI);
        }

        private void UpdateUI(InventoryItem equippedItem)
        {

            if (itemIcon != null)
            {
                itemIcon.sprite = equippedItem == null ? emptyIcon != null ? emptyIcon : null : equippedItem.icon;
                itemIcon.enabled = itemIcon.sprite != null;
            }

            if (itemName != null)
            {
                itemName.text = equippedItem == null ? string.Empty : equippedItem.DisplayName;
            }

            if (rarityColor != null)
            {
                rarityColor.LoadItem(equippedItem);
            }

            if (ammoContainer != null)
            {
                ammoContainer.SetActive(equippedItem != null && equippedItem.usesAmmo);
            }

            if(equippedItem != null && equippedItem.usesAmmo)
            {
                if (ammoContainer) ammoContainer.gameObject.SetActive(true);
                if (ammoCount)
                {
                    ammoCount.text = ammoCountFormat.Replace("{0}", inventoryCog.GetEquippedAmmoCount(equippedItem.ammoType).ToString());
                    ammoCount.gameObject.SetActive(true);

                }
                if (ammoIcon)
                {
                    ammoIcon.gameObject.SetActive(true);
                    ammoIcon.sprite = inventoryCog.GetSelectedAmmo(equippedItem.ammoType).icon;
                }
            }
            else
            {
                if (ammoContainer) ammoContainer.gameObject.SetActive(false);
                if (ammoCount) ammoCount.gameObject.SetActive(false);
                if (ammoIcon) ammoIcon.gameObject.SetActive(false);
            }

            if (ammoCount != null)
            {
                ammoCount.gameObject.SetActive(equippedItem != null && equippedItem.usesAmmo);
                if (equippedItem != null && equippedItem.usesAmmo)
                {
                    ammoCount.text = inventoryCog.GetEquippedAmmoCount(equippedItem.ammoType).ToString();
                }
            }
        }

        #endregion

    }
}