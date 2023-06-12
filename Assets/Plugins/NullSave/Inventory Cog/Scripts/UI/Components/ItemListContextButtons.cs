using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NullSave.TOCK.Inventory
{
    public class ItemListContextButtons : MonoBehaviour
    {

        #region Variables

        public InventoryItemList targetList;
        public Button btnEquip, btnRemove, btnModify, btnRepair, btnDrop, btnBreakdown, btnAttach, 
            btnUnattach, btnRename, btnContainer;

        #endregion

        #region Unity Events

        private void Start()
        {
            targetList.onSelectionChanged.AddListener(SelectionChanged);
            SelectionChanged(0);

            if (btnEquip != null) btnEquip.onClick.AddListener(() => targetList.EquipSelected());
            if (btnRemove != null) btnRemove.onClick.AddListener(() => targetList.UnequipSelected());
            if (btnModify != null) btnModify.onClick.AddListener(() => targetList.ShowAttachmentsUI());
            if (btnRepair != null) btnRepair.onClick.AddListener(() => targetList.RepairSelected());
            if (btnDrop != null) btnDrop.onClick.AddListener(() => targetList.DropSelected());
            if (btnBreakdown != null) btnBreakdown.onClick.AddListener(() => targetList.BreakdownSelected());
            if (btnAttach != null) btnAttach.onClick.AddListener(() => targetList.ShowAttachmentsUI());
            if (btnUnattach != null) btnUnattach.onClick.AddListener(() => targetList.ShowAttachmentsUI());
            if (btnRename != null) btnRename.onClick.AddListener(() => targetList.RenameSelected());
            if (btnContainer != null) btnContainer.onClick.AddListener(() => targetList.OpenItemContainer());
        }

        #endregion

        #region Private Methods

        private void SelectionChanged(int index)
        {
            if (targetList.SelectedItem == null || targetList.SelectedItem.Item == null)
            {
                btnEquip.gameObject.SetActive(false);
                btnRemove.gameObject.SetActive(false);
                btnModify.gameObject.SetActive(false);
                btnRepair.gameObject.SetActive(false);
                btnDrop.gameObject.SetActive(false);
                btnBreakdown.gameObject.SetActive(false);
                btnAttach.gameObject.SetActive(false);
                btnUnattach.gameObject.SetActive(false);
                btnRename.gameObject.SetActive(false);
                btnContainer.gameObject.SetActive(false);
                return;
            }

            InventoryItem item = targetList.SelectedItem.Item;

            // Container check
            btnContainer.gameObject.SetActive(item.itemType == ItemType.Container);

            if (item.itemType == ItemType.Attachment)
            {
                btnAttach.gameObject.SetActive(!item.IsAttached);
                btnUnattach.gameObject.SetActive(item.IsAttached);
            }
            else
            {
                btnAttach.gameObject.SetActive(false);
                btnUnattach.gameObject.SetActive(false);

                if (item.HasAttachments)
                {
                    btnModify.gameObject.SetActive(true);
                }
                else
                {
                    switch (item.attachRequirement)
                    {
                        case AttachRequirement.InCategory:
                        case AttachRequirement.InItemList:
                            if (targetList.Inventory.AttachmentsAvailableForItem(item))
                            {
                                btnModify.gameObject.SetActive(true);
                            }
                            else
                            {
                                btnModify.gameObject.SetActive(false);
                            }
                            break;
                        case AttachRequirement.NoneAllowed:
                            btnModify.gameObject.SetActive(false);
                            break;
                    }
                }
            }

            btnBreakdown.gameObject.SetActive(item.CanBreakdown);
            btnDrop.gameObject.SetActive(item.canDrop);


            btnEquip.gameObject.SetActive((item.CanEquip && item.EquipState == EquipState.NotEquipped) ||
                (item.itemType == ItemType.Ammo && targetList.Inventory.GetSelectedAmmo(item.ammoType).name != item.name));

            btnRemove.gameObject.SetActive(item.EquipState != EquipState.NotEquipped);
            btnRename.gameObject.SetActive(item.allowCustomName);
            btnRepair.gameObject.SetActive(item.CanRepair && item.condition < 1);
        }

        #endregion

    }
}