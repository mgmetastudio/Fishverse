using UnityEngine;
using UnityEngine.Events;

namespace NullSave.TOCK.Inventory
{
    public class InventoryListMonitor : MonoBehaviour
    {

        #region Variables

        public InventoryItemList targetList;
        public UnityEvent onCanEquip, onCannotEquip, onCanRemove, onCannotRemove, onCanModify, onCannotModify, 
            onCanRepair, onCannotRepair, onCanDrop, onCannotDrop, onCanBreakdown, onCannotBreakDown, 
            onCanAttach, onCannotAttach, onCanUnattach, onCannotUnAttach, onCanRename, onCannotRename,
            onIsContainer, onIsNotContainer, onIsSkill, onIsNotSkill, onIsConsumable, onIsNotConsumable;
        private int lastIndex;

        #endregion

        #region Unity Events

        private void Start()
        {
            targetList.onSelectionChanged.AddListener(SelectionChanged);
            targetList.inventoryCog.onItemEquipped.AddListener((InventoryItem item) => SelectionChanged(lastIndex));
            targetList.inventoryCog.onItemRemoved.AddListener((InventoryItem item, int count) => SelectionChanged(lastIndex));
            targetList.inventoryCog.onItemUnequipped.AddListener((InventoryItem item) => SelectionChanged(lastIndex));
            targetList.inventoryCog.onItemDropped.AddListener((InventoryItem item, int count) => SelectionChanged(lastIndex));
            SelectionChanged(0);
        }

        #endregion

        #region Private Methods

        private void SelectionChanged(int index)
        {
            lastIndex = index;

            if (targetList.SelectedItem == null || targetList.SelectedItem.Item == null)
            {
                onCannotAttach?.Invoke();
                onCannotBreakDown?.Invoke();
                onCannotDrop?.Invoke();
                onCannotEquip?.Invoke();
                onCannotModify?.Invoke();
                onCannotRemove?.Invoke();
                onCannotRepair?.Invoke();
                onCannotUnAttach?.Invoke();
                onCannotRename?.Invoke();
                onCannotUnAttach?.Invoke();
                onIsNotContainer?.Invoke();
                onIsNotSkill?.Invoke();
                return;
            }

            InventoryItem item = targetList.SelectedItem.Item;

            // Container check
            if (item.itemType == ItemType.Container)
            {
                onIsContainer?.Invoke();
            }
            else
            {
                onIsNotContainer?.Invoke();
            }

            if (item.itemType == ItemType.Attachment)
            {
                if(item.IsAttached)
                {
                    onCannotAttach?.Invoke();
                    onCanUnattach?.Invoke();
                }
                else
                {
                    onCanAttach?.Invoke();
                    onCannotUnAttach?.Invoke();
                }
            }
            else
            {
                onCannotAttach?.Invoke();
                if (item.HasAttachments)
                {
                    onCanModify?.Invoke();
                }
                else
                {
                    switch (item.attachRequirement)
                    {
                        case AttachRequirement.InCategory:
                        case AttachRequirement.InItemList:
                            if (targetList.Inventory.AttachmentsAvailableForItem(item))
                            {
                                onCanModify?.Invoke();
                            }
                            else
                            {
                                onCannotModify?.Invoke();
                            }
                            break;
                        case AttachRequirement.NoneAllowed:
                            onCannotModify?.Invoke();
                            break;
                    }
                }
            }

            if(item.itemType == ItemType.Skill)
            {
                onIsSkill?.Invoke();
            }
            else
            {
                onIsNotSkill?.Invoke();
            }

            if (item.CanBreakdown)
            {
                onCanBreakdown?.Invoke();
            }
            else
            {
                onCannotBreakDown?.Invoke();
            }

            if (item.canDrop)
            {
                onCanDrop?.Invoke();
            }
            else
            {
                onCannotDrop?.Invoke();
            }

            if ((item.CanEquip && item.EquipState == EquipState.NotEquipped) ||
                (item.itemType == ItemType.Ammo && targetList.Inventory.GetSelectedAmmo(item.ammoType).name != item.name))
            {
                onCanEquip?.Invoke();
            }
            else
            {
                onCannotEquip?.Invoke();
            }

            if(item.EquipState != EquipState.NotEquipped)
            {
                onCanRemove?.Invoke();
            }
            else
            {
                onCannotRemove?.Invoke();
            }
            
            if(item.allowCustomName)
            {
                onCanRename?.Invoke();
            }
            else
            {
                onCannotRename?.Invoke();
            }

            if (item.CanRepair && item.condition < 1)
            {
                onCanRepair?.Invoke();
            }
            else
            {
                onCannotRepair?.Invoke();
            }

            if (item.itemType == ItemType.Consumable)
            {
                onIsConsumable?.Invoke();
            }
            else
            {
                onIsNotConsumable?.Invoke();
            }

        }

        #endregion

    }
}