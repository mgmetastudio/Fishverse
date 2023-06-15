using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NullSave.TOCK.Inventory
{
    public class HotbarSlotUI : MonoBehaviour, IDropHandler
    {

        #region Variables

        public InventoryCog inventoryCog;
        public int slotId;
        public bool autoHandle;

        public Image itemImage;
        public TextMeshProUGUI displayName;
        public GameObject equippedIndicator;
        public string countPrefix;
        public TextMeshProUGUI count;
        public string countSuffix;
        public GameObject hideIfCountSub2;
        public RarityColorIndicator rarityColorIndicator;
        public Slider conditionSlider;
        public Slider raritySlider;
        public bool hideIfConditionZero, hideIfConditionOne;
        public bool hideIfRarityZero;
        public bool removeZeroCountItems;

        private InventoryHotbarSlot slot;

        #endregion

        #region Properties

        public InventoryCog Inventory
        {
            get { return inventoryCog; }
            set
            {
                Unsubscribe();
                inventoryCog = value;
                Subscribe();
                HotbarUpdated(slotId);
            }
        }

        public InventoryItem Item { get; private set; }

        #endregion

        #region Unity Methods

        public void OnDrop(PointerEventData eventData)
        {
            if (slot != null)
            {
                CompleteDrop(eventData);
            }
        }

        private void Reset()
        {
            autoHandle = true;
            countPrefix = "x";
        }

        #endregion

        #region Public Methods

        public void AssignItemFromSelection(InventoryItemList source)
        {
            slot.AssignItem(source.SelectedItem.Item);
        }

        public void UseItem()
        {
            if (slot.AssignedItem == null) return;

            switch(slot.AssignedItem.itemType)
            {
                case ItemType.Ammo:
                    inventoryCog.SetSelectedAmmo(slot.AssignedItem);
                    break;
                case ItemType.Weapon:
                    // Equip Item
                    switch(slot.AssignedItem.EquipState)
                    {
                        case EquipState.NotEquipped:
                            inventoryCog.EquipItem(slot.AssignedItem);
                            break;
                        case EquipState.Stored:
                            slot.AssignedItem.CurrentEquipPoint.EquipItem(slot.AssignedItem);
                            break;
                    }

                    // Attack (if manager available)
                    if (slot.AssignedItem.CurrentEquipPoint != null)
                    {
                        foreach (InventoryAttackManager attackManager in inventoryCog.AttackManagers)
                        {
                            if (attackManager.equipPointName == slot.AssignedItem.CurrentEquipPoint.pointId)
                            {
                                attackManager.Attack();
                                break;
                            }
                        }
                    }

                    break;
                case ItemType.Container:
                    inventoryCog.ActiveTheme.OpenItemContainer(slot.AssignedItem, null);
                    break;
                default:
                    inventoryCog.UseItem(slot.AssignedItem, 1);
                    break;
            }

            if(slot.AssignedItem.CurrentCount == 0 && removeZeroCountItems)
            {
                slot.AssignItem(null);
            }
        }

        #endregion

        #region Private Methods

        private void CompleteDrop(PointerEventData eventData)
        {
            ItemUI draggableItem = eventData.pointerDrag.gameObject.GetComponentInChildren<ItemUI>();
            if(draggableItem != null && draggableItem.Item != null && draggableItem.Item.allowHotbar)
            {
                slot.AssignItem(draggableItem.Item);
            }
        }

        private void HotbarUpdated(int slotId)
        {
            if (slotId != this.slotId || Inventory.Hotbar == null || !Inventory.Hotbar.Initialized || Inventory.Hotbar.HotbarSlots.Count <= slotId) return;

            slot = Inventory.Hotbar.HotbarSlots[slotId];
            Item = slot.AssignedItem;

            if (Item == null)
            {
                if (itemImage != null) itemImage.enabled = false;
                if (count != null) count.text = string.Empty;
                if (equippedIndicator != null) equippedIndicator.SetActive(false);
                if (displayName != null) displayName.text = string.Empty;
                if (rarityColorIndicator != null) rarityColorIndicator.gameObject.SetActive(false);
                if (conditionSlider != null)
                {
                    conditionSlider.value = conditionSlider.minValue;
                    if (hideIfConditionZero || hideIfConditionOne) conditionSlider.gameObject.SetActive(false);
                }
                if (raritySlider != null) raritySlider.value = raritySlider.minValue;
                if (hideIfCountSub2 != null) hideIfCountSub2.SetActive(false);
                if (hideIfRarityZero && raritySlider != null) raritySlider.gameObject.SetActive(false);
                return;
            }

            if (itemImage != null)
            {
                itemImage.sprite = Item.icon;
                itemImage.enabled = itemImage.sprite != null;
            }

            if (count != null)
            {
                count.text = countPrefix + Item.CurrentCount + countSuffix;
            }
            if (hideIfCountSub2 != null) hideIfCountSub2.SetActive(Item.CurrentCount > 1);

            if (conditionSlider != null)
            {
                conditionSlider.minValue = 0;
                conditionSlider.maxValue = 1;
                conditionSlider.value = Item.condition;
                if (hideIfConditionZero)
                {
                    conditionSlider.gameObject.SetActive(Item.condition > 0);
                }
                if (hideIfConditionOne)
                {
                    conditionSlider.gameObject.SetActive(Item.condition != 1);
                }
            }

            if (raritySlider != null)
            {
                raritySlider.minValue = 0;
                raritySlider.maxValue = 10;
                raritySlider.value = Item.rarity;
                if (hideIfRarityZero)
                {
                    raritySlider.gameObject.SetActive(Item.rarity > 0);
                }
            }

            if (Item.itemType == ItemType.Ammo)
            {
                if (equippedIndicator != null) equippedIndicator.SetActive(Inventory != null && Inventory.GetSelectedAmmo(Item.ammoType) == Item);
            }
            else if (Item.CanEquip)
            {
                if (equippedIndicator != null) equippedIndicator.SetActive(Item.EquipState != EquipState.NotEquipped);

            }
            else if (Item.itemType == ItemType.Skill)
            {
                if (equippedIndicator != null) equippedIndicator.SetActive(!string.IsNullOrEmpty(Item.AssignedSkillSlot));
            }
            else
            {
                if (equippedIndicator != null) equippedIndicator.SetActive(false);
            }

            if (displayName != null)
            {
                displayName.text = Item.DisplayName;
            }

            if (rarityColorIndicator != null) rarityColorIndicator.LoadItem(Item);

        }

        private void Subscribe()
        {
            if (inventoryCog != null && inventoryCog.Hotbar != null)
            {
                inventoryCog.Hotbar.onHotbarChanged.AddListener(HotbarUpdated);
            }
        }

        private void Unsubscribe()
        {
            if(inventoryCog != null && inventoryCog.Hotbar != null)
            {
                inventoryCog.Hotbar.onHotbarChanged.RemoveListener(HotbarUpdated);
            }
        }

        #endregion
    }
}