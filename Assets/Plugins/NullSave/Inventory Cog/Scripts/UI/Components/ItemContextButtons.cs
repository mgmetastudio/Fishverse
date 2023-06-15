using UnityEngine;
using UnityEngine.UI;

namespace NullSave.TOCK.Inventory
{
    public class ItemContextButtons : MonoBehaviour
    {

        #region Variables

        public ItemManagerMenuUI itemManagerMenu;
        public Button btnEquip, btnRemove, btnModify, btnRepair, btnDrop, btnBreakdown,
            btnAttach, btnUnattach, btnRename, btnOpen, btnConsumable;

        #endregion

        #region Properties

        public InventoryCog Inventory { get; set; }

        public InventoryItem Item { get; private set; }

        #endregion

        #region Unity Methods

        private void Start()
        {
            System.Action reloadIMM = new System.Action(() => { itemManagerMenu.LoadComponents(); itemManagerMenu.UpdateChildren(); });
            btnEquip.onClick.AddListener(() => itemManagerMenu.EquipItem());
            btnRemove.onClick.AddListener(() => itemManagerMenu.UnequipItem());
            btnModify.onClick.AddListener(() => itemManagerMenu.OpenAttachmentsUI());
            btnRepair.onClick.AddListener(() => itemManagerMenu.RepairItem());
            btnDrop.onClick.AddListener(() => itemManagerMenu.DropItem());
            btnBreakdown.onClick.AddListener(() => itemManagerMenu.BreakdownItem());
            btnAttach.onClick.AddListener(() => itemManagerMenu.OpenAttachmentsUI());
            btnUnattach.onClick.AddListener(() => itemManagerMenu.OpenAttachmentsUI());
            btnRename.onClick.AddListener(() => Inventory.ActiveTheme.OpenRename(Item, reloadIMM));
            btnOpen.onClick.AddListener(() => itemManagerMenu.OpenItemContainer());
            btnConsumable.onClick.AddListener(() => itemManagerMenu.ConsumeItem());
        }

        #endregion

        #region Public Methods

        public void UpdateContext(InventoryItem item)
        {
            Item = item;

            if (item == null)
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
                btnOpen.gameObject.SetActive(false);
                btnConsumable.gameObject.SetActive(false);
                return;
            }

            // Container check
            btnOpen.gameObject.SetActive(item.itemType == ItemType.Container);

            if (item.itemType == ItemType.Attachment)
            {
                btnAttach.gameObject.SetActive(!item.IsAttached);
                btnUnattach.gameObject.SetActive(item.IsAttached);
                btnModify.gameObject.SetActive(false);
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
                            if (Inventory.AttachmentsAvailableForItem(item))
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

            btnConsumable.gameObject.SetActive(item.itemType == ItemType.Consumable);
            btnBreakdown.gameObject.SetActive(item.CanBreakdown);
            btnDrop.gameObject.SetActive(item.canDrop);
            btnRename.gameObject.SetActive(item.allowCustomName);
            btnRepair.gameObject.SetActive(item.CanRepair && item.condition < 1);
            btnRemove.gameObject.SetActive(item.EquipState != EquipState.NotEquipped);
            btnEquip.gameObject.SetActive((item.CanEquip && item.EquipState == EquipState.NotEquipped) ||
                (item.itemType == ItemType.Ammo && Inventory.GetSelectedAmmo(item.ammoType).name != item.name));
        }

        #endregion

    }
}