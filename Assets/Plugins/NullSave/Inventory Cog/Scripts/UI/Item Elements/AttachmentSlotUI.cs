using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NullSave.TOCK.Inventory
{
    public class AttachmentSlotUI : MonoBehaviour, IDropHandler, IPointerClickHandler
    {

        #region Variables

        public TextMeshProUGUI displayName;
        public Image icon;
        public Image slotIcon;
        public Image equipedIcon;
        public GameObject selectionIndicator;

        public AttachmentSlotUIClick onClick;

        #endregion

        #region Properties

        public AttachmentSlot Slot { get; private set; }

        #endregion

        #region Unity Methods

        public void OnDrop(PointerEventData eventData)
        {
            ItemUI draggableItem = eventData.pointerDrag.gameObject.GetComponentInChildren<ItemUI>();
            if (draggableItem != null)
            {
                if (draggableItem != null && draggableItem.Item.itemType == ItemType.Attachment)
                {
                    Slot.AttachItem(draggableItem.Item);
                    draggableItem.OnEndDrag(eventData);
                }
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Click();
        }

        #endregion

        #region Public Methods

        public void Click()
        {
            onClick?.Invoke(this);
        }

        public void LoadSlot(AttachmentSlot slot)
        {
            Slot = slot;
            if (slot.AttachedItem == null)
            {
                if (displayName != null) displayName.gameObject.SetActive(false);
                if (icon != null) icon.gameObject.SetActive(false);
                if (equipedIcon != null) equipedIcon.gameObject.SetActive(false);
            }
            else
            {
                if (equipedIcon != null) equipedIcon.gameObject.SetActive(true);
                if (displayName != null) displayName.text = slot.AttachedItem.DisplayName;
                if (icon != null)
                {
                    icon.sprite = slot.AttachedItem.icon;
                    icon.gameObject.SetActive(slot.AttachedItem.icon != null);
                }
            }

            if (slotIcon)
            {
                slotIcon.gameObject.SetActive(slot.AttachPoint.slotIcon != null);
                slotIcon.sprite = slot.AttachPoint.slotIcon;
            }

        }

        public void Reload()
        {
            LoadSlot(Slot);
        }

        public void SetSelected(bool selected)
        {
            if (selectionIndicator != null) selectionIndicator.SetActive(selected);
        }


        #endregion

    }
}